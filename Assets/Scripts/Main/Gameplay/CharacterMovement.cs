using Spine.Unity;
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
        [SerializeField] float jumpCooldown = 0.2f;

        [Header("Detection Settings")]
        [SerializeField] Transform groundDetectorPoint;
        [SerializeField] Vector3 groundBoxSize = new Vector3(0.5f, 0.2f, 0.5f);
        [SerializeField] float groundCheckDistance = 0.3f;
        [SerializeField] LayerMask groundLayer;

        [Header("Spine Settings")]
        [SpineAnimation] public string idleAnimation = "idle";
        [SpineAnimation] public string walkAnimation = "run";
        [SpineAnimation] public string jumpAnimation = "jump";

        [HideInInspector] public Rigidbody rb;
        public bool _isGrounded;
        private float nextJumpTime = 0f;
        private string currentAnimation = "";

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
            base.Update();

            CheckGround();

            if (GameManager.Instance.isPaused)
                StopMoving();
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
            if (!GameManager.Instance.isGameActive || GameManager.Instance.isPaused) return;

            Vector3 vel = rb.linearVelocity;

            float accel = _isGrounded ? acceleration : airAcceleration;

            vel.x = direction.x * accel;
            vel.z = direction.z * accel;

            rb.linearVelocity = vel;

            SetAnimation(walkAnimation, true);
        }

        public void StopMoving()
        {
            //x y z
            Vector3 vel = rb.linearVelocity;

            vel.x = 0;
            vel.z = 0;

            rb.linearVelocity = vel;

            SetAnimation(idleAnimation, true);
        }

        public void Jump()
        {
            if (!GameManager.Instance.isGameActive || GameManager.Instance.isPaused) return;


            if (_isGrounded && Time.time >= nextJumpTime)
            {
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);

                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

                nextJumpTime = Time.time + jumpCooldown;

                SetAnimation(jumpAnimation, false);
            }
        }

        private void SetAnimation(string animName, bool loop)
        {
            if (currentAnimation == animName) return;

            skeletonAnimation.AnimationState.SetAnimation(0, animName, loop);
            currentAnimation = animName;
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

