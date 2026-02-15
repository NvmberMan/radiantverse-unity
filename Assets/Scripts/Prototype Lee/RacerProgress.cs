using UnityEngine;

namespace Main.Gameplay
{
    public class RacerProgress : MonoBehaviour
    {
        [Header("Progress Info")]
        public int currentWaypointIndex = 0;
        public int resetWayPointIndex = 0;
        public float progressValue; // SATU angka untuk rank

        private Transform currentWaypoint;
        private Transform nextWaypoint;
        public bool hasFinished = false; // baru: flag finish

        private void Start()
        {
            UpdateWaypoints();
        }

        private void Update()
        {
            UpdateProgressValue();
        }

        void UpdateProgressValue()
        {
            if (hasFinished)
            {
                // Tetap gunakan max progress, tapi jangan hilangkan dari ranking
                progressValue = RacePositionSystemWaypoint.Instance.allWaypoints.Count + 1;
                return;
            }

            if (currentWaypoint == null || nextWaypoint == null)
            {
                progressValue = currentWaypointIndex;
                return;
            }

            // logika continuous progress
            Vector3 a = currentWaypoint.position;
            Vector3 b = nextWaypoint.position;
            Vector3 p = transform.position;

            Vector3 segmentDir = (b - a);
            float segmentLength = segmentDir.magnitude;
            segmentDir.Normalize();

            float projectedDistance = Vector3.Dot(p - a, segmentDir);
            float segmentProgress = Mathf.Clamp01(projectedDistance / segmentLength);

            progressValue = currentWaypointIndex + segmentProgress;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Waypoint")) return;

            Waypoint wp = other.GetComponent<Waypoint>();
            if (wp == null) return;

            if (wp.waypointIndex == currentWaypointIndex + 1)
            {
                currentWaypointIndex++;
                UpdateWaypoints();

                // Jika melewati waypoint terakhir → finish
                if (nextWaypoint == null)
                {
                    hasFinished = true;
                }
            }
        }

        public void ResetTargetWayPoint()
        {
            currentWaypointIndex = resetWayPointIndex;

            hasFinished = false;

            UpdateWaypoints();
        }

        void UpdateWaypoints()
        {
            var wps = RacePositionSystemWaypoint.Instance.allWaypoints;

            currentWaypoint = currentWaypointIndex < wps.Count
                ? wps[currentWaypointIndex].transform
                : null;

            nextWaypoint = currentWaypointIndex + 1 < wps.Count
                ? wps[currentWaypointIndex + 1].transform
                : null;
        }
    }
}
