using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InventorySystem : MonoBehaviour
{

    public static InventorySystem Instance { get; set; }

    public GameObject inventoryScreenUI;
    public List<GameObject> slotList = new List<GameObject>();
    public List<string> itemList = new List<string>();
    private GameObject itemToAdd;
    private GameObject whatSlotToEquip;
    public bool isFull;
    public bool isOpen;


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


    void Start()
    {
        isOpen = false;
        PopulateSlotList();
    }

    private void PopulateSlotList()
    {
        foreach(Transform child in inventoryScreenUI.transform)
        {
            if(child.CompareTag("Slot"))
            {
                slotList.Add(child.gameObject);
            }
        }
    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.I) && !isOpen)
        {

            Debug.Log("i is pressed");
            inventoryScreenUI.SetActive(true);
            isOpen = true;
            Cursor.lockState = CursorLockMode.None;

        }
        else if (Input.GetKeyDown(KeyCode.I) && isOpen)
        {
            inventoryScreenUI.SetActive(false);
            isOpen = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }


    public void addToIventory(string itemName)
    {
         whatSlotToEquip = findNextEmptySlot();
         itemToAdd = Instantiate(Resources.Load<GameObject>(itemName), whatSlotToEquip.transform.position, whatSlotToEquip.transform.rotation);
         itemToAdd.transform.SetParent(whatSlotToEquip.transform);
         
         // Ρυθμίζουμε το scale και position για inventory UI
         itemToAdd.transform.localPosition = Vector3.zero;
         itemToAdd.transform.localRotation = Quaternion.identity;
         itemToAdd.transform.localScale = Vector3.one * 0.3f; // Πολύ μικρότερο scale για να χωρά σίγουρα στο slot
         
         // Αφαιρούμε colliders για αποφυγή προβλημάτων στο UI
         Collider[] colliders = itemToAdd.GetComponentsInChildren<Collider>();
         foreach (Collider col in colliders)
         {
             col.enabled = false;
         }
         
         itemList.Add(itemName);
    }
    
    [ContextMenu("Fix All Inventory Items")]
    public void FixAllInventoryItems()
    {
        // Φτιάχνουμε όλα τα υπάρχοντα items στο inventory
        foreach (GameObject slot in slotList)
        {
            if (slot.transform.childCount > 0)
            {
                GameObject item = slot.transform.GetChild(0).gameObject;
                
                // Ρυθμίζουμε το scale και position
                item.transform.localPosition = Vector3.zero;
                item.transform.localRotation = Quaternion.identity;
                item.transform.localScale = Vector3.one * 0.4f;
                
                // Αφαιρούμε colliders
                Collider[] colliders = item.GetComponentsInChildren<Collider>();
                foreach (Collider col in colliders)
                {
                    col.enabled = false;
                }
                
                Debug.Log($"Fixed item in slot: {item.name}");
            }
        }
        
        Debug.Log("All inventory items have been fixed!");
    }

    private GameObject findNextEmptySlot()
    {
        foreach(GameObject slot in slotList)
        {
            if(slot.transform.childCount == 0)
            {
                return slot;
            }
        }
        return new GameObject();
    }

    public bool CheckFull()
    {
        int counter = 0;
        foreach(GameObject slot in slotList)
        {
            if(slot.transform.childCount > 0)
            {
                counter++;
            }
        }
        if (counter == 14)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}