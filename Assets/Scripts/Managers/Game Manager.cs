using UnityEngine;

public enum GameState
{
    Playing,
    Paused,
    GameOver
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public GameState CurrentState { get; private set; } = GameState.Playing;
    public int Kills { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        Time.timeScale = 1f; // in case we arrive here from a paused/game-over scene
    }

    private void OnEnable()
    {
        GameEvents.OnEnemyDied += HandleEnemyDied;
        GameEvents.OnPlayerDied += HandlePlayerDied;
    }

    private void OnDisable()
    {
        GameEvents.OnEnemyDied -= HandleEnemyDied;
        GameEvents.OnPlayerDied -= HandlePlayerDied;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }


    public void SetState(GameState newState)
    {
        if (CurrentState == newState) return;

        CurrentState = newState;
        Time.timeScale = newState == GameState.Playing ? 1f : 0f;
        GameEvents.RaiseGameStateChanged(newState);
    }

    public void TogglePause()
    {
        if (CurrentState == GameState.GameOver) return;

        SetState(CurrentState == GameState.Paused ? GameState.Playing : GameState.Paused);
    }

    public void GameOver()
    {
        SetState(GameState.GameOver);
    }


    private void HandleEnemyDied(int scoreValue)
    {
        Kills++;
    }

    private void HandlePlayerDied()
    {
        GameOver();
    }
}