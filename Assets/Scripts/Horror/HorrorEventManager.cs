using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AHeavyToll.Managers;
using AHeavyToll.Gameplay;
using AHeavyToll.Horror;
using AHeavyToll.UI;
using AHeavyToll.Data;
using AHeavyToll.VFX;

namespace AHeavyToll.Horror
{
    [System.Serializable]
    public class HorrorEvent
    {
        public string eventId;
        public string eventName;
        [TextArea(2, 4)] public string description;

        [Header("Visual")]
        public GameObject visualEffectPrefab;
        public Transform spawnLocation;
        public float effectDuration = 5f;

        [Header("Audio")]
        public AudioClip soundEffect;
        [Range(0f, 1f)] public float volume = 1f;

        [Header("Booth Effects")]
        public bool flickerLights = false;
        public float flickerDuration = 2f;
        public bool spawnBloodWriting = false;
        public bool spawnScratches = false;
        public bool breakGlass = false;

        [Header("UI")]
        [TextArea(2, 3)] public string subtitleText;
        public float subtitleDuration = 3f;
        public bool screenShake = false;
        public float shakeIntensity = 0.5f;
        public float shakeDuration = 0.5f;

        [Header("Night Availability")]
        public bool canTriggerNight1 = false;
        public bool canTriggerNight2 = true;
        public bool canTriggerNight3 = true;

        [Header("Chance")]
        [Range(0f, 1f)] public float triggerChance = 0.3f;
    }

    public class HorrorEventManager : MonoBehaviour
    {
        public static HorrorEventManager Instance { get; private set; }

        [Header("Event Database")]
        public List<HorrorEvent> horrorEvents = new List<HorrorEvent>();

        [Header("Booth References")]
        public Light boothLight;
        public Transform bloodWritingParent;
        public Transform scratchesParent;
        public Transform glassParent;
        public List<GameObject> bloodWritingPrefabs = new List<GameObject>();
        public List<GameObject> scratchPrefabs = new List<GameObject>();
        public List<GameObject> brokenGlassPrefabs = new List<GameObject>();

        [Header("Camera")]
        public Camera mainCamera;

        [Header("State")]
        [SerializeField] private List<string> triggeredEvents = new List<string>();
        [SerializeField] private bool isEventPlaying = false;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        public void TriggerEvent(string eventId)
        {
            HorrorEvent evt = horrorEvents.Find(e => e.eventId == eventId);
            if (evt != null)
            {
                StartCoroutine(PlayEvent(evt));
            }
        }

        public void TryRandomEvent()
        {
            Day currentDay = GameManager.Instance.CurrentDay;

            List<HorrorEvent> validEvents = horrorEvents.FindAll(e =>
            {
                bool validDay = currentDay switch
                {
                    Day.Night1 => e.canTriggerNight1,
                    Day.Night2 => e.canTriggerNight2,
                    Day.Night3 => e.canTriggerNight3,
                    _ => false
                };
                return validDay && !triggeredEvents.Contains(e.eventId) && Random.value <= e.triggerChance;
            });

            if (validEvents.Count > 0)
            {
                HorrorEvent selected = validEvents[Random.Range(0, validEvents.Count)];
                StartCoroutine(PlayEvent(selected));
            }
        }

        private IEnumerator PlayEvent(HorrorEvent evt)
        {
            if (isEventPlaying) yield break;
            isEventPlaying = true;
            triggeredEvents.Add(evt.eventId);

            Debug.Log($"[HorrorEvent] Triggering: {evt.eventName}");

            // Subtitle
            if (!string.IsNullOrEmpty(evt.subtitleText))
            {
                SubtitleManager.Instance?.ShowSubtitle(evt.subtitleText, evt.subtitleDuration);
            }

            // Audio
            if (evt.soundEffect != null)
            {
                AudioManager.Instance?.PlayOneShot(evt.soundEffect, evt.volume);
            }

            // Screen shake
            if (evt.screenShake)
            {
                StartCoroutine(ShakeCamera(evt.shakeIntensity, evt.shakeDuration));
            }

            // Light flicker
            if (evt.flickerLights && boothLight != null)
            {
                StartCoroutine(FlickerLights(evt.flickerDuration));
            }

            // Spawn effects
            if (evt.visualEffectPrefab != null && evt.spawnLocation != null)
            {
                GameObject fx = Instantiate(evt.visualEffectPrefab, evt.spawnLocation);
                Destroy(fx, evt.effectDuration);
            }

            // Booth-specific effects
            if (evt.spawnBloodWriting) SpawnRandomEffect(bloodWritingPrefabs, bloodWritingParent);
            if (evt.spawnScratches) SpawnRandomEffect(scratchPrefabs, scratchesParent);
            if (evt.breakGlass) SpawnRandomEffect(brokenGlassPrefabs, glassParent);

            yield return new WaitForSeconds(evt.effectDuration);

            isEventPlaying = false;
        }

        private void SpawnRandomEffect(List<GameObject> prefabs, Transform parent)
        {
            if (prefabs.Count == 0 || parent == null) return;

            GameObject prefab = prefabs[Random.Range(0, prefabs.Count)];
            Vector3 randomPos = parent.position + new Vector3(
                Random.Range(-1f, 1f), 
                Random.Range(-0.5f, 0.5f), 
                Random.Range(-0.2f, 0.2f)
            );

            GameObject spawned = Instantiate(prefab, randomPos, Random.rotation, parent);
            spawned.transform.localScale *= Random.Range(0.8f, 1.2f);
        }

        private IEnumerator FlickerLights(float duration)
        {
            if (boothLight == null) yield break;

            float elapsed = 0f;
            float originalIntensity = boothLight.intensity;

            while (elapsed < duration)
            {
                boothLight.intensity = Random.value > 0.5f ? originalIntensity : originalIntensity * 0.1f;
                yield return new WaitForSeconds(Random.Range(0.05f, 0.2f));
                elapsed += Time.deltaTime;
            }

            boothLight.intensity = originalIntensity;
        }

        private IEnumerator ShakeCamera(float intensity, float duration)
        {
            if (mainCamera == null) yield break;

            Vector3 originalPos = mainCamera.transform.localPosition;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                float x = Random.Range(-1f, 1f) * intensity;
                float y = Random.Range(-1f, 1f) * intensity;
                mainCamera.transform.localPosition = originalPos + new Vector3(x, y, 0);
                elapsed += Time.deltaTime;
                yield return null;
            }

            mainCamera.transform.localPosition = originalPos;
        }
    }
}
