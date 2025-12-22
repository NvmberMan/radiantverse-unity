using UnityEngine;

namespace Main.Gameplay
{
    public class Checkpoint : MonoBehaviour
    {
        public BoxCollider trigger;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                other.GetComponent<CharacterSpawn>().spawnPoint = transform.position;

                gameObject.SetActive(false);
            }
        }
    }
}