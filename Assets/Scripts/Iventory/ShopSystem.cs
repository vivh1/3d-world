using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopSystem : MonoBehaviour
{
    public static ShopSystem Instance { get; set; }
    
    [Header("Shop UI")]
    public GameObject shopPanel;
    public Transform shopItemsParent;
    public GameObject shopItemPrefab;
    public Text goldText;
    
    [Header("Shop Items")]
    public List<ShopItem> availableItems = new List<ShopItem>();
    
    [Header("Systems")]
    public PaymentSystem paymentSystem;
    
    private bool isShopOpen = false;
    private List<GameObject> shopSlots = new List<GameObject>();
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }
    
    private void Start()
    {
        // Προσθέτουμε κάποια αρχικά items στο κατάστημα
        InitializeShopItems();
        
        // Αν δεν υπάρχει UI, το δημιουργούμε
        if (shopPanel == null)
        {
            CreateBasicShopUI();
        }
        
        if (shopPanel != null)
            shopPanel.SetActive(false);
            
        PopulateSlotList();
        CreateShopUI();
        UpdateGoldDisplay();
    }
    
    private void Update()
    {
        // Άνοιγμα/κλείσιμο καταστήματος με P
        if (Input.GetKeyDown(KeyCode.P))
        {
            ToggleShop();
        }
        
        // Ενημέρωση χρημάτων κάθε frame
        if (isShopOpen)
        {
            UpdateGoldDisplay();
        }
    }
    
    private void PopulateSlotList()
    {
        if (shopItemsParent == null) return;
        
        // Παίρνουμε όλα τα children ως slots
        foreach (Transform child in shopItemsParent)
        {
            shopSlots.Add(child.gameObject);
        }
    }
    
    private void InitializeShopItems()
    {
        // === WEAPONS SECTION === //
        availableItems.Add(new ShopItem 
        { 
            itemName = "🪓 Πολεμικός Τσεκούρι", 
            description = "Επικό δίχειρο τσεκούρι για βαριά μάχη", 
            price = 250, 
            prefabName = "Axe2H_Epic",
            itemType = ShopItemType.Weapon 
        });
        
        availableItems.Add(new ShopItem 
        { 
            itemName = "🏹 Πολεμικό Τόξο", 
            description = "Επικό τόξο για εξ αποστάσεως μάχη", 
            price = 180, 
            prefabName = "Bow_Epic",
            itemType = ShopItemType.Weapon 
        });
        
        availableItems.Add(new ShopItem 
        { 
            itemName = "🔱 Δόρυ Θυρεάτη", 
            description = "Εύχρηστο δόρυ μονόχειρο για ταχεία επίθεση", 
            price = 120, 
            prefabName = "Spear1H_Epic",
            itemType = ShopItemType.Weapon 
        });
        
        availableItems.Add(new ShopItem 
        { 
            itemName = "🛡️ Ασπίδα του Κάστρου", 
            description = "Επικής ποιότητας ασπίδα για προστασία", 
            price = 160, 
            prefabName = "Shield_Epic",
            itemType = ShopItemType.Weapon 
        });
        
        availableItems.Add(new ShopItem 
        { 
            itemName = "⚗️ Ραβδί Μάγου", 
            description = "Μυστικό ραβδί με μαγικές ιδιότητες", 
            price = 300, 
            prefabName = "Staff_Epic",
            itemType = ShopItemType.Weapon 
        });
        
        availableItems.Add(new ShopItem 
        { 
            itemName = "🔨 Σφυρί Πολέμου", 
            description = "Βαρύ δίχειρο σφυρί καταστροφής", 
            price = 220, 
            prefabName = "Mace2H_Epic",
            itemType = ShopItemType.Weapon 
        });
        
    }
    
    private void CreateShopUI()
    {
        if (shopItemsParent == null) return;
        
        // Τοποθετούμε τα items στα slots
        for (int i = 0; i < availableItems.Count && i < shopSlots.Count; i++)
        {
            GameObject slot = shopSlots[i];
            ShopItem item = availableItems[i];
            
            // Δημιουργούμε το item στο slot
            GameObject itemToShow = Instantiate(Resources.Load<GameObject>(item.prefabName), 
                slot.transform.position, slot.transform.rotation);
            itemToShow.transform.SetParent(slot.transform);
            
            // Προσθήκη price text
            CreatePriceText(slot, item.price);
            
            // Προσθήκη click handler
            AddClickHandler(slot, i);
        }
    }
    
    private void CreatePriceText(GameObject slot, int price)
    {
        GameObject priceTextGO = new GameObject("PriceText");
        priceTextGO.transform.SetParent(slot.transform, false);
        
        Text textComponent = priceTextGO.AddComponent<Text>();
        textComponent.text = price.ToString() + " 🪙";
        textComponent.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        textComponent.fontSize = 12;
        textComponent.color = Color.yellow;
        textComponent.alignment = TextAnchor.LowerCenter;
        
        RectTransform rectTransform = priceTextGO.GetComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(1, 0);
        rectTransform.pivot = new Vector2(0.5f, 0);
        rectTransform.offsetMin = new Vector2(0, 35);
        rectTransform.offsetMax = new Vector2(0, 55);
    }
    
    private void AddClickHandler(GameObject slot, int itemIndex)
    {
        Button slotButton = slot.GetComponent<Button>();
        if (slotButton == null)
        {
            slotButton = slot.AddComponent<Button>();
        }
        
        slotButton.onClick.RemoveAllListeners();
        slotButton.onClick.AddListener(() => PurchaseItem(availableItems[itemIndex]));
        
        ColorBlock colors = slotButton.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = Color.green;
        colors.pressedColor = Color.yellow;
        slotButton.colors = colors;
        
        // Προσθήκη tooltip
        TooltipSystem tooltip = slot.GetComponent<TooltipSystem>();
        if (tooltip == null)
        {
            tooltip = slot.AddComponent<TooltipSystem>();
        }
        
        ShopItem item = availableItems[itemIndex];
        tooltip.SetTooltipText(item.itemName + "\n" + item.description);
        tooltip.showDelay = 0.1f;
    }
    
    public void PurchaseItem(ShopItem item)
    {
        Debug.Log($"Attempting to purchase: {item.itemName} for {item.price} gold");
        
        // Έλεγχος αν έχουμε αρκετά χρήματα
        if (!CurrencyManager.Instance.CanAfford(item.price))
        {
            Debug.Log("Not enough gold!");
            return;
        }
        
        // Έλεγχος αν το inventory είναι γεμάτο
        if (InventorySystem.Instance.CheckFull())
        {
            Debug.Log("Inventory is full!");
            return;
        }
        
        // Εκκίνηση διαδικασίας πληρωμής
        if (paymentSystem != null)
        {
            paymentSystem.InitiatePayment(item, () => CompletePurchase(item));
        }
        else
        {
            // Εάν δεν υπάρχει payment system, κάνουμε την αγορά απευθείας
            CompletePurchase(item);
        }
    }
    
    private void CompletePurchase(ShopItem item)
    {
        // Αφαιρούμε τα χρήματα
        if (CurrencyManager.Instance.SpendGold(item.price))
        {
            // Προσθέτουμε το item στο inventory
            InventorySystem.Instance.addToIventory(item.prefabName);
            Debug.Log($"Successfully purchased {item.itemName}!");
        }
    }
    
    private void ToggleShop()
    {
        isShopOpen = !isShopOpen;
        
        if (shopPanel != null)
        {
            shopPanel.SetActive(isShopOpen);
        }
        
        if (isShopOpen)
        {
            Cursor.lockState = CursorLockMode.None;
            Debug.Log("Shop opened with P key");
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            // Κρύβουμε όλα τα tooltips όταν κλείνει το shop
            TooltipSystem.HideAllTooltips();
            Debug.Log("Shop closed");
        }
    }
    
    private void UpdateGoldDisplay()
    {
        if (goldText != null && CurrencyManager.Instance != null)
        {
            goldText.text = $"💰 ΧΡΥΣΟΣ: {CurrencyManager.Instance.GetGold()}";
        }
    }
    
    public void CloseShop()
    {
        isShopOpen = false;
        if (shopPanel != null)
            shopPanel.SetActive(false);
        
        // Κρύβουμε όλα τα tooltips όταν κλείνει το shop
        TooltipSystem.HideAllTooltips();
        
        if (!InventorySystem.Instance.isOpen)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
    
    private void CreateBasicShopUI()
    {
        // Βρίσκουμε ή δημιουργούμε Canvas
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObj = new GameObject("Canvas");
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
        }
        
        // Shop Panel
        GameObject panel = new GameObject("ShopPanel");
        panel.transform.SetParent(canvas.transform, false);
        
        RectTransform panelRect = panel.AddComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;
        
        Image panelImg = panel.AddComponent<Image>();
        panelImg.color = new Color(1f, 1f, 1f, 1f);
        panelImg.sprite = Resources.Load<Sprite>("barmid_ready");
        
        // Title
        GameObject title = new GameObject("Title");
        title.transform.SetParent(panel.transform, false);
        
        RectTransform titleRect = title.AddComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0, 0.9f);
        titleRect.anchorMax = new Vector2(1, 1);
        titleRect.offsetMin = Vector2.zero;
        titleRect.offsetMax = Vector2.zero;
        
        Text titleText = title.AddComponent<Text>();
        titleText.text = "🛒 ΚΑΤΑΣΤΗΜΑ ΤΟΥ ΚΑΣΤΡΟΥ";
        titleText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        titleText.fontSize = 30;
        titleText.color = Color.white;
        titleText.alignment = TextAnchor.MiddleCenter;
        titleText.fontStyle = FontStyle.Bold;
        
        // Gold Display
        GameObject gold = new GameObject("GoldDisplay");
        gold.transform.SetParent(panel.transform, false);
        
        RectTransform goldRect = gold.AddComponent<RectTransform>();
        goldRect.anchorMin = new Vector2(0, 0.8f);
        goldRect.anchorMax = new Vector2(1, 0.9f);
        goldRect.offsetMin = Vector2.zero;
        goldRect.offsetMax = Vector2.zero;
        
        Text goldTextComp = gold.AddComponent<Text>();
        goldTextComp.text = "💰 ΧΡΥΣΟΣ: 1000";
        goldTextComp.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        goldTextComp.fontSize = 18;
        goldTextComp.color = Color.yellow;
        goldTextComp.alignment = TextAnchor.MiddleCenter;
        goldTextComp.fontStyle = FontStyle.Bold;
        
        // Grid για slots (όπως το inventory)
        GameObject slotsContainer = new GameObject("SlotsContainer");
        slotsContainer.transform.SetParent(panel.transform, false);
        
        RectTransform slotsRect = slotsContainer.AddComponent<RectTransform>();
        slotsRect.anchorMin = new Vector2(0.1f, 0.2f);
        slotsRect.anchorMax = new Vector2(0.9f, 0.8f);
        slotsRect.offsetMin = Vector2.zero;
        slotsRect.offsetMax = Vector2.zero;
        
        GridLayoutGroup gridLayout = slotsContainer.AddComponent<GridLayoutGroup>();
        int columnCount = 5;
        float spacing = 10f;
        float padding = 20f;
        float containerWidth = Screen.width * 0.8f; 
        float totalSpacing = spacing * (columnCount - 1) + padding;
        float availableWidth = containerWidth - totalSpacing;
        float slotSize = availableWidth / columnCount;

        gridLayout.cellSize = new Vector2(slotSize, slotSize);
        gridLayout.spacing = new Vector2(spacing, spacing);
        gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayout.constraintCount = columnCount; 
        
        // Δημιουργία slots
        for (int i = 0; i < 10; i++)
        {
            GameObject slot = new GameObject($"ShopSlot_{i}");
            slot.transform.SetParent(slotsContainer.transform, false);
            
            Image slotImg = slot.AddComponent<Image>();
            slotImg.color = new Color(0.3f, 0.3f, 0.3f, 0.8f);
            slotImg.sprite = Resources.Load<Sprite>("Mini_frame2");
        }
                
        // Ανάθεση των references
        shopPanel = panel;
        goldText = goldTextComp;
        shopItemsParent = slotsContainer.transform;
        
    }
}