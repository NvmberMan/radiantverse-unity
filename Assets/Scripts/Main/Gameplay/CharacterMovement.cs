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
    Animator anim;
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
        anim = graphics.GetComponent<Animator>();
        rb.freezeRotation = true;
    }

    protected override void Update()
    {
        base.Update(); // RUN INPUT HERE
        CheckGround();

        anim.SetBool("IsGrounded", _isGrounded);
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
        anim.SetBool("Running", true);

        rb.linearVelocity = vel;
    }

    public void StopMoving()
    {
        //x y z
        Vector3 vel = rb.linearVelocity;

        vel.x = 0;
        vel.z = 0;

        rb.linearVelocity = vel;
        anim.SetBool("Running", false);
    }

    public void Jump()
    {
        if (_isGrounded)
        {
            rb.AddForce(Vector3.up * 2f, ForceMode.Impulse);
            anim.SetTrigger("Jump");
        }
    }
}
