using UnityEngine;
using System.Collections.Generic;
using Main.Mainmenu;

namespace Main.Gameplay
{
    public class RacePositionSystemWaypoint : MonoBehaviour
    {
        public static RacePositionSystemWaypoint Instance;

        private bool isPlayerFinished = false;
        private int finalRank = -1;

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

        //void UpdateRanking()
        //{
        //    if (isPlayerFinished) return; // kalau sudah finish, stop update rank

        //    allRacers.Sort((a, b) =>
        //        b.progressValue.CompareTo(a.progressValue));

        //    int playerRank =
        //        allRacers.FindIndex(r =>
        //            r.transform == GameManager.Instance.playerTransform) + 1;

        //    gameplayGUIView.UpdateRank(playerRank, allRacers.Count);
        //}

        void UpdateRanking()
        {
            allRacers.Sort((a, b) =>
            {
                if (a.finishRank > 0 && b.finishRank > 0)
                    return a.finishRank.CompareTo(b.finishRank);

                if (a.finishRank > 0) return -1;
                if (b.finishRank > 0) return 1;

                return b.progressValue.CompareTo(a.progressValue);
            });

            int playerRank =
                allRacers.FindIndex(r =>
                    r.transform == GameManager.Instance.playerTransform) + 1;

            gameplayGUIView.UpdateRank(playerRank, allRacers.Count);
        }


    }
}
