using UnityEngine;
using System.Collections.Generic;

namespace Main.Gameplay.AI
{
    public class WaypointCircuit : MonoBehaviour
    {
        [Header("Circuit Settings")]
        public List<Transform> waypoints = new List<Transform>();
        public Color lineColor = Color.yellow;
        public bool showGizmos = true;

        private void OnDrawGizmos()
        {
            if (!showGizmos || waypoints.Count < 2) return;

            Gizmos.color = lineColor;

            // Menggambar garis antar waypoint
            for (int i = 0; i < waypoints.Count; i++)
            {
                Vector3 current = waypoints[i].position;
                Vector3 next = waypoints[(i + 1) % waypoints.Count].position; // Loop kembali ke awal
                Gizmos.DrawLine(current, next);
                Gizmos.DrawSphere(current, 0.5f);
            }
        }

        // Tombol ajaib untuk Context Menu di Inspector
        [ContextMenu("Auto Find Waypoints")]
        public void AutoFindWaypoints()
        {
            waypoints.Clear();
            // Ambil semua child, lewati parentnya sendiri
            foreach (Transform child in transform)
            {
                if (child != transform)
                    waypoints.Add(child);
            }
            Debug.Log($"Ditemukan {waypoints.Count} waypoint.");
        }
    }
}