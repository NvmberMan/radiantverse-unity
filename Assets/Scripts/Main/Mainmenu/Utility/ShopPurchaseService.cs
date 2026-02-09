using Firebase.Auth;
using Main.Mainmenu;
using System.Collections.Generic;

public static class ShopPurchaseService
{
    public static void TryBuy(
        ShopItemData item,
        System.Action onSuccess,
        System.Action onNotEnoughMoney,
        System.Action onInvalidItem = null
    )
    {
        if (item == null)
        {
            onInvalidItem?.Invoke();
            return;
        }

        var stats = PlayerLocalData.playerStats;
        var inventory = PlayerLocalData.inventoryData;

        if (stats.ArradiusDollar < item.price)
        {
            onNotEnoughMoney?.Invoke();
            return;
        }

        stats.ArradiusDollar -= item.price;

        if (item is AccessoryData accessory)
        {
            if (!inventory.UnlockedAccessories.Contains(accessory.spineSkinName))
                inventory.UnlockedAccessories.Add(accessory.spineSkinName);
        }
        else if (item is CrateData crate)
        {
            MenuManager.instance
                .GetController<UniversalController>()
                .ShowCratePopup(crate);
        }

        SyncDatabase();
        onSuccess?.Invoke();
    }

    public static void TryGet(
    ShopItemData item,
    System.Action onSuccess,
    System.Action onInvalidItem = null
)
    {
        if (item == null)
        {
            onInvalidItem?.Invoke();
            return;
        }

        var stats = PlayerLocalData.playerStats;
        var inventory = PlayerLocalData.inventoryData;

        if (item is AccessoryData accessory)
        {
            if (!inventory.UnlockedAccessories.Contains(accessory.spineSkinName))
                inventory.UnlockedAccessories.Add(accessory.spineSkinName);
        }
        else if (item is CrateData crate)
        {
            MenuManager.instance
                .GetController<UniversalController>()
                .ShowCratePopup(crate);
        }

        SyncDatabase();
        onSuccess?.Invoke();
    }

    private static void SyncDatabase()
    {
        string uid = FirebaseAuth.DefaultInstance.CurrentUser.UserId;

        FirestoreModel.SavePlayerStats(uid, new Dictionary<string, object>
        {
            { "ArradiusDollar", PlayerLocalData.playerStats.ArradiusDollar }
        });

        FirestoreModel.SaveInventoryData(uid, new Dictionary<string, object>
        {
            { "UnlockedAccessories", PlayerLocalData.inventoryData.UnlockedAccessories }
        });
    }
}
