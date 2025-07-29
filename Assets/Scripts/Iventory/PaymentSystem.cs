using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PaymentSystem : MonoBehaviour
{
    [Header("Payment UI")]
    public GameObject paymentPanel;
    public InputField cardNumberInput;
    public InputField cardNameInput;
    public InputField expiryDateInput;
    public InputField cvvInput;
    public Text totalAmountText;
    public Button confirmPaymentButton;
    public Button cancelPaymentButton;
    public Text statusText;
    
    private int currentPurchaseAmount;
    private ShopItem currentItem;
    private System.Action onPaymentSuccess;
    
    private void Start()
    {
        if (paymentPanel != null)
            paymentPanel.SetActive(false);
            
        // Setup button listeners
        if (confirmPaymentButton != null)
            confirmPaymentButton.onClick.AddListener(ProcessPayment);
            
        if (cancelPaymentButton != null)
            cancelPaymentButton.onClick.AddListener(CancelPayment);
    }
    
    public void InitiatePayment(ShopItem item, System.Action onSuccess)
    {
        currentItem = item;
        currentPurchaseAmount = item.price;
        onPaymentSuccess = onSuccess;
        
        // Show payment panel
        paymentPanel.SetActive(true);
        totalAmountText.text = $"Σύνολο: {currentPurchaseAmount} Χρυσά Νομίσματα";
        statusText.text = "Παρακαλώ εισάγετε τα στοιχεία της κάρτας σας";
        
        // Clear previous inputs
        ClearInputFields();
    }
    
    private void ProcessPayment()
    {
        // Βασική επικύρωση
        if (ValidateCardInputs())
        {
            StartCoroutine(SimulatePaymentProcessing());
        }
    }
    
    private bool ValidateCardInputs()
    {
        if (string.IsNullOrEmpty(cardNumberInput.text) || cardNumberInput.text.Length < 16)
        {
            statusText.text = "Παρακαλώ εισάγετε έγκυρο αριθμό κάρτας (16 ψηφία)";
            return false;
        }
        
        if (string.IsNullOrEmpty(cardNameInput.text))
        {
            statusText.text = "Παρακαλώ εισάγετε το όνομα κατόχου";
            return false;
        }
        
        if (string.IsNullOrEmpty(expiryDateInput.text) || expiryDateInput.text.Length < 5)
        {
            statusText.text = "Παρακαλώ εισάγετε ημερομηνία λήξης (MM/YY)";
            return false;
        }
        
        if (string.IsNullOrEmpty(cvvInput.text) || cvvInput.text.Length < 3)
        {
            statusText.text = "Παρακαλώ εισάγετε CVV (3 ψηφία)";
            return false;
        }
        
        return true;
    }
    
    private IEnumerator SimulatePaymentProcessing()
    {
        confirmPaymentButton.interactable = false;
        statusText.text = "Επεξεργασία πληρωμής...";
        
        // Προσομοίωση καθυστέρησης δικτύου
        yield return new WaitForSeconds(2f);
        
        // Προσομοίωση 90% επιτυχίας
        bool paymentSuccess = Random.Range(0f, 1f) > 0.1f;
        
        if (paymentSuccess)
        {
            statusText.text = "Η πληρωμή ολοκληρώθηκε με επιτυχία!";
            yield return new WaitForSeconds(1f);
            
            // Καλούμε την επιτυχή αγορά
            onPaymentSuccess?.Invoke();
            ClosePaymentPanel();
        }
        else
        {
            statusText.text = "Η πληρωμή απέτυχε. Παρακαλώ δοκιμάστε ξανά.";
            confirmPaymentButton.interactable = true;
        }
    }
    
    private void CancelPayment()
    {
        ClosePaymentPanel();
    }
    
    private void ClosePaymentPanel()
    {
        paymentPanel.SetActive(false);
        ClearInputFields();
        confirmPaymentButton.interactable = true;
    }
    
    private void ClearInputFields()
    {
        if (cardNumberInput != null) cardNumberInput.text = "";
        if (cardNameInput != null) cardNameInput.text = "";
        if (expiryDateInput != null) expiryDateInput.text = "";
        if (cvvInput != null) cvvInput.text = "";
        if (statusText != null) statusText.text = "";
    }
} 