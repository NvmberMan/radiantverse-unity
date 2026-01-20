using Main.Mainmenu;
using UnityEngine;

namespace Main.Gameplay
{
    public class PlayerInputJoystick : MonoBehaviour, ICharacterInput
    {
        private Joystick joystick;
        private Transform cameraTransform;

        CharacterMovement CharacterMovement;
        private void Awake()
        {
            CharacterMovement = GetComponent<CharacterMovement>();
        }

        private void Start()
        {
            joystick = MenuManager.instance.GetController<GameplayController>().joystick;
            cameraTransform = Camera.main.transform;
        }

        private void Update()
        {
            HandleMovementInput();
        }

        public void HandleMovementInput()
        {
            float h = joystick.Horizontal;
            float v = joystick.Vertical;

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
            CharacterMovement.Jump();
        }
    }
}