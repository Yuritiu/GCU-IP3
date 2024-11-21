using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXManager : MonoBehaviour
{
    public static SFXManager instance;

    [SerializeField] private AudioSource SFXObject;
    [SerializeField] private AudioSource SFXLoopObject;
    [SerializeField] private AudioSource MusicObject;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void PlaySFXClip(AudioClip audioClip, Transform spawnTransform, float volume)
    {
        //spawn in gameObject
        AudioSource audioSource = Instantiate(SFXObject, spawnTransform.position, Quaternion.identity);

        //assign the audio clip
        audioSource.clip = audioClip;

        //assign the volume
        audioSource.volume = volume;

        //play sound
        audioSource.Play();

        //get length of sound FX clip
        float clipLength = audioSource.clip.length;

        //destroy thhe clip after it is done playing
        Destroy(audioSource.gameObject, clipLength);
    }
    public void PlaySFXClipLoop(AudioClip audioClip, Transform spawnTransform, float volume) //added for the heartbeat sfx to loop couldnt think of another way to do it
    {
        //spawn in gameObject
        AudioSource audioSource = Instantiate(SFXLoopObject, spawnTransform.position, Quaternion.identity);

        //assign the audio clip
        audioSource.clip = audioClip;

        //assign the volume
        audioSource.volume = volume;

        //play sound
        audioSource.Play();

        //get length of sound FX clip
        float clipLength = audioSource.clip.length;


    }
    public void PlayMusicClip(AudioClip audioClip, Transform spawnTransform, float volume) 
    {
        //spawn in gameObject
        AudioSource audioSource = Instantiate(MusicObject, spawnTransform.position, Quaternion.identity);

        //assign the audio clip
        audioSource.clip = audioClip;

        //assign the volume
        audioSource.volume = volume;

        //play sound
        audioSource.Play();

        //get length of sound FX clip
        float clipLength = audioSource.clip.length;

        
    }

    // To add an audio 
    //[SerializeField] private AudioClip Name;

    //SFXManager.instance.PlaySFXClip(SFX clip here , transform, 1f)
    //SFXManager.instance.PlaySFXClip(Music clip here , transform, 1f)
    //SFXManager.instance.PlayMusicClip(Music clip here , transform, 1f)

    //if you want to end a music clip 
    // Destroy(MusicClip);
}
