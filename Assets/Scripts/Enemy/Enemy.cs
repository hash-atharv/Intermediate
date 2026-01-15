using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] public EnemyData enemyData;


    private void Awake()
    {
        enemyData.Spawner();

    }

    //public void TakeDamage(float damage)
    //{
    //    //EnemyData.health -= damage;
    //}
    

}
