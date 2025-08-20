using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class RandomWalker : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkRadius = 10f;
    public float minWaitTime = 2f;
    public float maxWaitTime = 8f;

    [Header("Dialogue Settings")]
    public bool playerInRange;
    public bool isTalkingWithPlayer;
    public String characterName = "";
    [TextArea(3, 8)]
    public string dialogText = "";

    private NavMeshAgent agent;
    private Animator animator;
    private Vector3 startingPosition;
    private bool isWaiting = false;
    private bool conversationStarted = false;
    private Quaternion originalRotation;
    private bool wasWalkingBeforeConversation = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        startingPosition = transform.position;
        originalRotation = transform.rotation;

        SetRandomDestination();
    }

    void Update()
    {
        // Only handle movement if not talking to player
        if (!isTalkingWithPlayer)
        {
            if (!agent.pathPending && agent.remainingDistance < 0.5f && !isWaiting)
            {
                StartCoroutine(WaitAndWalk());
            }

            bool isMoving = agent.velocity.magnitude > 0.1f;
            if (animator != null)
                animator.SetBool("IsWalking", isMoving);
        }
    }

    // Movement Methods
    System.Collections.IEnumerator WaitAndWalk()
    {
        isWaiting = true;
        float waitTime = UnityEngine.Random.Range(minWaitTime, maxWaitTime);
        yield return new WaitForSeconds(waitTime);

        if (!isTalkingWithPlayer) // Only move if not in conversation
        {
            SetRandomDestination();
        }
        isWaiting = false;
    }

    void SetRandomDestination()
    {
        if (isTalkingWithPlayer) return;

        Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * walkRadius;
        randomDirection += startingPosition;
        randomDirection.y = startingPosition.y;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, walkRadius, 1))
        {
            agent.SetDestination(hit.position);
        }
        else
        {
            Invoke("SetRandomDestination", 1f);
        }
    }

    // Dialogue Methods
    public void StartConversation()
    {
        if (conversationStarted) return;

        // Stop movement during conversation
        wasWalkingBeforeConversation = !agent.isStopped;
        agent.isStopped = true;

        originalRotation = transform.rotation;
        conversationStarted = true;
        isTalkingWithPlayer = true;

        if (animator != null)
        {
            animator.SetBool("isTalking", true);
            animator.SetBool("IsWalking", false);
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
        conversationStarted = false;
        isTalkingWithPlayer = false;

        if (animator != null)
        {
            animator.SetBool("isTalking", false);
        }

        ReturnToOriginalRotation();
        DialogueSystem.Instance.CloseDialogUI();
        DialogueSystem.Instance.option1BTN.onClick.RemoveAllListeners();

        // Resume movement after conversation
        if (wasWalkingBeforeConversation)
        {
            agent.isStopped = false;
            Invoke("SetRandomDestination", 2f);
        }
    }

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
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}