using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BottleSmash : MonoBehaviour
{
    AudioSource audioSource;
    AudioClip[] audioClips;
    string playerAudioSourceName = "Player Bottle Smash Audio Source";
    string opponentAudioSourceName = "Opponent Bottle Smash Audio Source";

    GameObject audioObject;

    bool calledDestroy = false;

    void OnTriggerEnter(Collider other)
    {
        //Destroy The Bottle When Hits AI/ Player
        if (other.gameObject.CompareTag("Player") && !calledDestroy)
        {
            calledDestroy = true;
            StartCoroutine(DestroyBottle(true));
        }
        else if(other.gameObject.CompareTag("Opponent") && !calledDestroy)
        {
            calledDestroy = true;
            StartCoroutine(DestroyBottle(false));
        }
    }

    IEnumerator DestroyBottle(bool isPlayer)
    {
        //Hide Bottle Once Hits Player
        gameObject.GetComponent<MeshRenderer>().enabled = false;

        //Play Smash SFX
        RetrieveAudioSource(isPlayer);
        LoadAudioClips();
        PlayRandomClip();

        if(!isPlayer)
        {
            //Enable Opponent's Ragdoll
            RagdollToggle.Instance.ragdoll = true;
        }

        yield return new WaitForSeconds(1.5f);

        //Destroy Bottle
        Destroy(gameObject);
    }

    void RetrieveAudioSource(bool isPlayer)
    {
        if(isPlayer)
        {
            audioObject = GameObject.Find(playerAudioSourceName);
        }
        else
        {
            audioObject = GameObject.Find(opponentAudioSourceName);
        }

        audioSource = audioObject.GetComponent<AudioSource>();
    }


    void LoadAudioClips()
    {
        //Retrieve All Audio Clips In Bottle SFX Folder
        audioClips = Resources.LoadAll<AudioClip>("SFX/Bottle");
        //TODO: LOAD IN SCREAM CLIP TO PLAY IF HITS AI ASWELL
        if (audioClips.Length == 0)
        {
            Debug.LogWarning("No Audio Clips Found In Resources/SFX/Bottle Folder");
        }
    }

    public void PlayRandomClip()
    {
        if (audioSource != null && audioClips.Length > 0)
        {
            AudioClip chosenClip = audioClips[Random.Range(0, audioClips.Length)];
            audioSource.clip = chosenClip;
            //Set Random Pitch
            audioSource.pitch = Random.Range(0.98f, 1.02f);
            audioSource.Play();
        }
        else
        {
            Debug.LogWarning("Bottle AudioSource Not Set/ No Audio Clips Available");
        }
    }
}
