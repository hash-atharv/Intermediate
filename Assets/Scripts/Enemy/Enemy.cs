using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : MonoBehaviour, IKillable
{
    private static readonly int AttackHash = Animator.StringToHash("Attack");
    private static readonly int IsInRangeHash = Animator.StringToHash("isInRange");

    [Header("Data")]
    [SerializeField] private EnemyData data;   // set automatically by EnemyData.Spawn(); assign manually for scene-placed enemies

    [Header("References")]
    [SerializeField] private Animator animator;

    [Header("Detection")]
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private LayerMask obstacleMask;   // layers that block line of sight (walls, etc.). Leave empty = always visible
    [SerializeField] private float eyeHeight = 1.5f;

    // ---- Public read-only API (used by the states) ----
    public EnemyData Data => data;
    public NavMeshAgent Agent { get; private set; }
    public bool IsDead { get; private set; }
    public float Health => health;
    public bool IsArmoured => armor > 0f;
    public IReadOnlyList<Transform> Waypoints => waypoints;

    public bool AgentReady => Agent != null && Agent.isActiveAndEnabled && Agent.isOnNavMesh;
    public bool HasValidTarget => targetPlayer != null && !targetPlayer.IsDead;
    public Transform Target => targetPlayer != null ? targetPlayer.transform : null;
    public float DistanceToTarget => HasValidTarget
        ? Vector3.Distance(transform.position, targetPlayer.transform.position)
        : float.PositiveInfinity;

    // ---- States ----
    public PatrolState Patrolling { get; private set; }
    public ChaseState Chasing { get; private set; }
    public AttackState Attacking { get; private set; }
    public DeadState Dying { get; private set; }

    private StateMachine<Enemy> stateMachine;
    private PlayerBehaviour targetPlayer;
    private List<Transform> waypoints = new List<Transform>();

    private readonly Collider[] detectionBuffer = new Collider[8];
    private readonly HashSet<int> animatorParameters = new HashSet<int>();

    private float health;
    private float armor;
    private bool dataApplied;
    private bool started;

    private void Awake()
    {
        Agent = GetComponent<NavMeshAgent>();

        if (animator == null)
            animator = GetComponentInChildren<Animator>();
        CacheAnimatorParameters();

        stateMachine = new StateMachine<Enemy>();
        Patrolling = new PatrolState(this, stateMachine);
        Chasing = new ChaseState(this, stateMachine);
        Attacking = new AttackState(this, stateMachine);
        Dying = new DeadState(this, stateMachine);
    }

    private void Start()
    {
        if (data == null)
        {
            Debug.LogError($"{name}: no EnemyData assigned.", this);
            enabled = false;
            return;
        }

        if (!dataApplied)
            ApplyData();

        started = true;
        stateMachine.Initialize(Patrolling);
    }

    private void Update()
    {
        stateMachine.Tick();
    }

    // ---- Setup ----

    /// <summary>Called by EnemyData.Spawn() right after instantiating.</summary>
    public void Initialize(EnemyData enemyData)
    {
        if (enemyData == null) return;

        data = enemyData;
        ApplyData();
    }

    private void ApplyData()
    {
        health = data.MaxHealth;
        armor = data.IsArmoured ? data.ArmorHealth : 0f;
        dataApplied = true;
    }

    /// <summary>Called by the spawner right after spawning.</summary>
    public void SetWaypoints(List<Transform> assignedWaypoints)
    {
        waypoints = assignedWaypoints ?? new List<Transform>();

        // Already running and patrolling? Restart the route with the new waypoints.
        if (started && stateMachine.CurrentState == Patrolling)
            Patrolling.Enter();
    }

    // ---- Movement helpers (all NavMesh access is guarded) ----

    public void MoveTo(Vector3 destination, float speed, float stoppingDistance)
    {
        if (!AgentReady) return;

        Agent.isStopped = false;
        Agent.speed = speed;
        Agent.stoppingDistance = stoppingDistance;
        Agent.SetDestination(destination);
    }

    public void StopMoving()
    {
        if (AgentReady)
            Agent.isStopped = true;
    }

    public void FaceTarget()
    {
        if (!HasValidTarget) return;

        Vector3 direction = targetPlayer.transform.position - transform.position;
        direction.y = 0f;
        if (direction.sqrMagnitude < 0.001f) return;

        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10f);
    }

    // ---- Detection ----

    /// <summary>Looks for a living, visible player inside the detection radius.</summary>
    public bool TryAcquireTarget()
    {
        int count = Physics.OverlapSphereNonAlloc(transform.position, data.DetectionRadius, detectionBuffer, playerLayer);

        for (int i = 0; i < count; i++)
        {
            PlayerBehaviour player = detectionBuffer[i].GetComponentInParent<PlayerBehaviour>();
            if (player == null || player.IsDead) continue;
            if (!HasLineOfSightTo(player.transform)) continue;

            targetPlayer = player;
            return true;
        }

        return false;
    }

    public void ClearTarget()
    {
        targetPlayer = null;
    }

    public bool HasLineOfSight()
    {
        return HasValidTarget && HasLineOfSightTo(targetPlayer.transform);
    }

    private bool HasLineOfSightTo(Transform target)
    {
        Vector3 origin = transform.position + Vector3.up * eyeHeight;
        Vector3 destination = target.position + Vector3.up * eyeHeight;
        Vector3 toTarget = destination - origin;

        return !Physics.Raycast(origin, toTarget.normalized, toTarget.magnitude, obstacleMask, QueryTriggerInteraction.Ignore);
    }

    // ---- Combat ----

    /// <summary>
    /// Called by AttackState on the attack interval. Override in a subclass
    /// (e.g. a ranged enemy that fires a projectile instead).
    /// </summary>
    public virtual void Attack()
    {
        if (!HasValidTarget) return;

        SetAnimatorTrigger(AttackHash);
        targetPlayer.TakeDamage(data.AttackDamage);
    }

    // ---- IKillable ----

    public void TakeDamage(float damage)
    {
        if (IsDead || damage <= 0f) return;

        // Armor absorbs damage first; any leftover spills over onto health in the same hit.
        float remaining = damage;
        if (armor > 0f)
        {
            float absorbed = Mathf.Min(armor, remaining);
            armor -= absorbed;
            remaining -= absorbed;
        }

        health -= remaining;

        if (health <= 0f)
            Die();
    }

    public void Die()
    {
        if (IsDead) return;

        IsDead = true;
        stateMachine.ChangeState(Dying);
    }

    // ---- Animator helpers (skip parameters the controller doesn't have) ----

    public void SetInRange(bool value)
    {
        if (animator != null && animatorParameters.Contains(IsInRangeHash))
            animator.SetBool(IsInRangeHash, value);
    }

    private void SetAnimatorTrigger(int hash)
    {
        if (animator != null && animatorParameters.Contains(hash))
            animator.SetTrigger(hash);
    }

    private void CacheAnimatorParameters()
    {
        if (animator == null || animator.runtimeAnimatorController == null) return;

        foreach (AnimatorControllerParameter parameter in animator.parameters)
            animatorParameters.Add(parameter.nameHash);
    }

    // ---- Gizmos ----

    private void OnDrawGizmos()
    {
        if (data != null)
        {
            bool aggressive = Application.isPlaying && stateMachine != null &&
                              (stateMachine.CurrentState == Chasing || stateMachine.CurrentState == Attacking);

            Gizmos.color = aggressive ? Color.red : Color.yellow;
            Gizmos.DrawWireSphere(transform.position, data.DetectionRadius);
        }

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

        if (Application.isPlaying && Patrolling != null)
        {
            int index = Patrolling.CurrentWaypointIndex;
            if (index < waypoints.Count && waypoints[index] != null)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(waypoints[index].position, 0.3f);
            }
        }
    }
}