using UnityEngine;
using System.Collections;
using AHeavyToll.Managers;
using AHeavyToll.Gameplay;
using AHeavyToll.Horror;
using AHeavyToll.UI;
using AHeavyToll.Data;
using AHeavyToll.VFX;

namespace AHeavyToll.Managers
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [Header("Audio Sources")]
        [SerializeField] public AudioSource musicSource;
        [SerializeField] public AudioSource sfxSource;
        [SerializeField] public AudioSource ambienceSource;
        [SerializeField] public AudioSource voiceSource;

        [Header("PSX Audio Settings")]
        [SerializeField] private bool enableBitCrush = true;
        [SerializeField] [Range(0f, 1f)] private float bitCrushAmount = 0.3f;
        [SerializeField] private float masterVolume = 1f;

        [Header("Mixer")]
        [SerializeField] private UnityEngine.Audio.AudioMixer audioMixer;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        public void PlayMusic(AudioClip clip, bool loop = true, float volume = 1f)
        {
            if (musicSource == null || clip == null) return;

            musicSource.clip = clip;
            musicSource.loop = loop;
            musicSource.volume = volume * masterVolume;
            musicSource.Play();
        }

        public void PlayAmbience(AudioClip clip, bool loop = true, float volume = 0.5f)
        {
            if (ambienceSource == null || clip == null) return;

            ambienceSource.clip = clip;
            ambienceSource.loop = loop;
            ambienceSource.volume = volume * masterVolume;
            ambienceSource.Play();
        }

        public void PlayOneShot(AudioClip clip, float volume = 1f)
        {
            if (sfxSource == null || clip == null) return;
            sfxSource.PlayOneShot(clip, volume * masterVolume);
        }

        public void PlayVoice(AudioClip clip)
        {
            if (voiceSource == null || clip == null) return;

            voiceSource.clip = clip;
            voiceSource.volume = masterVolume;
            voiceSource.Play();
        }

        public void StopMusic()
        {
            if (musicSource != null) musicSource.Stop();
        }

        public void StopAmbience()
        {
            if (ambienceSource != null) ambienceSource.Stop();
        }

        public void SetMasterVolume(float volume)
        {
            masterVolume = Mathf.Clamp01(volume);

            if (audioMixer != null)
            {
                // Convert linear 0-1 to dB
                float dB = volume > 0 ? Mathf.Log10(volume) * 20f : -80f;
                audioMixer.SetFloat("MasterVolume", dB);
            }
        }

        public void FadeOutMusic(float duration)
        {
            StartCoroutine(FadeMusicCoroutine(0f, duration));
        }

        private IEnumerator FadeMusicCoroutine(float targetVolume, float duration)
        {
            if (musicSource == null) yield break;

            float startVolume = musicSource.volume;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                musicSource.volume = Mathf.Lerp(startVolume, targetVolume, elapsed / duration);
                elapsed += Time.deltaTime;
                yield return null;
            }

            musicSource.volume = targetVolume;
            if (targetVolume <= 0f) musicSource.Stop();
        }
    }
}
