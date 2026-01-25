using UnityEngine;
using System.Collections.Generic;
using Main.Mainmenu;

namespace Main.Gameplay
{
    public class RacePositionSystemWaypoint : MonoBehaviour
    {
        public static RacePositionSystemWaypoint Instance;

        [Header("Racers")]
        public List<RacerProgress> allRacers = new List<RacerProgress>();

        [Header("Waypoints (URUT!)")]
        public List<Waypoint> allWaypoints = new List<Waypoint>();

        private GameplayGUIView gameplayGUIView;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            gameplayGUIView =
                (GameplayGUIView)MenuManager.instance
                .GetController<GameplayController>()
                .GetView("gameplay gui");

            InvokeRepeating(nameof(UpdateRanking), 0f, 0.2f); // 5x per detik
        }

        public void SetupPositionAllRacer()
        {
            foreach (var racer in allRacers)
            {
                racer.GetComponent<CharacterSpawn>().SetupStartPoint();
            }
        }

        void UpdateRanking()
        {
            // ranking tetap update walau ada racer yang finish
            allRacers.Sort((a, b) =>
                b.progressValue.CompareTo(a.progressValue));

            int playerRank =
                allRacers.FindIndex(r =>
                    r.transform == GameManager.Instance.playerTransform) + 1;

            gameplayGUIView.UpdateRank(playerRank, allRacers.Count);
        }
    }
}
