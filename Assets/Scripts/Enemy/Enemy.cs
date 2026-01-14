using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] public EnemyData enemyData;


    private void Awake()
    {
        enemyData.ex();

    }

    public static void Damage()
    {
        //enemyData.TakeDamage(50);
        //Debug.Log(enemyData.health);
    }
    

}
