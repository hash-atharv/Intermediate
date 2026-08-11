using System.IO; // Required for File writing
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    private string savePath;

    void Awake()
    {
        // Path where Unity is allowed to save files on any device
        savePath = Application.persistentDataPath + "/savegame.json";
    }

    public void SaveGame(int score, float health, Vector3 position)
    {
        SaveData data = new SaveData(score, health, position);

        // Convert the Object into a JSON string
        string json = JsonUtility.ToJson(data, true); // 'true' makes it readable in Notepad

        // Write the string to a file
        File.WriteAllText(savePath, json);

        Debug.Log("Game Saved to: " + savePath);
    }

    public SaveData LoadGame()
    {
        if (File.Exists(savePath))
        {
            // Read the text back from the file
            string json = File.ReadAllText(savePath);

            // Convert the JSON string back into a SaveData object
            return JsonUtility.FromJson<SaveData>(json);
        }

        Debug.LogWarning("No save file found!");
        return null;
    }

    // Add this inside your SaveManager class
    public void TriggerLoad()
    {
        SaveData data = LoadGame();

        if (data != null)
        {
            // 1. Update the Score
            ScoreManager scoreMgr = FindFirstObjectByType<ScoreManager>();
            if (scoreMgr != null)
            {
                // We force the score to the saved value
                // You might need to add a 'SetScore' method in ScoreManager
                scoreMgr.LoadScoreFromSave(data.score);
            }

            // 2. Update Player Position
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                // Disable CharacterController briefly to "teleport"
                CharacterController cc = player.GetComponent<CharacterController>();
                if (cc != null) cc.enabled = false;

                player.transform.position = new Vector3(data.posX, data.posY, data.posZ);

                if (cc != null) cc.enabled = true;
            }

            Debug.Log("Load Complete!");
        }
    }
}