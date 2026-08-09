using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerBehaviour : MonoBehaviour, IKillable
{
    public float health;
    public TextMeshProUGUI text;
    public PlayerControls controls;

    [SerializeField] private float meleeDamage = 25f;   // damage dealt to an enemy when F is pressed in range

    [Header("Grenade")]
    [SerializeField] private Grenade grenadePrefab;
    [SerializeField] private Transform throwPoint;       // empty child transform marking where grenades spawn from (defaults to player position if unset)
    [SerializeField] private float throwDistance = 8f;    // horizontal distance the grenade travels before landing
    [SerializeField] private float throwArcHeight = 3f;   // peak height of the arc
    [SerializeField] private float throwDuration = 1f;    // seconds for the grenade to reach its landing point
    [SerializeField] private LayerMask groundLayer;       // optional: assign your ground/environment layer so the grenade lands at the correct height on uneven terrain

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
        controls.Gameplay.Throw.performed += ThrowGrenade;
    }

    private void OnDisable()
    {
        controls.Gameplay.Disable();
        controls.Gameplay.Fire.performed -= Attack;
        controls.Gameplay.Throw.performed -= ThrowGrenade;
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

    // Bound to the "Throw" action (T key). Launches a grenade forward from the player
    // along a scripted arc that lands throwDistance away.
    public void ThrowGrenade(InputAction.CallbackContext context)
    {
        if (grenadePrefab == null)
        {
            Debug.LogWarning("PlayerBehaviour: No grenade prefab assigned.");
            return;
        }

        Vector3 origin = throwPoint != null ? throwPoint.position : transform.position;

        Vector3 forward = transform.forward;
        forward.y = 0f;
        forward.Normalize();

        Vector3 targetPosition = origin + forward * throwDistance;

        // Try to snap the landing point to the ground so the arc doesn't end floating
        // above or clipped below uneven terrain. Falls back to origin height if no hit.
        Vector3 rayStart = targetPosition + Vector3.up * 10f;
        if (Physics.Raycast(rayStart, Vector3.down, out RaycastHit groundHit, 50f, groundLayer))
        {
            targetPosition.y = groundHit.point.y;
        }

        Grenade grenade = Instantiate(grenadePrefab, origin, Quaternion.identity);
        grenade.Launch(origin, targetPosition, throwDuration, throwArcHeight);
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