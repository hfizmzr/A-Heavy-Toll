using UnityEngine;
using AHeavyToll.Managers;
using AHeavyToll.Gameplay;
using AHeavyToll.Horror;
using AHeavyToll.UI;
using AHeavyToll.Data;
using AHeavyToll.VFX;

namespace AHeavyToll.Data
{
    [CreateAssetMenu(fileName = "NewCarData", menuName = "A Heavy Toll/Car Data")]
    public class CarData : ScriptableObject
    {
        [Header("Identity")]
        public string driverName;
        [TextArea(2, 4)] public string driverDialogue;
        [TextArea(2, 4)] public string destination;

        [Header("Visuals")]
        public GameObject carModelPrefab;
        public Sprite driverPortrait; // If using UI portraits

        [Header("Threat Level")]
        public bool isMalevolent = false;
        public bool isHer = false; // The hidden ending entity

        [Header("Vetting Clues")]
        [TextArea(2, 4)] public string documentsText;
        public bool documentsAreForged = false;
        public bool licensePlateSuspicious = false;
        public bool cargoIsUnusual = false;

        [Header("Audio")]
        public AudioClip engineSound;
        public AudioClip voiceClip;
        public AudioClip radioStaticOverride; // For supernatural entities

        [Header("Night Availability")]
        public bool appearsNight1 = true;
        public bool appearsNight2 = true;
        public bool appearsNight3 = true;

        [Header("Special Events")]
        public bool triggersHorrorEvent = false;
        public string horrorEventId = "";
    }
}
