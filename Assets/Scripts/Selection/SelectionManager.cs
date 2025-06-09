using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectionManager : MonoBehaviour
{

    public static SelectionManager Instance { get; set; }

    public GameObject interaction_Info_UI;
    public bool onTarget;
    Text interaction_text;

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

            if (ourInteractable && ourInteractable.playerInRange)
            {
                onTarget = true;
                interaction_text.text = ourInteractable.GetItemName();
                interaction_Info_UI.SetActive(true);
                Debug.Log(interaction_text);
            }
            else
            {
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