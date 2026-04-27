using UnityEngine;
using AHeavyToll.Managers;
using AHeavyToll.Gameplay;
using AHeavyToll.Horror;
using AHeavyToll.UI;
using AHeavyToll.Data;
using AHeavyToll.VFX;

namespace AHeavyToll.Gameplay
{
    public class CommUnit : MonoBehaviour, IInteractable
    {
        [Header("Audio")]
        [SerializeField] private AudioClip buttonPressSound;
        [SerializeField] private AudioClip staticSound;
        [SerializeField] private AudioSource audioSource;

        [Header("Visual")]
        [SerializeField] private Animator animator;
        [SerializeField] private string activateTrigger = "Activate";

        [Header("State")]
        [SerializeField] private bool isOnCooldown = false;
        [SerializeField] private float cooldownTime = 2f;

        public string GetPromptText() => "Use Comm Unit";

        public void Interact()
        {
            if (isOnCooldown) return;
            if (CarQueueManager.Instance?.CurrentCar != null) return; // Already processing

            PlayActivateAnimation();

            // Queue next car if available
            // This is handled by CarQueueManager automatically, but we can force a check
            SubtitleManager.Instance?.ShowSubtitle("Comm unit activated. Awaiting next vehicle...", 2f);

            StartCoroutine(Cooldown());
        }

        private void PlayActivateAnimation()
        {
            if (audioSource != null && buttonPressSound != null)
                audioSource.PlayOneShot(buttonPressSound);

            if (animator != null)
                animator.SetTrigger(activateTrigger);
        }

        private System.Collections.IEnumerator Cooldown()
        {
            isOnCooldown = true;
            yield return new WaitForSeconds(cooldownTime);
            isOnCooldown = false;
        }
    }
}
