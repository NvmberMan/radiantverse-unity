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
        public int claimedDay = 1;

        [Header("UI References")]
        public Transform itemContainer;
        public List<GameObject> dailyRewardItemPrefab = new List<GameObject>();


        public override void Show()
        {
            base.Show();

            foreach (Transform child in itemContainer)
            {
                Destroy(child.gameObject);
            }

            for (int i = 1; i <= dailyRewardItemPrefab.Count; i++)
            {
                ItemDailyReward itemReward = Instantiate(dailyRewardItemPrefab[i - 1], itemContainer).GetComponent<ItemDailyReward>();
                if (i <= claimedDay + 1)
                {
                    itemReward.OnClaim();
                }

                else if (claimedDay < dailyStreak && i <= dailyStreak + 1)
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