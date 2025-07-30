using UnityEngine;

public class DebugKeyChecker : MonoBehaviour
{
    void Update()
    {
        // Έλεγχος για όλα τα σημαντικά keys
        if (Input.GetKeyDown(KeyCode.C))
        {
            Debug.Log("🔍 C key detected by DebugKeyChecker!");
            
            // Έλεγχος αν υπάρχει InformationManager
            InformationManager infoManager = FindObjectOfType<InformationManager>();
            if (infoManager != null)
            {
                Debug.Log($"✅ InformationManager found: {infoManager.name} (Active: {infoManager.gameObject.activeInHierarchy})");
            }
            else
            {
                Debug.LogError("❌ InformationManager not found in scene!");
            }
        }
        
        if (Input.GetKeyDown(KeyCode.I))
        {
            Debug.Log("🎒 I key detected (Inventory)");
        }
        
        if (Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log("🛒 P key detected (Shop)");
        }
        
        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("💬 E key detected (Interact)");
        }
    }
} 