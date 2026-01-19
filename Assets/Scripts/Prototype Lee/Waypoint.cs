using UnityEngine;

namespace Main.Gameplay
{
    public class Waypoint : MonoBehaviour
    {
        public int waypointIndex;

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(transform.position, 0.4f);
        }
    }
}
