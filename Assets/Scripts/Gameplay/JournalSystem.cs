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
            UnlockEntriesForNight(1);
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
                    isUnlocked = false,
                    hasBeenRead = false
                });
            }
        }

        public void UnlockEntriesForNight(int nightNumber)
        {
            foreach (var entry in journalEntries)
            {
                if (entry.unlockNight == nightNumber && !entry.isUnlocked)
                {
                    entry.isUnlocked = true;
                    Debug.Log($"[Journal] Unlocked: {entry.entryTitle}");
                }
            }
            RefreshEntryList();
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
            if (journalContentText != null) journalContentText.text = entry.entryText;
            AudioManager.Instance?.PlayOneShot(pageTurnSound);
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
                tmp.color = entry.hasBeenRead ? Color.gray : Color.white;
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

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.J))
            {
                if (journalPanel != null && journalPanel.activeSelf)
                    CloseJournal();
                else
                    OpenJournal();
            }
        }
    }
}
