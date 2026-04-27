using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using AHeavyToll.Managers;
using AHeavyToll.Gameplay;
using AHeavyToll.Horror;
using AHeavyToll.UI;
using AHeavyToll.Data;
using AHeavyToll.VFX;

namespace AHeavyToll.Managers
{
    public class SettingsManager : MonoBehaviour
    {
        [Header("Audio Settings")]
        [SerializeField] private Slider masterVolumeSlider;
        [SerializeField] private Slider musicVolumeSlider;
        [SerializeField] private Slider sfxVolumeSlider;

        [Header("Graphics Settings")]
        [SerializeField] private Toggle fullscreenToggle;
        [SerializeField] private Toggle vhsEffectToggle;
        [SerializeField] private TMP_Dropdown resolutionDropdown;

        [Header("Gameplay Settings")]
        [SerializeField] private Slider lookSensitivitySlider;
        [SerializeField] private Toggle subtitlesToggle;

        [Header("PSX Settings")]
        [SerializeField] private Toggle ditheringToggle;
        [SerializeField] private Slider renderScaleSlider;

        private Resolution[] resolutions;

        private void Start()
        {
            LoadSettings();
            SetupUI();
        }

        private void SetupUI()
        {
            if (masterVolumeSlider != null)
            {
                masterVolumeSlider.value = PlayerPrefs.GetFloat("MasterVolume", 1f);
                masterVolumeSlider.onValueChanged.AddListener(SetMasterVolume);
            }
            if (musicVolumeSlider != null)
            {
                musicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1f);
                musicVolumeSlider.onValueChanged.AddListener(SetMusicVolume);
            }
            if (sfxVolumeSlider != null)
            {
                sfxVolumeSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1f);
                sfxVolumeSlider.onValueChanged.AddListener(SetSFXVolume);
            }
            if (fullscreenToggle != null)
            {
                fullscreenToggle.isOn = Screen.fullScreen;
                fullscreenToggle.onValueChanged.AddListener(SetFullscreen);
            }
            if (vhsEffectToggle != null)
            {
                vhsEffectToggle.isOn = PlayerPrefs.GetInt("VHSEffect", 1) == 1;
                vhsEffectToggle.onValueChanged.AddListener(SetVHSEffect);
            }
            if (resolutionDropdown != null)
            {
                resolutions = Screen.resolutions;
                resolutionDropdown.ClearOptions();
                var options = new System.Collections.Generic.List<string>();
                int currentResolutionIndex = 0;
                for (int i = 0; i < resolutions.Length; i++)
                {
                    string option = $"{resolutions[i].width} x {resolutions[i].height}";
                    options.Add(option);
                    if (resolutions[i].width == Screen.currentResolution.width &&
                        resolutions[i].height == Screen.currentResolution.height)
                        currentResolutionIndex = i;
                }
                resolutionDropdown.AddOptions(options);
                resolutionDropdown.value = currentResolutionIndex;
                resolutionDropdown.onValueChanged.AddListener(SetResolution);
            }
            if (lookSensitivitySlider != null)
            {
                lookSensitivitySlider.value = PlayerPrefs.GetFloat("LookSensitivity", 2f);
                lookSensitivitySlider.onValueChanged.AddListener(SetLookSensitivity);
            }
            if (subtitlesToggle != null)
            {
                subtitlesToggle.isOn = PlayerPrefs.GetInt("Subtitles", 1) == 1;
                subtitlesToggle.onValueChanged.AddListener(SetSubtitles);
            }
            if (ditheringToggle != null)
            {
                ditheringToggle.isOn = PlayerPrefs.GetInt("Dithering", 1) == 1;
                ditheringToggle.onValueChanged.AddListener(SetDithering);
            }
            if (renderScaleSlider != null)
            {
                renderScaleSlider.value = PlayerPrefs.GetFloat("RenderScale", 0.5f);
                renderScaleSlider.onValueChanged.AddListener(SetRenderScale);
            }
        }

        private void SetMasterVolume(float value)
        {
            AudioManager.Instance?.SetMasterVolume(value);
            PlayerPrefs.SetFloat("MasterVolume", value);
        }
        private void SetMusicVolume(float value)
        {
            if (AudioManager.Instance?.musicSource != null)
                AudioManager.Instance.musicSource.volume = value;
            PlayerPrefs.SetFloat("MusicVolume", value);
        }
        private void SetSFXVolume(float value)
        {
            if (AudioManager.Instance?.sfxSource != null)
                AudioManager.Instance.sfxSource.volume = value;
            PlayerPrefs.SetFloat("SFXVolume", value);
        }
        private void SetFullscreen(bool fullscreen)
        {
            Screen.fullScreen = fullscreen;
            PlayerPrefs.SetInt("Fullscreen", fullscreen ? 1 : 0);
        }
        private void SetVHSEffect(bool enabled)
        {
            UIManager.Instance?.ToggleVHS(enabled);
            PlayerPrefs.SetInt("VHSEffect", enabled ? 1 : 0);
        }
        private void SetResolution(int index)
        {
            if (resolutions != null && index < resolutions.Length)
            {
                Resolution resolution = resolutions[index];
                Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
            }
        }
        private void SetLookSensitivity(float value)
        {
            PlayerPrefs.SetFloat("LookSensitivity", value);
        }
        private void SetSubtitles(bool enabled)
        {
            PlayerPrefs.SetInt("Subtitles", enabled ? 1 : 0);
        }
        private void SetDithering(bool enabled)
        {
            PlayerPrefs.SetInt("Dithering", enabled ? 1 : 0);
        }
        private void SetRenderScale(float value)
        {
            var psxPost = FindObjectOfType<VFX.PSXPostProcess>();
            if (psxPost != null) psxPost.SetRenderScale(value);
            PlayerPrefs.SetFloat("RenderScale", value);
        }
        private void LoadSettings() { }
        public void SaveSettings() { PlayerPrefs.Save(); }
    }
}
