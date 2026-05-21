using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace AHeavyToll.UI
{
    public class EndingVisualController : MonoBehaviour
    {
        [Header("Components")]
        public Image background;
        public Image vignette;
        public Image glowOverlay;
        public TextMeshProUGUI titleText;
        public TextMeshProUGUI subtitleText;

        [Header("Timing")]
        public float fadeInDuration = 2f;
        public float holdDuration = 5f;
        public float fadeOutDuration = 2f;

        [Header("Glow Effect")]
        public bool enableGlowPulse = false;
        public float pulseSpeed = 1.5f;
        public float pulseMinAlpha = 0.1f;
        public float pulseMaxAlpha = 0.5f;

        [Header("Color Animation")]
        public bool enableColorShift = false;
        public Color targetColor = Color.white;
        public float colorShiftDuration = 3f;

        private Coroutine activeCoroutine;
        private bool isPlaying = false;

        private void OnEnable()
        {
            PlayEndingVisual();
        }

        public void PlayEndingVisual()
        {
            if (isPlaying) return;
            isPlaying = true;

            if (activeCoroutine != null)
                StopCoroutine(activeCoroutine);

            activeCoroutine = StartCoroutine(PlaySequence());
        }

        private IEnumerator PlaySequence()
        {
            // Reset state
            if (background != null) background.color = new Color(background.color.r, background.color.g, background.color.b, 0f);
            if (vignette != null) vignette.color = new Color(vignette.color.r, vignette.color.g, vignette.color.b, 0f);
            if (glowOverlay != null) glowOverlay.color = new Color(glowOverlay.color.r, glowOverlay.color.g, glowOverlay.color.b, 0f);
            if (titleText != null) titleText.alpha = 0f;
            if (subtitleText != null) subtitleText.alpha = 0f;

            // Fade in
            yield return StartCoroutine(FadeIn());

            // Hold with effects
            if (enableGlowPulse && glowOverlay != null)
            {
                yield return StartCoroutine(PulseGlow(holdDuration));
            }
            else
            {
                yield return new WaitForSeconds(holdDuration);
            }

            // Fade out
            yield return StartCoroutine(FadeOut());

            isPlaying = false;
        }

        private IEnumerator FadeIn()
        {
            float elapsed = 0f;
            float titleStart = fadeInDuration * 0.3f;
            float subtitleStart = fadeInDuration * 0.6f;

            while (elapsed < fadeInDuration)
            {
                float t = elapsed / fadeInDuration;

                if (background != null)
                    background.color = SetAlpha(background.color, t);
                if (vignette != null)
                    vignette.color = SetAlpha(vignette.color, t);
                if (glowOverlay != null)
                    glowOverlay.color = SetAlpha(glowOverlay.color, t);
                if (titleText != null && elapsed > titleStart)
                    titleText.alpha = Mathf.InverseLerp(titleStart, fadeInDuration, elapsed);
                if (subtitleText != null && elapsed > subtitleStart)
                    subtitleText.alpha = Mathf.InverseLerp(subtitleStart, fadeInDuration, elapsed);

                if (enableColorShift && background != null)
                {
                    float colorT = Mathf.Clamp01(elapsed / colorShiftDuration);
                    background.color = Color.Lerp(background.color, targetColor, colorT * 0.3f);
                }

                elapsed += Time.deltaTime;
                yield return null;
            }
        }

        private IEnumerator PulseGlow(float duration)
        {
            float elapsed = 0f;
            while (elapsed < duration)
            {
                float pulse = Mathf.Lerp(pulseMinAlpha, pulseMaxAlpha,
                    (Mathf.Sin(elapsed * pulseSpeed * Mathf.PI * 2f) + 1f) * 0.5f);
                if (glowOverlay != null)
                    glowOverlay.color = SetAlpha(glowOverlay.color, pulse);
                elapsed += Time.deltaTime;
                yield return null;
            }
        }

        private IEnumerator FadeOut()
        {
            float elapsed = 0f;
            while (elapsed < fadeOutDuration)
            {
                float t = 1f - (elapsed / fadeOutDuration);

                if (background != null)
                    background.color = SetAlpha(background.color, t);
                if (vignette != null)
                    vignette.color = SetAlpha(vignette.color, t);
                if (glowOverlay != null)
                    glowOverlay.color = SetAlpha(glowOverlay.color, t);
                if (titleText != null)
                    titleText.alpha = t;
                if (subtitleText != null)
                    subtitleText.alpha = t;

                elapsed += Time.deltaTime;
                yield return null;
            }
        }

        private Color SetAlpha(Color color, float alpha)
        {
            return new Color(color.r, color.g, color.b, alpha);
        }
    }
}
