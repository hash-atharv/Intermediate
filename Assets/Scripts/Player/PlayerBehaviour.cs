using TMPro;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerBehaviour : MonoBehaviour, IKillable
{
    public float health;

    //public GameObject player;
    public TextMeshProUGUI text;

    public PlayerControls controls;
    //[SerializeField] public EnemyData enemyData;
    
    private Enemy currentEnemy;

    
    public float damage;


    private void Awake()
    {
        controls = new PlayerControls();
        health = 100;
    }

    private void Update()
    {
        text.text = $"Health:{health}";
    }
    private void OnEnable()
    {
        controls.Gameplay.Enable();
        controls.Gameplay.Fire.performed += Attack;
    }

    private void OnDisable()
    {
        controls.Gameplay.Disable();
        controls.Gameplay.Fire.performed -= Attack;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            
            if (other.TryGetComponent<Enemy>(out var enemy))
            {
                currentEnemy = enemy;
               Debug.Log("Enemy in range!");
               

            }
        }
        if (other.CompareTag("Health"))
        {
            health += 25;
            Destroy(other.gameObject);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            currentEnemy= null;
            Debug.Log("Enemy out range!");
        }
    }

    public void DamageDecider()
    {
        if (ownedData.weapon == "Owned")
        {
            damage = 100;
        }
        else
        {
            damage = 50;
        }
    }

    public void Attack(InputAction.CallbackContext context)
    {
        DamageDecider();
        Debug.Log("yes");
        currentEnemy.TakeDamage(damage);

    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        Die();
    }

    public void Die()
    {
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }
}
