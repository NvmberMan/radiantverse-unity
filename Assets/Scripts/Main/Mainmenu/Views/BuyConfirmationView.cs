using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Main.Mainmenu
{
    public class BuyConfirmationView : View
    {
        [SerializeField] private TMP_Text itemName;
        [SerializeField] private TMP_Text itemPrice;
        [SerializeField] private Image itemPreview;
        [SerializeField] private Button buyButton;
        [SerializeField] private Button cancelButton;

        ShopView shopView;

        void Start ()
        {
            shopView = MenuManager.instance.GetController<ShopController>().GetView<ShopView>();
            cancelButton.onClick.AddListener(CancelButton);
        }

        public void SetupButton(ShopItemData buyItem)
        {
            itemName.text = buyItem.name;
            itemPrice.text = buyItem.price.ToString();
            itemPreview.sprite = buyItem.icon;
            buyButton.onClick.RemoveAllListeners();

            buyButton.onClick.AddListener(() => shopView.TryBuy(buyItem));

            AudioManager.Instance.PlaySFX("button click");
        }

        void CancelButton()
        {
            Hide();

            AudioManager.Instance.PlaySFX("button click");

        }
    }
}