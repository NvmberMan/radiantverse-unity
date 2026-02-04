using Main.Mainmenu;
using UnityEngine;


namespace Main.Gameplay
{
    public class ItemCount : MonoBehaviour
    {
        public int itemPowerCount = 0;
        public int itemSlowerCount = 0;
        public int total;

        public void GetItem(bool isPower)
        {
            total++;
            if (isPower)
            {
                itemPowerCount++;
            }
            else
            {
                itemSlowerCount++;
            }

            if (itemPowerCount >= 3 && !AchievementManager.Instance.CheckAchievement("Chain Reaction"))
            {
                if (PlayerLocalData.playerStats != null)
                {
                    FirestoreModel.UnlockAchievement("Chain Reaction");
                    MenuManager.instance.GetController<UniversalController>().ShowAchievementUnlockedPopup("Chain Reaction");
                }
            }
        }

    }

}
