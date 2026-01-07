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

            isGameActive = true;
            currentFinishRank = 1;
            finishedRacers.Clear();
        }

        IEnumerator StartCountdown(GameplayController gameplayController)
        {
            CountDownView countDownView = (CountDownView)gameplayController.GetView("countdown");
            countDownView.Show();
            countDownView.UpdateCount(3);

            yield return new WaitForSeconds(1);

            countDownView.UpdateCount(2);

            yield return new WaitForSeconds(1);

            countDownView.UpdateCount(1);

            yield return new WaitForSeconds(1);
            countDownView.Hide();
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
      
        public void RestartGame()
        {
            Time.timeScale = 1;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }


}
