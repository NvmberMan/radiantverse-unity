using Firebase.Auth;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Main.Mainmenu
{
    public class ShopView : View
    {
        [Header("UI References")]
        public Transform itemContainer;
        public ShopItemUI itemPrefab;
        public TextMeshProUGUI playerMoneyText;

        [Header("Database")]
        public List<AccessoryData> shopDatabase;

        public override void Show()
        {
            base.Show();
            RefreshShopUI();
        }

        public void RefreshShopUI()
        {
            if (PlayerLocalData.playerStats != null)
                playerMoneyText.text = PlayerLocalData.playerStats.ArradiusDollar.ToString() + " $";

            foreach (Transform child in itemContainer) Destroy(child.gameObject);

            foreach (var item in shopDatabase)
            {
                bool isOwned = PlayerLocalData.inventoryData.UnlockedAccessories.Contains(item.spineSkinName);
                ShopItemUI newItem = Instantiate(itemPrefab, itemContainer);
                newItem.Setup(item, this, isOwned);
            }
        }

        public void TryBuyItem(AccessoryData item)
        {
            var stats = PlayerLocalData.playerStats;
            var inventory = PlayerLocalData.inventoryData;
            string uid = FirebaseAuth.DefaultInstance.CurrentUser.UserId;

            if (stats.ArradiusDollar >= item.price)
            {
                stats.ArradiusDollar -= item.price;
                inventory.UnlockedAccessories.Add(item.spineSkinName);

                Dictionary<string, object> statsUpdates = new Dictionary<string, object> {
                    { "ArradiusDollar", stats.ArradiusDollar }
                };
                FirestoreModel.SavePlayerStats(uid, statsUpdates);

                Dictionary<string, object> invUpdates = new Dictionary<string, object> {
                    { "UnlockedAccessories", inventory.UnlockedAccessories }
                };
                FirestoreModel.SaveInventoryData(uid, invUpdates);

                Debug.Log($"Purchased {item.displayName}!");
                RefreshShopUI();
            }
            else
            {
                Debug.LogError("Not enough money!");
            }
        }
    }
}