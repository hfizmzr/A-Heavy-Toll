using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal; // If using URP
using AHeavyToll.Managers;
using AHeavyToll.Gameplay;
using AHeavyToll.Horror;
using AHeavyToll.UI;
using AHeavyToll.Data;
using AHeavyToll.VFX;

namespace AHeavyToll.Managers
{
    public class DayCycleManager : MonoBehaviour
    {
        public static DayCycleManager Instance { get; private set; }

        [Header("Atmosphere Profiles")]
        [SerializeField] private Material skyboxNight1;
        [SerializeField] private Material skyboxNight2;
        [SerializeField] private Material skyboxNight3;

        [Header("Fog Settings")]
        [SerializeField] private Color fogNight1 = new Color(0.05f, 0.05f, 0.08f, 1f);
        [SerializeField] private Color fogNight2 = new Color(0.08f, 0.02f, 0.02f, 1f);
        [SerializeField] private Color fogNight3 = new Color(0.3f, 0.0f, 0.0f, 1f); // Red fog
        [SerializeField] private float fogDensityNight1 = 0.02f;
        [SerializeField] private float fogDensityNight2 = 0.04f;
        [SerializeField] private float fogDensityNight3 = 0.08f;

        [Header("Lighting")]
        [SerializeField] private Light sceneLight;
        [SerializeField] private Color lightNight1 = new Color(0.8f, 0.8f, 0.9f);
        [SerializeField] private Color lightNight2 = new Color(0.7f, 0.6f, 0.5f);
        [SerializeField] private Color lightNight3 = new Color(0.9f, 0.2f, 0.2f);

        [Header("Post Processing")]
        [SerializeField] private Volume postProcessVolume;
        // References to URP volume overrides would go here

        [Header("Ambient Audio")]
        [SerializeField] private AudioSource ambientSource;
        [SerializeField] private AudioClip ambienceNight1;
        [SerializeField] private AudioClip ambienceNight2;
        [SerializeField] private AudioClip ambienceNight3;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        public void ApplyDayAtmosphere(Day day)
        {
            switch (day)
            {
                case Day.Night1:
                    ApplyNight1();
                    break;
                case Day.Night2:
                    ApplyNight2();
                    break;
                case Day.Night3:
                    ApplyNight3();
                    break;
            }
        }

        private void ApplyNight1()
        {
            RenderSettings.fog = true;
            RenderSettings.fogColor = fogNight1;
            RenderSettings.fogDensity = fogDensityNight1;
            RenderSettings.fogMode = FogMode.Exponential;

            if (sceneLight != null) sceneLight.color = lightNight1;
            if (skyboxNight1 != null) RenderSettings.skybox = skyboxNight1;

            PlayAmbience(ambienceNight1);

            SubtitleManager.Instance?.ShowSubtitle("Night 1. Just another shift.", 1f);
            SubtitleManager.Instance?.ShowSubtitle("Night 1. Just another shift.", 3f);
            SubtitleManager.Instance?.ShowSubtitle("New Journal Entry Logged. Press 'J' to view.", 3f);
        }

        private void ApplyNight2()
        {
            RenderSettings.fog = true;
            RenderSettings.fogColor = fogNight2;
            RenderSettings.fogDensity = fogDensityNight2;

            if (sceneLight != null) sceneLight.color = lightNight2;
            if (skyboxNight2 != null) RenderSettings.skybox = skyboxNight2;

            PlayAmbience(ambienceNight2);

            SubtitleManager.Instance?.ShowSubtitle("Night 2. Something feels wrong out there.", 4f);
            SubtitleManager.Instance?.ShowSubtitle("New Journal Entry Logged. Press 'J' to view.", 3f);
        }

        private void ApplyNight3()
        {
            RenderSettings.fog = true;
            RenderSettings.fogColor = fogNight3;
            RenderSettings.fogDensity = fogDensityNight3;

            if (sceneLight != null) 
            {
                sceneLight.color = lightNight3;
                sceneLight.intensity *= 0.7f; // Dim the lights
            }
            if (skyboxNight3 != null) RenderSettings.skybox = skyboxNight3;

            PlayAmbience(ambienceNight3);

            SubtitleManager.Instance?.ShowSubtitle("Night 3. The red fog rolls in. Don't let Her out.", 5f);
            SubtitleManager.Instance?.ShowSubtitle("New Journal Entry Logged. Press 'J' to view.", 3f);
        }

        private void PlayAmbience(AudioClip clip)
        {
            if (ambientSource != null && clip != null)
            {
                ambientSource.clip = clip;
                ambientSource.loop = true;
                ambientSource.Play();
            }
        }
    }
}
