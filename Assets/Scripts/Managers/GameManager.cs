using UnityEngine;
using UnityEngine.SceneManagement;
using AHeavyToll.Managers;
using AHeavyToll.Gameplay;
using AHeavyToll.Horror;
using AHeavyToll.UI;
using AHeavyToll.Data;
using AHeavyToll.VFX;

namespace AHeavyToll.Managers
{
    public enum GameState { MainMenu, Playing, Paused, GameOver, Jumpscare }
    public enum Day { Night1, Night2, Night3 }
    public enum EndingType { Good, Bad, Hidden, Fired }

    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("Game State")]
        public GameState CurrentState = GameState.Playing;
        public Day CurrentDay = Day.Night1;
        public int CurrentNightNumber => (int)CurrentDay + 1;

        [Header("Ending Tracking")]
        [SerializeField] private int malevolentSpiritsLetIn = 0;
        [SerializeField] private bool herWasLetIn = false;
        [SerializeField] private int totalCarsProcessed = 0;
        [SerializeField] private int carsLetThrough = 0;

        [Header("Events")]
        public UnityEngine.Events.UnityEvent OnDayStart;
        public UnityEngine.Events.UnityEvent OnDayEnd;
        public UnityEngine.Events.UnityEvent OnGameOver;

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
            StartDay(CurrentDay);
        }

        public void StartDay(Day day)
        {
            CurrentDay = day;
            CurrentState = GameState.Playing;

            string dayName = day switch
            {
                Day.Night1 => "Night 1: Normal Working Shift",
                Day.Night2 => "Night 2: Something Feels Off",
                Day.Night3 => "Night 3: Everything Is Off",
                _ => "Unknown Night"
            };

            Debug.Log($"[GameManager] Starting {dayName}");

            JournalSystem.Instance?.UnlockEntriesForNight(CurrentNightNumber);
            UIManager.Instance?.ShowHUD();

            OnDayStart?.Invoke();

            // Trigger day-specific atmosphere
            DayCycleManager.Instance?.ApplyDayAtmosphere(day);

            // Start car queue
            CarQueueManager.Instance?.BeginQueueForDay(day);
        }

        public void AdvanceToNextDay()
        {
            if (CurrentDay == Day.Night3)
            {
                TriggerEnding();
                return;
            }

            CurrentDay++;
            StartDay(CurrentDay);
        }

        public void RegisterCarDecision(bool letThrough, bool isMalevolent, bool isHer)
        {
            totalCarsProcessed++;

            if (letThrough)
            {
                carsLetThrough++;

                if (isHer)
                {
                    herWasLetIn = true;
                    Debug.Log("[GameManager] SHE was let in. The threshold is broken.");
                }
                else if (isMalevolent)
                {
                    malevolentSpiritsLetIn++;
                    Debug.Log($"[GameManager] Malevolent spirit let in. Count: {malevolentSpiritsLetIn}");
                }
            }
        }

        public void TriggerEnding()
        {
            CurrentState = GameState.GameOver;

            EndingType ending = DetermineEnding();
            Debug.Log($"[GameManager] Ending triggered: {ending}");

            EndingManager.Instance?.PlayEnding(ending);
            OnGameOver?.Invoke();
        }

        private EndingType DetermineEnding()
        {
            if (malevolentSpiritsLetIn > 0)
                return EndingType.Bad;

            if (herWasLetIn)
                return EndingType.Hidden;

            if (carsLetThrough == 0 && totalCarsProcessed > 0)
                return EndingType.Fired;

            return EndingType.Good;
        }

        public void RestartGame()
        {
            malevolentSpiritsLetIn = 0;
            herWasLetIn = false;
            totalCarsProcessed = 0;
            carsLetThrough = 0;
            CurrentDay = Day.Night1;
            CurrentState = GameState.Playing;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public void QuitToMenu()
        {
            SceneManager.LoadScene("MainMenu");
        }
    }
}
