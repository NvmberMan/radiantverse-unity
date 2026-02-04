using Firebase.Auth;
using NUnit.Framework;
using Spine;
using Spine.Unity;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.UI;
using static Unity.Cinemachine.IInputAxisOwner.AxisDescriptor;

namespace Main.Mainmenu
{
    [System.Serializable]
    public class CustomizeCategory
    {
        public string categoryName;
        public NavbarItemButton navbarController;
    }

    public class CustomizeView : View
    {
        [Header("Spine Components")]
        public SkeletonGraphic skeletonGraphic;

        [Header("Global UI References")]
        public ScrollRect scrollRect;
        public Transform itemContainer;
        public AccessoryItemUI itemPrefab;

        [Header("Flexible Categories")]
        public List<CustomizeCategory> categories;

        [Header("Database & Settings")]
        public List<AccessoryData> allAccessoryDatabase;
        public GameObject saveButton;

        private List<string> draftSkins;
        private int currentCategoryIndex = 0;


        private List<string> officeLook = new List<string>
        {
            "shoes/Black-formal",
            "shirts/Kemeja&dasi-hitam",
            "pants/Black-formal"
        };

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
            int index = draftSkins.FindIndex(s => s.Contains(category + "/"));

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

            if (PlayerLocalData.playerStats != null)
            {
                if (IsOfficeLookEquipped(draftSkins) && !AchievementManager.Instance.CheckAchievement("9-to-5 Ready"))
                {
                    FirestoreModel.UnlockAchievement("9-to-5 Ready");
                    MenuManager.instance.GetController<UniversalController>().ShowAchievementUnlockedPopup("9-to-5 Ready");
                }
            }
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

        private bool IsOfficeLookEquipped(List<string> currentEquipped)
        {
            foreach (string requiredSkin in officeLook)
            {
                if (!currentEquipped.Contains(requiredSkin))
                {
                    return false;
                }
            }
            return true;
        }
    }
}