using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using AHeavyToll.Managers;
using AHeavyToll.Gameplay;
using AHeavyToll.Horror;
using AHeavyToll.UI;
using AHeavyToll.Data;
using AHeavyToll.VFX;

namespace AHeavyToll.UI
{
    public class SubtitleManager : MonoBehaviour
    {
        public static SubtitleManager Instance { get; private set; }

        [Header("UI References")]
        [SerializeField] private TextMeshProUGUI subtitleText;
        [SerializeField] private CanvasGroup subtitleCanvasGroup;
        [SerializeField] private GameObject subtitlePanel;

        [Header("Settings")]
        [SerializeField] private float fadeInTime = 0.2f;
        [SerializeField] private float fadeOutTime = 0.5f;
        [SerializeField] private int maxConcurrentSubtitles = 3;

        private Queue<(string text, float duration)> subtitleQueue = new Queue<(string, float)>();
        private Coroutine currentSubtitleCoroutine;
        private bool isDisplaying = false;

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
            if (subtitlePanel != null) subtitlePanel.SetActive(false);
            if (subtitleCanvasGroup != null) subtitleCanvasGroup.alpha = 0f;
        }

        public void ShowSubtitle(string text, float duration)
        {
            if (string.IsNullOrEmpty(text)) return;

            subtitleQueue.Enqueue((text, duration));

            if (!isDisplaying)
            {
                currentSubtitleCoroutine = StartCoroutine(ProcessSubtitleQueue());
            }
        }

        public void ClearSubtitles()
        {
            subtitleQueue.Clear();
            if (currentSubtitleCoroutine != null)
            {
                StopCoroutine(currentSubtitleCoroutine);
                currentSubtitleCoroutine = null;
            }
            isDisplaying = false;
            if (subtitlePanel != null) subtitlePanel.SetActive(false);
        }

        private IEnumerator ProcessSubtitleQueue()
        {
            isDisplaying = true;

            while (subtitleQueue.Count > 0)
            {
                var (text, duration) = subtitleQueue.Dequeue();

                if (subtitlePanel != null) subtitlePanel.SetActive(true);
                if (subtitleText != null) subtitleText.text = text;

                // Fade in
                yield return StartCoroutine(FadeSubtitle(0f, 1f, fadeInTime));

                // Hold
                yield return new WaitForSeconds(duration);

                // Fade out
                yield return StartCoroutine(FadeSubtitle(1f, 0f, fadeOutTime));
            }

            if (subtitlePanel != null) subtitlePanel.SetActive(false);
            isDisplaying = false;
            currentSubtitleCoroutine = null;
        }

        private IEnumerator FadeSubtitle(float from, float to, float time)
        {
            if (subtitleCanvasGroup == null) yield break;

            float elapsed = 0f;
            while (elapsed < time)
            {
                subtitleCanvasGroup.alpha = Mathf.Lerp(from, to, elapsed / time);
                elapsed += Time.deltaTime;
                yield return null;
            }
            subtitleCanvasGroup.alpha = to;
        }
    }
}
