using UnityEngine;
using UnityEngine.UI;

public class AudioSettingsManager : MonoBehaviour
{
    [SerializeField] private MixerManager mixerManager;

    [Header("Master Volume")]
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Toggle masterVolumeToggle;

    [Header("Music Volume")]
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Toggle musicVolumeToggle;

    [Header("SFX Volume")]
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private Toggle sfxVolumeToggle;

    private const int minVolume = 1;
    private const int maxVolume = 100;

    void Start()
    {
        LoadSettings();
        masterVolumeSlider.onValueChanged.AddListener(UpdateMasterVolume);
        masterVolumeToggle.onValueChanged.AddListener(ToggleMasterVolume);

        musicVolumeSlider.onValueChanged.AddListener(UpdateMusicVolume);
        musicVolumeToggle.onValueChanged.AddListener(ToggleMusicVolume);

        sfxVolumeSlider.onValueChanged.AddListener(UpdateSFXVolume);
        sfxVolumeToggle.onValueChanged.AddListener(ToggleSFXVolume);
    }

    private void UpdateMasterVolume(float value)
    {
        int adjustedValue = Mathf.Clamp(Mathf.RoundToInt(value), minVolume, maxVolume);
        mixerManager.MaxVolume(adjustedValue);
    }

    private void ToggleMasterVolume(bool isOn)
    {
        mixerManager.MaxVolume(isOn ? Mathf.RoundToInt(masterVolumeSlider.value) : 1);
    }

    private void UpdateMusicVolume(float value)
    {
        int adjustedValue = Mathf.Clamp(Mathf.RoundToInt(value), minVolume, maxVolume);
        mixerManager.MusicVolume(adjustedValue);
    }

    private void ToggleMusicVolume(bool isOn)
    {
        mixerManager.MusicVolume(isOn ? Mathf.RoundToInt(musicVolumeSlider.value) : 1);
    }

    private void UpdateSFXVolume(float value)
    {
        int adjustedValue = Mathf.Clamp(Mathf.RoundToInt(value), minVolume, maxVolume);
        mixerManager.SFXVolume(adjustedValue);
    }

    private void ToggleSFXVolume(bool isOn)
    {
        mixerManager.SFXVolume(isOn ? Mathf.RoundToInt(sfxVolumeSlider.value) : 1);
    }

    public void SaveAudioSettings()
    {
        PlayerPrefs.SetFloat("MasterVolume", masterVolumeSlider.value);
        PlayerPrefs.SetFloat("MusicVolume", musicVolumeSlider.value);
        PlayerPrefs.SetFloat("SFXVolume", sfxVolumeSlider.value);

        PlayerPrefs.SetInt("MasterVolumeToggle", masterVolumeToggle.isOn ? 1 : 0);
        PlayerPrefs.SetInt("MusicVolumeToggle", musicVolumeToggle.isOn ? 1 : 0);
        PlayerPrefs.SetInt("SFXVolumeToggle", sfxVolumeToggle.isOn ? 1 : 0);

        PlayerPrefs.Save();
    }

    public void LoadSettings()
    {
        float masterVolume = PlayerPrefs.GetFloat("MasterVolume", 10);
        masterVolumeSlider.value = masterVolume;
        mixerManager.MaxVolume(Mathf.RoundToInt(masterVolume));

        float musicVolume = PlayerPrefs.GetFloat("MusicVolume", 100);
        musicVolumeSlider.value = musicVolume;
        mixerManager.MusicVolume(Mathf.RoundToInt(musicVolume));

        float sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 100);
        sfxVolumeSlider.value = sfxVolume;
        mixerManager.SFXVolume(Mathf.RoundToInt(sfxVolume));

        bool masterVolumeToggleState = PlayerPrefs.GetInt("MasterVolumeToggle", 1) == 1;
        masterVolumeToggle.isOn = masterVolumeToggleState;

        bool musicVolumeToggleState = PlayerPrefs.GetInt("MusicVolumeToggle", 1) == 1;
        musicVolumeToggle.isOn = musicVolumeToggleState;

        bool sfxVolumeToggleState = PlayerPrefs.GetInt("SFXVolumeToggle", 1) == 1;
        sfxVolumeToggle.isOn = sfxVolumeToggleState;

        ToggleMasterVolume(masterVolumeToggleState);
        ToggleMusicVolume(musicVolumeToggleState);
        ToggleSFXVolume(sfxVolumeToggleState);
    }
}
