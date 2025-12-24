using Firebase.Auth;
using Spine;
using Spine.Unity;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Tambahkan ini untuk Button

namespace Main.Mainmenu
{
    public class CustomizeView : View
    {
        [Header("Spine Components")]
        public SkeletonGraphic skeletonGraphic;

        [Header("Category Panels")]
        // Daftarkan GameObject induk dari Scroll View masing-masing
        public GameObject headPanel;
        public GameObject bodyPanel;

        [Header("UI Containers")]
        public Transform headContainer;
        public Transform bodyContainer;

        [Header("Settings")]
        public AccessoryItemUI itemPrefab;
        public List<AccessoryData> allAccessoryDatabase;
        public GameObject saveButton;
        private List<string> draftSkins;

        public override void Show()
        {
            base.Show();

            draftSkins = new List<string>(PlayerLocalData.inventoryData.SelectedSkins);

            saveButton.SetActive(false);
            SwitchCategory("head");
            RefreshUI();
            RefreshCharacterPreview();
        }

        public void SwitchCategory(string category)
        {
            headPanel.SetActive(false);
            bodyPanel.SetActive(false);

            switch (category.ToLower())
            {
                case "head":
                    headPanel.SetActive(true);
                    break;
                case "body":
                    bodyPanel.SetActive(true);
                    break;
            }
        }

        public void RefreshUI()
        {
            ClearContainer(headContainer);
            ClearContainer(bodyContainer);

            if (PlayerLocalData.inventoryData == null) return;

            foreach (string unlockedID in PlayerLocalData.inventoryData.UnlockedAccessories)
            {
                AccessoryData data = allAccessoryDatabase.Find(x => x.spineSkinName == unlockedID);
                if (data != null)
                {
                    Transform targetContainer = GetTargetContainer(data.category);
                    if (targetContainer != null)
                    {
                        AccessoryItemUI newItem = Instantiate(itemPrefab, targetContainer);
                        newItem.Setup(data, this);
                    }
                }
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(headContainer.GetComponent<RectTransform>());
            LayoutRebuilder.ForceRebuildLayoutImmediate(bodyContainer.GetComponent<RectTransform>());
        }

        private Transform GetTargetContainer(string category)
        {
            switch (category.ToLower())
            {
                case "head": return headContainer;
                case "body": return bodyContainer;
                default: return null;
            }
        }

        private void ClearContainer(Transform container)
        {
            if (container == null) return;
            foreach (Transform child in container) Destroy(child.gameObject);
        }

        public void OnItemClicked(string category, string skinName)
        {
            int index = draftSkins.FindIndex(s => s.StartsWith(category + "/"));
            if (index != -1) draftSkins[index] = skinName;
            else draftSkins.Add(skinName);

            // Cek apakah draft berbeda dengan data asli yang tersimpan
            bool isModified = !IsListEqual(draftSkins, PlayerLocalData.inventoryData.SelectedSkins);
            saveButton.SetActive(isModified);

            RefreshCharacterPreview();
        }

        public void SaveChanges()
        {
            PlayerLocalData.inventoryData.SelectedSkins = new List<string>(draftSkins);

            string uid = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
            FirestoreModel.UpdateSelectedSkins(uid, PlayerLocalData.inventoryData.SelectedSkins);

            saveButton.SetActive(false);
            Debug.Log("Skins saved to cloud!");
        }

        public void RefreshCharacterPreview()
        {
            if (draftSkins == null) return;
            var skeleton = skeletonGraphic.Skeleton;
            Skin combinedSkin = new Skin("CombinedPreview");

            foreach (string skinName in draftSkins)
            {
                Skin sourceSkin = skeleton.Data.FindSkin(skinName);
                if (sourceSkin != null) combinedSkin.AddSkin(sourceSkin);
            }

            skeleton.SetSkin(combinedSkin);
            skeleton.SetSlotsToSetupPose();
            skeletonGraphic.UpdateMesh();
        }

        private bool IsListEqual(List<string> a, List<string> b)
        {
            if (a.Count != b.Count) return false;
            for (int i = 0; i < a.Count; i++) if (a[i] != b[i]) return false;
            return true;
        }
    }
}