using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class JournalUIController : MonoBehaviour
{
    [Header("UI References")]
    public GameObject journalPanel;
    public Transform contentParent;
    public GameObject categoryHeaderPrefab;
    public GameObject entryBoxPrefab;

    [Header("Animation Settings")]
    public float fadeInDuration = 0.1f; // Μικρότερο για πιο γρήγορο animation
    public float scaleAnimationDuration = 0.2f;

    private CanvasGroup journalCanvasGroup;
    private bool isAnimating = false;

    private void Awake()
    {
        // Προσθέτουμε CanvasGroup αν δεν υπάρχει
        journalCanvasGroup = journalPanel.GetComponent<CanvasGroup>();
        if (journalCanvasGroup == null)
        {
            journalCanvasGroup = journalPanel.AddComponent<CanvasGroup>();
        }
    }

    private void Start()
    {
        // Κρύβουμε το journal στην αρχή
        journalPanel.SetActive(false);
    }

    public void ShowJournal()
    {
        Debug.Log("JournalUIController.ShowJournal() called");
        Debug.Log($"journalPanel: {(journalPanel != null ? "Found" : "Missing")}");
        Debug.Log($"isAnimating: {isAnimating}");
        
        if (isAnimating) 
        {
            Debug.Log("Journal is animating, skipping...");
            return;
        }

        if (journalPanel == null)
        {
            Debug.LogError("journalPanel is null! Cannot show journal.");
            return;
        }

        Debug.Log("Setting journalPanel active and starting animation...");
        journalPanel.SetActive(true);
        StartCoroutine(AnimateJournalIn());
    }

    public void HideJournal()
    {
        Debug.Log("JournalUIController.HideJournal() called");
        Debug.Log($"isAnimating: {isAnimating}");
        
        if (isAnimating) 
        {
            Debug.Log("Journal is animating, skipping hide...");
            return;
        }

        Debug.Log("Starting hide animation...");
        StartCoroutine(AnimateJournalOut());
    }

    private IEnumerator AnimateJournalIn()
    {
        Debug.Log("AnimateJournalIn started");
        isAnimating = true;

        // Έλεγχος CanvasGroup
        if (journalCanvasGroup == null)
        {
            Debug.LogError("journalCanvasGroup is null!");
            isAnimating = false;
            yield break;
        }

        Debug.Log("Setting initial animation values...");
        Debug.Log($"fadeInDuration: {fadeInDuration}");
        
        // Αρχικές τιμές
        journalCanvasGroup.alpha = 0f;
        journalPanel.transform.localScale = Vector3.one * 0.9f;

        float elapsedTime = 0f;
        int frameCount = 0;
        while (elapsedTime < fadeInDuration && frameCount < 1000) // Safety net
        {
            elapsedTime += Time.unscaledDeltaTime;
            frameCount++;
            float progress = elapsedTime / fadeInDuration;

            journalCanvasGroup.alpha = Mathf.Lerp(0f, 1f, progress);
            journalPanel.transform.localScale = Vector3.Lerp(Vector3.one * 0.9f, Vector3.one, progress);

            // Debug κάθε 30 frames
            if (frameCount % 30 == 0)
            {
                Debug.Log($"Animation progress: {progress:F2}, alpha: {journalCanvasGroup.alpha:F2}, frame: {frameCount}");
            }

            yield return null;
        }
        
        if (frameCount >= 1000)
        {
            Debug.LogWarning("Animation loop exceeded 1000 frames, forcing completion!");
        }

        journalCanvasGroup.alpha = 1f;
        journalPanel.transform.localScale = Vector3.one;
        isAnimating = false;
        
        Debug.Log("AnimateJournalIn completed! Journal should now be visible.");
    }

    private IEnumerator AnimateJournalOut()
    {
        isAnimating = true;

        float elapsedTime = 0f;
        while (elapsedTime < fadeInDuration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            float progress = elapsedTime / fadeInDuration;

            journalCanvasGroup.alpha = Mathf.Lerp(1f, 0f, progress);
            journalPanel.transform.localScale = Vector3.Lerp(Vector3.one, Vector3.one * 0.95f, progress);

            yield return null;
        }

        journalPanel.SetActive(false);
        isAnimating = false;
    }

    public void PopulateJournal(List<InformationData> collectedInfo)
    {
        // Καθαρίζουμε το journal
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        // Οργανώνουμε τις πληροφορίες ανά κατηγορία
        Dictionary<string, List<InformationData>> categorizedInfo = new Dictionary<string, List<InformationData>>();

        foreach (var info in collectedInfo)
        {
            if (!categorizedInfo.ContainsKey(info.category))
            {
                categorizedInfo[info.category] = new List<InformationData>();
            }
            categorizedInfo[info.category].Add(info);
        }

        // Δημιουργούμε τα UI elements για κάθε κατηγορία
        foreach (var category in categorizedInfo)
        {
            CreateCategoryHeader(category.Key);

            foreach (var info in category.Value)
            {
                CreateEntryBox(info);
            }
        }
    }

    private void CreateCategoryHeader(string categoryName)
    {
        GameObject header = Instantiate(categoryHeaderPrefab, contentParent);

        // Βρίσκουμε το TextMeshPro component
        TextMeshProUGUI headerText = header.GetComponentInChildren<TextMeshProUGUI>();
        if (headerText != null)
        {
            headerText.text = $"{categoryName.ToUpper()}";
        }
    }

    private void CreateEntryBox(InformationData info)
    {
        GameObject entry = Instantiate(entryBoxPrefab, contentParent);

        // Βρίσκουμε τα TextMeshPro components
        TextMeshProUGUI[] texts = entry.GetComponentsInChildren<TextMeshProUGUI>();

        // Ψάχνουμε για TitleText και DescriptionText
        foreach (var text in texts)
        {
            if (text.gameObject.name == "TitleText")
            {
                text.text = info.title;
            }
            else if (text.gameObject.name == "DescriptionText")
            {
                text.text = info.description;
            }
        }

        // Προσθέτουμε functionality στο button (προαιρετικό)
        Button entryButton = entry.GetComponent<Button>();
        if (entryButton != null)
        {
            entryButton.onClick.RemoveAllListeners();
            entryButton.onClick.AddListener(() => OnEntryClicked(info));
        }
    }

    private void OnEntryClicked(InformationData info)
    {
        Debug.Log($"Clicked on: {info.title}");
        // Εδώ μπορείς να προσθέσεις extra functionality
        // π.χ. να δείξεις περισσότερες πληροφορίες
    }
}