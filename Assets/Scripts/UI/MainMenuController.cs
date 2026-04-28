using UnityEngine;
using UnityEngine.SceneManagement;
using AHeavyToll.Managers;
using AHeavyToll.Gameplay;
using AHeavyToll.Horror;
using AHeavyToll.UI;
using AHeavyToll.Data;
using AHeavyToll.VFX;

namespace AHeavyToll.UI
{
    public class MainMenuController : MonoBehaviour
    {
        [Header("Menu Panels")]
        [SerializeField] private GameObject mainPanel;
        [SerializeField] private GameObject settingsPanel;
        [SerializeField] private GameObject creditsPanel;

        [Header("Buttons")]
        [SerializeField] private UnityEngine.UI.Button startButton;
        [SerializeField] private UnityEngine.UI.Button settingsButton;
        [SerializeField] private UnityEngine.UI.Button creditsButton;
        [SerializeField] private UnityEngine.UI.Button quitButton;
        [SerializeField] private UnityEngine.UI.Button backButton;

        [Header("Title Animation")]
        [SerializeField] private Animator titleAnimator;
        [SerializeField] private string titleFloatTrigger = "Float";

        [Header("Audio")]
        [SerializeField] private AudioClip menuMusic;
        [SerializeField] private AudioClip buttonHoverSound;
        [SerializeField] private AudioClip buttonClickSound;

        private void Start()
        {
            ShowMainPanel();
            AudioManager.Instance?.PlayMusic(menuMusic, true, 0.7f);

            if (startButton != null)
                startButton.onClick.AddListener(StartGame);
            if (settingsButton != null)
                settingsButton.onClick.AddListener(ShowSettings);
            if (creditsButton != null)
                creditsButton.onClick.AddListener(ShowCredits);
            if (quitButton != null)
                quitButton.onClick.AddListener(QuitGame);
            if (backButton != null)
                backButton.onClick.AddListener(ShowMainPanel);

            if (titleAnimator != null)
                titleAnimator.SetTrigger(titleFloatTrigger);

            if (AHeavyToll.Managers.CursorManager.Instance != null)
                AHeavyToll.Managers.CursorManager.Instance.RequestCursor();
            else
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }

        public void StartGame()
        {
            PlayClickSound();
            AudioManager.Instance?.FadeOutMusic(1f);
            SceneManager.LoadScene("GameScene");
        }

        public void ShowSettings()
        {
            PlayClickSound();
            if (mainPanel != null) mainPanel.SetActive(false);
            if (creditsPanel != null) creditsPanel.SetActive(false);
            if (settingsPanel != null) settingsPanel.SetActive(true);
        }

        public void ShowCredits()
        {
            PlayClickSound();
            if (mainPanel != null) mainPanel.SetActive(false);
            if (settingsPanel != null) settingsPanel.SetActive(false);
            if (creditsPanel != null) creditsPanel.SetActive(true);
        }

        public void ShowMainPanel()
        {
            PlayClickSound();
            if (settingsPanel != null) settingsPanel.SetActive(false);
            if (creditsPanel != null) creditsPanel.SetActive(false);
            if (mainPanel != null) mainPanel.SetActive(true);
        }

        public void QuitGame()
        {
            PlayClickSound();
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }

        private void PlayClickSound()
        {
            AudioManager.Instance?.PlayOneShot(buttonClickSound);
        }
    }
}
