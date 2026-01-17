using UnityEngine;


[CreateAssetMenu(fileName ="EnemyDataSO", menuName ="ScriptableObjects/EnemyDataSO", order =1)]
public class EnemyData : ScriptableObject
{
    [SerializeField] private GameObject prefab;
    
    [SerializeField] private Quaternion direction;

    public void Spawner( Vector3 spawnPosition)
    {
        Instantiate(prefab, spawnPosition, direction);
        
    }

   


}
