using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class AccessoryItemUI : MonoBehaviour
{
    public TextMeshProUGUI itemNameText;
    public Image itemIcon;
    public Button equipButton;

    public void Setup(Main.Mainmenu.AccessoryData data, Main.Mainmenu.CustomizeView owner)
    {
        itemNameText.text = data.displayName;
        if (itemIcon != null) itemIcon.sprite = data.icon;

        equipButton.onClick.RemoveAllListeners();
        equipButton.onClick.AddListener(() => owner.OnItemClicked(data.category, data.spineSkinName));
    }
}