using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class InformationData : MonoBehaviour
{
    public string title;
    public string category;
    [TextArea(3, 8)]
    public string description;
    public bool isCollected;

    public InformationData(string _title, string _category, string _description)
    {
        title = _title;
        category = _category;
        description = _description;
        isCollected = false;
    }
    
}
