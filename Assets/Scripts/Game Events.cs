using System;
using UnityEngine;

public static class GameEvents
{

    public static Action<int> OnScoreChanged;
    public static Action<float> OnPlayerHealthChanged;
    public static Action OnEnemyDied;


    public static void RaiseScoreChanged(int amount)
    {
        OnScoreChanged?.Invoke(amount);
    }

    public static void RaisePlayerHealthChanged(float currentHealth)
    {
        OnPlayerHealthChanged?.Invoke(currentHealth);
    }
}