using UnityEngine;

[System.Serializable]
public class ShopItem
{
    public string itemName;
    public string description;
    public int price;
    public Sprite icon;
    public string prefabName; // Για το Resources.Load
    public ShopItemType itemType;
}

public enum ShopItemType
{
    Weapon,
    Collectible,
    Book,
    Decoration
} 