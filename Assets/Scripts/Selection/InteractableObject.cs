using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    public string ItemName;
    public bool playerInRange;

    public string GetItemName()
    {
        return ItemName;
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && playerInRange &&  SelectionManager.Instance.onTarget)
        {
            Debug.Log($"InteractableObject: Trying to pick up item with name: '{ItemName}'");
            
            if(!InventorySystem.Instance.CheckFull())
            {
                InventorySystem.Instance.addToIventory(ItemName);
                Destroy(gameObject);
            }
            else if(InventorySystem.Instance.CheckFull())
            {
                Debug.Log("The iventory is full");
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}