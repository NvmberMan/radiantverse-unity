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

        [Header("UI Timing")]
        public float resultDelay = 3f;

        [Header("UI")]
        public GameObject winCanvas;
        public GameObject loseCanvas;
        public SummaryUI summaryPopup;
        public TextMeshProUGUI rankUIText;
        public TextMeshProUGUI loseRankText;

        [Header("Rank Rewards")]
        public List<RankReward> rankRewards = new List<RankReward>();

        private HashSet<GameObject> finishedRacers = new HashSet<GameObject>();

        // Data player (disimpan untuk summary)
        private int playerFinalRank;
        private int playerExp;
        private int playerCoin;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        private void Start()
        {
            Time.timeScale = 1;
            isGameActive = true;
            currentFinishRank = 1;
            finishedRacers.Clear();

            if (winCanvas) winCanvas.SetActive(false);
            if (loseCanvas) loseCanvas.SetActive(false);
            if (summaryPopup) summaryPopup.gameObject.SetActive(false);
        }

        // DIPANGGIL SAAT FINISH LINE
        public void OnFinishLineCrossed(GameObject racer)
        {
            if (finishedRacers.Contains(racer)) return;

            finishedRacers.Add(racer);

            int finishRank = currentFinishRank;
            currentFinishRank++;

            Debug.Log($"{racer.name} finish di posisi #{finishRank}");

            if (racer.CompareTag("Player"))
            {
                HandlePlayerFinish(finishRank);
            }
        }

        private void HandlePlayerFinish(int rank)
        {
            isGameActive = false;
            Time.timeScale = 0;

            playerFinalRank = rank;

            // Hitung reward
            var reward = rankRewards.Find(r => r.rank == rank);
            if (reward != null)
            {
                playerExp = reward.exp;
                playerCoin = reward.coin;
            }
            else
            {
                playerExp = 0;
                playerCoin = 0;
            }

            if (rank == 1)
                winCanvas.SetActive(true);
            else
            {
                loseCanvas.SetActive(true);
                // ✅ SET RANK TEXT DI LOSE PANEL
                if (loseRankText != null)
                {
                    loseRankText.text = $"{GetRankSuffix(rank)}";
                }
            }
            // ⏳ TUNGGU 3 DETIK → SUMMARY
            StartCoroutine(ShowSummaryAfterDelay());
        }

        private IEnumerator ShowSummaryAfterDelay()
        {
            yield return new WaitForSecondsRealtime(resultDelay);

            if (winCanvas) winCanvas.SetActive(false);
            if (loseCanvas) loseCanvas.SetActive(false);

            if (summaryPopup != null)
            {
                summaryPopup.Show(playerFinalRank, playerExp, playerCoin);
            }
        }


        private string GetRankSuffix(int rank)
        {
            if (rank % 100 >= 11 && rank % 100 <= 13)
                return $"{rank}th";

            switch (rank % 10)
            {
                case 1: return $"{rank}st";
                case 2: return $"{rank}nd";
                case 3: return $"{rank}rd";
                default: return $"{rank}th";
            }
        }

        // DIPANGGIL BUTTON SUMMARY
        public void ShowSummary()
        {
            Debug.Log("🔥 SHOW SUMMARY DIPANGGIL");

            if (winCanvas) winCanvas.SetActive(false);
            if (loseCanvas) loseCanvas.SetActive(false);

            if (summaryPopup != null)
            {
                summaryPopup.Show(playerFinalRank, playerExp, playerCoin);
            }
            else
            {
                Debug.LogError("❌ SummaryPopup NULL");
            }
        }



        // DIPANGGIL BUTTON RESET
        public void RestartGame()
        {
            Time.timeScale = 1;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    [System.Serializable]
    public class RankReward
    {
        public int rank;
        public int exp;
        public int coin;
    }
}
