using Main.Gameplay.AI;
using Main.Mainmenu;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using Unity.Barracuda;
using Unity.Cinemachine;
using Unity.MLAgents.Policies;
using UnityEngine;
using UnityEngine.Playables;

namespace Main.Gameplay
{

    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;

        [Header("Game State")]
        public bool isGameActive = true;
        public bool isPaused = false;
        public int currentFinishRank = 1;
        public bool isTesting = false;
        public bool isTraining = false;

        public Transform playerTransform;
        [SerializeField] private GameObject aiAgentPrefab;

        private HashSet<GameObject> finishedRacers = new HashSet<GameObject>();
        [HideInInspector] public GameObject[] spawnPoints;
        private GameEndedController gameEndedController;


        [Header("Cinemachine")]
        public CinemachineOrbitalFollow orbitalFollow;
        public CinemachineCamera orbitalFollowAI;
        public PlayableDirector cinematicDirector;
        [HideInInspector] public bool isCinematic = true;


        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        private void Start()
        {

            GlobalEnvironment.instance.ShuffleAIData();

            Time.timeScale = 1;
            gameEndedController = MenuManager.instance.GetController<GameEndedController>();

            if(isTesting)
            {
                GlobalEnvironment.instance.RefreshTargetPoints();
                spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");

                RacePositionSystemWaypoint.Instance.allRacers.Add(playerTransform.GetComponent<RacerProgress>());

                for (int i = 0; i < spawnPoints.Length - 1; i++)
                {
                    GameObject AiAgent = Instantiate(aiAgentPrefab);
                    ApplyRandomBrain(AiAgent, i);
                    RacePositionSystemWaypoint.Instance.allRacers.Add(AiAgent.GetComponent<RacerProgress>());
                }

                if (isTraining)
                {
                    orbitalFollowAI.Follow = RacePositionSystemWaypoint.Instance.allRacers[1].transform;
                }

                RacePositionSystemWaypoint.Instance.SetupPositionAllRacer();

                GameplayController gameplayController = MenuManager.instance.GetController<GameplayController>();
                gameplayController.Activate("gameplay gui");

                isGameActive = true;
                currentFinishRank = 1;
                finishedRacers.Clear();
            }
            else
                StartCoroutine(InitializeMap());
        }


        IEnumerator InitializeMap()
        {
            isCinematic = true;


            LoadingMapPreviewController loadingMapPreviewController = MenuManager.instance.GetController<LoadingMapPreviewController>();
            loadingMapPreviewController.Activate("base");
            loadingMapPreviewController.SetLoadingProgress(50);


            // spawn agent
            GlobalEnvironment.instance.RefreshTargetPoints();
            spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");

            RacePositionSystemWaypoint.Instance.allRacers.Add(playerTransform.GetComponent<RacerProgress>());

            for(int i = 0; i < spawnPoints.Length - 1; i++)
            {
                GameObject AiAgent = Instantiate(aiAgentPrefab);

                ApplyRandomBrain(AiAgent, i);

                RacePositionSystemWaypoint.Instance.allRacers.Add(AiAgent.GetComponent<RacerProgress>());
            }

            RacePositionSystemWaypoint.Instance.SetupPositionAllRacer();

            yield return new WaitForSeconds(0.5f);
            loadingMapPreviewController.SetLoadingProgress(80);

            yield return new WaitForSeconds(1);
            loadingMapPreviewController.DisactivateAll();
            cinematicDirector.gameObject.SetActive(true);
            orbitalFollow.gameObject.SetActive(true);

            Controller helpController = MenuManager.instance.GetController("help");
            helpController.Activate("help");
            Debug.Log(helpController.name);
        }

        private void ApplyRandomBrain(GameObject agentObj, int index)
        {
            if (GlobalEnvironment.instance.aiData == null || index >= GlobalEnvironment.instance.aiData.Count) return;

            BehaviorParameters bp = agentObj.GetComponent<BehaviorParameters>();
            AIInput aiInput = agentObj.GetComponent<AIInput>();
            CharacterCustomization customization = agentObj.GetComponent<CharacterCustomization>();

            if (bp != null)
            {
                AIData selectedData = GlobalEnvironment.instance.aiData[index];

                bp.Model = selectedData.brain;

                if (aiInput != null) aiInput.wayIndex = selectedData.wayIndex;

                if (customization != null)
                {
                    if (customization.nameCharacter != null)
                        customization.nameCharacter.text = selectedData.characterName;

                    if (selectedData.skinConfigs != null && selectedData.skinConfigs.Count > 0)
                    {
                        customization.skinSource = CharacterCustomization.SkinSource.CustomManual;
                        customization.CombineSkins(selectedData.skinConfigs.ToArray());
                    }
                }
            }
        }

        public void OnCinematicFinished()
        {
            isCinematic = false;

            GameplayController gameplayController = MenuManager.instance.GetController<GameplayController>();
            gameplayController.Activate("gameplay gui");

            Controller helpController = MenuManager.instance.GetController("help");
            helpController.Disactivate("help");

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




        //public void OnFinishLineCrossed(GameObject racer)
        //{
        //    if (finishedRacers.Contains(racer)) return;

        //    finishedRacers.Add(racer);

        //    int finishRank = currentFinishRank;
        //    currentFinishRank++;

        //    if (racer.CompareTag("Player"))
        //    {
        //        isGameActive = false;

        //        gameEndedController.GameEnded(finishRank);
        //    }
        //}


        public void OnFinishLineCrossed(GameObject racer)
        {
            if (finishedRacers.Contains(racer)) return;

            finishedRacers.Add(racer);

            int finishRank = currentFinishRank;
            currentFinishRank++;

            // 🔥 SINKRONKAN ke RacerProgress
            RacerProgress rp = racer.GetComponent<RacerProgress>();
            if (rp != null)
            {
                rp.finishRank = finishRank;
                rp.hasFinished = true;
            }

            if (racer.CompareTag("Player"))
            {
                isGameActive = false;
                gameEndedController.GameEnded(finishRank);
            }
        }

    }


}
