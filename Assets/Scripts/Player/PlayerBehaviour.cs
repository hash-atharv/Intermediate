using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerBehaviour : MonoBehaviour, IKillable
{
    public float health;
    public TextMeshProUGUI text;
    public PlayerControls controls;

    [SerializeField] private float meleeDamage = 25f;   // damage dealt to an enemy when F is pressed in range

    private Enemy currentEnemy;

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
            currentEnemy = null;
            Debug.Log("Enemy out range!");
        }
    }

    // Bound to the "Fire" action (F key), only deals damage if an enemy is in range
    public void Attack(InputAction.CallbackContext context)
    {
        if (currentEnemy == null)
        {
            Debug.Log("No enemy in range to attack.");
            return;
        }

        currentEnemy.TakeDamage(meleeDamage);
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