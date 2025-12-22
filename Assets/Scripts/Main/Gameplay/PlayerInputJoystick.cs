using UnityEngine;

namespace Main.Gameplay
{
    public class PlayerInputJoystick : MonoBehaviour, ICharacterInput
    {
        CharacterMovement characterMovement;

        public Joystick joystick;

        private void Awake()
        {
            characterMovement = GetComponent<CharacterMovement>();
        }

        private void Update()
        {
            HandleMovementInput();
            //HandleJumpInput();
        }

        public void HandleMovementInput()
        {
            float h = joystick.Horizontal;
            float v = joystick.Vertical;

            //Debug.Log("Joystick H: " + h + " V: " + v);

            Vector3 dir = new Vector3(h, 0, v).normalized;

            if (h != 0 || v != 0)
            {
                characterMovement.MoveToDir(dir);
            }
            else
            {
                characterMovement.StopMoving();
            }
        }

        public void HandleJumpInput()
        {
            characterMovement.Jump();
        }
    }
}