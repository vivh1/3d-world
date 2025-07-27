using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InformationTrigger : MonoBehaviour
{
    [Header("Information Settings")]
    public string informationTitle;
    public string informationCategory;
    [TextArea(3, 8)]
    public string informationDescription;

    [Header("Trigger Settings")]
    public bool triggerOnce = true;
    public float displayDuration = 5f;

    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (triggerOnce && hasTriggered) return;

            // ???????????? ??? ??????????
            InformationData newInfo = new InformationData(informationTitle, informationCategory, informationDescription);

            // ??? ??????????? ??? ???????
            InformationManager.Instance.CollectInformation(newInfo);

            // ??????????? ?? pop-up
            InformationManager.Instance.ShowInformationPopup(newInfo, displayDuration);

            hasTriggered = true;
        }
    }

    // ??? testing ??? editor
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position, GetComponent<Collider>().bounds.size);
    }
}