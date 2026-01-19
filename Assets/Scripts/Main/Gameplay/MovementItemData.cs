using UnityEngine;
using System.Collections;

namespace Main.Gameplay
{
    [CreateAssetMenu(menuName = "Item/Movement Item Data")]
    public class MovementItemData : ScriptableObject
    {
        [Header("Movement Boost")]
        public float speedBonus = 0f; // +30, +15, -20

        [Header("Duration (Pickup Only)")]
        public float duration = 10f;

        [Header("Camera Effect")]
        public bool useCameraShake = true;
        public float shakeStrength = 0.4f;

        [Header("Post Processing")]
        public bool usePostProcess = true;
        [ColorUsage(true, true)]
        public Color effectColor = Color.cyan;



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
            if (useCameraShake && CameraShake.Instance != null)
            {
                CameraShake.Instance.ShakeForDuration(shakeStrength, duration);
            }

            if (usePostProcess && PostProcessController.Instance != null)
            {
                PostProcessController.Instance.PlayEffect(duration, effectColor);
            }



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
