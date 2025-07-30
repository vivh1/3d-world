using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TooltipSystem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Tooltip Settings")]
    public string tooltipText;
    public float showDelay = 0.5f;
    
    private static GameObject tooltipObject;
    private static Text tooltipTextComponent;
    private static RectTransform tooltipRect;
    private bool isHovering = false;
    private float hoverTimer = 0f;
    
    void Update()
    {
        if (isHovering)
        {
            hoverTimer += Time.deltaTime;

            if (hoverTimer >= showDelay && tooltipObject != null && !tooltipObject.activeInHierarchy)
            {
                ShowTooltip();
            }

            if (tooltipObject != null && tooltipObject.activeInHierarchy)
            {
                UpdateTooltipPosition();
            }
        }

    }

    
    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;
        hoverTimer = 0f;
        CreateTooltipIfNeeded();
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
        hoverTimer = 0f;
        HideTooltip();
    }

    
    void CreateTooltipIfNeeded()
    {
        if (tooltipObject == null)
        {
            // Βρίσκουμε το Canvas
            Canvas canvas = FindObjectOfType<Canvas>();
            if (canvas == null) return;
            
            // Δημιουργία tooltip object
            tooltipObject = new GameObject("Tooltip");
            tooltipObject.transform.SetParent(canvas.transform, false);
            
            // Background
            Image bgImage = tooltipObject.AddComponent<Image>();
            bgImage.color = new Color(0.1f, 0.1f, 0.1f, 0.9f);
            bgImage.raycastTarget = false; // Αποτρέπει το tooltip να παίρνει mouse events
            
            // RectTransform
            tooltipRect = tooltipObject.GetComponent<RectTransform>();
            tooltipRect.sizeDelta = new Vector2(200, 40);
            
            // Text component
            GameObject textObj = new GameObject("Text");
            textObj.transform.SetParent(tooltipObject.transform, false);
            
            tooltipTextComponent = textObj.AddComponent<Text>();
            tooltipTextComponent.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            tooltipTextComponent.fontSize = 14;
            tooltipTextComponent.color = new Color(1f, 0.9f, 0.7f, 1f);
            tooltipTextComponent.alignment = TextAnchor.MiddleCenter;
            
            RectTransform textRect = textObj.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = new Vector2(5, 5);
            textRect.offsetMax = new Vector2(-5, -5);
            
            // Outline για καλύτερη ορατότητα
            Outline outline = textObj.AddComponent<Outline>();
            outline.effectColor = Color.black;
            outline.effectDistance = new Vector2(1, -1);
            
            // CanvasGroup για να αποτρέψουμε όλα τα mouse events στο tooltip
            CanvasGroup canvasGroup = tooltipObject.AddComponent<CanvasGroup>();
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            
            tooltipObject.SetActive(false);
        }
    }
    
    void ShowTooltip()
    {
        if (tooltipObject != null && !string.IsNullOrEmpty(tooltipText) && isHovering)
        {
            tooltipTextComponent.text = tooltipText;
            tooltipObject.SetActive(true);
            UpdateTooltipPosition();
        }
    }
    
    void HideTooltip()
    {
        if (tooltipObject != null)
        {
            tooltipObject.SetActive(false);
        }
    }
    
    void UpdateTooltipPosition()
    {
        if (tooltipRect != null && isHovering)
        {
            Vector2 mousePos = Input.mousePosition;
            Vector2 offset = new Vector2(25, -25); // Μεγαλύτερο offset για αποφυγή flashing
            
            // Υπολογίζουμε την επιθυμητή θέση
            Vector2 targetPosition = mousePos + offset;
            
            // Έλεγχος ώστε να μην βγει εκτός οθόνης
            Vector2 screenBounds = new Vector2(Screen.width, Screen.height);
            Vector2 tooltipSize = tooltipRect.sizeDelta;
            
            // Προσαρμογή x-axis
            if (targetPosition.x + tooltipSize.x > screenBounds.x)
                targetPosition.x = mousePos.x - tooltipSize.x - 25;
                
            // Προσαρμογή y-axis
            if (targetPosition.y < 0)
                targetPosition.y = mousePos.y + 25;
            else if (targetPosition.y + tooltipSize.y > screenBounds.y)
                targetPosition.y = mousePos.y - tooltipSize.y - 25;
            
            tooltipRect.position = targetPosition;
        }
    }
    
    public void SetTooltipText(string text)
    {
        tooltipText = text;
    }
    
    public static void HideAllTooltips()
    {
        if (tooltipObject != null)
        {
            tooltipObject.SetActive(false);
        }
    }
    
    void OnDisable()
    {
        // Κρύβουμε το tooltip όταν απενεργοποιείται το GameObject
        HideTooltip();
    }
}