using UnityEngine;
using Main.Gameplay;

public abstract class BasePowerUpController : MonoBehaviour
{
    protected CharacterMovement movement;
    protected float defaultAcceleration;
    protected float defaultAirAcceleration;

    protected float timer;
    protected bool isActive;

    protected virtual void Awake()
    {
        movement = GetComponent<CharacterMovement>();
        defaultAcceleration = movement.Acceleration;
        defaultAirAcceleration = movement.AirAcceleration;
    }

    protected virtual void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.isPaused) return;
        if (!isActive) return;

        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            EndPowerUp();
        }
    }

    public virtual void ApplyPowerUp(MovementItemData item)
    {
        isActive = true;
        timer = item.duration;

        movement.Acceleration = defaultAcceleration + item.speedBonus;
        movement.AirAcceleration = defaultAirAcceleration + item.speedBonus;

        OnPowerUpApplied(item);
    }

    public virtual void EndPowerUp()
    {
        isActive = false;
        timer = 0f;

        movement.Acceleration = defaultAcceleration;
        movement.AirAcceleration = defaultAirAcceleration;

        // Reset item count jika ada componentnya (opsional check)
        var itemCount = GetComponent<ItemCount>();
        if (itemCount != null)
        {
            itemCount.itemPowerCount = 0;
            itemCount.itemSlowerCount = 0;
        }

        OnPowerUpEnded();
    }

    // Method "Hook" untuk di-override di subclass (Visual, Sound, dll)
    protected abstract void OnPowerUpApplied(MovementItemData item);
    protected abstract void OnPowerUpEnded();
}