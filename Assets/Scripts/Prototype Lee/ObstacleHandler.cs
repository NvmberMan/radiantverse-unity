using UnityEngine;

namespace Main.Gameplay
{
    [RequireComponent(typeof(Collider))]
    public class ObstacleHandler : MonoBehaviour
    {
        private IObstacleBehavior behavior;

        public Detector detector;
        public TargetType targetType = TargetType.Both; // Default ke keduanya

        private void Awake()
        {
            behavior = GetComponent<IObstacleBehavior>();

            if (behavior == null)
                Debug.LogError("ObstacleBehavior tidak ditemukan di " + name);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (detector == Detector.Collision) return;

            // Cek apakah objek yang masuk valid sesuai targetType
            if (IsValidTarget(other.gameObject))
            {
                behavior?.OnPlayerHit(other.gameObject);
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (detector == Detector.Trigger) return;

            // Cek apakah objek yang menabrak valid sesuai targetType
            if (IsValidTarget(collision.gameObject))
            {
                behavior?.OnPlayerHit(collision.gameObject);
            }
        }

        // Fungsi bantuan untuk memvalidasi tag berdasarkan pilihan di Inspector
        private bool IsValidTarget(GameObject obj)
        {
            switch (targetType)
            {
                case TargetType.PlayerOnly:
                    return obj.CompareTag("Player");
                case TargetType.NPCOnly:
                    return obj.CompareTag("NPC");
                case TargetType.Both:
                    return obj.CompareTag("Player") || obj.CompareTag("NPC");
                default:
                    return false;
            }
        }
    }

    public enum Detector
    {
        Trigger,
        Collision,
        Both
    }

    public enum TargetType
    {
        PlayerOnly,
        NPCOnly,
        Both
    }
}