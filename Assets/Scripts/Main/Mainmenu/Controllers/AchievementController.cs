using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Main.Mainmenu
{
    public class AchievementController : Controller
    {
        [SerializeField] private AchievementItemUI[] items; 
        [SerializeField] private GameObject achievementPrefab; 
        [SerializeField] private AchievementDetailUI achievementDetail;
        [SerializeField] private AchievementLockedDetailUI achievementLockedDetail;

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
            foreach (AchievementItemUI item in items)
            {
                bool isUnlocked = PlayerLocalData.inventoryData.UnlockedAchievements.Contains(item.achievementItem.id);

                item.achievementPreviewImage.sprite = item.achievementItem.iconPreview;

                item.achievementPreviewImage.color = isUnlocked ? Color.white : new Color(0, 0, 0, 0.5f);


                item.achievementDetailButton.onClick.RemoveAllListeners();

                if(isUnlocked)
                {
                    item.achievementDetailButton.onClick.AddListener(() => OpenDetail(item));
                }
                else
                {
                    item.achievementDetailButton.onClick.AddListener(() => OpenLockedDetail(item));
                }
            }
        }
        private void OpenDetail(AchievementItemUI item)
        {
            achievementLockedDetail.gameObject.SetActive(false);

            achievementDetail.achievementNameText.text = item.achievementItem.achievementName;
            achievementDetail.achievementDescriptionText.text = item.achievementItem.description;

            achievementDetail.gameObject.SetActive(true);
            RectTransform rectTransform = achievementDetail.GetComponent<RectTransform>();

            rectTransform.anchoredPosition = item.achievementDetailPosition;
        }

        private void OpenLockedDetail(AchievementItemUI item)
        {
            achievementDetail.gameObject.SetActive(false);

            achievementLockedDetail.achievementDescriptionText.text = item.achievementItem.hoverDescription;

            achievementLockedDetail.gameObject.SetActive(true);
            RectTransform rectTransform = achievementLockedDetail.GetComponent<RectTransform>();

            rectTransform.anchoredPosition = item.achievementLockedDetailPosition;
        }

        public void CloseDetail()
        {
            achievementLockedDetail.gameObject.SetActive(false);
            achievementDetail.gameObject.SetActive(false);
        }
    }
}