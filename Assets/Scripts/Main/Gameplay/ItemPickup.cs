using UnityEngine;

namespace Main.Gameplay
{
    public class ItemPickup : MonoBehaviour
    {
        public MovementItemData itemData;

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;

            var controller = other.GetComponent<PlayerPowerUpController>();
            if (controller == null) return;

            controller.ApplyPowerUp(itemData);
            Destroy(gameObject);
        }
    }
}
