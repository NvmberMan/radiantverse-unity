using Firebase.Auth;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Main.Mainmenu
{
    public class DailyRewardView : View
    {
        public int dailyStreak = 2;

        [Header("UI References")]
        public Transform itemContainer;
        public List<GameObject> dailyRewardItemPrefab = new List<GameObject>();

        private void Start()
        {
            foreach(Transform child in itemContainer)
            {
                Destroy(child.gameObject);
            }

            for(int i = 1; i <= dailyRewardItemPrefab.Count; i++)
            {
                ItemDailyReward itemReward = Instantiate(dailyRewardItemPrefab[i-1], itemContainer).GetComponent<ItemDailyReward>();
                if(i <= dailyStreak)
                {
                    itemReward.OnClaim();
                }
                else if(i == dailyStreak+1)
                {
                    itemReward.OnReady();
                }
                else
                {
                    itemReward.OnUnReady();
                }
            }
        }
    }
}