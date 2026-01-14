using UnityEngine;


[CreateAssetMenu(fileName ="EnemyDataSO", menuName ="ScriptableObjects/EnemyDataSO", order =1)]
public class EnemyData : ScriptableObject, IKillable
{
    [SerializeField] private GameObject prefab;
    [SerializeField] public float health;
    [SerializeField] public float damage;
    [SerializeField] private Vector3 spawnPosition;
    [SerializeField] private Quaternion direction;

    public void ex()
    {
        Instantiate(prefab, spawnPosition, direction);
    }

    public void TakeDamage()
    {
        health -= damage;
        
        Debug.Log(health);
    }
    
}
