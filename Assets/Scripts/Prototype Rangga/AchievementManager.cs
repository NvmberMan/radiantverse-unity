using Main.Mainmenu;
using System.Collections.Generic;
using UnityEngine;

public class AchievementManager : MonoBehaviour
{
    public static AchievementManager Instance;

    public List<AchievementItem> achievementItems = new List<AchievementItem>();

    public void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    public AchievementItem GetAchievement(string id)
    {
        return achievementItems.Find(a => a.id == id);
    }

    public bool CheckAchievement(string id)
    {
        return PlayerLocalData.inventoryData.UnlockedAchievements.Contains(id);
    }
}
