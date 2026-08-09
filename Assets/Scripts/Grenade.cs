using UnityEngine;

// Attach this to the Grenade prefab. Spawned and launched by PlayerBehaviour.ThrowGrenade().
public class Grenade : MonoBehaviour
{
    [Header("Explosion")]
    [SerializeField] private float damage = 0f;          // TODO: set final grenade damage once decided
    [SerializeField] private float explosionRadius = 4f;

    private Vector3 startPosition;
    private Vector3 targetPosition;
    private float throwDuration;
    private float arcHeight;
    private float travelTimer = 0f;
    private bool isLaunched = false;

    // Called immediately after Instantiate to send the grenade on its scripted arc.
    // start / target are world positions, duration is how long the flight takes,
    // peakHeight is how high the arc rises above a straight line from start to target.
    public void Launch(Vector3 start, Vector3 target, float duration, float peakHeight)
    {
        startPosition = start;
        targetPosition = target;
        throwDuration = Mathf.Max(0.01f, duration);
        arcHeight = peakHeight;
        travelTimer = 0f;
        isLaunched = true;

        transform.position = startPosition;
    }

    private void Update()
    {
        if (!isLaunched) return;

        travelTimer += Time.deltaTime;
        float t = Mathf.Clamp01(travelTimer / throwDuration);

        // Straight-line lerp from start to target...
        Vector3 position = Vector3.Lerp(startPosition, targetPosition, t);

        // ...with a parabolic arc layered on top (0 at t=0 and t=1, peaks at t=0.5)
        position.y += 4f * arcHeight * t * (1f - t);

        transform.position = position;

        if (t >= 1f)
        {
            isLaunched = false;
            Explode();
        }
    }

    private void Explode()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, explosionRadius);

        int hitCount = 0;
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Enemy") && hit.TryGetComponent<Enemy>(out var enemy))
            {
                enemy.TakeDamage(damage);
                hitCount++;
            }
        }

        Debug.Log($"{name}: Exploded at {transform.position}, hit {hitCount} enemy(ies).");

        // TODO: instantiate explosion VFX / play a sound here before destroying
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}