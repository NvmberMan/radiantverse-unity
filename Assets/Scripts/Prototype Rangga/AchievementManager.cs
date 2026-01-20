using Main.Mainmenu;
using System.Collections.Generic;
using UnityEngine;

public class AchievementManager : MonoBehaviour
{
    public static AchievementManager Instance;

    public List<AchievementItem> achievementItems = new List<AchievementItem>();

    public void Awake()
    {
        Instance = this;
    }

    public AchievementItem GetAchievement(string id)
    {
        return achievementItems.Find(a => a.id == id);
    }
}
