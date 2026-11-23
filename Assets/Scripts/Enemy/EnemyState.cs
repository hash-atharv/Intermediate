using UnityEngine;

// All enemy states. Each one decides its own transitions based on
// distance, line of sight, and health (death is triggered from Enemy.TakeDamage).

public class PatrolState : State<Enemy>
{
    private const float ScanInterval = 0.2f;   // how often we look for the player (seconds)

    private int waypointIndex;
    private float scanTimer;

    public int CurrentWaypointIndex => waypointIndex;

    public PatrolState(Enemy owner, StateMachine<Enemy> machine) : base(owner, machine) { }

    public override void Enter()
    {
        owner.ClearTarget();
        owner.SetInRange(false);
        scanTimer = 0f;

        waypointIndex = FindClosestWaypointIndex();
        MoveToCurrentWaypoint();
    }

    public override void Tick()
    {
        // Player spotted (in detection radius AND visible) -> chase
        scanTimer -= Time.deltaTime;
        if (scanTimer <= 0f)
        {
            scanTimer = ScanInterval;
            if (owner.TryAcquireTarget())
            {
                machine.ChangeState(owner.Chasing);
                return;
            }
        }

        // Walk the waypoint loop
        if (owner.Waypoints.Count == 0 || !owner.AgentReady) return;

        if (!owner.Agent.pathPending && owner.Agent.remainingDistance <= owner.Data.WaypointReachThreshold)
        {
            waypointIndex = (waypointIndex + 1) % owner.Waypoints.Count;
            MoveToCurrentWaypoint();
        }
    }

    private void MoveToCurrentWaypoint()
    {
        if (owner.Waypoints.Count == 0)
        {
            owner.StopMoving();
            return;
        }

        Transform waypoint = owner.Waypoints[waypointIndex];
        if (waypoint != null)
        {
            owner.MoveTo(waypoint.position, owner.Data.PatrolSpeed, 0f);
        }
    }

    private int FindClosestWaypointIndex()
    {
        int closestIndex = 0;
        float closestSqrDistance = float.PositiveInfinity;

        for (int i = 0; i < owner.Waypoints.Count; i++)
        {
            Transform waypoint = owner.Waypoints[i];
            if (waypoint == null) continue;

            float sqrDistance = (waypoint.position - owner.transform.position).sqrMagnitude;
            if (sqrDistance < closestSqrDistance)
            {
                closestSqrDistance = sqrDistance;
                closestIndex = i;
            }
        }

        return closestIndex;
    }
}

public class ChaseState : State<Enemy>
{
    private const float RepathInterval = 0.2f;

    private float repathTimer;
    private float lostSightTimer;

    public ChaseState(Enemy owner, StateMachine<Enemy> machine) : base(owner, machine) { }

    public override void Enter()
    {
        repathTimer = 0f;      // repath on the first tick
        lostSightTimer = 0f;
    }

    public override void Tick()
    {
        if (!owner.HasValidTarget)
        {
            machine.ChangeState(owner.Patrolling);
            return;
        }

        float distance = owner.DistanceToTarget;
        bool canSee = owner.HasLineOfSight();

        // Lose the player: too far away, or out of sight for too long
        lostSightTimer = canSee ? 0f : lostSightTimer + Time.deltaTime;
        if (distance > owner.Data.DetectionRadius || lostSightTimer >= owner.Data.LoseSightTime)
        {
            machine.ChangeState(owner.Patrolling);
            return;
        }

        // Close enough with a clear line -> attack
        if (canSee && distance <= owner.Data.AttackRange)
        {
            machine.ChangeState(owner.Attacking);
            return;
        }

        repathTimer -= Time.deltaTime;
        if (repathTimer <= 0f)
        {
            repathTimer = RepathInterval;
            // Stop slightly inside attack range so the enemy really reaches it
            owner.MoveTo(owner.Target.position, owner.Data.ChaseSpeed, owner.Data.AttackRange * 0.8f);
        }
    }
}

public class AttackState : State<Enemy>
{
    private const float ExitBuffer = 0.5f;   // extra distance before we give up attacking (prevents flickering)

    private float attackTimer;

    public AttackState(Enemy owner, StateMachine<Enemy> machine) : base(owner, machine) { }

    public override void Enter()
    {
        attackTimer = 0f;   // start the interval fresh each time we enter attack range
        owner.StopMoving();
        owner.SetInRange(true);
    }

    public override void Tick()
    {
        if (!owner.HasValidTarget)
        {
            machine.ChangeState(owner.Patrolling);
            return;
        }

        if (owner.DistanceToTarget > owner.Data.AttackRange + ExitBuffer || !owner.HasLineOfSight())
        {
            machine.ChangeState(owner.Chasing);
            return;
        }

        owner.FaceTarget();

        attackTimer += Time.deltaTime;
        if (attackTimer >= owner.Data.AttackInterval)
        {
            attackTimer = 0f;
            owner.Attack();
        }
    }

    public override void Exit()
    {
        owner.SetInRange(false);
    }
}

public class DeadState : State<Enemy>
{
    public DeadState(Enemy owner, StateMachine<Enemy> machine) : base(owner, machine) { }

    public override void Enter()
    {
        // Tell the rest of the game (score, kill count, UI...) and remove the enemy.
        GameEvents.RaiseEnemyDied(owner.Data.ScoreValue);
        Object.Destroy(owner.gameObject);
    }
}