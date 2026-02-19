//using UnityEngine;

//namespace Main.Gameplay
//{
//    public class ItemPickup : MonoBehaviour
//    {
//        public MovementItemData itemData;

//        ItemCount playerItemCount;

//        private void Start()
//        {
//            playerItemCount = GameManager.Instance.playerTransform.GetComponent<ItemCount>();
//        }

//        private void OnTriggerEnter(Collider other)
//        {
//            if (!other.CompareTag("Player") && !other.CompareTag("NPC")) return;

//            var controller = other.GetComponent<BasePowerUpController>();
//            if (controller == null) return;

//            if (other.CompareTag("Player"))
//            {
//                if(itemData.powerUpType == MovementPowerUpType.Sprint && playerItemCount.itemSlowerCount > 0)
//                {
//                    playerItemCount.itemSlowerCount = 0;
//                    playerItemCount.itemPowerCount = 0;

//                }
//                else if (itemData.powerUpType == MovementPowerUpType.Slow && playerItemCount.itemPowerCount > 0)
//                {
//                    playerItemCount.itemSlowerCount = 0;
//                    playerItemCount.itemPowerCount = 0;

//                    Debug.Log("Pwer downn");
//                }

//                playerItemCount.GetItem(itemData.powerUpType == MovementPowerUpType.Sprint);
//            }

//            controller.ApplyPowerUp(itemData);
//            Destroy(gameObject);
//        }
//    }
//}


using UnityEngine;

namespace Main.Gameplay
{
    public class ItemPickup : MonoBehaviour
    {
        public MovementItemData itemData;

        private ItemCount playerItemCount;
        private PowerUpSpawner mySpawner; // Menyimpan referensi ke spawner

        private void Start()
        {
            playerItemCount = GameManager.Instance.playerTransform.GetComponent<ItemCount>();
        }

        // Fungsi ini dipanggil otomatis oleh PowerUpSpawner saat spawn
        public void SetSpawner(PowerUpSpawner spawner)
        {
            mySpawner = spawner;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player") && !other.CompareTag("NPC")) return;

            var controller = other.GetComponent<BasePowerUpController>();
            if (controller == null) return;

            if (other.CompareTag("Player"))
            {
                if (itemData.powerUpType == MovementPowerUpType.Sprint && playerItemCount.itemSlowerCount > 0)
                {
                    playerItemCount.itemSlowerCount = 0;
                    playerItemCount.itemPowerCount = 0;
                }
                else if (itemData.powerUpType == MovementPowerUpType.Slow && playerItemCount.itemPowerCount > 0)
                {
                    playerItemCount.itemSlowerCount = 0;
                    playerItemCount.itemPowerCount = 0;
                    Debug.Log("Power down");
                }

                playerItemCount.GetItem(itemData.powerUpType == MovementPowerUpType.Sprint);
            }

            controller.ApplyPowerUp(itemData);

            // 1. Lapor ke Spawner untuk mulai hitung mundur memunculkan item baru
            if (mySpawner != null)
            {
                mySpawner.StartRespawnTimer();
            }

            // 2. Hancurkan item ini secara permanen
            Destroy(gameObject);
        }
    }
}

