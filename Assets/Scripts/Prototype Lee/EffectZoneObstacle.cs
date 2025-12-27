using UnityEngine;

namespace Main.Gameplay
{
    [RequireComponent(typeof(Collider))]
    public class EffectZoneObstacle : MonoBehaviour, IObstacleBehavior
    {
        [SerializeField] MovementItemData effectData;

        public void OnPlayerHit(GameObject player)
        {
            CharacterMovement movement = player.GetComponent<CharacterMovement>();
            if (movement != null && effectData != null)
            {
                effectData.Apply(movement);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag("Player")) return;

            CharacterMovement movement = other.GetComponent<CharacterMovement>();
            if (movement != null && effectData != null)
            {
                effectData.Remove(movement);
            }
        }
    }
}
