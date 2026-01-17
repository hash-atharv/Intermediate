using UnityEngine;
using UnityEngine.InputSystem;

public class Enemy : MonoBehaviour, IKillable
{
    

    public float health = 100f;

    private void Update()
    {
        
    }



    public void TakeDamage(float damage)
    {
        //EnemyData.health -= damage;

        health -= damage;
        Debug.Log(health);

        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }


}
