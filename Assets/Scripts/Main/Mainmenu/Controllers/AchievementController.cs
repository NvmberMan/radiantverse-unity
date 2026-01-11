using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Main.Mainmenu
{
    public class AchievementController : Controller
    {
        [SerializeField] private Transform container; 
        [SerializeField] private GameObject achievementPrefab; 
        [SerializeField] private AchievementDetailUI achievementDetail;

        public override void Activate(string targetView)
        {
            base.Activate(targetView);
            FirestoreModel.UnlockAchievement("gravity-defier");

            RefreshUI();
        }

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.Space))
            {
                MenuManager.instance.GetController<UniversalController>().ShowAchievementUnlockedPopup("gravity-defier");
            }
        }

        public void RefreshUI()
        {
            AchievementItemUI[] items = container.GetComponentsInChildren<AchievementItemUI>();

            foreach (AchievementItemUI item in items)
            {
                bool isUnlocked = PlayerLocalData.inventoryData.UnlockedAchievements.Contains(item.achievementItem.id);

                item.achievementPreviewImage.sprite = item.achievementItem.iconPreview;

                item.achievementPreviewImage.color = isUnlocked ? Color.white : Color.black;

                item.achievementDetailButton.onClick.RemoveAllListeners();

                if(isUnlocked)
                {
                    AchievementItemUI currentItem = item;
                    item.achievementDetailButton.onClick.AddListener(() => OpenDetail(currentItem));
                }
            }
        }
        private void OpenDetail(AchievementItemUI item)
        {
            achievementDetail.achievementNameText.text = item.achievementItem.achievementName;
            achievementDetail.achievementDescriptionText.text = item.achievementItem.description;

            achievementDetail.gameObject.SetActive(true);
            RectTransform rectTransform = achievementDetail.GetComponent<RectTransform>();

            rectTransform.anchoredPosition = item.achievementDetailPosition;
        }

        public void CloseDetail()
        {
            achievementDetail.gameObject.SetActive(false);
        }
    }
}