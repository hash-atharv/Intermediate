using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour, IKillable
{
    //  State 
    private enum EnemyState { Patrol, Chase }
    private EnemyState currentState = EnemyState.Patrol;

    //  References 
    [SerializeField] public Animator animator;
    private NavMeshAgent agent;
    private PlayerBehaviour playerBehaviour;   // set via melee trigger, used for attack()
    private Transform playerTransform;         // set via detection sphere, used for chase movement

    //  Combat 
    [Header("Combat")]
    public float EnemyHealth = 100f;
    [SerializeField] private float attackDamage = 20f;      // damage dealt to the player per hit
    [SerializeField] private float attackInterval = 1.5f;   // seconds between attacks
    private float attackTimer = 0f;

    //  Armor 
    [Header("Armor")]
    [SerializeField] private bool isArmoured = false;
    [SerializeField] private float armorHealth = 50f;   // only used if isArmoured is true

    //  Patrol Settings 
    [Header("Patrol")]
    private List<Transform> waypoints = new List<Transform>();
    [SerializeField] private float waypointReachThreshold = 0.5f;
    [SerializeField] private float patrolSpeed = 2f;
    private int currentWaypointIndex = 0;
    private bool waypointsAssigned = false;

    //  Detection Settings (long range - triggers Chase state) 
    [Header("Detection")]
    [SerializeField] private float detectionRadius = 8f;
    [SerializeField] private LayerMask playerLayer;

    //  Chase Settings 
    [Header("Chase")]
    [SerializeField] private float chaseSpeed = 5f;
    [SerializeField] private float attackRange = 2f;   // how far away the enemy stops and starts attacking

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.stoppingDistance = attackRange;
    }

    // Called by EnemySpawnManager right after spawning
    public void SetWaypoints(List<Transform> assignedWaypoints)
    {
        waypoints = assignedWaypoints;
        waypointsAssigned = true;
        EnterPatrol();
    }

    void Update()
    {
        // Wait until waypoints are received from the spawn manager
        if (!waypointsAssigned) return;

        switch (currentState)
        {
            case EnemyState.Patrol:
                PatrolUpdate();
                DetectPlayer();
                break;

            case EnemyState.Chase:
                ChaseUpdate();
                LosePlayer();
                break;
        }

        // Only attack while actively chasing and the player is in melee range
        if (currentState == EnemyState.Chase)
            AttackUpdate();
    }

    //  Attack Timer 
    private void AttackUpdate()
    {
        // Only try to attack while the player is within melee range
        if (playerBehaviour == null) return;

        attackTimer += Time.deltaTime;

        if (attackTimer >= attackInterval)
        {
            attackTimer = 0f;

            // Play the attack animation if one is assigned (visual only).
            if (animator != null)
                animator.SetTrigger("Attack");

            // Deal damage directly on the interval so combat works even
            // without an Animation Event wired up on the Attack clip.
            attack();
        }
    }

    //  Patrol 
    private void EnterPatrol()
    {
        currentState = EnemyState.Patrol;
        agent.speed = patrolSpeed;

        currentWaypointIndex = GetClosestWaypointIndex();
        SetDestinationToWaypoint();

        Debug.Log($"{name}: Entering PATROL");
    }

    private void PatrolUpdate()
    {
        if (waypoints.Count == 0) return;

        if (!agent.pathPending && agent.remainingDistance <= waypointReachThreshold)
            GoToNextWaypoint();
    }

    private void GoToNextWaypoint()
    {
        currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Count;
        SetDestinationToWaypoint();
    }

    private void SetDestinationToWaypoint()
    {
        if (waypoints[currentWaypointIndex] != null)
            agent.SetDestination(waypoints[currentWaypointIndex].position);
    }

    private int GetClosestWaypointIndex()
    {
        int closestIndex = 0;
        float closestDistance = Mathf.Infinity;

        for (int i = 0; i < waypoints.Count; i++)
        {
            if (waypoints[i] == null) continue;
            float distance = Vector3.Distance(transform.position, waypoints[i].position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestIndex = i;
            }
        }
        return closestIndex;
    }

    //  Chase 
    private void EnterChase()
    {
        currentState = EnemyState.Chase;
        agent.speed = chaseSpeed;
        Debug.Log($"{name}: Entering CHASE");
    }

    private void ChaseUpdate()
    {
        if (playerTransform == null) return;

        agent.SetDestination(playerTransform.position);

        // Once close enough, stop moving and turn to face the player
        if (!agent.pathPending && agent.remainingDistance <= attackRange)
        {
            FacePlayer();
        }
    }

    private void FacePlayer()
    {
        Vector3 direction = playerTransform.position - transform.position;
        direction.y = 0f;
        if (direction.sqrMagnitude < 0.001f) return;

        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10f);
    }

    //  Detection (long range, drives Patrol -> Chase) 
    private void DetectPlayer()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius, playerLayer);
        if (hits.Length > 0)
        {
            playerTransform = hits[0].transform;
            EnterChase();
        }
    }

    private void LosePlayer()
    {
        if (playerTransform == null) { EnterPatrol(); return; }

        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
        if (distanceToPlayer > detectionRadius)
        {
            playerTransform = null;
            EnterPatrol();
        }
    }

    //  Melee Range (short range trigger, drives attack() + animator "isInRange") 
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (other.TryGetComponent<PlayerBehaviour>(out var player))
            {
                playerBehaviour = player;
                animator.SetBool("isInRange", true);
                attackTimer = 0f;   // start the interval fresh each time the player enters range
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerBehaviour = null;
            animator.SetBool("isInRange", false);
            attackTimer = 0f;
        }
    }

    // Called by AttackUpdate() on the interval (also safe to call from an
    // Animation Event on the Attack clip if you want it re-synced to the anim).
    public void attack()
    {
        if (playerBehaviour != null)
            playerBehaviour.TakeDamage(attackDamage);
    }

    //  Combat / IKillable 
    public void TakeDamage(float damage)
    {
        // If the enemy is armoured and still has armor left, damage hits the
        // armor first. Any leftover damage that exceeds the remaining armor
        // "spills over" onto the enemy's actual health in the same hit.
        if (isArmoured && armorHealth > 0f)
        {
            armorHealth -= damage;

            if (armorHealth < 0f)
            {
                float overflowDamage = -armorHealth;   // amount that exceeded the armor
                armorHealth = 0f;
                isArmoured = false;                    // armor is broken, future hits go straight to health
                EnemyHealth -= overflowDamage;
            }
        }
        else
        {
            EnemyHealth -= damage;
        }

        Debug.Log($"ArmorHealth: {armorHealth} | EnemyHealth: {EnemyHealth}");
        Die();
    }

    public void Die()
    {
        if (EnemyHealth <= 0)
        {
            Destroy(gameObject);
        }
    }

    //  Gizmos 
    private void OnDrawGizmos()
    {
        Gizmos.color = currentState == EnemyState.Chase ? Color.red : Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        if (waypoints == null || waypoints.Count == 0) return;

        Gizmos.color = Color.cyan;
        for (int i = 0; i < waypoints.Count; i++)
        {
            if (waypoints[i] == null) continue;
            Gizmos.DrawSphere(waypoints[i].position, 0.2f);

            int nextIndex = (i + 1) % waypoints.Count;
            if (waypoints[nextIndex] != null)
                Gizmos.DrawLine(waypoints[i].position, waypoints[nextIndex].position);
        }

        if (Application.isPlaying && waypoints.Count > 0 && waypoints[currentWaypointIndex] != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(waypoints[currentWaypointIndex].position, 0.3f);
        }
    }
}