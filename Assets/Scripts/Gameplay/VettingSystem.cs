using UnityEngine;
using AHeavyToll.Managers;
using AHeavyToll.Gameplay;
using AHeavyToll.Horror;
using AHeavyToll.UI;
using AHeavyToll.Data;
using AHeavyToll.VFX;
using System.Collections;

namespace AHeavyToll.Managers
{
    public class VettingSystem : MonoBehaviour
    {
        public static VettingSystem Instance { get; private set; }

        [Header("UI References")]
        [SerializeField] private GameObject vettingPanel;
        [SerializeField] private TMPro.TextMeshProUGUI driverNameText;
        [SerializeField] private TMPro.TextMeshProUGUI dialogueText;
        [SerializeField] private TMPro.TextMeshProUGUI documentsText;
        [SerializeField] private TMPro.TextMeshProUGUI destinationText;
        [SerializeField] private UnityEngine.UI.Image driverPortrait;

        [Header("Buttons")]
        [SerializeField] private UnityEngine.UI.Button allowButton;
        [SerializeField] private UnityEngine.UI.Button denyButton;

        [Header("State")]
        [SerializeField] private bool isVetting = false;
        [SerializeField] private CarController currentCar;
        [SerializeField] private bool pendingDecision;

        public bool IsVetting => isVetting;

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
            if (allowButton != null)
                allowButton.onClick.AddListener(() => MakeDecision(true));

            if (denyButton != null)
                denyButton.onClick.AddListener(() => MakeDecision(false));

            if (vettingPanel != null)
                vettingPanel.SetActive(false);
        }

        // public void BeginVetting(CarController car)
        // {
        //     if (car == null || car.Data == null) return;

        //     currentCar = car;
        //     isVetting = true;

        //     var data = car.Data;

        //     // Populate UI
        //     if (driverNameText != null) driverNameText.text = data.driverName;
        //     if (dialogueText != null) dialogueText.text = $"\"{data.driverDialogue}\"";
        //     if (documentsText != null) documentsText.text = data.documentsText;
        //     if (destinationText != null) destinationText.text = $"Destination: {data.destination}";
        //     if (driverPortrait != null) driverPortrait.sprite = data.driverPortrait;

        //     if (vettingPanel != null) vettingPanel.SetActive(true);

        //     // Play voice if available
        //     if (data.voiceClip != null)
        //         AudioManager.Instance?.PlayOneShot(data.voiceClip);

        //     SubtitleManager.Instance?.ShowSubtitle(data.driverDialogue, 4f);

        //     // Enable buttons
        //     SetButtonsInteractable(true);

        //     CursorManager.Instance?.RequestCursor();
        // }

        public void BeginVetting(CarController car)
        {
            if (car == null || car.Data == null) return;

            currentCar = car;
            isVetting = true;

            StartCoroutine(VettingSequence(car));
        }

        private IEnumerator VettingSequence(CarController car)
        {
            var data = car.Data;
            float waitTime = 3f;

            // 1. Play voice first
            if (data.voiceClip != null)
            {
                waitTime = Mathf.Max(waitTime, data.voiceClip.length);
                AudioManager.Instance?.PlayOneShot(data.voiceClip);

                // Show subtitle for the same length as the voice
                SubtitleManager.Instance?.ShowSubtitle(data.driverDialogue, data.voiceClip.length);
            }
            else
            {
                // No voice, just show subtitle for fallback duration
                SubtitleManager.Instance?.ShowSubtitle(data.driverDialogue, 3f);
            }

            // 3. Wait for audio OR fallback 4 seconds, whichever is longer
            yield return new WaitForSeconds(waitTime);

            // 4. Now show full UI
            ShowVettingUI(data);
        }

        private void ShowVettingUI(CarData data)
        {
            if (driverNameText != null) driverNameText.text = data.driverName;
            if (dialogueText != null) dialogueText.text = $"\"{data.driverDialogue}\"";
            if (documentsText != null) documentsText.text = data.documentsText;
            if (destinationText != null) destinationText.text = $"Destination: {data.destination}";
            if (driverPortrait != null) driverPortrait.sprite = data.driverPortrait;

            if (vettingPanel != null) vettingPanel.SetActive(true);

            SetButtonsInteractable(true);

            CursorManager.Instance?.RequestCursor();
        }

        public void MakeDecision(bool allowThrough)
        {
            if (!isVetting || currentCar == null) return;

            SetButtonsInteractable(false);

            pendingDecision = allowThrough;

            string decisionText = allowThrough ? "PROCEED." : "STOP. RETURN.";
            SubtitleManager.Instance?.ShowSubtitle(decisionText, 2f);

            Invoke(nameof(ExecuteDecision), 1.5f);
        }

        private void ExecuteDecision()
        {
            CarQueueManager.Instance?.ProcessDecision(pendingDecision); 
            FinishVetting();
        }

        private void FinishVetting()
        {
            if (vettingPanel != null) vettingPanel.SetActive(false);
            isVetting = false;
            currentCar = null;
            CursorManager.Instance?.ReleaseCursor();
        }

        private void SetButtonsInteractable(bool state)
        {
            if (allowButton != null) allowButton.interactable = state;
            if (denyButton != null) denyButton.interactable = state;
        }
    }
}
