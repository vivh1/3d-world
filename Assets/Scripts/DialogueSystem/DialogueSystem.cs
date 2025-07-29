using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueSystem : MonoBehaviour
{
    public static DialogueSystem Instance { get; set; }

    [Header("UI Refrences")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI speakerText;
    public TextMeshProUGUI dialogueText;
    public GameObject continueButton;
    public Image speakerImage;

    [Header("Dialogue Settings")]
    public float typingSpeed = 0.05f;

    private Queue<string> sentences;
    private string activeSpeakerName;
    private Sprite activeSpeakerImage;
    private bool isTyping = false;
    private Coroutine typingCoroutine;

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = null;
        }
    }

    private void Start()
    {
        sentences = new Queue<string>();
        dialoguePanel.SetActive(false);
    }

    private void Update()
    {
        // Αν πατήσει Space ή E ενώ γράφεται το κείμενο, εμφάνισε όλο το κείμενο
        if (dialoguePanel.activeInHierarchy && isTyping && (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.E)))
        {
            CompleteTyping();
        }
        // Αν πατήσει Space ή E και έχει ολοκληρωθεί το typing, πήγαινε στην επόμενη πρόταση
        else if (dialoguePanel.activeInHierarchy && !isTyping && (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.E)))
        {
            DisplayNextSentence();
        }
    }

    public void StartDialogue(DialogueData dialogue)
    {
        // Ενεργοποίησε το dialogue panel
        dialoguePanel.SetActive(true);

        // Απενεργοποίησε την κίνηση του παίκτη
        if (InventorySystem.Instance != null)
        {
            // Χρησιμοποιούμε το InventorySystem για να ελέγξουμε αν ο παίκτης μπορεί να κινηθεί
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        // Ορισμός του ονόματος και της εικόνας του ομιλητή
        activeSpeakerName = dialogue.speakerName;
        activeSpeakerImage = dialogue.speakerImage;

        speakerText.text = activeSpeakerName;
        if (speakerImage != null && activeSpeakerImage != null)
        {
            speakerImage.sprite = activeSpeakerImage;
            speakerImage.gameObject.SetActive(true);
        }
        else
        {
            speakerImage.gameObject.SetActive(false);
        }

        // Καθαρισμός προηγούμενων προτάσεων
        sentences.Clear();

        // Προσθήκη των νέων προτάσεων στην ουρά
        foreach (string sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }

        // Εμφάνιση της πρώτης πρότασης
        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        // Αν δεν υπάρχουν άλλες προτάσεις, τελείωσε τον διάλογο
        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        // Πάρε την επόμενη πρόταση
        string sentence = sentences.Dequeue();

        // Σταμάτα το προηγούμενο typing αν υπάρχει
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        // Ξεκίνα το typing effect
        typingCoroutine = StartCoroutine(TypeSentence(sentence));
    }

    private IEnumerator TypeSentence(string sentence)
    {
        isTyping = true;
        dialogueText.text = "";

        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }

    private void CompleteTyping()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        // Εμφάνισε όλο το κείμενο της τρέχουσας πρότασης
        if (sentences.Count >= 0 && dialogueText.text.Length < dialogueText.text.Length)
        {
            dialogueText.text = dialogueText.text;
        }

        isTyping = false;
    }

    public void EndDialogue()
    {
        // Απενεργοποίησε το dialogue panel
        dialoguePanel.SetActive(false);

        // Επανενεργοποίησε την κίνηση του παίκτη
        if (InventorySystem.Instance != null)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        // Καθαρισμός
        dialogueText.text = "";
        sentences.Clear();
    }

    // Μέθοδος για το Continue Button (αν θέλεις να χρησιμοποιήσεις button αντί για Space/E)
    public void OnContinueButton()
    {
        if (!isTyping)
        {
            DisplayNextSentence();
        }
        else
        {
            CompleteTyping();
        }
    }

}
