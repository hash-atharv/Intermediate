using UnityEngine;


[CreateAssetMenu(fileName ="EnemyDataSO", menuName ="ScriptableObjects/EnemyDataSO", order =1)]
public class EnemyData : ScriptableObject
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private float health;
    //[SerializeField] public float damage;
    [SerializeField] private Vector3 spawnPosition;
    [SerializeField] private Quaternion direction;

    public void Spawner()
    {
        Instantiate(prefab, spawnPosition, direction);
        
    }

    public void healthControl(float damage)
    {
        health -= damage;

        Debug.Log(health);
    }


}
