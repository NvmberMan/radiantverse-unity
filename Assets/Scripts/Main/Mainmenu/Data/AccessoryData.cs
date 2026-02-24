using UnityEngine;

namespace Main.Mainmenu
{
    [CreateAssetMenu(fileName = "NewAccessory", menuName = "Game/Accessory Data")]
    public class AccessoryData : ShopItemData
    {
        public string spineSkinName;
        public string category;

        public override void OnPurchaseSuccess()
        {
        }
    }
}