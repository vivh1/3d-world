using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MedievalButtonEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Button button;
    private Image buttonImage;
    private Color originalColor;
    private Color hoverColor;
    
    void Start()
    {
        button = GetComponent<Button>();
        buttonImage = GetComponent<Image>();
        
        if (buttonImage != null)
        {
            originalColor = buttonImage.color;
            // Δημιουργία hover color που είναι πιο φωτεινό
            hoverColor = new Color(
                Mathf.Min(originalColor.r + 0.2f, 1f),
                Mathf.Min(originalColor.g + 0.15f, 1f),
                Mathf.Min(originalColor.b + 0.1f, 1f),
                originalColor.a
            );
        }
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (button != null && button.interactable && buttonImage != null)
        {
            buttonImage.color = hoverColor;
            
            // Slight scale effect
            transform.localScale = Vector3.one * 1.05f;
        }
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        if (buttonImage != null)
        {
            buttonImage.color = originalColor;
            
            // Reset scale
            transform.localScale = Vector3.one;
        }
    }
} 