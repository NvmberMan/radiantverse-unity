using UnityEngine;

namespace Main.Gameplay
{
    public class PlayerInput : MonoBehaviour, ICharacterInput
    {
        CharacterMovement CharacterMovement;
        [Header("Camera Reference")]
        [SerializeField] Transform cameraTransform;

        void Awake()
        {
            CharacterMovement = GetComponent<CharacterMovement>();
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

            ////Debug.Log("Input H:" + h + "  V:" + v);  // ← Tambahkan ini

            //Vector3 dir = new Vector3(h, 0, v).normalized;


            //if (h != 0 || v != 0)
            //{
            //    CharacterMovement.MoveToDir(dir);
            //}
            //else
            //{
            //    CharacterMovement.StopMoving();
            //}

            if (!cameraTransform)
            {
                Debug.LogWarning("Camera Transform belum di-assign");
                return;
            }

            // Arah kamera
            Vector3 camForward = cameraTransform.forward;
            Vector3 camRight = cameraTransform.right;

            // Hilangkan pengaruh naik/turun kamera
            camForward.y = 0;
            camRight.y = 0;

            camForward.Normalize();
            camRight.Normalize();

            // Gabungkan WASD dengan arah kamera
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
