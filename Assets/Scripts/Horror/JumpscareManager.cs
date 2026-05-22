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
        public Sprite[] midGameSprites;
        public AudioClip[] midGameAudioClips;
        [SerializeField] private float midGameDuration = 1f;
        [SerializeField] private float midGameFadeIn = 0.05f;
        [SerializeField] private float midGameFadeOut = 0.5f;

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

        public void PlayMidGameJumpscare()
        {
            if (midGameSprites == null || midGameSprites.Length == 0) return;

            Sprite sprite = midGameSprites[Random.Range(0, midGameSprites.Length)];
            AudioClip clip = null;
            if (midGameAudioClips != null && midGameAudioClips.Length > 0)
                clip = midGameAudioClips[Random.Range(0, midGameAudioClips.Length)];

            StartCoroutine(PlayMidGameSequence(sprite, clip));
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
