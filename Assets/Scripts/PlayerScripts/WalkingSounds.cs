using UnityEngine;
using System.Collections.Generic;
using System.Collections;
public class WalkingSounds : MonoBehaviour
{
    public AudioSource WalkingSound;

    void Update()
    {
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
        {
            WalkingSound.enabled = true;
        }
        else
        {
            WalkingSound.enabled = false;
        }
    }
}
