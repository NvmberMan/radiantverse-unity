using Firebase.Auth;
using Spine;
using Spine.Unity;
using Spine.Unity.AttachmentTools;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.UI;

namespace Main.Mainmenu
{
    public class LobbyView : View
    {
        [SerializeField] private TMP_Text arradiusDollarView;
        [SerializeField] private TMP_Text usernameView;
        [SerializeField] private Slider experienceSlider;
        [SerializeField] private TMP_Text experienceText;
        [SerializeField] private TMP_Text levelText;

        [Space(5)]
        public SkeletonGraphic skeletonGraphic;
        public GameObject arradiusDollarCircleTargetAtractor;


        private FirebaseUser user;

        private DailyRewardView dailyRewardView;

        private void Awake()
        {
             dailyRewardView = MenuManager.instance.GetController<LobbyController>().GetView<DailyRewardView>();
        }

        public override void Show()
        {
            base.Show();

            ApplySkinsFromData();
            RefreshPlayerStats();
            RefreshUserdata();
        }

        public void RefreshUserdata()
        {
            if (PlayerLocalData.userData != null)
            {
                usernameView.text = PlayerLocalData.userData.Username.ToString();
            }
            else
            {
                user = AuthManager.instance.CurrentUser;
                FirestoreModel.GetUserData(user, (data) =>
                {
                    PlayerLocalData.userData = data;

                    usernameView.text = data.Username;
                }, Debug.LogError);
            }

            if(PlayerLocalData.playerStats != null)
            {
                if (PlayerLocalData.playerStats.Level >= 5 && !AchievementManager.Instance.CheckAchievement("Ascension"))
                {
                    FirestoreModel.UnlockAchievement("Ascension");
                    MenuManager.instance.GetController<UniversalController>().ShowAchievementUnlockedPopup("Ascension");
                }
            }
        }

        public void RefreshPlayerStats()
        {
            if (PlayerLocalData.playerStats != null)
            {
                arradiusDollarView.text = PlayerLocalData.playerStats.ArradiusDollar.ToString();
                levelText.text = PlayerLocalData.playerStats.Level.ToString();

                experienceSlider.minValue = ExpManager.instance.expList[PlayerLocalData.playerStats.Level - 1].Min;
                experienceSlider.maxValue = ExpManager.instance.expList[PlayerLocalData.playerStats.Level - 1].Max;
                experienceSlider.value = PlayerLocalData.playerStats.Experience;
                
                experienceText.text = PlayerLocalData.playerStats.Experience.ToString() + "/" + ExpManager.instance.expList[PlayerLocalData.playerStats.Level - 1].Max;
                dailyRewardView.dailyStreak = PlayerLocalData.playerStats.DailyStreak;
                dailyRewardView.claimedDay = PlayerLocalData.playerStats.LastClaimedDay;
            }
            else
            {
                user = AuthManager.instance.CurrentUser;

                FirestoreModel.GetPlayerStats(user, (stats) =>
                {
                    PlayerLocalData.playerStats = stats;

                    arradiusDollarView.text = stats.ArradiusDollar.ToString();
                    levelText.text = stats.Level.ToString();
                    dailyRewardView.dailyStreak = stats.DailyStreak;
                    dailyRewardView.claimedDay = stats.LastClaimedDay;

                    experienceSlider.minValue = ExpManager.instance.expList[stats.Level - 1].Min;
                    experienceSlider.maxValue = ExpManager.instance.expList[stats.Level - 1].Max;
                    experienceSlider.value = stats.Experience;


                    experienceText.text = stats.Experience.ToString() + "/" + ExpManager.instance.expList[PlayerLocalData.playerStats.Level - 1].Max;
                }, Debug.LogError);
            }
        }

        public void ApplySkinsFromData()
        {
            if (PlayerLocalData.inventoryData == null) return;

            List<string> selected = PlayerLocalData.inventoryData.SelectedSkins;
            CombineSkins(selected.ToArray());
        }

        public void CombineSkins(string[] skinNames)
        {
            var skeleton = skeletonGraphic.Skeleton;
            Skin combinedSkin = new Skin("Combined");

            foreach (string skinName in skinNames)
            {
                Skin sourceSkin = skeleton.Data.FindSkin(skinName);
                if (sourceSkin != null)
                    combinedSkin.AddSkin(sourceSkin);
                else
                    Debug.LogWarning($"Skin {skinName} tidak ditemukan di Atlas!");
            }

            skeleton.SetSkin(combinedSkin);
            skeleton.SetSlotsToSetupPose();
            skeletonGraphic.OverrideTexture = null;
            skeletonGraphic.UpdateMesh();
        }

        public void ChangeEquipment(string category, string skinName)
        {
            List<string> currentSkins = PlayerLocalData.inventoryData.SelectedSkins;

            // Cari dan ganti skin yang satu kategori
            for (int i = 0; i < currentSkins.Count; i++)
            {
                if (currentSkins[i].StartsWith(category))
                {
                    currentSkins[i] = skinName;
                    break;
                }
            }

            FirestoreModel.UpdateSelectedSkins(AuthManager.instance.CurrentUser.UserId, currentSkins);

            ApplySkinsFromData();
        }
    }
}