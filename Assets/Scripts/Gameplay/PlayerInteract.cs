using UnityEngine;
using AHeavyToll.Managers;
using AHeavyToll.Gameplay;
using AHeavyToll.Horror;
using AHeavyToll.UI;
using AHeavyToll.Data;
using AHeavyToll.VFX;

namespace AHeavyToll.Gameplay
{
    public class PlayerInteract : MonoBehaviour
    {
        [Header("Interaction")]
        [SerializeField] private float interactRange = 3f;
        [SerializeField] private LayerMask interactLayer;
        [SerializeField] private Transform cameraTransform;

        [Header("UI")]
        [SerializeField] private GameObject interactPrompt;
        [SerializeField] private TMPro.TextMeshProUGUI promptText;

        [Header("Comm Unit")]
        [SerializeField] private Transform commUnit;
        [SerializeField] private AudioClip commStatic;

        private IInteractable currentInteractable;

        private void Update()
        {
            if (GameManager.Instance != null && GameManager.Instance.CurrentState != GameState.Playing)
                return;

            CheckInteraction();

            if (Input.GetKeyDown(KeyCode.E) && currentInteractable != null)
            {
                currentInteractable.Interact();
            }
        }

        private void CheckInteraction()
        {
            Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);

            if (Physics.Raycast(ray, out RaycastHit hit, interactRange, interactLayer))
            {
                Debug.Log("Hit object: " + hit.collider.name);
                IInteractable interactable = hit.collider.GetComponentInParent<IInteractable>();

                if (interactable != null)
                {
                    Debug.Log("Found interactable!");
                    currentInteractable = interactable;
                    ShowPrompt(interactable.GetPromptText());
                    return;
                }else{
                    Debug.Log("No interactable component found on hit object.");
                }
            }else{
                Debug.Log("No object hit by raycast.");
            }

            currentInteractable = null;
            HidePrompt();
        }

        private void ShowPrompt(string text)
        {
            if (interactPrompt != null) interactPrompt.SetActive(true);
            if (promptText != null) promptText.text = $"[E] {text}";
        }

        private void HidePrompt()
        {
            if (interactPrompt != null) interactPrompt.SetActive(false);
        }

        private void OnDrawGizmos()
        {
            if (cameraTransform != null)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawRay(cameraTransform.position, cameraTransform.forward * interactRange);
            }
        }
    }

    public interface IInteractable
    {
        string GetPromptText();
        void Interact();
        
    }
}
