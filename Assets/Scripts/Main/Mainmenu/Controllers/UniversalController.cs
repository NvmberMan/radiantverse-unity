using UnityEngine;

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
    }
}