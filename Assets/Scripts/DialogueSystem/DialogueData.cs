using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueData
{
    public string speakerName;
    public Sprite speakerImage;

    [TextArea(3, 10)]
    public string[] sentences;
}