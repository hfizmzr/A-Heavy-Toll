using System.Collections.Generic;
using UnityEngine;
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

        [Header("UI")]
        [SerializeField] private GameObject journalPanel;
        [SerializeField] private TextMeshProUGUI journalTitleText;
        [SerializeField] private TextMeshProUGUI journalContentText;
        [SerializeField] private Transform entryListParent;
        [SerializeField] private GameObject entryButtonPrefab;

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
            if (journalPanel != null) journalPanel.SetActive(false);
            UnlockEntriesForNight(1);
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
            if (entryListParent == null || entryButtonPrefab == null) return;
            foreach (Transform child in entryListParent)
                Destroy(child.gameObject);

            foreach (var entry in journalEntries)
            {
                if (!entry.isUnlocked) continue;
                GameObject btn = Instantiate(entryButtonPrefab, entryListParent);
                TextMeshProUGUI btnText = btn.GetComponentInChildren<TextMeshProUGUI>();
                UnityEngine.UI.Button button = btn.GetComponent<UnityEngine.UI.Button>();

                if (btnText != null)
                {
                    btnText.text = entry.hasBeenRead ? entry.entryTitle : $"{entry.entryTitle} [NEW]";
                    btnText.color = entry.hasBeenRead ? Color.gray : Color.white;
                }
                if (button != null)
                {
                    string id = entry.entryId;
                    button.onClick.AddListener(() => SelectEntry(id));
                }
            }
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
