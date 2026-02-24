using UnityEngine;

public abstract class ShopItemData : ScriptableObject
{
    [Header("Common Shop Info")]
    public string displayName;
    public Sprite icon;
    public int price;
    public abstract void OnPurchaseSuccess();
}