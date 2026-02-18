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
using System.Collections; // Dibutuhkan untuk Coroutine

namespace Main.Gameplay
{
    public class ItemPickup : MonoBehaviour
    {
        public MovementItemData itemData;
        public float respawnTime = 5f; // Waktu tunggu sebelum respawn

        private ItemCount playerItemCount;
        private Collider itemCollider;
        private MeshRenderer itemRenderer; // Ganti dengan komponen visual kamu jika bukan MeshRenderer

        private void Start()
        {
            playerItemCount = GameManager.Instance.playerTransform.GetComponent<ItemCount>();
            itemCollider = GetComponent<Collider>();
            itemRenderer = GetComponent<MeshRenderer>(); // Ambil komponen visual
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

            // Panggil fungsi Respawn daripada Destroy
            StartCoroutine(RespawnSequence());
        }

        private IEnumerator RespawnSequence()
        {
            // 1. Sembunyikan item (Matikan visual dan collider)
            ToggleItemState(false);

            // 2. Tunggu selama beberapa detik
            yield return new WaitForSeconds(respawnTime);

            // 3. Munculkan kembali
            ToggleItemState(true);
        }

        private void ToggleItemState(bool state)
        {
            if (itemCollider != null) itemCollider.enabled = state;
            if (itemRenderer != null) itemRenderer.enabled = state;

            // Jika item kamu punya partikel atau objek anak (child objects), 
            // kamu bisa pakai ini sebagai alternatif:
            // transform.GetChild(0).gameObject.SetActive(state);
        }
    }
}

