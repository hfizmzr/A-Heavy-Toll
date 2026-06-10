using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using AHeavyToll.Managers;
using AHeavyToll.Gameplay;
using AHeavyToll.Horror;
using AHeavyToll.UI;
using AHeavyToll.Data;
using AHeavyToll.VFX;

namespace AHeavyToll.Gameplay
{
    [System.Serializable]
    public class JournalEntry
    {
        public string entryId;
        public string entryTitle;
        [TextArea(3, 10)] public string entryText;
        public int unlockNight;
        public int unlockAfterCarCount;
        public bool isUnlocked = false;
        public bool hasBeenRead = false;
    }

    public class JournalSystem : MonoBehaviour
    {
        public static JournalSystem Instance { get; private set; }

        [Header("Journal Data")]
        [SerializeField] private List<JournalEntry> journalEntries = new List<JournalEntry>();
        [SerializeField] private List<PredecessorJournal> predecessorJournals = new List<PredecessorJournal>();

        [Header("UI")]
        [SerializeField] private GameObject journalPanel;
        [SerializeField] private TextMeshProUGUI journalTitleText;
        [SerializeField] private TextMeshProUGUI journalContentText;
        [SerializeField] private Transform entryListParent;

        [Header("Scroll")]
        [SerializeField] private RectTransform entryListContentView;

        [Header("Audio")]
        [SerializeField] private AudioClip pageTurnSound;
        [SerializeField] private AudioClip journalOpenSound;

        private JournalEntry currentEntry;
        private ScrollRect _contentScrollRect;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void Start()
        {
            ConvertPredecessorJournals();
            if (journalPanel != null) journalPanel.SetActive(false);
            SetupContentScrolling();
            UnlockEntriesForNight(1);
        }

        private void SetupContentScrolling()
        {
            if (journalContentText == null) return;

            // Remove ContentSizeFitter from the text itself
            ContentSizeFitter csf = journalContentText.GetComponent<ContentSizeFitter>();
            if (csf != null) Destroy(csf);

            // Remove ContentSizeFitter from the content container (parent) too —
            // it keeps marking layout dirty, which causes ScrollRect.LateUpdate to
            // recalculate bounds and snap the scroll position back to top.
            RectTransform parentRt = journalContentText.transform.parent as RectTransform;
            if (parentRt != null)
            {
                ContentSizeFitter parentCsf = parentRt.GetComponent<ContentSizeFitter>();
                if (parentCsf != null) Destroy(parentCsf);
            }

            _contentScrollRect = journalContentText.GetComponentInParent<ScrollRect>();
            if (_contentScrollRect != null)
            {
                _contentScrollRect.horizontal = false;
                _contentScrollRect.scrollSensitivity = 30f;

                // Widen the scroll rect and its container so more text fits per line
                RectTransform scrollRt = _contentScrollRect.GetComponent<RectTransform>();
                if (scrollRt != null)
                    scrollRt.sizeDelta = new Vector2(350, scrollRt.sizeDelta.y);

                RectTransform containerParentRt = scrollRt != null ? scrollRt.parent as RectTransform : null;
                if (containerParentRt != null)
                    containerParentRt.sizeDelta = new Vector2(350, containerParentRt.sizeDelta.y);
            }
        }

        private void ConvertPredecessorJournals()
        {
            foreach (PredecessorJournal pj in predecessorJournals)
            {
                if (pj == null) continue;
                if (journalEntries.Exists(e => e.entryId == pj.entryId)) continue;
                journalEntries.Add(new JournalEntry
                {
                    entryId = pj.entryId,
                    entryTitle = pj.title,
                    entryText = pj.entryContent,
                    unlockNight = pj.unlockNight,
                    unlockAfterCarCount = pj.unlockAfterCarCount,
                    isUnlocked = false,
                    hasBeenRead = false
                });
            }
        }

        public void UnlockEntriesForNight(int nightNumber)
        {
            foreach (var entry in journalEntries)
            {
                if (entry.unlockNight == nightNumber && entry.unlockAfterCarCount == 0 && !entry.isUnlocked)
                {
                    entry.isUnlocked = true;
                    Debug.Log($"[Journal] Unlocked: {entry.entryTitle}");
                }
            }
            RefreshEntryList();
        }

        public void UnlockEntriesAfterCarCount(int nightNumber, int carCount)
        {
            bool anyUnlocked = false;
            foreach (var entry in journalEntries)
            {
                if (entry.unlockNight == nightNumber && entry.unlockAfterCarCount == carCount && !entry.isUnlocked)
                {
                    entry.isUnlocked = true;
                    anyUnlocked = true;
                    Debug.Log($"[Journal] Unlocked after car {carCount}: {entry.entryTitle}");
                }
            }
            if (anyUnlocked)
            {
                SubtitleManager.Instance?.ShowSubtitle("New entry found. Press 'J' to view.", 3f);
                RefreshEntryList();
            }
        }

