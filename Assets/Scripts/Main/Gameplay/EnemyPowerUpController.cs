using Main.Gameplay;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Main.Gameplay
{
    public class EnemyPowerUpController : BasePowerUpController
    {
        protected override void OnPowerUpApplied(MovementItemData item)
        {
            if (item.powerUpType == MovementPowerUpType.Sprint)
                AudioManager.Instance.PlaySFXAtPoint("power up", transform.position, 6);
            else
                AudioManager.Instance.PlaySFXAtPoint("power down", transform.position, 6);
        }

        protected override void OnPowerUpEnded()
        {
            //Debug.Log($"Enemy {gameObject.name} power up expired.");
        }
    }
}