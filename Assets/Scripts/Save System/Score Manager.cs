using UnityEngine;


public class ScoreManager : MonoBehaviour
{
    public int TotalScore { get; private set; }

    private void OnEnable()
    {
        GameEvents.OnEnemyDied += HandleEnemyDied;
    }

    private void OnDisable()
    {
        GameEvents.OnEnemyDied -= HandleEnemyDied;
    }

    private void Start()
    {
        GameEvents.RaiseScoreChanged(TotalScore);
    }

    public void AddScore(int amount)
    {
        if (amount == 0) return;

        TotalScore = Mathf.Max(0, TotalScore + amount);
        GameEvents.RaiseScoreChanged(TotalScore);
    }

    public void LoadScoreFromSave(int savedScore)
    {
        TotalScore = Mathf.Max(0, savedScore);
        GameEvents.RaiseScoreChanged(TotalScore);
    }

    public void ResetScore()
    {
        TotalScore = 0;
        GameEvents.RaiseScoreChanged(TotalScore);
    }

    public int GetCurrentScore() => TotalScore;

    private void HandleEnemyDied(int scoreValue)
    {
        AddScore(scoreValue);
    }
}