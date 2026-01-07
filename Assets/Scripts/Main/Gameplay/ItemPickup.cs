using UnityEngine;

namespace Main.Gameplay
{
    public class ItemPickup : MonoBehaviour
    {
        public MovementItemData itemData;

        private void Awake()
        {
            if (itemData != null)
            {

                // 🔹 Nama mengikuti ScriptableObject
                gameObject.name = $"Item_{itemData.name}";
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;

            CharacterMovement movement = other.GetComponent<CharacterMovement>();
            if (movement == null) return;

            itemData.ApplyWithDuration(movement, this);
            //Destroy(gameObject);
        }
    }
}
