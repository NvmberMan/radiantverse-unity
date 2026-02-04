using Firebase.Auth;
using Main.Mainmenu;
using System.Collections.Generic;
using UnityEngine;

public class ItemDailyRewardCrate : ItemDailyReward
{
    public ShopItemData ShopItemData;

    public override void ClaimReward()
    {
        ShopPurchaseService.TryGet(
            ShopItemData, 
            onSuccess: () =>
            {
                base.ClaimReward();
            },
            onInvalidItem: () => 
            {

            }
        );
    }
}
