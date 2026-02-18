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
        public float showAchievementDelay = 2f;

        public GameObject GoalEffectView;

        public void GameEnded(int rank)
        {
            GameManager.Instance.isPaused = true;
            StartCoroutine(InitializedSummary(rank));

            GoalEffectView.SetActive(true);
        }

        IEnumerator InitializedSummary(int rank)
        {
            AudioManager.Instance.PlaySFX("game finish");

            if (PlayerLocalData.inventoryData != null)
            {
                if (GameManager.Instance.playerTransform.GetComponent<ItemCount>().total == 0 && !AchievementManager.Instance.CheckAchievement("The Naturalist"))
                {
                    if (PlayerLocalData.inventoryData != null)
                    {
                        FirestoreModel.UnlockAchievement("The Naturalist");
                        MenuManager.instance.GetController<UniversalController>().ShowAchievementUnlockedPopup("The Naturalist");

                        yield return new WaitForSeconds(showAchievementDelay);
                    }
                }
            }
            yield return new WaitForSeconds(showRankDelay);

            RankPreviewView rankPreviewView = (RankPreviewView)GetView("rank preview");
            RankReward reward = rankRewards.Find((r) => r.rank == rank);

            if (reward == null && rankRewards.Count > 0)
            {
                reward = rankRewards[rankRewards.Count - 1];
            }

            rankPreviewView.Show();
            rankPreviewView.UpdatePreview(rank);

            if (PlayerLocalData.playerStats != null)
            {
                FirestoreModel.AddExperience(reward.exp);
                FirestoreModel.AddArradiusDollar(reward.arradiusDollar);
            }

            if (reward.mapUnlockedName != null && reward.mapUnlockedName != "null")
            {
                if(PlayerLocalData.inventoryData != null)
                {
                    FirestoreModel.UnlockMap(reward.mapWorldKey);
                    FirestoreModel.RecordMapWin(reward.mapWorldKey);
                }

                yield return new WaitForSeconds(showUnlockedMapDelay);
                rankPreviewView.Hide();

                UnlockedNewArenaView unlockedNewArenaView = (UnlockedNewArenaView)GetView("unlocked new arena");
                unlockedNewArenaView.UpdatePreview(reward.mapUnlockedImage);
                unlockedNewArenaView.Show();
                AudioManager.Instance.PlaySFX("level unlocked");


                yield return new WaitForSeconds(showSummaryDelay);
                unlockedNewArenaView.Hide();

                WinPopupView winPopupView = (WinPopupView)GetView("win popup");
                winPopupView.UpdateSummary(reward.exp, reward.arradiusDollar, reward.mapUnlockedName);
                winPopupView.Show();

                GoalEffectView.SetActive(false);
            }
            else
            {
                yield return new WaitForSeconds(showSummaryDelay);
                rankPreviewView.Hide();

                LosePopupView losePopupView = (LosePopupView)GetView("lose popup");
                losePopupView.UpdateSummary(reward.exp, reward.arradiusDollar, rank);
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
        public int arradiusDollar;

        [Header("Unlocked Arena")]
        public string mapUnlockedName = null;
        public string mapWorldKey;
        public Sprite mapUnlockedImage;
    }
}