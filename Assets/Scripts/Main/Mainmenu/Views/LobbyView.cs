using Firebase.Auth;
using Spine;
using Spine.Unity;
using Spine.Unity.AttachmentTools;
using System.Collections.Generic;
using TMPro;
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
        public SkeletonGraphic skeletonGraphic;

        private FirebaseUser user;
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
        }

        public void RefreshPlayerStats()
        {
            if (PlayerLocalData.playerStats != null)
            {
                arradiusDollarView.text = PlayerLocalData.playerStats.ArradiusDollar.ToString();
                levelText.text = PlayerLocalData.playerStats.Level.ToString();
                experienceSlider.value = PlayerLocalData.playerStats.Experience;
                experienceText.text = PlayerLocalData.playerStats.Experience.ToString() + "/100";
            }
            else
            {
                user = AuthManager.instance.CurrentUser;

                FirestoreModel.GetPlayerStats(user, (stats) =>
                {
                    PlayerLocalData.playerStats = stats;

                    arradiusDollarView.text = stats.ArradiusDollar.ToString();
                    levelText.text = stats.Level.ToString();
                    experienceSlider.value = stats.Experience;
                    experienceText.text = stats.Experience.ToString() + "/100";
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