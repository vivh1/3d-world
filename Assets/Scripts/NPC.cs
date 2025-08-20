using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class NPC : MonoBehaviour
{
    [Header("Dialogue Settings")]
    public bool hasDialogue = true; // Enable/disable dialogue for this NPC
    public bool playerInRange;
    public bool isTalkingWithPlayer;
    public String characterName;
    [TextArea(3, 8)]
    public string dialogText;

    [Header("Movement Settings")]
    public bool canWalk = true; // Enable/disable walking for this NPC
    public float walkRadius = 8f;
    public float minWaitTime = 3f;
    public float maxWaitTime = 10f;
    public float minWalkDistance = 3f; // Minimum distance to walk in one direction
    public float maxWalkDistance = 6f; // Maximum distance to walk in one direction

    private bool conversationStarted = false;
    private Animator animator;
    private Quaternion originalRotation;

    // Movement variables
    private NavMeshAgent agent;
    private Vector3 startingPosition;
    private bool isWaiting = false;
    private bool wasWalkingBeforeConversation = false;
    private Vector3 lastDirection = Vector3.zero; // Keep track of last walking direction

    void Start()
    {
        animator = GetComponent<Animator>();

        // Only get NavMeshAgent if walking is enabled
        if (canWalk)
        {
            agent = GetComponent<NavMeshAgent>();
            startingPosition = transform.position;
        }

        originalRotation = transform.rotation;

        // Start walking if movement is enabled
        if (canWalk && agent != null)
        {
            SetRandomDestination();
        }
    }

    void Update()
    {
        // Only handle movement if walking is enabled and not talking to player
        if (canWalk && agent != null && !isTalkingWithPlayer)
        {
            // Check if reached destination
            if (!agent.pathPending && agent.remainingDistance < 0.5f && !isWaiting)
            {
                StartCoroutine(WaitAndWalk());
            }

            // Update walking animation
            bool isMoving = agent.velocity.magnitude > 0.1f;
            if (animator != null)
                animator.SetBool("IsWalking", isMoving);
        }
    }

    public void StartConversation()
    {
        // Only start conversation if dialogue is enabled
        if (!hasDialogue || conversationStarted) return;

        // Stop movement IMMEDIATELY during conversation if walking is enabled
        if (canWalk && agent != null)
        {
            wasWalkingBeforeConversation = !agent.isStopped;

            // Stop the agent immediately
            agent.isStopped = true;
            agent.ResetPath(); // Clear current path
            agent.velocity = Vector3.zero; // Stop all momentum immediately
        }

        originalRotation = transform.rotation;
        conversationStarted = true;
        isTalkingWithPlayer = true;

        if (animator != null)
        {
            animator.SetBool("isTalking", true);
            if (canWalk) animator.SetBool("IsWalking", false); // Stop walking animation
        }

        LookAtPlayer();
        DialogueSystem.Instance.OpenDialogUI();
        DialogueSystem.Instance.dialogText.text = dialogText;
        DialogueSystem.Instance.CharacterText.text = characterName;
        DialogueSystem.Instance.option1BTN.onClick.RemoveAllListeners();
        DialogueSystem.Instance.option1BTN.onClick.AddListener(() =>
        {
            EndConversation();
        });
    }

    public void EndConversation()
    {
        if (!hasDialogue) return;

        conversationStarted = false;
        isTalkingWithPlayer = false;

        if (animator != null)
        {
            animator.SetBool("isTalking", false);
        }

        ReturnToOriginalRotation();
        DialogueSystem.Instance.CloseDialogUI();
        DialogueSystem.Instance.option1BTN.onClick.RemoveAllListeners();

        // Resume movement after conversation if walking is enabled
        if (canWalk && agent != null && wasWalkingBeforeConversation)
        {
            agent.isStopped = false;
            // Set new destination after a short delay
            Invoke("SetRandomDestination", 2f);
        }
    }

    // Improved Movement Methods
    System.Collections.IEnumerator WaitAndWalk()
    {
        isWaiting = true;
        float waitTime = UnityEngine.Random.Range(minWaitTime, maxWaitTime);
        yield return new WaitForSeconds(waitTime);

        if (!isTalkingWithPlayer && canWalk) // Only move if not in conversation and walking is enabled
        {
            SetRandomDestination();
        }
        isWaiting = false;
    }

    void SetRandomDestination()
    {
        if (!canWalk || agent == null || isTalkingWithPlayer) return;

        Vector3 newDestination = GetNaturalWalkingDestination();

        NavMeshHit hit;
        if (NavMesh.SamplePosition(newDestination, out hit, 2f, 1))
        {
            agent.SetDestination(hit.position);
        }
        else
        {
            // If the destination is invalid, try again with a different approach
            Invoke("SetRandomDestination", 1f);
        }
    }

    Vector3 GetNaturalWalkingDestination()
    {
        Vector3 newDestination;
        Vector3 currentPosition = transform.position;

        // Generate a random direction that's different from the last one
        Vector3 newDirection;
        int attempts = 0;

        do
        {
            // Create 8 possible directions (forward, back, left, right, and diagonals)
            float angle = UnityEngine.Random.Range(0f, 360f);
            // Snap to 45-degree intervals for more natural movement
            angle = Mathf.Round(angle / 45f) * 45f;

            newDirection = new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad), 0, Mathf.Cos(angle * Mathf.Deg2Rad));
            attempts++;
        }
        while (Vector3.Dot(newDirection, lastDirection) > 0.5f && attempts < 8); // Avoid going in the same direction

        // Store this direction for next time
        lastDirection = newDirection;

        // Calculate walk distance
        float walkDistance = UnityEngine.Random.Range(minWalkDistance, maxWalkDistance);

        // Calculate new destination
        newDestination = currentPosition + (newDirection * walkDistance);

        // Make sure we stay within the walk radius from starting position
        Vector3 directionFromStart = newDestination - startingPosition;
        if (directionFromStart.magnitude > walkRadius)
        {
            // Scale back to stay within radius
            directionFromStart = directionFromStart.normalized * walkRadius;
            newDestination = startingPosition + directionFromStart;
        }

        // Keep the same Y position
        newDestination.y = currentPosition.y;

        return newDestination;
    }

    // Dialogue methods (unchanged)
    public void LookAtPlayer()
    {
        var player = GameObject.FindGameObjectWithTag("Player").transform;
        Vector3 direction = player.position - transform.position;
        transform.rotation = Quaternion.LookRotation(direction);
        var yRotation = transform.eulerAngles.y;
        transform.rotation = Quaternion.Euler(0, yRotation, 0);
    }

    public void ReturnToOriginalRotation()
    {
        StartCoroutine(SmoothRotateToOriginal());
    }

    private System.Collections.IEnumerator SmoothRotateToOriginal()
    {
        Quaternion startRotation = transform.rotation;
        float duration = 1f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            transform.rotation = Quaternion.Slerp(startRotation, originalRotation, t);
            yield return null;
        }

        transform.rotation = originalRotation;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasDialogue && other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (hasDialogue && other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}