using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerBehaviour : MonoBehaviour, IKillable
{
    [Header("Health")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float healthPickupAmount = 25f;

    [Header("Melee")]
    [SerializeField] private float meleeDamage = 25f;   // damage dealt to the closest enemy in range when Fire is pressed

    [Header("Grenade")]
    [SerializeField] private Grenade grenadePrefab;
    [SerializeField] private Transform throwPoint;       // empty child transform marking where grenades spawn from (defaults to player position if unset)
    [SerializeField] private float throwDistance = 8f;    // horizontal distance the grenade travels before landing
    [SerializeField] private float throwArcHeight = 3f;   // peak height of the arc
    [SerializeField] private float throwDuration = 1f;    // seconds for the grenade to reach its landing point
    [SerializeField] private LayerMask groundLayer;       // optional: assign your ground/environment layer so the grenade lands at the correct height on uneven terrain

    public float Health { get; private set; }
    public float MaxHealth => maxHealth;
    public bool IsDead { get; private set; }

    private PlayerControls controls;
    private readonly List<Enemy> enemiesInRange = new List<Enemy>();
    private int countCollectibles;

    private void Awake()
    {
        controls = new PlayerControls();
        Health = maxHealth;
    }

    private void OnEnable()
    {
        controls.Gameplay.Enable();
        controls.Gameplay.Fire.performed += Attack;
        controls.Gameplay.Throw.performed += ThrowGrenade;
    }

    private void OnDisable()
    {
        controls.Gameplay.Fire.performed -= Attack;
        controls.Gameplay.Throw.performed -= ThrowGrenade;
        controls.Gameplay.Disable();
    }

    private void OnDestroy()
    {
        controls?.Dispose();
    }

    private void Start()
    {
        // Listeners (UI) subscribe in OnEnable, so they are ready to receive the starting values.
        GameEvents.RaisePlayerHealthChanged(Health, maxHealth);

        if (Collectibles.Instance != null)
        {
            Collectibles.Instance.UpdateCollectibleText(countCollectibles);
        }
    }

    // ---- Pickups / melee range tracking ----

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            if (other.TryGetComponent<Enemy>(out var enemy) && !enemiesInRange.Contains(enemy))
            {
                enemiesInRange.Add(enemy);
            }
        }

        if (other.CompareTag("Health"))
        {
            Heal(healthPickupAmount);
            Destroy(other.gameObject);
        }

        if (other.CompareTag("Collectible"))
        {
            countCollectibles++;
            if (Collectibles.Instance != null)
            {
                Collectibles.Instance.UpdateCollectibleText(countCollectibles);
            }
            Destroy(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy") && other.TryGetComponent<Enemy>(out var enemy))
        {
            enemiesInRange.Remove(enemy);
        }
    }

    // ---- Actions ----

    private bool CanAct()
    {
        if (IsDead) return false;
        return GameManager.Instance == null || GameManager.Instance.CurrentState == GameState.Playing;
    }

    // Bound to the "Fire" action. Damages the closest enemy in melee range.
    private void Attack(InputAction.CallbackContext context)
    {
        if (!CanAct()) return;

        Enemy target = GetClosestEnemyInRange();
        if (target != null)
        {
            target.TakeDamage(meleeDamage);
        }
    }

    private Enemy GetClosestEnemyInRange()
    {
        Enemy closest = null;
        float closestSqrDistance = float.MaxValue;

        // Iterate backwards so dead/destroyed/pooled enemies can be removed safely.
        for (int i = enemiesInRange.Count - 1; i >= 0; i--)
        {
            Enemy enemy = enemiesInRange[i];
            if (enemy == null || !enemy.gameObject.activeInHierarchy)
            {
                enemiesInRange.RemoveAt(i);
                continue;
            }

            float sqrDistance = (enemy.transform.position - transform.position).sqrMagnitude;
            if (sqrDistance < closestSqrDistance)
            {
                closestSqrDistance = sqrDistance;
                closest = enemy;
            }
        }

        return closest;
    }

    // Bound to the "Throw" action. Launches a grenade forward from the player
    // along a scripted arc that lands throwDistance away.
    private void ThrowGrenade(InputAction.CallbackContext context)
    {
        if (!CanAct()) return;

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

    // ---- Health (IKillable) ----

    public void TakeDamage(float damage)
    {
        if (IsDead || damage <= 0f) return;

        Health = Mathf.Max(0f, Health - damage);
        GameEvents.RaisePlayerHealthChanged(Health, maxHealth);

        if (Health <= 0f)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        if (IsDead || amount <= 0f) return;

        Health = Mathf.Min(maxHealth, Health + amount);
        GameEvents.RaisePlayerHealthChanged(Health, maxHealth);
    }

    /// <summary>Used by the Save System when loading a checkpoint.</summary>
    public void SetHealth(float value)
    {
        IsDead = false;
        Health = Mathf.Clamp(value, 0f, maxHealth);
        GameEvents.RaisePlayerHealthChanged(Health, maxHealth);

        if (Health <= 0f)
        {
            Die();
        }
    }

    public void Die()
    {
        if (IsDead) return;

        IsDead = true;
        GameEvents.RaisePlayerDied(); // GameManager switches to GameOver, UI shows the panel
    }
}