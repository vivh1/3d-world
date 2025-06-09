using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI_Movement : MonoBehaviour
{
    Animator animator;
    Rigidbody rb;

    public float moveSpeed = 0.2f;
    Vector3 stopPosition;
    float walkTime;
    public float walkCounter;
    float waitTime;
    public float waitCounter;
    int WalkDirection;
    public bool isWalking;

    // ��� �� ������� �� ������ ����
    private float originalY;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();

        // �� ���� rigidbody, �������� ��� �����������
        if (rb != null)
        {
            rb.freezeRotation = true;
        }

        // ����� �� ������ ����
        originalY = transform.position.y;

        // So that all the prefabs don't move/stop at the same time
        walkTime = Random.Range(3, 8);
        waitTime = Random.Range(2, 4);
        waitCounter = waitTime;
        walkCounter = walkTime;
        ChooseDirection();
    }

    void Update()
    {
        if (isWalking)
        {
            animator.SetBool("isRunning", true);
            walkCounter -= Time.deltaTime;

            switch (WalkDirection)
            {
                case 0:
                    transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
                    transform.position += transform.forward * moveSpeed * Time.deltaTime;
                    break;
                case 1:
                    transform.localRotation = Quaternion.Euler(0f, 90, 0f);
                    transform.position += transform.forward * moveSpeed * Time.deltaTime;
                    break;
                case 2:
                    transform.localRotation = Quaternion.Euler(0f, -90, 0f);
                    transform.position += transform.forward * moveSpeed * Time.deltaTime;
                    break;
                case 3:
                    transform.localRotation = Quaternion.Euler(0f, 180, 0f);
                    transform.position += transform.forward * moveSpeed * Time.deltaTime;
                    break;
            }

            if (walkCounter <= 0)
            {
                isWalking = false;
                animator.SetBool("isRunning", false);

                // ������� ������� �� rigidbody �� �������
                if (rb != null)
                {
                    rb.linearVelocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                }

                waitCounter = waitTime;
            }
        }
        else
        {
            waitCounter -= Time.deltaTime;

            // �������� �� ��������� � wait counter ��� �� animation
            if (waitCounter <= 0 && !IsAnimationPlaying("Idle"))
            {
                ChooseDirection();
            }
        }

        // �������� �� ���� ��� �� ��� "����� �� �� ������"
        KeepOriginalHeight();
    }

    // ������� �� ������ ������������ animation
    bool IsAnimationPlaying(string animationName)
    {
        if (animator == null) return false;

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        return stateInfo.IsName(animationName) && stateInfo.normalizedTime < 1.0f;
    }

    // ������� �� ������ ����
    void KeepOriginalHeight()
    {
        Vector3 currentPos = transform.position;
        if (Mathf.Abs(currentPos.y - originalY) > 0.01f)
        {
            transform.position = new Vector3(currentPos.x, originalY, currentPos.z);
        }
    }

    public void ChooseDirection()
    {
        WalkDirection = Random.Range(0, 4);
        isWalking = true;
        walkCounter = walkTime;
    }
}