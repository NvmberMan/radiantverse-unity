using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemUI : MonoBehaviour
{
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI priceText;
    public Image itemIcon;
    public Button buyButton;

    [Space(5)]
    public GameObject ownedVisual;

    private ShopItemData itemData;
    private Main.Mainmenu.ShopView shopView;

    public void Setup(ShopItemData data, Main.Mainmenu.ShopView view, bool isOwned)
    {
        itemData = data;
        shopView = view;

        itemNameText.text = data.displayName;

        if (data is Main.Mainmenu.CrateData)
        {
            priceText.text = data.price + " $";
            buyButton.interactable = true;
        }
        else
        {
            priceText.text = data.price + " $";
            buyButton.interactable = !isOwned;

            ownedVisual.SetActive(isOwned);
        }

        if (itemIcon != null) itemIcon.sprite = data.icon;

        buyButton.onClick.RemoveAllListeners();
        buyButton.onClick.AddListener(() => shopView.TryBuy(itemData));
    }
}