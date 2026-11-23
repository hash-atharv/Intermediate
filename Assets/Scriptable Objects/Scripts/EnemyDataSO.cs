using UnityEngine;

/// <summary>
/// Designer-editable data for one enemy type. Create one asset per type
/// (e.g. a basic melee enemy, an armoured enemy) and assign it to the spawner.
/// Runtime values (current health etc.) live on the Enemy, never here.
/// </summary>
[CreateAssetMenu(fileName = "EnemyData", menuName = "ScriptableObjects/EnemyData", order = 1)]
public class EnemyData : ScriptableObject
{
    [Header("Spawning")]
    [SerializeField] private GameObject prefab;
    [SerializeField] private Vector3 spawnEulerAngles;

    [Header("Health")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private bool isArmoured = false;
    [SerializeField] private float armorHealth = 50f;      // only used if isArmoured is true

    [Header("Combat")]
    [SerializeField] private float attackDamage = 20f;     // damage dealt to the player per hit
    [SerializeField] private float attackInterval = 1.5f;  // seconds between attacks
    [SerializeField] private float attackRange = 2f;       // distance at which the enemy stops and attacks

    [Header("Movement")]
    [SerializeField] private float patrolSpeed = 2f;
    [SerializeField] private float chaseSpeed = 5f;
    [SerializeField] private float waypointReachThreshold = 0.5f;

    [Header("Detection")]
    [SerializeField] private float detectionRadius = 8f;
    [SerializeField] private float loseSightTime = 3f;     // seconds without line of sight before giving up the chase

    [Header("Rewards")]
    [SerializeField] private int scoreValue = 10;          // score/XP given to the player on death

    public GameObject Prefab => prefab;
    public float MaxHealth => maxHealth;
    public bool IsArmoured => isArmoured;
    public float ArmorHealth => armorHealth;
    public float AttackDamage => attackDamage;
    public float AttackInterval => attackInterval;
    public float AttackRange => attackRange;
    public float PatrolSpeed => patrolSpeed;
    public float ChaseSpeed => chaseSpeed;
    public float WaypointReachThreshold => waypointReachThreshold;
    public float DetectionRadius => detectionRadius;
    public float LoseSightTime => loseSightTime;
    public int ScoreValue => scoreValue;

    /// <summary>Spawns this enemy type at the position and hands it its data. Returns the new Enemy.</summary>
    public Enemy Spawn(Vector3 spawnPosition)
    {
        if (prefab == null)
        {
            Debug.LogError($"{name}: no prefab assigned.", this);
            return null;
        }

        GameObject instance = Instantiate(prefab, spawnPosition, Quaternion.Euler(spawnEulerAngles));

        if (!instance.TryGetComponent(out Enemy enemy))
        {
            Debug.LogError($"{name}: prefab '{prefab.name}' has no Enemy component.", this);
            Destroy(instance);
            return null;
        }

        enemy.Initialize(this);
        return enemy;
    }
}