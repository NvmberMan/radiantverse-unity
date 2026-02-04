using Firebase.Auth;
using Spine;
using Spine.Unity;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Main.Mainmenu
{
    [System.Serializable]
    public class CustomizeCategory
    {
        public string categoryName; // Contoh: "Faces", "Shirts"
        public NavbarItemButton navbarController;
        // Tidak butuh panel/container sendiri lagi karena pakai yang global
    }

    public class CustomizeView : View
    {
        [Header("Spine Components")]
        public SkeletonGraphic skeletonGraphic;

        [Header("Global UI References")]
        public ScrollRect scrollRect;
        public Transform itemContainer; // Cukup satu container untuk semua
        public AccessoryItemUI itemPrefab;

        [Header("Flexible Categories")]
        public List<CustomizeCategory> categories;

        [Header("Database & Settings")]
        public List<AccessoryData> allAccessoryDatabase;
        public GameObject saveButton;

        private List<string> draftSkins;
        private int currentCategoryIndex = 0;

        private void Awake()
        {
            for (int i = 0; i < categories.Count; i++)
            {
                int index = i;
                if (categories[i].navbarController != null)
                {
                    Button btn = categories[i].navbarController.GetComponent<Button>();
                    if (btn != null)
                    {
                        btn.onClick.AddListener(() => SwitchCategory(index));
                    }
                }
            }
        }

        public override void Show()
        {
            base.Show();
            draftSkins = new List<string>(PlayerLocalData.inventoryData.SelectedSkins);
            saveButton.SetActive(false);

            SwitchCategory(0);
        }

        public void SwitchCategory(int index)
        {
            if (index < 0 || index >= categories.Count) return;

            currentCategoryIndex = index;

            scrollRect.horizontalNormalizedPosition = 0;

            UpdateNavbarVisuals();
            RefreshUI();
        }

        private void UpdateNavbarVisuals()
        {
            for (int i = 0; i < categories.Count; i++)
            {
                var nav = categories[i].navbarController;
                if (nav != null)
                {
                    bool isActive = (i == currentCategoryIndex);
                    if (nav.ActivePreview != null) nav.ActivePreview.SetActive(isActive);
                    if (nav.DisactivePreview != null) nav.DisactivePreview.SetActive(!isActive);
                }
            }
        }

        public void RefreshUI()
        {
            if (PlayerLocalData.inventoryData == null) return;

            foreach (Transform child in itemContainer) Destroy(child.gameObject);

            string currentCatName = categories[currentCategoryIndex].categoryName;

            foreach (string unlockedID in PlayerLocalData.inventoryData.UnlockedAccessories)
            {
                AccessoryData data = allAccessoryDatabase.Find(x => x.spineSkinName == unlockedID);

                if (data != null && data.category == currentCatName)
                {
                    AccessoryItemUI newItem = Instantiate(itemPrefab, itemContainer);
                    newItem.Setup(data, this);
                }
            }

            Canvas.ForceUpdateCanvases();
            if (itemContainer.TryGetComponent<RectTransform>(out var rect))
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
            }
        }

        public void OnItemClicked(string category, string skinName)
        {
            int index = draftSkins.FindIndex(s => s.Contains("Component/" + category + "/"));

            if (index != -1) draftSkins[index] = skinName;
            else draftSkins.Add(skinName);

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
        }

        public void RefreshCharacterPreview()
        {
            if (draftSkins == null || skeletonGraphic == null) return;

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
            List<string> sortedA = new List<string>(a);
            List<string> sortedB = new List<string>(b);
            sortedA.Sort();
            sortedB.Sort();
            for (int i = 0; i < sortedA.Count; i++) if (sortedA[i] != sortedB[i]) return false;
            return true;
        }
    }
}