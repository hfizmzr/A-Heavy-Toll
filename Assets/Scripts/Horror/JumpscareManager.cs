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
        [SerializeField] private GameObject jumpscareCanvas;
        [SerializeField] private Image jumpscareImage;
        [SerializeField] private Animator jumpscareAnimator;
        [SerializeField] private AudioSource jumpscareAudio;

        [Header("Jumpscare Clips")]
        [SerializeField] private Sprite badEndingSprite;
        [SerializeField] private Sprite hiddenEndingSprite;
        [SerializeField] private AudioClip badEndingScream;
        [SerializeField] private AudioClip hiddenEndingAmbience;

        [Header("Timing")]
        [SerializeField] private float jumpscareDuration = 3f;
        [SerializeField] private float fadeInDuration = 0.1f;
        [SerializeField] private float fadeOutDuration = 2f;

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
