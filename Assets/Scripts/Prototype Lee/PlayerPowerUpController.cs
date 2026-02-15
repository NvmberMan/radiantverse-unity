using Main.Gameplay;
using UnityEngine;

public class PlayerPowerUpController : BasePowerUpController
{
    [Header("Visual")]
    [SerializeField] private ParticleSystem powerUpParticle;

    protected override void OnPowerUpApplied(MovementItemData item)
    {
        // Logika Suara
        if (item.powerUpType == MovementPowerUpType.Sprint)
            AudioManager.Instance.PlaySFX("power up");
        else
            AudioManager.Instance.PlaySFX("power down");

        PlayVisual(item);
    }

    protected override void OnPowerUpEnded()
    {
        StopVisual();
    }

    private void PlayVisual(MovementItemData item)
    {
        StopVisual();
        if (item.useParticle && item.powerUpType == MovementPowerUpType.Sprint)
        {
            powerUpParticle.Play(true);
        }

        if (item.usePostProcess && PostProcessController.Instance != null)
        {
            float chromatic = item.powerUpType == MovementPowerUpType.Slow ? 1f : 0.25f;
            PostProcessController.Instance.EnableEffect(item.effectColor, chromatic);
        }

        if (item.useCameraShake && CameraShake.Instance != null)
        {
            CameraShake.Instance.ShakeForSeconds(item.shakeStrength, Mathf.Min(3f, item.duration));
        }
    }

    private void StopVisual()
    {
        if (powerUpParticle != null) powerUpParticle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        if (PostProcessController.Instance != null) PostProcessController.Instance.DisableEffect();
        if (CameraShake.Instance != null) CameraShake.Instance.StopShake();
    }
}