using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class AIEntity : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 10f; // Nilai default di prefab
    public float rotationSpeed = 10f;
    public float arrivalDistance = 1.5f;

    [Header("Ground Detection")]
    public LayerMask groundLayer;
    public float rayDistance = 2f;

    private Transform[] waypoints;
    private int currentWaypointIndex = 0;
    private Rigidbody rb;

    public void SetRoute(Transform[] newWaypoints)
    {
        waypoints = newWaypoints;
        currentWaypointIndex = 0;
    }

    // Fungsi baru untuk mengatur speed dari Spawner
    public void SetSpeed(float newSpeed)
    {
        if (newSpeed > 0) // Hanya ganti jika nilainya di atas 0
        {
            speed = newSpeed;
        }
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.constraints = RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationY;
    }

    void FixedUpdate()
    {
        if (waypoints == null || waypoints.Length == 0 || currentWaypointIndex >= waypoints.Length)
            return;

        Transform target = waypoints[currentWaypointIndex];
        Vector3 direction = target.position - rb.position;
        direction.y = 0;

        Vector3 moveVelocity = direction.normalized * speed;
        moveVelocity.y = rb.linearVelocity.y;
        rb.linearVelocity = moveVelocity;

        AdjustRotation(direction);

        float distance = Vector2.Distance(new Vector2(rb.position.x, rb.position.z),
                                         new Vector2(target.position.x, target.position.z));

        if (distance < arrivalDistance)
        {
            currentWaypointIndex++;
            if (currentWaypointIndex >= waypoints.Length)
            {
                Destroy(gameObject);
            }
        }
    }

    void AdjustRotation(Vector3 moveDirection)
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up * 0.5f, Vector3.down, out hit, rayDistance, groundLayer))
        {
            Quaternion targetRot = Quaternion.LookRotation(moveDirection, hit.normal);
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRot, rotationSpeed * Time.fixedDeltaTime));
        }
    }
}