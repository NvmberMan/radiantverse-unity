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
        public GameObject accessoriesPanel;
        public GameObject facesPanel;
        public GameObject hairsPanel;
        public GameObject pantsPanel;
        public GameObject shirtsPanel;
        public GameObject shoesPanel;

        [Header("UI Containers")]
        public Transform accessoriesContainer;
        public Transform facesContainer;
        public Transform hairsContainer;
        public Transform pantsContainer;
        public Transform shirtsContainer;
        public Transform shoesContainer;

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
            SwitchCategory("Faces");
            RefreshUI();
            RefreshCharacterPreview();
        }

        public void SwitchCategory(string category)
        {
            accessoriesPanel.SetActive(false);
            facesPanel.SetActive(false);
            hairsPanel.SetActive(false);
            pantsPanel.SetActive(false);
            shirtsPanel.SetActive(false);
            shoesPanel.SetActive(false);


            switch (category)
            {
                case "Accessories":
                    accessoriesPanel.SetActive(true);
                    break;
                case "Faces":
                    Debug.Log(category);
                    facesPanel.SetActive(true);
                    break;
                case "Hairs":
                    hairsPanel.SetActive(true);
                    break;
                case "Pants":
                    pantsPanel.SetActive(true);
                    break;
                case "Shirts":
                    shirtsPanel.SetActive(true);
                    break;
                case "Shoes":
                    shoesPanel.SetActive(true);
                    break;
            }
        }

        public void RefreshUI()
        {
            ClearContainer(accessoriesContainer);
            ClearContainer(facesContainer);
            ClearContainer(hairsContainer);
            ClearContainer(pantsContainer);
            ClearContainer(shirtsContainer);
            ClearContainer(shoesContainer);

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

            LayoutRebuilder.ForceRebuildLayoutImmediate(accessoriesContainer.GetComponent<RectTransform>());
            LayoutRebuilder.ForceRebuildLayoutImmediate(facesContainer.GetComponent<RectTransform>());
            LayoutRebuilder.ForceRebuildLayoutImmediate(hairsContainer.GetComponent<RectTransform>());
            LayoutRebuilder.ForceRebuildLayoutImmediate(pantsContainer.GetComponent<RectTransform>());
            LayoutRebuilder.ForceRebuildLayoutImmediate(shirtsContainer.GetComponent<RectTransform>());
            LayoutRebuilder.ForceRebuildLayoutImmediate(shoesContainer.GetComponent<RectTransform>());
        }

        private Transform GetTargetContainer(string category)
        {
            switch (category)
            {
                case "Accessories": return accessoriesContainer;
                case "Faces": return facesContainer;
                case "Hairs": return hairsContainer;
                case "Pants": return pantsContainer;
                case "Shirts": return shirtsContainer;
                case "Shoes": return shoesContainer;
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
            int index = draftSkins.FindIndex(s => s.StartsWith("Component/" + category + "/"));
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