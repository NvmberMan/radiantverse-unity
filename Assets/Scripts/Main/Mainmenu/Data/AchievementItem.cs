using UnityEngine;

namespace Main.Mainmenu
{
    [CreateAssetMenu(fileName = "NewAchievement", menuName = "Achievements/AchievementItem")]
    public class AchievementItem : ScriptableObject
    {
        public string id;
        public string achievementName;
        [TextArea] public string description;
        public Sprite iconPreview;
    }
}