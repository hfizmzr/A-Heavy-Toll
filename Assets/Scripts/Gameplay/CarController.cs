using UnityEngine;
using UnityEngine.Events;
using AHeavyToll.Managers;
using AHeavyToll.Gameplay;
using AHeavyToll.Horror;
using AHeavyToll.UI;
using AHeavyToll.Data;
using AHeavyToll.VFX;

namespace AHeavyToll.Gameplay
{
    public class CarController : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float approachSpeed = 3f;
        [SerializeField] private float exitSpeed = 8f;
        [SerializeField] private float uTurnSpeed = 5f;
        [SerializeField] private float stopDistance = 2f;
        [SerializeField] private Transform stopPoint;

        [Header("State")]
        [SerializeField] private CarState currentState = CarState.Approaching;
        [SerializeField] private bool decisionMade = false;
        [SerializeField] private bool decisionAllow = false;

        public CarData Data { get; private set; }
        public bool IsAtBooth => currentState == CarState.AtBooth;
        public CarState CurrentState => currentState;

        [HideInInspector] public UnityEvent<CarController> OnCarExited = new UnityEvent<CarController>();

        private Transform boothPoint;
        private Transform exitPoint;
        private Transform startingPoint;
        private AudioSource audioSource;
        private bool isInitialized = false;

        public enum CarState { Approaching, AtBooth, Exiting }

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
                audioSource = gameObject.AddComponent<AudioSource>();
        }

        public void Initialize(CarData data, Transform booth, Transform exit, Transform start, Transform stop)
        {
            Data = data;
            boothPoint = booth;
            exitPoint = exit;
            startingPoint = start;
            stopPoint = stop;
            currentState = CarState.Approaching;
            decisionMade = false;

            // Apply model
            if (data.carModelPrefab != null)
            {
                Instantiate(data.carModelPrefab, transform);
            }

            // Apply sound
            if (audioSource != null && data.engineSound != null)
            {
                audioSource.clip = data.engineSound;
                audioSource.loop = true;
                audioSource.spatialBlend = 1f; // 3D audio
                audioSource.Play();
            }

            isInitialized = true;
        }

        private void Update()
        {
            if (!isInitialized) return;

            switch (currentState)
            {
                case CarState.Approaching:
                    MoveTowards(boothPoint.position, approachSpeed);
                    if (Vector3.Distance(transform.position, stopPoint.position) <= stopDistance)
                    {
                        ArriveAtBooth();
                    }
                    break;

                case CarState.AtBooth:
                    // Idle at booth, wait for player decision
                    break;

                case CarState.Exiting:
                    if (!decisionAllow)
                    {
                        Vector3 uTurnTarget = startingPoint.position;
                        MoveTowards(uTurnTarget, uTurnSpeed);
                        if (Vector3.Distance(transform.position, uTurnTarget) < 2f)
                        {
                            CompleteExit();
                        }
                    }
                    else
                    {
                        bar_open.Instance?.OpenBar();
                        MoveTowards(exitPoint.position, exitSpeed);
                        if (Vector3.Distance(transform.position, exitPoint.position) < 1f)
                        {
                            CompleteExit();
                            bar_open.Instance?.CloseBar();
                        }
                    }
                    break;
            }
        }

        private void MoveTowards(Vector3 target, float speed)
        {
            Vector3 direction = (target - transform.position);
            if (direction.sqrMagnitude < 0.0001f) return;
            direction.Normalize();
            transform.position += direction * speed * Time.deltaTime;
            transform.rotation = Quaternion.LookRotation(direction);
        }

        private void ArriveAtBooth()
        {
            currentState = CarState.AtBooth;

            // Play radio/static sound for supernatural entities
            if (Data.radioStaticOverride != null && audioSource != null)
            {
                audioSource.clip = Data.radioStaticOverride;
                audioSource.Play();
            }

            // Notify systems
            VettingSystem.Instance?.BeginVetting(this);
            SubtitleManager.Instance?.ShowSubtitle($"Incoming: {Data.driverName}", 2f);
        }

        public void SetDecision(bool allow)
        {
            decisionMade = true;
            decisionAllow = allow;
        }

        public void DriveAway()
        {
            currentState = CarState.Exiting;

            if (audioSource != null && Data.engineSound != null)
            {
                audioSource.clip = Data.engineSound;
                audioSource.Play();
            }
        }

        private void CompleteExit()
        {
            OnCarExited?.Invoke(this);
        }

        private void OnDestroy()
        {
            OnCarExited?.RemoveAllListeners();
        }
    }
}
