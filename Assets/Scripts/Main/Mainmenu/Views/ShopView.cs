using Firebase.Auth;
using Main.Gameplay;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Main.Mainmenu
{
    [System.Serializable]
    public class ShopCategory
    {
        public string categoryName;
        public NavbarItemButton navbarController;
        public List<ShopItemData> itemDatabase;
    }

    public class ShopView : View
    {
        [Header("UI Scroll References")]
        public ScrollRect scrollRect;
        public Transform itemContainer;
        public ShopItemUI itemPrefab;
        public TextMeshProUGUI playerMoneyText;

        [Header("Flexible Categories")]
        public List<ShopCategory> categories;

        private int currentCategoryIndex = 0;
        private int spending = 0;

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
            SwitchCategory(0);
            spending = 0;
        }

        public void SwitchCategory(int index)
        {
            if (index < 0 || index >= categories.Count) return;

            currentCategoryIndex = index;

            StopAllCoroutines();

            scrollRect.horizontalNormalizedPosition = 0;

            UpdateNavbarVisuals();
            RefreshShopUI();
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

        public void RefreshShopUI()
        {
            if (PlayerLocalData.playerStats != null)
                playerMoneyText.text = PlayerLocalData.playerStats.ArradiusDollar.ToString();

            foreach (Transform child in itemContainer) Destroy(child.gameObject);

            List<ShopItemData> activeList = categories[currentCategoryIndex].itemDatabase;

            foreach (var item in activeList)
            {
                bool isOwned = CheckOwnership(item);
                ShopItemUI newItem = Instantiate(itemPrefab, itemContainer);
                newItem.Setup(item, this, isOwned);
            }

            Canvas.ForceUpdateCanvases();
            if (itemContainer.TryGetComponent<RectTransform>(out var rect))
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
            }
        }

        private bool CheckOwnership(ShopItemData item)
        {
            if (item is AccessoryData acc)
                return PlayerLocalData.inventoryData.UnlockedAccessories.Contains(acc.spineSkinName);
            return false;
        }

        public void NextItem() => MoveStep(1);
        public void PreviousItem() => MoveStep(-1);

        private void MoveStep(int direction)
        {
            float step = GetStepSize();
            if (step <= 0) return;

            float currentPos = scrollRect.horizontalNormalizedPosition;

            float targetPos = Mathf.Clamp01((Mathf.Round(currentPos / step) + direction) * step);

            StopAllCoroutines();
            StartCoroutine(LerpScroll(targetPos));
        }

        private float GetStepSize()
        {
            if (itemContainer.childCount <= 1) return 0;

            var layout = itemContainer.GetComponent<HorizontalLayoutGroup>();
            var itemRect = itemContainer.GetChild(0).GetComponent<RectTransform>();

            float totalItemStep = itemRect.rect.width + layout.spacing;

            float totalScrollableWidth = itemContainer.GetComponent<RectTransform>().rect.width - scrollRect.viewport.rect.width;

            return (totalScrollableWidth <= 0) ? 0 : totalItemStep / totalScrollableWidth;
        }

        System.Collections.IEnumerator LerpScroll(float target)
        {
            float time = 0;
            float startPos = scrollRect.horizontalNormalizedPosition;
            while (time < 1f)
            {
                time += Time.deltaTime * 10f;
                scrollRect.horizontalNormalizedPosition = Mathf.Lerp(startPos, target, time);
                yield return null;
            }
            scrollRect.horizontalNormalizedPosition = target;
        }

        public void TryBuy(ShopItemData item)
        {
            ShopPurchaseService.TryBuy(
                item,
                onSuccess: () =>
                {
                    RefreshShopUI();
                    FirestoreModel.IncrementPlayerStat("TotalItemsPurchased", 1);
                    spending += item.price;

                    if(PlayerLocalData.playerStats.TotalItemsPurchased >= 10 && !AchievementManager.Instance.CheckAchievement("Shopaholic"))
                    {
                        FirestoreModel.UnlockAchievement("Shopaholic");
                        MenuManager.instance.GetController<UniversalController>().ShowAchievementUnlockedPopup("Shopaholic");
                    }

                    if (spending >= 200 && !AchievementManager.Instance.CheckAchievement("Whale Spending"))
                    {
                        FirestoreModel.UnlockAchievement("Whale Spending");
                        MenuManager.instance.GetController<UniversalController>().ShowAchievementUnlockedPopup("Whale Spending");
                    }

                    AudioManager.Instance.PlaySFX("arradius dollar");
                },
                onNotEnoughMoney: () =>
                { 
                    MoneyNotEnoughView moneyNotEnoughView = MenuManager.instance.GetController<ShopController>().GetView<MoneyNotEnoughView>();
                    moneyNotEnoughView.Setup("Money not enough!");
                    moneyNotEnoughView.Show();

                    AudioManager.Instance.PlaySFX("error");
                },
                onInvalidItem: () =>
                {
                    Debug.LogError("Invalid item!");
                    AudioManager.Instance.PlaySFX("error");

                }
            );
        }
    }
}