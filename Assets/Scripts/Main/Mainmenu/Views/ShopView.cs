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
        public List<ShopItemData> shopDatabase;

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
                bool isOwned = false;
                if (item is AccessoryData acc)
                    isOwned = PlayerLocalData.inventoryData.UnlockedAccessories.Contains(acc.spineSkinName);

                ShopItemUI newItem = Instantiate(itemPrefab, itemContainer);
                newItem.Setup(item, this, isOwned);
            }
        }

        private void GrantAccessory(AccessoryData accessory)
        {
            var inventory = PlayerLocalData.inventoryData;
            if (!inventory.UnlockedAccessories.Contains(accessory.spineSkinName))
            {
                inventory.UnlockedAccessories.Add(accessory.spineSkinName);
            }
        }

        public void TryBuy(ShopItemData item)
        {
            var stats = PlayerLocalData.playerStats;
            if (stats.ArradiusDollar < item.price)
            {
                Debug.LogError("Money not enough!");
                return;
            }

            stats.ArradiusDollar -= item.price;

            if (item is CrateData crate)
            {
                MenuManager.instance.GetController<UniversalController>().ShowCratePopup(crate);


            }
            else if (item is AccessoryData accessory)
            {
                GrantAccessory(accessory);
            }

            UpdateDatabaseSync();
            RefreshShopUI();
        }

        private void UpdateDatabaseSync()
        {
            string uid = FirebaseAuth.DefaultInstance.CurrentUser.UserId;

            var statsUpdates = new Dictionary<string, object> { { "ArradiusDollar", PlayerLocalData.playerStats.ArradiusDollar } };
            FirestoreModel.SavePlayerStats(uid, statsUpdates);

            var invUpdates = new Dictionary<string, object> { { "UnlockedAccessories", PlayerLocalData.inventoryData.UnlockedAccessories } };
            FirestoreModel.SaveInventoryData(uid, invUpdates);
        }
    }
}