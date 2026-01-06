using UnityEngine;

namespace Main.Gameplay
{
    [RequireComponent(typeof(Rigidbody))]
    public class CharacterMovement : CharacterSystem
    {
        [Header("Movement Settings")]
        [SerializeField] float acceleration = 8f;
        [SerializeField] float airAcceleration = 4f;
        [SerializeField] float jumpForce = 4f;

        [Header("Detection Settings")]
        [SerializeField] Transform groundDetectorPoint;
        [SerializeField] Vector3 groundBoxSize = new Vector3(0.5f, 0.2f, 0.5f);
        [SerializeField] float groundCheckDistance = 0.3f;
        [SerializeField] LayerMask groundLayer;

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
            base.Update();

            CheckGround();

            anim?.SetBool("IsGrounded", _isGrounded);
        }

        void CheckGround()
        {
            Vector3 origin = groundDetectorPoint.position;

            // BoxCast downward
            RaycastHit hit;
            bool grounded = Physics.BoxCast(
                origin,
                groundBoxSize * 0.5f,
                Vector3.down,
                out hit,
                Quaternion.identity,
                groundCheckDistance,
                groundLayer
            );

            _isGrounded = grounded;
        }

        public void MoveToDir(Vector3 direction)
        {
            Vector3 vel = rb.linearVelocity;

            float accel = _isGrounded ? acceleration : airAcceleration;

            vel.x = direction.x * accel;
            vel.z = direction.z * accel;

            //if (vel.z < 0) { vel.z = 0; }
            anim?.SetBool("Running", true);

            rb.linearVelocity = vel;
        }

        public void StopMoving()
        {
            //x y z
            Vector3 vel = rb.linearVelocity;

            vel.x = 0;
            vel.z = 0;

            rb.linearVelocity = vel;
            anim?.SetBool("Running", false);
        }

        public void Jump()
        {
            if (_isGrounded)
            {
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                anim?.SetTrigger("Jump");
            }
        }


        private void OnDrawGizmos()
        {
            if (groundDetectorPoint == null) return;

            Gizmos.color = _isGrounded ? Color.green : Color.red;

            Vector3 origin = groundDetectorPoint.position;
            Vector3 halfExtents = groundBoxSize * 0.5f;
            Vector3 end = origin + Vector3.down * groundCheckDistance;

            // START BOX
            Gizmos.matrix = Matrix4x4.TRS(origin, Quaternion.identity, Vector3.one);
            Gizmos.DrawWireCube(Vector3.zero, groundBoxSize);

            // END BOX
            Gizmos.matrix = Matrix4x4.TRS(end, Quaternion.identity, Vector3.one);
            Gizmos.DrawWireCube(Vector3.zero, groundBoxSize);

            // RESET MATRIX (BIAR GA NGACAU)
            Gizmos.matrix = Matrix4x4.identity;

            // CONNECTING LINE
            Gizmos.DrawLine(origin, end);
        }

    }
}

