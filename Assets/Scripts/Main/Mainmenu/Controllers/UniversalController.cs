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
        }
        public void ShowGetItemPopup(AccessoryData data)
        {
            GetItemView getItemView = (GetItemView)GetView("get item");
            getItemView.Show();
            getItemView.Init(data);
        }


        public void ShowCratePopup(CrateData crateData)
        {
            CrateView crateView = (CrateView)GetView("crate");
            crateView.Show();
            crateView.Init(crateData);
        }
    }
}