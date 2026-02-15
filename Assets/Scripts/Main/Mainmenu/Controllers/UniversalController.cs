using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

namespace Main.Mainmenu
{
    public class UniversalController : Controller
    {
        public void ShowAchievementUnlockedPopup(string achievementId)
        {
            AchievementUnlockedView achievementUnlockedView = (AchievementUnlockedView)GetView("achievement unlocked");
            achievementUnlockedView.Show();
            achievementUnlockedView.Init(AchievementManager.Instance.GetAchievement(achievementId));

            AudioManager.Instance.PlaySFX("get achievement");
        }
        public void ShowGetItemPopup(AccessoryData data)
        {
            GetItemView getItemView = (GetItemView)GetView("get item");
            getItemView.Show();
            getItemView.Init(data);

            AudioManager.Instance.PlaySFX("get item");
        }


        public void ShowCratePopup(CrateData crateData)
        {
            CrateView crateView = (CrateView)GetView("crate");
            crateView.Show();
            crateView.Init(crateData);
        }
    }
}