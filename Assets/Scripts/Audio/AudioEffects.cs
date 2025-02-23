using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioEffects : MonoBehaviour
{
    [SerializeField] AudioMixer audioMixer;
    bool effectsEnabled = false;

    [Header("Initial Variables")]
    float startLowpass;
    float startDistortion;
    float startReverb;

    [Header("Target Variables")]
    float targetLowpass = 525f;
    float targetDistortion = 0.5f;
    float targetReverb = 0f;

    [Header("Audio Effect Transitions")]
    //Coroutine transitionEffectsCoroutine;
    float transitionTime = 3f;

    void Start()
    {
        startLowpass = 22000f;
        startDistortion = 0f;
        startReverb = -80f;

        if (audioMixer != null)
        {
            //Reset Variables On Game Start/ Restart
            audioMixer.SetFloat("Lowpass", startLowpass);
            audioMixer.SetFloat("Distortion", startDistortion);
            audioMixer.SetFloat("Reverb", startReverb);

            //Debug.Log("RESET VALUES");
        }
    }

    void FixedUpdate()
    {
        //Start When Bloodloss < 90 Seconds Left
        if (BloodlossSystem.Instance.currentHealth < 90)
        {
            if (!effectsEnabled)
            {
                effectsEnabled = true;

                StartCoroutine(LerpEffects());
            }
        }
    }

    IEnumerator LerpEffects()
    {
        float elapsedTime = 0f;

        //Current Values
        audioMixer.GetFloat("Lowpass", out startLowpass);
        audioMixer.GetFloat("Distortion", out startDistortion);
        audioMixer.GetFloat("Reverb", out startReverb);

        while (elapsedTime < transitionTime)
        {
            elapsedTime += Time.deltaTime;
            float time = elapsedTime / transitionTime;

            //Lerp Effect Values
            if (startLowpass != targetLowpass)
            {
                float newLowpass = Mathf.Lerp(startLowpass, targetLowpass, time);
                audioMixer.SetFloat("Lowpass", newLowpass);
            }

            if (startDistortion != targetDistortion)
            {
                float newDistortion = Mathf.Lerp(startDistortion, targetDistortion, time);
                audioMixer.SetFloat("Distortion", newDistortion);
            }

            if (startReverb != targetReverb)
            {
                float newReverb = Mathf.Lerp(startReverb, targetReverb, time);
                audioMixer.SetFloat("Reverb", newReverb);
            }

            yield return null;
        }

        //Set Final Target Values
        audioMixer.SetFloat("Lowpass", 500f);
        audioMixer.SetFloat("Distortion", 0.5f);
        audioMixer.SetFloat("Reverb", 0f);
    }
}
