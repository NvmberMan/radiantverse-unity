using UnityEngine;

namespace Main.Gameplay
{
    public class ItemPickup : MonoBehaviour
    {
        public MovementItemData itemData;

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
