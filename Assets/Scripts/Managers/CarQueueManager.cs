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

        [Header("Car Database")]
        public List<CarData> allCarData = new List<CarData>();
        public List<CarData> night1Pool = new List<CarData>();
        public List<CarData> night2Pool = new List<CarData>();
        public List<CarData> night3Pool = new List<CarData>();

        [Header("Prefabs")]
        [SerializeField] private GameObject carPrefab; // Base car with CarController

        [Header("State")]
        [SerializeField] private List<CarController> activeCars = new List<CarController>();
        [SerializeField] private CarController currentCar = null;
        [SerializeField] private int carsProcessedThisNight = 0;
        [SerializeField] private bool isQueueActive = false;

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

            List<CarData> pool = day switch
            {
                Day.Night1 => night1Pool,
                Day.Night2 => night2Pool,
                Day.Night3 => night3Pool,
                _ => night1Pool
            };

            StartCoroutine(RunQueue(pool));
        }

        private IEnumerator RunQueue(List<CarData> pool)
        {
            while (carsProcessedThisNight < carsPerNight && isQueueActive)
            {
                if (currentCar == null)
                {
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
            if (pool.Count == 0) return;

            if (boothApproachPoint == null || exitPoint == null || spawnPoint == null)
            {
                Debug.LogError("CarQueueManager: boothApproachPoint, exitPoint, or spawnPoint is not assigned in the Inspector.");
                return;
            }

            // Filter by availability and randomize
            List<CarData> validCars = pool.FindAll(c => 
                (GameManager.Instance.CurrentDay == Day.Night1 && c.appearsNight1) ||
                (GameManager.Instance.CurrentDay == Day.Night2 && c.appearsNight2) ||
                (GameManager.Instance.CurrentDay == Day.Night3 && c.appearsNight3)
            );

            if (validCars.Count == 0) validCars = pool;

            CarData selectedData = validCars[Random.Range(0, validCars.Count)];

            GameObject carObj = Instantiate(carPrefab, spawnPoint.position, spawnPoint.rotation);
            CarController car = carObj.GetComponent<CarController>();

            if (car != null)
            {
                car.Initialize(selectedData, boothApproachPoint, exitPoint, spawnPoint);
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
