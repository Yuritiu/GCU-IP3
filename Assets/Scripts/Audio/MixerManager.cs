using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MixerManager : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;

    [Range(1, 100)][SerializeField] private int musicVolume = 100;
    [Range(1, 100)][SerializeField] private int sfxVolume = 100;
    [Range(1, 100)][SerializeField] private int masterVolume = 100;

    private void OnValidate()
    {
        if (audioMixer == null) return;

        audioMixer.SetFloat("musicVolume", Mathf.Log10(musicVolume / 100f) * 20f);
        audioMixer.SetFloat("sfxVolume", Mathf.Log10(sfxVolume / 100f) * 20f);
        audioMixer.SetFloat("masterVolume", Mathf.Log10(masterVolume / 100f) * 20f);
    }

    public void MusicVolume(int level)
    {
        musicVolume = Mathf.Clamp(level, 1, 100);
        audioMixer.SetFloat("musicVolume", Mathf.Log10(level / 100f) * 20f);
    }

    public void SFXVolume(int level)
    {
        sfxVolume = Mathf.Clamp(level, 1, 100);
        audioMixer.SetFloat("sfxVolume", Mathf.Log10(level / 100f) * 20f);
    }

    public void MaxVolume(int level)
    {
        masterVolume = Mathf.Clamp(level, 1, 100);
        audioMixer.SetFloat("masterVolume", Mathf.Log10(level / 100f) * 20f);
    }
}
