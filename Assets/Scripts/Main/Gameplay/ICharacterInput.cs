using UnityEngine;

namespace Main.Gameplay
{
    public interface ICharacterInput
    {
        void HandleMovementInput();
        void HandleJumpInput();
    }

}