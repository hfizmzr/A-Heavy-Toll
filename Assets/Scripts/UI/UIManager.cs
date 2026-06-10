using UnityEngine;
using UnityEngine.UI;
using TMPro;
using AHeavyToll.Managers;
using AHeavyToll.Gameplay;
using AHeavyToll.Horror;
using AHeavyToll.UI;
using AHeavyToll.Data;
using AHeavyToll.VFX;

namespace AHeavyToll.UI
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        [Header("Screens")]
        public GameObject mainMenuPanel;
        public GameObject hudPanel;
        public GameObject pausePanel;
        public GameObject gameOverPanel;
        public GameObject settingsPanel;

        [Header("HUD Elements")]
        public TextMeshProUGUI nightText;
        public TextMeshProUGUI carsRemainingText;
        public Image incomingCarIndicator;
        public float indicatorFlashDuration = 1f;

        [Header("Game Over")]
        public TextMeshProUGUI endingText;
        public TextMeshProUGUI endingDescription;
        public Button restartButton;
        public Button menuButton;

        [Header("VHS Effect Toggle")]
        public GameObject vhsEffectObject;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            transform.localScale = Vector3.one;
        }

        private void Start()
        {
            ShowHUD();
        }

        private void Update()
        {
            if (GameManager.Instance != null && GameManager.Instance.CurrentState == GameState.Playing)
            {
                UpdateHUD();
            }
        }

        public void ShowMainMenu()
        {
            HideAllPanels();
            if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
            CursorManager.Instance?.RequestCursor();
        }

        public void ShowHUD()
        {
            HideAllPanels();
            if (hudPanel != null) hudPanel.SetActive(true);
            CursorManager.Instance?.ReleaseCursor();
        }

        public void ShowPause()
        {
            if (pausePanel != null) pausePanel.SetActive(true);
            CursorManager.Instance?.RequestCursor();
        }

        public void HidePause()
        {
            if (pausePanel != null) pausePanel.SetActive(false);
            CursorManager.Instance?.ReleaseCursor();
        }

        public void ShowGameOver(EndingType ending)
        {
            CursorManager.Instance?.RequestCursor();

            if (GameManager.Instance != null)
                GameManager.Instance.CurrentState = GameState.GameOver;

            // Deactivate ending visuals that have conflicting GraphicRaycasters
            var endingVis = GameObject.Find("EndingVisual_Hidden");
            if (endingVis != null)
                endingVis.SetActive(false);

            var endingVisBad = GameObject.Find("EndingVisual_Bad");
            if (endingVisBad != null)
                endingVisBad.SetActive(false);

            var endingVisGood = GameObject.Find("EndingVisual_Good");
            if (endingVisGood != null)
                endingVisGood.SetActive(false);

            var endingVisFired = GameObject.Find("EndingVisual_Fired");
            if (endingVisFired != null)
                endingVisFired.SetActive(false);

            HideAllPanels();
            if (gameOverPanel != null)
            {
                gameOverPanel.SetActive(true);
                gameOverPanel.transform.SetAsLastSibling();
            }

            if (restartButton != null)
            {
                restartButton.interactable = true;
                restartButton.onClick = new Button.ButtonClickedEvent();
                restartButton.onClick.AddListener(() => GameManager.Instance?.RestartGame());
            }
            else
            {
                Debug.LogWarning("[UIManager] restartButton is null in ShowGameOver!");
            }

            if (menuButton != null)
            {
                menuButton.interactable = true;
                menuButton.onClick = new Button.ButtonClickedEvent();
                menuButton.onClick.AddListener(() => GameManager.Instance?.QuitToMenu());
            }
            else
            {
                Debug.LogWarning("[UIManager] menuButton is null in ShowGameOver!");
            }

            string title = ending switch
            {
                EndingType.Good => "GOOD ENDING",
                EndingType.Bad => "BAD ENDING",
                EndingType.Hidden => "HIDDEN ENDING",
                EndingType.Fired => "FIRED ENDING",
                _ => "UNKNOWN"
            };

            string desc = ending switch
            {
                EndingType.Good => "You held the line. The threshold remains sealed. You survive to see the dawn.",
                EndingType.Bad => "You let them through. The booth is breached. Something follows you home.",
                EndingType.Hidden => "May her soul rest in peace, for those who silenced her will find no such mercy.",
                EndingType.Fired => "You denied every car. Your superiors have questions.",
                _ => "..."
            };

            if (endingText != null) endingText.text = title;
            if (endingDescription != null) endingDescription.text = desc;
        }

        public void ShowIncomingCar()
        {
            if (incomingCarIndicator != null)
            {
                StartCoroutine(FlashIndicator());
            }
        }

        private System.Collections.IEnumerator FlashIndicator()
        {
            if (incomingCarIndicator == null) yield break;

            incomingCarIndicator.gameObject.SetActive(true);
            float elapsed = 0f;

            while (elapsed < indicatorFlashDuration)
            {
                float alpha = Mathf.PingPong(elapsed * 4f, 1f);
                incomingCarIndicator.color = new Color(1, 0, 0, alpha);
                elapsed += Time.deltaTime;
                yield return null;
            }

            incomingCarIndicator.gameObject.SetActive(false);
        }

        private void UpdateHUD()
        {
            if (nightText != null)
                nightText.text = $"NIGHT {GameManager.Instance.CurrentNightNumber}";
        }

        private void HideAllPanels()
        {
            if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
            if (hudPanel != null) hudPanel.SetActive(false);
            if (pausePanel != null) pausePanel.SetActive(false);
            if (gameOverPanel != null) gameOverPanel.SetActive(false);
            if (settingsPanel != null) settingsPanel.SetActive(false);
        }

        public void ToggleVHS(bool enabled)
        {
            if (vhsEffectObject != null)
                vhsEffectObject.SetActive(enabled);
        }

    }
}
