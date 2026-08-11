using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    private int totalScore = 0;

    private void OnEnable()
    {
        // Tune in to the "OnScoreChanged" radio station
        GameEvents.OnScoreChanged += AddScore;
    }

    private void OnDisable()
    {
        // Stop listening when the object is destroyed/disabled
        GameEvents.OnScoreChanged -= AddScore;
    }


    public void LoadScoreFromSave(int savedAmount)
    {
        totalScore = savedAmount;
        // Manually trigger the UI update event so the screen changes
        GameEvents.RaiseScoreChanged(0);
    }

    void AddScore(int amount)
    {
        totalScore += amount;
        Debug.Log("Current Total Score: " + totalScore);
    }

    // This will be useful for the Save System in Phase 5
    public int GetCurrentScore() => totalScore;
}