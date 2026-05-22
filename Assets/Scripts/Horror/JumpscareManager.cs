using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using AHeavyToll.Managers;
using AHeavyToll.Gameplay;
using AHeavyToll.Horror;
using AHeavyToll.UI;
using AHeavyToll.Data;
using AHeavyToll.VFX;

namespace AHeavyToll.Horror
{
    [System.Serializable]
    public class MidGameJumpscareConfig
    {
        public Sprite[] sprites;
        public AudioClip[] audioClips;
        [Range(0f, 1f)] public float chance = 0.15f;
    }

    public class JumpscareManager : MonoBehaviour
    {
        public static JumpscareManager Instance { get; private set; }

        [Header("Jumpscare Elements")]
        public GameObject jumpscareCanvas;
        public Image jumpscareImage;
        public Animator jumpscareAnimator;
        public AudioSource jumpscareAudio;

        [Header("Jumpscare Clips")]
        public Sprite badEndingSprite;
        public Sprite hiddenEndingSprite;
        public AudioClip badEndingScream;
        public AudioClip hiddenEndingAmbience;

        [Header("Timing")]
        [SerializeField] private float jumpscareDuration = 3f;
        [SerializeField] private float fadeInDuration = 0.1f;
        [SerializeField] private float fadeOutDuration = 2f;

        [Header("Mid-Game Jumpscare")]
        [SerializeField] private float midGameDuration = 1f;
        [SerializeField] private float midGameFadeIn = 0.05f;
        [SerializeField] private float midGameFadeOut = 0.5f;

        [Header("Mid-Game Jumpscare Per Night")]
        public MidGameJumpscareConfig night1Config;
        public MidGameJumpscareConfig night2Config;
        public MidGameJumpscareConfig night3Config;

        private GraphicRaycaster _canvasRaycaster;
        private bool _raycasterWasEnabled;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            if (jumpscareCanvas != null)
                _canvasRaycaster = jumpscareCanvas.GetComponent<GraphicRaycaster>();
        }

        private void Start()
        {
            if (jumpscareCanvas != null)
                jumpscareCanvas.SetActive(false);
        }

        public void PlayBadEndingJumpscare()
        {
            StartCoroutine(PlayJumpscareSequence(badEndingSprite, badEndingScream, false));
        }

        public void PlayHiddenEndingJumpscare()
        {
            StartCoroutine(PlayJumpscareSequence(hiddenEndingSprite, hiddenEndingAmbience, true));
        }

        public bool TryPlayMidGameJumpscare()
        {
            MidGameJumpscareConfig config = GetCurrentNightConfig();
            if (config == null) return false;
            if (config.sprites == null || config.sprites.Length == 0) return false;
            if (Random.value >= config.chance) return false;

            Sprite sprite = config.sprites[Random.Range(0, config.sprites.Length)];
            AudioClip clip = null;
            if (config.audioClips != null && config.audioClips.Length > 0)
                clip = config.audioClips[Random.Range(0, config.audioClips.Length)];

            StartCoroutine(PlayMidGameSequence(sprite, clip));
            return true;
        }

        private MidGameJumpscareConfig GetCurrentNightConfig()
        {
            if (GameManager.Instance == null) return null;
            return GameManager.Instance.CurrentDay switch
            {
                Day.Night1 => night1Config,
                Day.Night2 => night2Config,
                Day.Night3 => night3Config,
                _ => null
            };
        }

        private IEnumerator PlayMidGameSequence(Sprite image, AudioClip sound)
        {
            if (_canvasRaycaster != null)
            {
                _raycasterWasEnabled = _canvasRaycaster.enabled;
                _canvasRaycaster.enabled = false;
            }

            if (jumpscareCanvas != null) jumpscareCanvas.SetActive(true);
            if (jumpscareImage != null)
            {
                jumpscareImage.sprite = image;
                jumpscareImage.color = new Color(1, 1, 1, 0);
            }

            // Play sound
            if (jumpscareAudio != null && sound != null)
            {
                jumpscareAudio.clip = sound;
                jumpscareAudio.Play();
            }

            // Fade in
            float elapsed = 0f;
            while (elapsed < midGameFadeIn)
            {
                float alpha = Mathf.Lerp(0f, 1f, elapsed / midGameFadeIn);
                if (jumpscareImage != null) jumpscareImage.color = new Color(1, 1, 1, alpha);
                elapsed += Time.deltaTime;
                yield return null;
            }
            if (jumpscareImage != null) jumpscareImage.color = Color.white;

            // Hold
            yield return new WaitForSeconds(midGameDuration);

            // Fade out
            elapsed = 0f;
            while (elapsed < midGameFadeOut)
            {
                float alpha = Mathf.Lerp(1f, 0f, elapsed / midGameFadeOut);
                if (jumpscareImage != null) jumpscareImage.color = new Color(1, 1, 1, alpha);
                elapsed += Time.deltaTime;
                yield return null;
            }

            if (jumpscareCanvas != null) jumpscareCanvas.SetActive(false);

            if (_canvasRaycaster != null)
                _canvasRaycaster.enabled = _raycasterWasEnabled;
        }

        private IEnumerator PlayJumpscareSequence(Sprite image, AudioClip sound, bool slowFade)
        {
            GameManager.Instance.CurrentState = GameState.Jumpscare;

            if (jumpscareCanvas != null) jumpscareCanvas.SetActive(true);
            if (jumpscareImage != null) 
            {
                jumpscareImage.sprite = image;
                jumpscareImage.color = new Color(1, 1, 1, 0);
            }

            // Play sound
            if (jumpscareAudio != null && sound != null)
            {
                jumpscareAudio.clip = sound;
                jumpscareAudio.Play();
            }

            // Flash/fade in
            float elapsed = 0f;
            while (elapsed < fadeInDuration)
            {
                float alpha = Mathf.Lerp(0f, 1f, elapsed / fadeInDuration);
                if (jumpscareImage != null) jumpscareImage.color = new Color(1, 1, 1, alpha);
                elapsed += Time.deltaTime;
                yield return null;
            }
            if (jumpscareImage != null) jumpscareImage.color = Color.white;

            // Hold
            yield return new WaitForSeconds(jumpscareDuration);

            // Fade out
            float fadeTime = slowFade ? fadeOutDuration * 2f : fadeOutDuration;
            elapsed = 0f;
            while (elapsed < fadeTime)
            {
                float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeTime);
                if (jumpscareImage != null) jumpscareImage.color = new Color(1, 1, 1, alpha);
                elapsed += Time.deltaTime;
                yield return null;
            }

            if (jumpscareCanvas != null) jumpscareCanvas.SetActive(false);
        }
    }
}
