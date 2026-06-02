using UnityEngine;
using AHeavyToll.Managers;
using AHeavyToll.Gameplay;
using AHeavyToll.Horror;
using AHeavyToll.UI;
using AHeavyToll.Data;
using AHeavyToll.VFX;

namespace AHeavyToll.Gameplay
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float moveSpeed = 2f;
        [SerializeField] private float lookSpeed = 2f;
        [SerializeField] private float lookLimitX = 60f;
        [SerializeField] private float lookLimitY = 45f;

        [Header("Booth Constraints")]
        [SerializeField] private Transform boothCenter;
        [SerializeField] private float maxDistanceFromCenter = 2f;

        [Header("Camera")]
        [SerializeField] private Camera playerCamera;
        [SerializeField] private Transform cameraHolder;

        [Header("Head Bob")]
        [SerializeField] private bool enableHeadBob = true;
        [SerializeField] private float bobFrequency = 1.5f;
        [SerializeField] private float bobAmplitude = 0.05f;

        private CharacterController characterController;
        private float rotationX = 0f;
        private float rotationY = 0f;
        private Vector3 originalCameraPos;
        private float bobTimer = 0f;

        private void Awake()
        {
            characterController = GetComponent<CharacterController>();
            if (playerCamera == null) playerCamera = Camera.main;
            originalCameraPos = playerCamera.transform.localPosition;
        }

        private void Start()
        {
        }

        private void Update()
        {
            if (GameManager.Instance != null && 
                GameManager.Instance.CurrentState != GameState.Playing)
            {
                return;
            }

            HandleLook();
            HandleMovement();

            if (enableHeadBob)
                ApplyHeadBob();

            if (boothCenter != null)
            {
                Vector3 offset = transform.position - boothCenter.position;
                if (offset.magnitude > maxDistanceFromCenter)
                {
                    transform.position = boothCenter.position + offset.normalized * maxDistanceFromCenter;
                }
            }
        }

        private void HandleLook()
        {
            float mouseX = Input.GetAxis("Mouse X") * lookSpeed;
            float mouseY = Input.GetAxis("Mouse Y") * lookSpeed;

            rotationX -= mouseY;
            rotationX = Mathf.Clamp(rotationX, -lookLimitX, lookLimitX);

            rotationY += mouseX;
            // rotationY = Mathf.Clamp(rotationY, -lookLimitY, lookLimitY);

            if (cameraHolder != null)
            {
                cameraHolder.localRotation = Quaternion.Euler(rotationX, rotationY, 0f);
            }
            else
            {
                playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0f, 0f);
                transform.rotation *= Quaternion.Euler(0f, mouseX, 0f);
            }
        }

        private void HandleMovement()
        {
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            // Use camera direction for movement so walking aligns with where you're looking
            Vector3 forward = cameraHolder != null ? cameraHolder.forward : transform.forward;
            Vector3 right = cameraHolder != null ? cameraHolder.right : transform.right;

            // Flatten to the ground plane so looking up/down doesn't affect movement
            forward.y = 0f;
            right.y = 0f;
            forward.Normalize();
            right.Normalize();

            Vector3 move = right * horizontal + forward * vertical;
            characterController.Move(move * moveSpeed * Time.deltaTime);
        }

        private void ApplyHeadBob()
        {
            if (Mathf.Abs(Input.GetAxis("Horizontal")) > 0.1f || Mathf.Abs(Input.GetAxis("Vertical")) > 0.1f)
            {
                bobTimer += Time.deltaTime * bobFrequency;
                float bobOffset = Mathf.Sin(bobTimer) * bobAmplitude;
                playerCamera.transform.localPosition = originalCameraPos + new Vector3(0, bobOffset, 0);
            }
            else
            {
                bobTimer = 0f;
                playerCamera.transform.localPosition = Vector3.Lerp(
                    playerCamera.transform.localPosition, 
                    originalCameraPos, 
                    Time.deltaTime * 5f
                );
            }
        }

        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            if (hit.collider.GetComponentInParent<CarController>() != null)
            {
                Vector3 pushDirection = (transform.position - hit.collider.transform.position);
                pushDirection.y = 0;
                pushDirection.Normalize();
                characterController.Move(pushDirection * 0.5f);
            }
        }
    }
}
