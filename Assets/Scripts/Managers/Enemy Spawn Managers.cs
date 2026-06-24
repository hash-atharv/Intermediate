using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnManager : MonoBehaviour
{
    [Header("Enemy Prefabs")]
    [SerializeField] private GameObject enemyType1;
    [SerializeField] private GameObject enemyType2;

    [Header("Spawn Settings")]
    [SerializeField] private List<Transform> spawnPoints = new List<Transform>();
    [SerializeField] private float spawnInterval = 3f;
    [SerializeField] private int maxEnemies = 10;

    private List<GameObject> activeEnemies = new List<GameObject>();
    private bool isSpawning = false;

    void Start()
    {
        StartSpawning();
    }

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

    private IEnumerator SpawnRoutine()
    {
        while (isSpawning)
        {

            if (activeEnemies.Count < maxEnemies)
                SpawnEnemy();

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnEnemy()
    {
        // Pick a random spawn point
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];

        // Pick a random enemy type
        GameObject prefab = Random.value > 0.5f ? enemyType1 : enemyType2;

        GameObject newEnemy = Instantiate(prefab, spawnPoint.position, spawnPoint.rotation);
        activeEnemies.Add(newEnemy);
    }




    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        foreach (Transform point in spawnPoints)
        {
            if (point != null)
            {
                Gizmos.DrawWireSphere(point.position, 0.5f);
                Gizmos.DrawLine(point.position, point.position + point.forward * 1.5f);
            }
        }
    }
}