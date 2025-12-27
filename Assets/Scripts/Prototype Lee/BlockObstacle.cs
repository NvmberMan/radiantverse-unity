using UnityEngine;

namespace Main.Gameplay
{
    public class BlockObstacle : MonoBehaviour, IObstacleBehavior
    {
        public void OnPlayerHit(GameObject player)
        {
            // Tidak perlu apa-apa
            // Collider fisik yang menghalangi
        }
    }
}
