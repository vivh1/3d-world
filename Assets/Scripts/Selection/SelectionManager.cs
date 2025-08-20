using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;

public class SelectionManager : MonoBehaviour
{

    public static SelectionManager Instance { get; set; }

    public GameObject interaction_Info_UI;
    public bool onTarget;
    Text interaction_text;
    public KeyCode journalToggleKey = KeyCode.E;

    private void Start()
    {
        onTarget = false;
        interaction_text = interaction_Info_UI.GetComponent<Text>();
    }

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            var selectionTransform = hit.transform;
            InteractableObject ourInteractable = selectionTransform.GetComponent<InteractableObject>();
            NPC npc = selectionTransform.GetComponent<NPC>();

            if (npc && npc.playerInRange)
            {
                interaction_text.text = "Press [E] to Talk";
                interaction_Info_UI.SetActive(true);
                if(Input.GetKeyDown(journalToggleKey) && npc.isTalkingWithPlayer == false)
                {
                    npc.StartConversation();
                }
                if (DialogueSystem.Instance.dialogUIActive)
                {
                    interaction_Info_UI.SetActive(false);
                }
            }
            else if (ourInteractable && ourInteractable.playerInRange)
            {
                onTarget = true;
                interaction_text.text = ourInteractable.GetItemName();
                interaction_Info_UI.SetActive(true);
            }
            else
            {
                interaction_text.text = "";
                interaction_Info_UI.SetActive(false);
                onTarget = false;
            }
        }
        else
        {
            interaction_Info_UI.SetActive(false);
            onTarget = false;
        }
    }
}