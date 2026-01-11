using System.Collections.Generic;
using UnityEngine;

namespace Main.Mainmenu
{
    [CreateAssetMenu(fileName = "New Crate Data", menuName = "Game/Crate Data")]
    public class CrateData : ShopItemData
    {
        public Sprite iconNoBackground;
        public List<AccessoryData> rewardPool;
        public AccessoryData RollGacha()
        {
            int randomIndex = Random.Range(0, rewardPool.Count);
            return rewardPool[randomIndex];
        }

        public override void OnPurchaseSuccess()
        {
        }
    }
}