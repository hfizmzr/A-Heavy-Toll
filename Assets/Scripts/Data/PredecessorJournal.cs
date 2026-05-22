using UnityEngine;
using AHeavyToll.Managers;
using AHeavyToll.Gameplay;
using AHeavyToll.Horror;
using AHeavyToll.UI;
using AHeavyToll.Data;
using AHeavyToll.VFX;

namespace AHeavyToll.Data
{
    [CreateAssetMenu(fileName = "NewJournalEntry", menuName = "A Heavy Toll/Journal Entry")]
    public class PredecessorJournal : ScriptableObject
    {
        [Header("Display")]
        public string title;

        [Header("Metadata")]
        public string entryId;
        public string entryDate;
        public int unlockNight = 1;

        [Header("Content")]
        [TextArea(5, 20)] public string entryContent;

        [Header("Horror Level")]
        [Range(1, 5)] public int sanityLevel = 1;
        public bool containsWarning = false;
        public bool mentionsHer = false;

        [Header("Visual Style")]
        public bool isScrawled = false;
        public bool isBloodstained = false;
        public bool isTorn = false;

        [Header("Audio")]
        public AudioClip readingVoice;
    }
}
