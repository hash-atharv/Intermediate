using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    //  State 
    private enum EnemyState { Patrol, Chase }
    private EnemyState currentState = EnemyState.Patrol;

    //  References 
    private NavMeshAgent agent;
    private Transform playerTransform;

    //  Patrol Settings 
    [Header("Patrol")]

    private List<Transform> waypoints = new List<Transform>();
    [SerializeField] private float waypointReachThreshold = 0.5f;
    [SerializeField] private float patrolSpeed = 2f;
    private int currentWaypointIndex = 0;
    private bool waypointsAssigned = false;

    //  Detection Settings 
    [Header("Detection")]
    [SerializeField] private float detectionRadius = 8f;
    [SerializeField] private LayerMask playerLayer;

    //  Chase Settings 
    [Header("Chase")]
    [SerializeField] private float chaseSpeed = 5f;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
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
        if (playerTransform != null)
            agent.SetDestination(playerTransform.position);
    }

    //  Detection 
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