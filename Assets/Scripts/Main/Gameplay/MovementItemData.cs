using UnityEngine;

namespace Main.Gameplay
{
    public enum MovementPowerUpType
    {
        Sprint,
        Slow
    }

    [CreateAssetMenu(menuName = "Item/Movement Item Data")]
    public class MovementItemData : ScriptableObject
    {
        [Header("Type")]
        public MovementPowerUpType powerUpType;

        [Header("Movement")]
        public float speedBonus;

        [Header("Duration")]
        public float duration = 5f;

        [Header("Visual")]
        public bool useParticle = true;

        [Header("Camera")]
        public bool useCameraShake = true;
        public float shakeStrength = 0.4f;

        [Header("Post Process")]
        public bool usePostProcess = true;

        [ColorUsage(true, true)]
        public Color effectColor = Color.cyan;

        // =====================
        // APPLY / REMOVE
        // =====================
        public void Apply(CharacterMovement movement)
        {
            //movement.Acceleration += speedBonus;
        }

        public void Remove(CharacterMovement movement)
        {
            //movement.Acceleration -= speedBonus;
        }
    }
}
