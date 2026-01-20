using UnityEngine;

namespace Main.Gameplay
{
    public class PlayerInput : MonoBehaviour, ICharacterInput
    {
        private CharacterMovement CharacterMovement;
        private Transform cameraTransform;

        void Awake()
        {
            CharacterMovement = GetComponent<CharacterMovement>();
        }

        private void Start()
        {
            cameraTransform = Camera.main.transform;
        }

        private void Update()
        {
            HandleMovementInput();
            HandleJumpInput();
        }

        public void HandleMovementInput()
        {
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");

            if (!cameraTransform)
            {
                Debug.LogWarning("Camera Transform belum di-assign");
                return;
            }

            Vector3 camForward = cameraTransform.forward;
            Vector3 camRight = cameraTransform.right;

            camForward.y = 0;
            camRight.y = 0;

            camForward.Normalize();
            camRight.Normalize();

            Vector3 dir = (camForward * v + camRight * h).normalized;

            if (dir.magnitude > 0.1f)
            {
                CharacterMovement.MoveToDir(dir);
            }
            else
            {
                CharacterMovement.StopMoving();
            }
        }

        public void HandleJumpInput()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                CharacterMovement.Jump();
            }
        }
    }
}