        public void OpenJournal()
        {
            if (journalPanel == null) return;
            journalPanel.SetActive(true);
            AudioManager.Instance?.PlayOneShot(journalOpenSound);
            CursorManager.Instance?.RequestCursor();
            RefreshEntryList();
        }

        public void CloseJournal()
        {
            if (journalPanel != null) journalPanel.SetActive(false);
            if (GameManager.Instance.CurrentState == GameState.Playing)
            {
                CursorManager.Instance?.ReleaseCursor();
            }
        }

        public void SelectEntry(string entryId)
        {
            JournalEntry entry = journalEntries.Find(e => e.entryId == entryId);
            if (entry == null || !entry.isUnlocked) return;

            currentEntry = entry;
            entry.hasBeenRead = true;

            if (journalTitleText != null) journalTitleText.text = entry.entryTitle;
            if (journalContentText != null)
            {
                journalContentText.text = entry.entryText;
                journalContentText.ForceMeshUpdate();

                RectTransform textRt = journalContentText.rectTransform;
                float textHeight = journalContentText.preferredHeight;

                textRt.sizeDelta = new Vector2(textRt.sizeDelta.x, textHeight);

                RectTransform containerRt = textRt.parent as RectTransform;
                if (containerRt != null)
                    containerRt.sizeDelta = new Vector2(containerRt.sizeDelta.x, textHeight + 40f);

                Canvas.ForceUpdateCanvases();
                StartCoroutine(ResetScrollNextFrame());
            }
            AudioManager.Instance?.PlayOneShot(pageTurnSound);

            RefreshEntryList();
        }

        private void RefreshEntryList()
        {
            if (entryListParent == null) return;
            Transform content = entryListContentView != null ? entryListContentView : entryListParent;
            foreach (Transform child in content)
                Destroy(child.gameObject);

            float buttonHeight = 40f;
            int index = 0;

            foreach (var entry in journalEntries)
            {
                if (!entry.isUnlocked) continue;

                GameObject btnObj = new GameObject(entry.entryTitle, typeof(RectTransform));
                btnObj.transform.SetParent(content, false);

                RectTransform btnRt = btnObj.GetComponent<RectTransform>();
                btnRt.anchorMin = new Vector2(0, 1);
                btnRt.anchorMax = new Vector2(1, 1);
                btnRt.sizeDelta = new Vector2(0, buttonHeight);
                btnRt.anchoredPosition = new Vector2(0, -index * buttonHeight);

                Button button = btnObj.AddComponent<Button>();
                Image img = btnObj.AddComponent<Image>();
                img.color = new Color(1, 1, 1, 0.1f);

                GameObject textObj = new GameObject("Text", typeof(RectTransform));
                textObj.transform.SetParent(btnObj.transform, false);
                TextMeshProUGUI tmp = textObj.AddComponent<TextMeshProUGUI>();
                tmp.text = entry.hasBeenRead ? entry.entryTitle : $"{entry.entryTitle} [NEW]";
                tmp.color = entry.hasBeenRead ? Color.white : Color.yellow;
                tmp.fontSize = 18;
                tmp.alignment = TextAlignmentOptions.Center;

                RectTransform textRt = tmp.rectTransform;
                textRt.anchorMin = Vector2.zero;
                textRt.anchorMax = Vector2.one;
                textRt.sizeDelta = Vector2.zero;

                string id = entry.entryId;
                button.onClick.AddListener(() => SelectEntry(id));

                index++;
            }

            if (entryListContentView != null)
                entryListContentView.sizeDelta = new Vector2(entryListContentView.sizeDelta.x, index * buttonHeight);
        }

        private System.Collections.IEnumerator ResetScrollNextFrame()
        {
            yield return null;
            if (_contentScrollRect != null) _contentScrollRect.normalizedPosition = new Vector2(0, 1);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.J))
            {
                if (journalPanel != null && journalPanel.activeSelf)
                    CloseJournal();
                else
                    OpenJournal();
            }

            if (journalPanel != null && journalPanel.activeSelf && _contentScrollRect != null)
            {
                float scroll = Input.mouseScrollDelta.y;
                if (Mathf.Abs(scroll) > 0.001f)
                {
                    _contentScrollRect.verticalNormalizedPosition =
                        Mathf.Clamp01(_contentScrollRect.verticalNormalizedPosition + scroll * 0.05f);
                }
            }
        }
    }
}
