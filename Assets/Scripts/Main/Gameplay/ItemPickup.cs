using UnityEngine;

namespace Main.Gameplay
{
    public class ItemPickup : MonoBehaviour
    {
        public MovementItemData itemData;

        ItemCount playerItemCount;

        private void Start()
        {
            playerItemCount = GameManager.Instance.playerTransform.GetComponent<ItemCount>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;

            if (other.CompareTag("Player"))
            {
                if(itemData.powerUpType == MovementPowerUpType.Sprint && playerItemCount.itemSlowerCount > 0)
                {
                    playerItemCount.itemSlowerCount = 0;
                    playerItemCount.itemPowerCount = 0;
                }
                else if (itemData.powerUpType == MovementPowerUpType.Slow && playerItemCount.itemPowerCount > 0)
                {
                    playerItemCount.itemSlowerCount = 0;
                    playerItemCount.itemPowerCount = 0;
                }

                playerItemCount.GetItem(itemData.powerUpType == MovementPowerUpType.Sprint);
            }

            var controller = other.GetComponent<PlayerPowerUpController>();
            if (controller == null) return;

            controller.ApplyPowerUp(itemData);
            Destroy(gameObject);
        }
    }
}
