using Main.Mainmenu;
using UnityEngine;


namespace Main.Gameplay
{
    public class JumpCount : MonoBehaviour
    {
        CharacterMovement characterMovement;
        int jumpCount = 0;

        private void Start()
        {
            characterMovement = GetComponent<CharacterMovement>();
            characterMovement.isJumping += IsJumping;
        }

        private void IsJumping()
        {
            jumpCount++;

            if (PlayerLocalData.inventoryData != null)
            {
                if (jumpCount >= 100 && !AchievementManager.Instance.CheckAchievement("Boing Boing!"))
                {
                    if (PlayerLocalData.playerStats != null)
                    {
                        FirestoreModel.UnlockAchievement("Boing Boing!");
                        MenuManager.instance.GetController<UniversalController>().ShowAchievementUnlockedPopup("Boing Boing!");
                    }
                }
            }
        }
        
    }

}
