using Main.Gameplay;
using UnityEngine;

namespace Main.Mainmenu
{
    public class GameplayController : Controller
    {
        private CharacterMovement characterMovement;

        private void Start()
        {
            characterMovement = GameManager.Instance.playerTransform.GetComponent<CharacterMovement>();
        }
        public void Jump()
        {
            characterMovement.Jump();
        }
    }
}