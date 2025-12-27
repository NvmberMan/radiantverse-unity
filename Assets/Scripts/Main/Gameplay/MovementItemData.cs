//using UnityEngine;

//namespace Main.Gameplay
//{
//    [CreateAssetMenu(menuName = "Item/Movement Item Data")]
//    public class MovementItemData : ScriptableObject
//    {
//        [Header("Item Info")]
//        public string itemName;

//        [Header("Movement Boost")]
//        public float speedBonus = 0f; // contoh: +30, +15, -20

//        [Header("Duration")]
//        public float duration = 10f; // detik
//    }
//}

using UnityEngine;
using System.Collections;

namespace Main.Gameplay
{
    [CreateAssetMenu(menuName = "Item/Movement Item Data")]
    public class MovementItemData : ScriptableObject
    {
        [Header("Item Info")]
        public string itemName;

        [Header("Movement Boost")]
        public float speedBonus = 0f; // +30, +15, -20

        [Header("Duration (Pickup Only)")]
        public float duration = 10f;

        // ============================
        // APPLY EFFECT
        // ============================
        public void Apply(CharacterMovement movement)
        {
            movement.Acceleration += speedBonus;
        }

        // ============================
        // REMOVE EFFECT
        // ============================
        public void Remove(CharacterMovement movement)
        {
            movement.Acceleration -= speedBonus;
        }

        // ============================
        // PICKUP MODE (WITH TIMER)
        // ============================
        public void ApplyWithDuration(CharacterMovement movement, MonoBehaviour runner)
        {
            runner.StartCoroutine(EffectRoutine(movement));
        }

        private IEnumerator EffectRoutine(CharacterMovement movement)
        {
            Apply(movement);
            yield return new WaitForSeconds(duration);
            Remove(movement);
        }
    }
}
