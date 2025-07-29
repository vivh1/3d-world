using UnityEngine;
using UnityEngine.UI;

public class ShopItemUI : MonoBehaviour
{
    [Header("UI References")]
    public Text itemNameText;
    public Text descriptionText;
    public Text priceText;
    public Button buyButton;
    public Image itemIcon;
    
    private ShopItem currentItem;
    
    public void SetupItem(ShopItem item)
    {
        currentItem = item;
        
        if (itemNameText != null)
            itemNameText.text = item.itemName;
            
        if (descriptionText != null)
            descriptionText.text = item.description;
            
        if (priceText != null)
            priceText.text = $"{item.price} Χρυσά";
            
        if (itemIcon != null && item.icon != null)
            itemIcon.sprite = item.icon;
            
        if (buyButton != null)
        {
            buyButton.onClick.RemoveAllListeners();
            buyButton.onClick.AddListener(() => ShopSystem.Instance.PurchaseItem(item));
        }
    }
    
    private void Update()
    {
        // Ενημέρωση του buy button ανάλογα με τα διαθέσιμα χρήματα
        if (buyButton != null && currentItem != null && CurrencyManager.Instance != null)
        {
            bool canAfford = CurrencyManager.Instance.CanAfford(currentItem.price);
            bool inventoryFull = InventorySystem.Instance.CheckFull();
            
            buyButton.interactable = canAfford && !inventoryFull;
            
            // Αλλάζουμε το χρώμα του button
            ColorBlock colors = buyButton.colors;
            colors.normalColor = canAfford && !inventoryFull ? Color.green : Color.red;
            buyButton.colors = colors;
        }
    }
} 