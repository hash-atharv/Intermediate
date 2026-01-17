using UnityEngine;
using UnityEngine.InputSystem;

public class Enemy : MonoBehaviour, IKillable
{
    [SerializeField] public Animator animator;
    private PlayerBehaviour playerBehaviour;

    public float EnemyHealth = 100f;

    private void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (other.TryGetComponent<PlayerBehaviour>(out var player))
            {
                playerBehaviour = player;
                animator.SetBool("isInRange", true);
                
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //currentSpeed = speed;
            //animator.SetBool("isMoving", true);
            playerBehaviour = null;
            animator.SetBool("isInRange", false);

        }
    }

    public void attack()
    {
        playerBehaviour.TakeDamage(50);
        Debug.Log("Yes");
    }

    public void TakeDamage(float damage)
    {
        //EnemyData.health -= damage;

        EnemyHealth -= damage;
        Debug.Log(EnemyHealth);
        Die();
       
    }

    public void Die()
    {
        if (EnemyHealth <= 0)
        {
            Destroy(gameObject);
            attack();
        }
    }


}
