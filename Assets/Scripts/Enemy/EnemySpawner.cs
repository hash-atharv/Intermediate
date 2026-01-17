using UnityEngine;

public class EnemySpawner : MonoBehaviour
{


    [SerializeField] public EnemyData enemyData;
    [SerializeField] public Vector3 spawnPosition;


    private void Awake()
    {
        enemyData.Spawner(spawnPosition);
    }



}
