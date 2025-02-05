using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BottleSmash : MonoBehaviour
{
    [Header("Smash Audio Variables")]
    GameObject smashAudioObject;
    AudioSource smashAudioSource;
    AudioClip[] smashAudioClips;
    string playerSmashAudioSourceName = "Player Bottle Smash Audio Source";
    string opponentSmashAudioSourceName = "Opponent Bottle Smash Audio Source";

    [Header("Scream Audio Variables")]
    GameObject screamAudioObject;
    AudioSource screamAudioSource;
    AudioClip[] playerScreamAudioClips;
    AudioClip[] opponentScreamAudioClips;
    string playerScreamAudioSourceName = "Player Scream Smash Audio Source";
    string opponentScreamAudioSourceName = "Opponent Scream Smash Audio Source";

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
        PlayRandomClip(isPlayer);

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
            smashAudioObject = GameObject.Find(playerSmashAudioSourceName);
            screamAudioObject = GameObject.Find(playerScreamAudioSourceName);
        }
        else
        {
            smashAudioObject = GameObject.Find(opponentSmashAudioSourceName);
            screamAudioObject = GameObject.Find(opponentScreamAudioSourceName);
        }

        smashAudioSource = smashAudioObject.GetComponent<AudioSource>();
        screamAudioSource = screamAudioObject.GetComponent<AudioSource>();
    }

    void LoadAudioClips()
    {
        //Retrieve All Audio Clips In Bottle SFX Folder
        smashAudioClips = Resources.LoadAll<AudioClip>("SFX/Bottle");
        playerScreamAudioClips = Resources.LoadAll<AudioClip>("SFX/Hit Screams/Player");
        opponentScreamAudioClips = Resources.LoadAll<AudioClip>("SFX/Hit Screams/Opponent");

        if (smashAudioClips.Length == 0)
        {
            Debug.LogWarning("No Audio Clips Found In Resources/SFX/Bottle Folder");
        }

        if (playerScreamAudioClips.Length == 0)
        {
            Debug.LogWarning("No Audio Clips Found In Resources/SFX/Hit Screams/Player Folder");
        }

        if (playerScreamAudioClips.Length == 0)
        {
            Debug.LogWarning("No Audio Clips Found In Resources/SFX/Hit Screams/Opponent Folder");
        }
    }

    public void PlayRandomClip(bool isPlayer)
    {
        if (smashAudioSource != null && smashAudioClips.Length > 0)
        {
            AudioClip chosenClip = smashAudioClips[Random.Range(0, smashAudioClips.Length)];
            smashAudioSource.clip = chosenClip;
            //Set Random Pitch
            smashAudioSource.pitch = Random.Range(0.98f, 1.02f);
            smashAudioSource.Play();
        }
        else
        {
            Debug.LogWarning("Bottle Smash AudioSource Not Set/ No Audio Clips Available");
        }

        if (isPlayer)
        {
            if (screamAudioSource != null && playerScreamAudioClips.Length > 0)
            {
                AudioClip chosenClip = playerScreamAudioClips[Random.Range(0, playerScreamAudioClips.Length)];
                screamAudioSource.clip = chosenClip;
                //Set Random Pitch
                screamAudioSource.pitch = Random.Range(0.98f, 1.02f);
                screamAudioSource.Play();
            }
            else
            {
                Debug.LogWarning("Bottle Player Scream AudioSource Not Set/ No Audio Clips Available");
            }
        }
        else
        {
            if (screamAudioSource != null && opponentScreamAudioClips.Length > 0)
            {
                AudioClip chosenClip = opponentScreamAudioClips[Random.Range(0, opponentScreamAudioClips.Length)];
                screamAudioSource.clip = chosenClip;
                //Set Random Pitch
                screamAudioSource.pitch = Random.Range(0.98f, 1.02f);
                screamAudioSource.Play();
            }
            else
            {
                Debug.LogWarning("Bottle Opponent Scream AudioSource Not Set/ No Audio Clips Available");
            }
        }
    }
}
