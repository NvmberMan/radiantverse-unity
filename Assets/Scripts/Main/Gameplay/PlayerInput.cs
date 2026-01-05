using UnityEngine;

namespace Main.Gameplay
{
    public class PlayerInput : MonoBehaviour, ICharacterInput
    {
        CharacterMovement CharacterMovement;

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

            Vector3 dir = new Vector3(h, 0, v).normalized;


            if (h != 0 || v != 0)
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
