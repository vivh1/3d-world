using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NPCDialogue : MonoBehaviour
{
    [Header("Dialogue Data")]
    public DialogueData dialogue;

    [Header("Interaction Settings")]
    public float interactionRange = 3f;
    public GameObject interactionPrompt; // UI που θα δείχνει "Press E to talk"
    public string promptText = "Πάτησε E για να μιλήσεις";

    [Header("Visual Feedback")]
    public bool turnToPlayer = true;
    public float turnSpeed = 5f;

    private GameObject player;
    private bool playerInRange = false;
    private bool dialogueStarted = false;
    private Transform originalTransform;
    private Quaternion originalRotation;

    private void Start()
    {
        // Βρες τον παίκτη
        player = GameObject.FindGameObjectWithTag("Player");

        // Κρύψε το interaction prompt στην αρχή
        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(false);

            // Αν έχει Text component, βάλε το κείμενο
            Text promptTextComponent = interactionPrompt.GetComponentInChildren<Text>();
            if (promptTextComponent != null)
            {
                promptTextComponent.text = promptText;
            }
        }

        // Αποθήκευσε την αρχική rotation
        originalRotation = transform.rotation;
    }

    private void Update()
    {
        // Έλεγχος απόστασης από τον παίκτη
        if (player != null)
        {
            float distance = Vector3.Distance(transform.position, player.transform.position);

            if (distance <= interactionRange && !dialogueStarted)
            {
                // Ο παίκτης είναι κοντά
                if (!playerInRange)
                {
                    playerInRange = true;
                    ShowInteractionPrompt();
                }

                // Έλεγχος για πάτημα του E
                if (Input.GetKeyDown(KeyCode.E))
                {
                    StartDialogue();
                }
            }
            else if (distance > interactionRange && playerInRange)
            {
                // Ο παίκτης απομακρύνθηκε
                playerInRange = false;
                HideInteractionPrompt();
            }

            // Αν μιλάει με τον παίκτη, γύρνα προς το μέρος του
            if (dialogueStarted && turnToPlayer)
            {
                TurnToPlayer();
            }
        }
    }

    private void ShowInteractionPrompt()
    {
        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(true);
        }
    }

    private void HideInteractionPrompt()
    {
        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(false);
        }
    }

    private void StartDialogue()
    {
        if (DialogueSystem.Instance != null && dialogue != null)
        {
            dialogueStarted = true;
            HideInteractionPrompt();
            DialogueSystem.Instance.StartDialogue(dialogue);

            // Ξεκίνα coroutine για να ελέγχεις πότε τελειώνει ο διάλογος
            StartCoroutine(CheckDialogueEnd());
        }
    }

    private IEnumerator CheckDialogueEnd()
    {
        // Περίμενε μέχρι να κλείσει το dialogue panel
        while (DialogueSystem.Instance.dialoguePanel.activeInHierarchy)
        {
            yield return null;
        }

        // Ο διάλογος τελείωσε
        dialogueStarted = false;

        // Επαναφορά στην αρχική rotation
        if (!turnToPlayer)
        {
            transform.rotation = originalRotation;
        }
    }

    private void TurnToPlayer()
    {
        if (player != null)
        {
            // Υπολόγισε την κατεύθυνση προς τον παίκτη
            Vector3 direction = player.transform.position - transform.position;
            direction.y = 0; // Κράτα μόνο την οριζόντια περιστροφή

            // Υπολόγισε την rotation
            Quaternion targetRotation = Quaternion.LookRotation(direction);

            // Ομαλή περιστροφή
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
        }
    }

    // Για debugging - δείχνει το range στο Scene view
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}