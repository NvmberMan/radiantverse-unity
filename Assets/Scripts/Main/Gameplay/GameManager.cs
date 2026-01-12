using Main.Gameplay.AI;
using Main.Mainmenu;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

namespace Main.Gameplay
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;

        [Header("Game State")]
        public bool isGameActive = true;
        public bool isPaused = false;
        public int currentFinishRank = 1;

        public Transform playerTransform;
        [SerializeField] private GameObject aiAgentPrefab;

        private HashSet<GameObject> finishedRacers = new HashSet<GameObject>();
        [HideInInspector] public GameObject[] spawnPoints;
        private GameEndedController gameEndedController;


        [Header("Cinemachine")]
        public CinemachineOrbitalFollow orbitalFollow;
        public PlayableDirector cinematicDirector;


        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        private void Start()
        {
            Time.timeScale = 1;
            gameEndedController = MenuManager.instance.GetController<GameEndedController>();
            StartCoroutine(InitializeMap());
        }


        IEnumerator InitializeMap()
        {
            LoadingMapPreviewController loadingMapPreviewController = MenuManager.instance.GetController<LoadingMapPreviewController>();
            loadingMapPreviewController.Activate("base");
            loadingMapPreviewController.SetLoadingProgress(50);


            // spawn agent
            GlobalEnvironment.instance.RefreshTargetPoints();
            spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");

            RacePositionSystemWaypoint.instance.allRacers.Add(playerTransform.GetComponent<RacerProgress>());

            for(int i = 0; i < spawnPoints.Length - 1; i++)
            {
                GameObject AiAgent = Instantiate(aiAgentPrefab);
                RacePositionSystemWaypoint.instance.allRacers.Add(AiAgent.GetComponent<RacerProgress>());
            }

            RacePositionSystemWaypoint.instance.SetupPositionAllRacer();

            yield return new WaitForSeconds(0.5f);
            loadingMapPreviewController.SetLoadingProgress(80);

            yield return new WaitForSeconds(1);
            loadingMapPreviewController.DisactivateAll();
            cinematicDirector.gameObject.SetActive(true);
            orbitalFollow.gameObject.SetActive(true);
        }

        public void OnCinematicFinished()
        {
            GameplayController gameplayController = MenuManager.instance.GetController<GameplayController>();
            gameplayController.Activate("gameplay gui");

            StartCoroutine(StartCountdown(gameplayController));
        }

        IEnumerator StartCountdown(GameplayController gameplayController)
        {
            CountDownView countDownView =
                (CountDownView)gameplayController.GetView("countdown");

            countDownView.Show();

            int countdownValue = 3;

            while (countdownValue > 0)
            {
                countDownView.UpdateText(countdownValue.ToString());
                if (countdownValue == 3)
                    AudioManager.Instance.PlaySFX("3");
                if (countdownValue == 2)
                    AudioManager.Instance.PlaySFX("2");
                if (countdownValue == 1)
                    AudioManager.Instance.PlaySFX("1");

                yield return StartCoroutine(WaitForSecondsRealtimeWithPause(0.5f));
                countdownValue--;
            }

            AudioManager.Instance.PlaySFX("Go!");
            AudioManager.Instance.PlaySFX("Cheers");
            countDownView.UpdateText("GO!");
            isGameActive = true;
            currentFinishRank = 1;
            finishedRacers.Clear();

            yield return StartCoroutine(WaitForSecondsRealtimeWithPause(0.5f));

            countDownView.Hide();
        }

        IEnumerator WaitForSecondsRealtimeWithPause(float duration)
        {
            float timeLeft = duration;

            while (timeLeft > 0f)
            {
                yield return new WaitWhile(() => isPaused);

                float delta = Mathf.Min(Time.unscaledDeltaTime, timeLeft);
                timeLeft -= delta;

                yield return null;
            }
        }




        public void OnFinishLineCrossed(GameObject racer)
        {
            if (finishedRacers.Contains(racer)) return;

            finishedRacers.Add(racer);

            int finishRank = currentFinishRank;
            currentFinishRank++;

            if (racer.CompareTag("Player"))
            {
                isGameActive = false;

                gameEndedController.GameEnded(finishRank);
            }
        }
    }


}
