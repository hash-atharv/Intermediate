using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // 1. Get the current data
            int currentScore = FindFirstObjectByType<ScoreManager>().GetCurrentScore();
            Vector3 playerPos = other.transform.position;

            // 2. Tell SaveManager to save it
            FindFirstObjectByType<SaveManager>().SaveGame(currentScore, 100f, playerPos);

            Debug.Log("Checkpoint Reached!");
        }
    }
}