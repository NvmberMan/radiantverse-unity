using UnityEngine;

namespace Main.Gameplay
{
    [RequireComponent(typeof(Collider))]
    public class ObstacleHandler : MonoBehaviour
    {
        private IObstacleBehavior behavior;

        private void Awake()
        {
            behavior = GetComponent<IObstacleBehavior>();

            if (behavior == null)
                Debug.LogError("ObstacleBehavior tidak ditemukan di " + name);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;

            behavior?.OnPlayerHit(other.gameObject);
        }
    }
}
