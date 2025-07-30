using UnityEngine;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance { get; set; }
    
    [SerializeField] private int playerGold = 2000; // Αρχικά χρήματα - αυξημένα για τα νέα όπλα
    
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
    
    public int GetGold()
    {
        return playerGold;
    }
    
    public bool CanAfford(int amount)
    {
        return playerGold >= amount;
    }
    
    public bool SpendGold(int amount)
    {
        if (CanAfford(amount))
        {
            playerGold -= amount;
            Debug.Log($"Spent {amount} gold. Remaining: {playerGold}");
            return true;
        }
        Debug.Log("Not enough gold!");
        return false;
    }
    
    public void AddGold(int amount)
    {
        playerGold += amount;
        Debug.Log($"Added {amount} gold. Total: {playerGold}");
    }
} 