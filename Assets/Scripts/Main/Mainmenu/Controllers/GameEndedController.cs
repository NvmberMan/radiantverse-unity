using Main.Gameplay;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.InputSystem.LowLevel.InputStateHistory;

namespace Main.Mainmenu
{
    public class GameEndedController : Controller
    {
        [Header("Rank Rewards")]
        public List<RankReward> rankRewards = new List<RankReward>();

        public float showRankDelay = 0f;
        public float showUnlockedMapDelay = 2f;
        public float showSummaryDelay = 2f;
        public void GameEnded(int rank)
        {
            GameManager.Instance.isPaused = true;
            StartCoroutine(InitializedSummary(rank));
        }

        IEnumerator InitializedSummary(int rank)
        {
            yield return new WaitForSeconds(showRankDelay);

            RankPreviewView rankPreviewView = (RankPreviewView)GetView("rank preview");
            RankReward reward = rankRewards.Find((r) => r.rank == rank);

            rankPreviewView.Show();
            rankPreviewView.UpdatePreview(reward.rankPreviewImage);


            if (reward.mapUnlockedName != null && reward.mapUnlockedName != "null")
            {
                yield return new WaitForSeconds(showUnlockedMapDelay);
                rankPreviewView.Hide();

                UnlockedNewArenaView unlockedNewArenaView = (UnlockedNewArenaView)GetView("unlocked new arena");
                unlockedNewArenaView.UpdatePreview(reward.mapUnlockedImage);
                unlockedNewArenaView.Show();

                yield return new WaitForSeconds(showSummaryDelay);
                unlockedNewArenaView.Hide();

                WinPopupView winPopupView = (WinPopupView)GetView("win popup");
                winPopupView.UpdateSummary(reward.exp, reward.coin, reward.mapUnlockedName);
                winPopupView.Show();
            }
            else
            {
                yield return new WaitForSeconds(showSummaryDelay);
                rankPreviewView.Hide();

                LosePopupView losePopupView = (LosePopupView)GetView("lose popup");
                losePopupView.UpdateSummary(reward.exp, reward.coin, reward.rankPreviewImage);
                losePopupView.Show();
            }
        }

        public void RestartGame()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public void BackToMainMenu()
        {
            SceneManager.LoadScene("MainMenu");
        }
    }

    [System.Serializable]
    public class RankReward
    {
        public int rank;
        public int exp;
        public int coin;
        public Sprite rankPreviewImage;

        [Header("Unlocked Arena")]
        public string mapUnlockedName = null;
        public Sprite mapUnlockedImage;
    }
}