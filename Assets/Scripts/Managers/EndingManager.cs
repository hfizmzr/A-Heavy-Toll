using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using AHeavyToll.Managers;
using AHeavyToll.Gameplay;
using AHeavyToll.Horror;
using AHeavyToll.UI;
using AHeavyToll.Data;
using AHeavyToll.VFX;

namespace AHeavyToll.Managers
{
    public class EndingManager : MonoBehaviour
    {
        public static EndingManager Instance { get; private set; }

        [Header("Ending Sequences")]
        [SerializeField] private float endingDelay = 2f;
        [SerializeField] private float endingDuration = 10f;

        [Header("Audio")]
        [SerializeField] private AudioClip goodEndingMusic;
        [SerializeField] private AudioClip badEndingMusic;
        [SerializeField] private AudioClip hiddenEndingMusic;
        [SerializeField] private AudioClip firedEndingMusic;

        [Header("Visual")]
        public GameObject goodEndingVisuals;
        public GameObject badEndingVisuals;
        public GameObject hiddenEndingVisuals;
        public GameObject firedEndingVisuals;

        private bool endingPlaying = false;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        public void PlayEnding(EndingType ending)
        {
            if (endingPlaying) return;
            endingPlaying = true;

            StartCoroutine(EndingSequence(ending));
        }

        private IEnumerator EndingSequence(EndingType ending)
        {
            yield return new WaitForSeconds(endingDelay);

            // Stop all gameplay systems
            CarQueueManager.Instance?.StopQueue();

            switch (ending)
            {
                case EndingType.Good:
                    yield return StartCoroutine(GoodEnding());
                    break;
                case EndingType.Bad:
                    yield return StartCoroutine(BadEnding());
                    break;
                case EndingType.Hidden:
                    yield return StartCoroutine(HiddenEnding());
                    break;
                case EndingType.Fired:
                    yield return StartCoroutine(FiredEnding());
                    break;
            }

            yield return new WaitForSeconds(endingDuration);

            UIManager.Instance?.ShowGameOver(ending);
        }

        private IEnumerator GoodEnding()
        {
            Debug.Log("[EndingManager] Playing Good Ending");

            SubtitleManager.Instance?.ShowSubtitle("The sun rises. You survived.", 4f);

            if (goodEndingVisuals != null) goodEndingVisuals.SetActive(true);
            AudioManager.Instance?.PlayMusic(goodEndingMusic);

            // Fade in light
            // This would connect to your day cycle to slowly brighten

            yield return null;
        }

        private IEnumerator BadEnding()
        {
            Debug.Log("[EndingManager] Playing Bad Ending");

            SubtitleManager.Instance?.ShowSubtitle("You shouldn't have let them through.", 3f);

            // Jumpscare first
            JumpscareManager.Instance?.PlayBadEndingJumpscare();

            yield return new WaitForSeconds(3f);

            if (badEndingVisuals != null) badEndingVisuals.SetActive(true);
            AudioManager.Instance?.PlayMusic(badEndingMusic);

            // Trigger final horror events
            HorrorEventManager.Instance?.TriggerEvent("final_bad_ending");
        }

        private IEnumerator HiddenEnding()
        {
            Debug.Log("[EndingManager] Playing Hidden Ending");

            SubtitleManager.Instance?.ShowSubtitle("She is free. The road is Hers now.", 4f);

            JumpscareManager.Instance?.PlayHiddenEndingJumpscare();

            yield return new WaitForSeconds(4f);

            if (hiddenEndingVisuals != null) hiddenEndingVisuals.SetActive(true);
            AudioManager.Instance?.PlayMusic(hiddenEndingMusic);

            // Maximum horror
            HorrorEventManager.Instance?.TriggerEvent("final_hidden_ending");

            // Red fog intensifies
            RenderSettings.fogDensity = 0.15f;
        }

        private IEnumerator FiredEnding()
        {
            Debug.Log("[EndingManager] Playing Fired Ending");

            SubtitleManager.Instance?.ShowSubtitle("You stopped them all. They're not happy about it.", 4f);

            yield return new WaitForSeconds(3f);

            if (firedEndingVisuals != null) firedEndingVisuals.SetActive(true);
            AudioManager.Instance?.PlayMusic(firedEndingMusic);
        }
    }
}
