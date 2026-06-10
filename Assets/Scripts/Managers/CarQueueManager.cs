using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AHeavyToll.Managers;
using AHeavyToll.Gameplay;
using AHeavyToll.Horror;
using AHeavyToll.UI;
using AHeavyToll.Data;
using AHeavyToll.VFX;

namespace AHeavyToll.Managers
{
    public class CarQueueManager : MonoBehaviour
    {
        public static CarQueueManager Instance { get; private set; }

        [Header("Queue Settings")]
        [SerializeField] private int carsPerNight = 8;
        [SerializeField] private float timeBetweenCars = 15f;
        [SerializeField] private Transform spawnPoint;
        [SerializeField] private Transform boothApproachPoint;
        [SerializeField] private Transform exitPoint;
        [SerializeField] private Transform stopPoint;
        [SerializeField] private CarData specialCarData;

        [Header("Car Database")]
        public List<CarData> allCarData = new List<CarData>();

        [Header("Prefabs")]
        [SerializeField] private GameObject carPrefab; // Base car with CarController

        [Header("State")]
        [SerializeField] private List<CarController> activeCars = new List<CarController>();
        [SerializeField] private CarController currentCar = null;
        [SerializeField] private int carsProcessedThisNight = 0;
        [SerializeField] private bool isQueueActive = false;

        // private HashSet<CarData> _usedCars = new HashSet<CarData>();
        private HashSet<CarData> usedCarsThisNight = new HashSet<CarData>();
        private bool specialCarSpawned = false;

        public CarController CurrentCar => currentCar;
        public bool IsProcessingCar => currentCar != null && currentCar.IsAtBooth;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        public void BeginQueueForDay(Day day)
        {
            carsProcessedThisNight = 0;
            isQueueActive = true;
            usedCarsThisNight.Clear();
            StartCoroutine(RunQueue(allCarData));
        }

        private IEnumerator RunQueue(List<CarData> pool)
        {
            while (carsProcessedThisNight < carsPerNight && isQueueActive)
            {
                if (currentCar == null)
                {
                    // Random mid-game jumpscare during idle gap
                    if (carsProcessedThisNight > 0)
                        JumpscareManager.Instance?.TryPlayMidGameJumpscare();

                    yield return new WaitForSeconds(timeBetweenCars);

                    if (!isQueueActive) yield break;

                    SpawnNextCar(pool);
                }
                yield return null;
            }

            // Night complete
            yield return new WaitForSeconds(3f);
            GameManager.Instance?.AdvanceToNextDay();
        }

        private void SpawnNextCar(List<CarData> pool)
        {
            if (pool.Count == 0)
            {
                Debug.LogWarning("CarQueueManager: allCarData is empty. Assign car assets in the Inspector.");
                return;
            }

            if (boothApproachPoint == null || exitPoint == null || spawnPoint == null)
            {
                Debug.LogError("CarQueueManager: boothApproachPoint, exitPoint, or spawnPoint is not assigned in the Inspector.");
                return;
            }

            // Filter by night availability
            Day currentDay = GameManager.Instance != null ? GameManager.Instance.CurrentDay : Day.Night1;
            List<CarData> validCars = pool.FindAll(c => c != null && (
                (currentDay == Day.Night1 && c.appearsNight1) ||
                (currentDay == Day.Night2 && c.appearsNight2) ||
                (currentDay == Day.Night3 && c.appearsNight3)
            ));

            if (validCars.Count == 0)
            {
                Debug.LogWarning($"CarQueueManager: No cars match night filter for {GameManager.Instance?.CurrentDay}. Check appearsNight flags on CarData assets.");
                return;
            }

            // Prefer unused cars; fall back to repeats if pool exhausted
            // List<CarData> freshCars = validCars.FindAll(c => !_usedCars.Contains(c));
            // CarData selectedData = freshCars.Count > 0
            //     ? freshCars[Random.Range(0, freshCars.Count)]
            //     : validCars[Random.Range(0, validCars.Count)];

            // _usedCars.Add(selectedData);

            List<CarData> availableCars = validCars.FindAll(c => !usedCarsThisNight.Contains(c));
            CarData selectedData;

            if (currentDay == Day.Night3)
            {
                // Reference to your special car
                CarData specialCar = specialCarData;

                // If it's the last car of the night and special car hasn't spawned yet → force it
                if (!specialCarSpawned && carsProcessedThisNight == carsPerNight - 1)
                {
                    selectedData = specialCar;
                    specialCarSpawned = true;
                }
                else
                {
                    // Otherwise pick randomly
                    selectedData = availableCars.Count > 0
                        ? availableCars[Random.Range(0, availableCars.Count)]
                        : validCars[Random.Range(0, validCars.Count)];

                    // Mark if the special car was chosen randomly
                    if (selectedData == specialCar)
                        specialCarSpawned = true;
                }
            }
            else
            {
                // Normal random selection for other nights
                selectedData = availableCars.Count > 0
                    ? availableCars[Random.Range(0, availableCars.Count)]
                    : validCars[Random.Range(0, validCars.Count)];
            }
            usedCarsThisNight.Add(selectedData);

            GameObject carObj = Instantiate(carPrefab, spawnPoint.position, spawnPoint.rotation);
            CarController car = carObj.GetComponent<CarController>();

            if (car != null)
            {
                car.Initialize(selectedData, boothApproachPoint, exitPoint, spawnPoint, stopPoint);
                activeCars.Add(car);
                currentCar = car;

                car.OnCarExited.AddListener(HandleCarExited);
            }

            // Notify UI
            UIManager.Instance?.ShowIncomingCar();
        }

        public void ProcessDecision(bool allowThrough)
        {
            if (currentCar == null) return;

            currentCar.SetDecision(allowThrough);
            GameManager.Instance?.RegisterCarDecision(
                allowThrough, 
                currentCar.Data.isMalevolent, 
                currentCar.Data.isHer
            );

            // Check for horror trigger
            if (currentCar.Data.triggersHorrorEvent && allowThrough)
            {
                HorrorEventManager.Instance?.TriggerEvent(currentCar.Data.horrorEventId);
            }

            carsProcessedThisNight++;
            JournalSystem.Instance?.UnlockEntriesAfterCarCount(
                GameManager.Instance?.CurrentNightNumber ?? 1,
                carsProcessedThisNight);
            currentCar.DriveAway();
        }

        private void HandleCarExited(CarController car)
        {
            activeCars.Remove(car);
            if (currentCar == car)
            {
                currentCar = null;
            }
            Destroy(car.gameObject, 2f);
        }

        public void StopQueue()
        {
            isQueueActive = false;
            StopAllCoroutines();
        }
    }
}
