using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnManager : MonoBehaviour
{
    //  Spawn Point Data 
    // Groups a spawn location with its own set of patrol waypoints
    [System.Serializable]
    public class SpawnPointData
    {
        public string label;                          // just for Inspector readability
        public Transform spawnPoint;
        public List<Transform> waypoints = new List<Transform>();
    }

    //  Enemy Prefabs 
    [Header("Enemy Prefabs")]
    [SerializeField] private GameObject enemyType1;
    [SerializeField] private GameObject enemyType2;

    //  Spawn Settings 
    [Header("Spawn Settings")]
    [SerializeField] private List<SpawnPointData> spawnPoints = new List<SpawnPointData>();
    [SerializeField] private float spawnInterval = 3f;
    [SerializeField] private int maxEnemies = 10;

    private List<GameObject> activeEnemies = new List<GameObject>();
    private bool isSpawning = false;

    void Start()
    {
        if (!ValidateSetup()) return;
        StartSpawning();
    }

    //  Public Controls 
    public void StartSpawning()
    {
        if (isSpawning) return;
        isSpawning = true;
        StartCoroutine(SpawnRoutine());
    }

    public void StopSpawning()
    {
        isSpawning = false;
        StopAllCoroutines();
    }

    //  Core Spawn Loop 
    private IEnumerator SpawnRoutine()
    {
        while (isSpawning)
        {
            CleanDeadEnemies();

            if (activeEnemies.Count < maxEnemies)
                SpawnEnemy();

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnEnemy()
    {
        // Pick a random spawn point data entry
        SpawnPointData spawnData = spawnPoints[Random.Range(0, spawnPoints.Count)];
        if (spawnData.spawnPoint == null) return;

        // Pick a random enemy type
        GameObject prefab = Random.value > 0.5f ? enemyType1 : enemyType2;

        // Spawn the enemy
        GameObject newEnemy = Instantiate(prefab, spawnData.spawnPoint.position, spawnData.spawnPoint.rotation);

        // Pass the waypoints from this spawn zone to the enemy
        AssignWaypoints(newEnemy, spawnData);

        activeEnemies.Add(newEnemy);
    }

    //  Waypoint Assignment 
    private void AssignWaypoints(GameObject enemy, SpawnPointData spawnData)
    {
        EnemyAI ai = enemy.GetComponent<EnemyAI>();

        if (ai == null)
        {
            Debug.LogError($"EnemySpawnManager: {enemy.name} is missing EnemyAI component.");
            return;
        }

        if (spawnData.waypoints.Count == 0)
        {
            Debug.LogWarning($"EnemySpawnManager: Spawn point '{spawnData.label}' has no waypoints assigned.");
            return;
        }

        ai.SetWaypoints(spawnData.waypoints);
    }

    //  Cleanup 
    private void CleanDeadEnemies()
    {
        activeEnemies.RemoveAll(enemy => enemy == null);
    }

    //  Validation 
    private bool ValidateSetup()
    {
        if (enemyType1 == null || enemyType2 == null)
        {
            Debug.LogError("EnemySpawnManager: Assign both enemy prefabs in the Inspector.");
            return false;
        }
        if (spawnPoints.Count == 0)
        {
            Debug.LogError("EnemySpawnManager: Add at least one SpawnPointData entry.");
            return false;
        }
        return true;
    }

    //  Gizmos 
    private void OnDrawGizmos()
    {
        if (spawnPoints == null) return;

        foreach (SpawnPointData data in spawnPoints)
        {
            if (data.spawnPoint == null) continue;

            // Spawn location
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(data.spawnPoint.position, 0.5f);

            // Waypoints for this spawn zone
            Gizmos.color = Color.cyan;
            for (int i = 0; i < data.waypoints.Count; i++)
            {
                if (data.waypoints[i] == null) continue;
                Gizmos.DrawSphere(data.waypoints[i].position, 0.2f);

                int nextIndex = (i + 1) % data.waypoints.Count;
                if (data.waypoints[nextIndex] != null)
                    Gizmos.DrawLine(data.waypoints[i].position, data.waypoints[nextIndex].position);
            }

            // Line from spawn point to first waypoint
            if (data.waypoints.Count > 0 && data.waypoints[0] != null)
            {
                Gizmos.color = Color.magenta;
                Gizmos.DrawLine(data.spawnPoint.position, data.waypoints[0].position);
            }
        }
    }
}