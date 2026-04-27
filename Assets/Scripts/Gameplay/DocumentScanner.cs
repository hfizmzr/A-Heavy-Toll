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
    public class DocumentScanner : MonoBehaviour, IInteractable
    {
        [Header("UI")]
        [SerializeField] private GameObject scannerUI;
        [SerializeField] private TextMeshProUGUI scannedText;
        [SerializeField] private UnityEngine.UI.Image documentImage;

        [Header("Audio")]
        [SerializeField] private AudioClip scanBeep;
        [SerializeField] private AudioClip scanComplete;

        [Header("State")]
        [SerializeField] private bool isScanning = false;

        public string GetPromptText() => "Scan Documents";

        public void Interact()
        {
            if (isScanning) return;
            if (CarQueueManager.Instance?.CurrentCar == null)
            {
                SubtitleManager.Instance?.ShowSubtitle("No vehicle at the booth.", 2f);
                return;
            }
            StartScan();
        }

        private void StartScan()
        {
            isScanning = true;
            if (scannerUI != null) scannerUI.SetActive(true);
            AudioManager.Instance?.PlayOneShot(scanBeep);
            if (scannedText != null) scannedText.text = "SCANNING...";
            Invoke(nameof(CompleteScan), 1.5f);
        }

        private void CompleteScan()
        {
            var carData = CarQueueManager.Instance.CurrentCar.Data;
            if (scannedText != null)
            {
                string forgedWarning = carData.documentsAreForged ? @"

<color=red>WARNING: FORGED DOCUMENTS DETECTED</color>" : "";
                scannedText.text = $@"DRIVER: {carData.driverName}
DESTINATION: {carData.destination}
PLATE: {(carData.licensePlateSuspicious ? "<color=red>SUSPICIOUS</color>" : "CLEAR")}{forgedWarning}";
            }
            AudioManager.Instance?.PlayOneShot(scanComplete);
            Invoke(nameof(CloseScanner), 4f);
        }

        private void CloseScanner()
        {
            if (scannerUI != null) scannerUI.SetActive(false);
            isScanning = false;
        }
    }
}
