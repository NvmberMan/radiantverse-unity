using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ShopItemUI : MonoBehaviour
{
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI priceText;
    public Image itemIcon;
    public Button buyButton;

    private Main.Mainmenu.AccessoryData data;
    private Main.Mainmenu.ShopView shopView;

    public void Setup(Main.Mainmenu.AccessoryData itemData, Main.Mainmenu.ShopView view, bool isOwned)
    {
        data = itemData;
        shopView = view;

        itemNameText.text = data.displayName;
        priceText.text = isOwned ? "Owned" : data.price.ToString() + " $";
        if (itemIcon != null) itemIcon.sprite = data.icon;

        buyButton.interactable = !isOwned;
        buyButton.onClick.RemoveAllListeners();

        if (!isOwned)
        {
            buyButton.onClick.AddListener(() => shopView.TryBuyItem(data));
        }
    }
}