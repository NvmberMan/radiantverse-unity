using UnityEngine;

namespace Main.Gameplay
{
    public class Checkpoint : MonoBehaviour
    {
        private BoxCollider trigger;

        private void Awake()
        {
            trigger = GetComponent<BoxCollider>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                other.GetComponent<CharacterSpawn>().SetSpawnPoint(transform.position);

                gameObject.SetActive(false);
            }
        }
    }
}