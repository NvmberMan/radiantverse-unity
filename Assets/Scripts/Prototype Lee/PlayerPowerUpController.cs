using Main.Gameplay;
using UnityEngine;

public class PlayerPowerUpController : MonoBehaviour
{
    [Header("Visual")]
    [SerializeField] private ParticleSystem powerUpParticle;

    private CharacterMovement movement;
    private float defaultAcceleration;
    private float defaultAirAcceleration;

    private float timer;
    private bool isActive;

    private void Awake()
    {
        movement = GetComponent<CharacterMovement>();
        defaultAcceleration = movement.Acceleration;
        defaultAirAcceleration = movement.AirAcceleration;
    }

    private void Update()
    {
        if (!isActive) return;

        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            EndPowerUp();
        }
    }

    // ============================
    // APPLY = OVERRIDE TOTAL
    // ============================
    public void ApplyPowerUp(MovementItemData item)
    {
        // ❌ JANGAN RESTORE MOVEMENT
        StopVisual(); // hanya matikan efek lama

        // 🔒 SET BARU
        isActive = true;
        timer = item.duration;

        movement.Acceleration = defaultAcceleration + item.speedBonus;
        movement.AirAcceleration = defaultAirAcceleration + item.speedBonus;

        if(item.powerUpType == MovementPowerUpType.Sprint)
        {
            AudioManager.Instance.PlaySFX("power up");
        }
        else
        {
            AudioManager.Instance.PlaySFX("power down");
        }

        PlayVisual(item);
    }

    // ============================
    // END = SATU-SATUNYA RESTORE
    // ============================
    private void EndPowerUp()
    {
        isActive = false;
        timer = 0f;

        movement.Acceleration = defaultAcceleration;
        movement.AirAcceleration = defaultAirAcceleration;
        StopVisual();

        movement.GetComponent<ItemCount>().itemPowerCount = 0;
        movement.GetComponent<ItemCount>().itemSlowerCount = 0;
    }

    // ============================
    // VISUAL
    // ============================
    private void PlayVisual(MovementItemData item)
    {
        StopVisual();

        if (item.useParticle && item.powerUpType == MovementPowerUpType.Sprint)
        {
            powerUpParticle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            powerUpParticle.Play(true);
        }

        if (item.usePostProcess && PostProcessController.Instance != null)
        {
            float chromatic = item.powerUpType == MovementPowerUpType.Slow ? 1f : 0.25f;
            PostProcessController.Instance.EnableEffect(item.effectColor, chromatic);
        }

        if (item.useCameraShake && CameraShake.Instance != null)
        {
            CameraShake.Instance.ShakeForSeconds(
                item.shakeStrength,
                Mathf.Min(3f, item.duration)
            );
        }
    }

    private void StopVisual()
    {
        if (powerUpParticle != null)
            powerUpParticle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        if (PostProcessController.Instance != null)
            PostProcessController.Instance.DisableEffect();

        if (CameraShake.Instance != null)
            CameraShake.Instance.StopShake();
    }
}
