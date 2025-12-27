using UnityEngine;

namespace Main.Mainmenu
{
    [CreateAssetMenu(fileName = "NewAccessory", menuName = "Game/Accessory Data")]
    public class AccessoryData : ScriptableObject
    {
        public string spineSkinName;
        public string displayName;
        public Sprite icon;
        public string category;
        public int price;
    }
}