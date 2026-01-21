using UnityEngine;

namespace Main.Gameplay
{
    [RequireComponent(typeof(Collider))]
    public class ObstacleHandler : MonoBehaviour
    {
        private IObstacleBehavior behavior;

        public Detector detector;

        private void Awake()
        {
            behavior = GetComponent<IObstacleBehavior>();

            if (behavior == null)
                Debug.LogError("ObstacleBehavior tidak ditemukan di " + name);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (detector == Detector.Collision) return;
            if (!other.CompareTag("Player")) return;

            behavior?.OnPlayerHit(other.gameObject);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (detector == Detector.Trigger) return;

            if (collision.gameObject.tag != "Player") return;

            behavior?.OnPlayerHit(collision.gameObject);
        }
    }

    public enum Detector
    {
        Trigger,
        Collision,
        Both
    }
}
