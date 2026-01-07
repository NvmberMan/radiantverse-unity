using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Main.Mainmenu;

namespace Main.Gameplay
{
    public class RacePositionSystemWaypoint : MonoBehaviour
    {
        [Header("References")]
        public List<RacerProgress> allRacers;


        private GameplayGUIView gameplayGUIView;
        private void Start()
        {
            gameplayGUIView = (GameplayGUIView)MenuManager.instance.GetController<GameplayController>().GetView("gameplay gui");
        }

        private void Update()
        {
            if (!GameManager.Instance.isGameActive) return;

            // Sort berdasarkan progress
            allRacers = allRacers
                .OrderByDescending(r => r.currentWaypointIndex)
                .ThenBy(r => r.distanceToNextWaypoint)
                .ToList();

            int playerRank = allRacers
                .FindIndex(r => r.transform == GameManager.Instance.playerTransform) + 1;

            gameplayGUIView.UpdateRank(playerRank, allRacers.Count);
        }
    }
}
