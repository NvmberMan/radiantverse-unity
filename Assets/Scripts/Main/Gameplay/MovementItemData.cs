using UnityEngine;

namespace Main.Gameplay
{
    [CreateAssetMenu(menuName = "Item/Movement Item Data")]
    public class MovementItemData : ScriptableObject
    {
        [Header("Item Info")]
        public string itemName;

        [Header("Movement Boost")]
        public float speedBonus = 0f; // contoh: +30, +15, -20

        [Header("Duration")]
        public float duration = 10f; // detik
    }
}