using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualCardAudioEffect : MonoBehaviour
{
    AudioSource audioSource;
    bool calledSFX = false;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void FixedUpdate()
    {
        if(!calledSFX && transform.position.x < -0.4f)
        {
            calledSFX = true;
            audioSource.Play();
        }
    }
}
