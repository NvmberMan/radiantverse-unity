using Spine.Unity;
using System;
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
        public bool _isFreeze;
        private float nextJumpTime = 0f;
        private string currentAnimation = "";
        private bool _wasGrounded;

        [Header("Slope Settings")]
        [SerializeField] float maxSlopeAngle = 45f;
        private Vector3 slopeNormal;
        private Vector3 prevScale;

        [Header("Step Offset Settings")]
        [SerializeField] float stepHeight = 0.3f;
        [SerializeField] float stepSmooth = 0.1f;

        public Action isJumping;

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

        private void Start()
        {
            prevScale = skeletonAnimation.transform.localScale;
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
            _wasGrounded = _isGrounded;

            Vector3 origin = groundDetectorPoint.position;
            Vector3 halfExtents = groundBoxSize * 0.5f;

            bool grounded = Physics.CheckBox(
                origin,
                halfExtents,
                Quaternion.identity,
                groundLayer
            );

            RaycastHit hit;

            // Kalau belum kena dari CheckBox, lakukan BoxCast
            if (!grounded)
            {
                grounded = Physics.BoxCast(
                    origin,
                    halfExtents,
                    Vector3.down,
                    out hit,
                    Quaternion.identity,
                    groundCheckDistance,
                    groundLayer
                );
            }
            else
            {
                // Kalau CheckBox sudah grounded, kita tetap perlu normal tanah
                Physics.Raycast(origin, Vector3.down, out hit, groundCheckDistance + 0.5f, groundLayer);
            }

            if (grounded)
            {
                if (hit.collider != null)
                {
                    slopeNormal = hit.normal;

                    // Hitung sudut kemiringan tanah
                    float slopeAngle = Vector3.Angle(slopeNormal, Vector3.up);

                    // selama tidak lebih curam dari batas → tetap grounded
                    _isGrounded = slopeAngle <= maxSlopeAngle;
                }
                else
                {
                    _isGrounded = true;
                }
            }
            else
            {
                _isGrounded = false;
            }

            // Landing SFX
            if (_isGrounded && !_wasGrounded)
            {
                AudioManager.Instance.PlaySFX("Landing");
            }
        }


        public void MoveToDir(Vector3 direction, float horizontalInput = 1)
        {
            if (!GameManager.Instance.isGameActive || GameManager.Instance.isPaused || _isFreeze) return;

            Vector3 vel = rb.linearVelocity;

            float accel = _isGrounded ? acceleration : airAcceleration;

            vel.x = direction.x * accel;
            vel.z = direction.z * accel;

            rb.linearVelocity = vel;

            SetAnimation(walkAnimation, true);

            Flip(horizontalInput);
        }

        void Flip(float horizontalInput)
        {
            if (horizontalInput > 0.01f)
                skeletonAnimation.Skeleton.ScaleX = 1;
            else if (horizontalInput < -0.01f)
                skeletonAnimation.Skeleton.ScaleX = -1;
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

                AudioManager.Instance.PlaySFXAtPoint("Jump", this.transform.position, 6);

                isJumping?.Invoke();
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

