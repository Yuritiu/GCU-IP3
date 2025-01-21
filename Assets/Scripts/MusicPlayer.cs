using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    [SerializeField] private AudioClip[] MusicClips;

    // Start is called before the first frame update
    void Start()
    {
        SFXManager.instance.PlayRandomMusicClip(MusicClips, transform, 0.1f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
