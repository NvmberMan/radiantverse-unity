using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using TMPro;

namespace Main.Gameplay
{
    public class RacePositionSystemWaypoint : MonoBehaviour
    {
        [Header("References")]
        public Transform playerTransform;
        public List<RacerProgress> allRacers;

        private void Update()
        {
            if (!GameManager.Instance.isGameActive) return;

            // Sort berdasarkan progress
            allRacers = allRacers
                .OrderByDescending(r => r.currentWaypointIndex)
                .ThenBy(r => r.distanceToNextWaypoint)
                .ToList();

            int playerRank = allRacers
                .FindIndex(r => r.transform == playerTransform) + 1;

            if (GameManager.Instance.rankUIText != null)
            {
                GameManager.Instance.rankUIText.text =
                    $"Position: {playerRank}/{allRacers.Count}";
            }
        }
    }
}
