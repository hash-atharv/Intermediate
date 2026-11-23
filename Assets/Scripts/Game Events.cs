using System;
using UnityEngine;


public static class GameEvents
{

    public static event Action<int> OnScoreChanged;

    public static event Action<int> OnEnemyDied;

    public static event Action<float, float> OnPlayerHealthChanged;
    public static event Action OnPlayerDied;

    public static event Action<GameState> OnGameStateChanged;

    public static void RaiseScoreChanged(int totalScore) => OnScoreChanged?.Invoke(totalScore);
    public static void RaiseEnemyDied(int scoreValue) => OnEnemyDied?.Invoke(scoreValue);
    public static void RaisePlayerHealthChanged(float currentHealth, float maxHealth) => OnPlayerHealthChanged?.Invoke(currentHealth, maxHealth);
    public static void RaisePlayerDied() => OnPlayerDied?.Invoke();
    public static void RaiseGameStateChanged(GameState newState) => OnGameStateChanged?.Invoke(newState);


    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetEvents()
    {
        OnScoreChanged = null;
        OnEnemyDied = null;
        OnPlayerHealthChanged = null;
        OnPlayerDied = null;
        OnGameStateChanged = null;
    }
}