using UnityEngine;
using System.Collections.Generic;

namespace Main.Gameplay
{
    public class RacerProgress : MonoBehaviour
    {
        [Header("Progress Info")]
        public int currentWaypointIndex = 0;
        public float distanceToNextWaypoint;

        [Header("Waypoint References")]
        public List<Waypoint> allWaypoints;   // isi berurutan 0 → akhir
        public Transform nextWaypoint;

        private void Start()
        {
            UpdateNextWaypoint();
        }

        private void Update()
        {
            if (nextWaypoint != null)
            {
                distanceToNextWaypoint =
                    Vector3.Distance(transform.position, nextWaypoint.position);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Waypoint")) return;

            Waypoint wp = other.GetComponent<Waypoint>();
            if (wp == null) return;

            // HANYA waypoint yang sesuai index sekarang yang boleh menaikkan progress
            if (wp.waypointIndex == currentWaypointIndex)
            {
                currentWaypointIndex++;
                UpdateNextWaypoint();
            }
        }

        private void UpdateNextWaypoint()
        {
            if (currentWaypointIndex < allWaypoints.Count)
            {
                nextWaypoint = allWaypoints[currentWaypointIndex].transform;
            }
            else
            {
                // Semua waypoint sudah dilewati
                nextWaypoint = null;
                distanceToNextWaypoint = 0f;
            }
        }
    }
}
