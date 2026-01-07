using Main.Mainmenu;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
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

        private HashSet<GameObject> finishedRacers = new HashSet<GameObject>();
        private GameEndedController gameEndedController;

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


            yield return new WaitForSeconds(0.5f);
            loadingMapPreviewController.SetLoadingProgress(80);

            yield return new WaitForSeconds(1);
            loadingMapPreviewController.DisactivateAll();

            GameplayController gameplayController = MenuManager.instance.GetController<GameplayController>();
            gameplayController.Activate("gameplay gui");

            yield return StartCoroutine(StartCountdown(gameplayController));
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

                yield return StartCoroutine(WaitForSecondsRealtimeWithPause(0.5f));

                countdownValue--;
            }

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
