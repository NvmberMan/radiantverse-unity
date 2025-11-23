using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CharacterMovement : CharacterSystem
{
    [Header("Movement Settings")]
    [SerializeField] float acceleration = 8f;
    [SerializeField] float airAcceleration = 4f;

    [Header("Detection Settings")]
    [SerializeField] Transform groundDetectorPoint;
    [SerializeField] float groundCheckDistance = 0.3f;

    Rigidbody rb;
    public bool _isGrounded;

    public float Acceleration
    {
        get => acceleration;
        set => acceleration = value;
    }

    protected override void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    protected override void Update()
    {
        base.Update(); // RUN INPUT HERE
        CheckGround();
    }

    void CheckGround()
    {
        _isGrounded = Physics.Raycast(
            groundDetectorPoint.position,
            Vector3.down,
            groundCheckDistance
        );
    }

    public void MoveToDir(Vector3 direction)
    {
        Vector3 vel = rb.linearVelocity;
        vel.x = direction.x * acceleration;
        vel.z = direction.z * acceleration;

        //if (vel.z < 0) { vel.z = 0; }

        rb.linearVelocity = vel;
    }

    public void Jump()
    {
        if (_isGrounded)
        {
            rb.AddForce(Vector3.up * 2f, ForceMode.Impulse);
        }
    }
}
