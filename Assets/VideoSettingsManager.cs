using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VideoSettingsManager : MonoBehaviour
{
    [System.Serializable]
    public class Setting
    {
        [Header("UI Components")]
        public TextMeshProUGUI valueText;
        public Button buttonRight;
        public Button buttonLeft;

        [Header("Options and State")]
        public string[] options;
        public int currentIndex;
    }

    [Header("Settings Blocks")]
    public Setting resolutionSetting;
    public Setting refreshRateSetting;
    public Setting fullscreenSetting;
    public Setting vsyncSetting;
    public Setting overallQualitySetting;
    public Setting motionBlurSetting;
    public Setting volumetricLightingSetting;
    public Setting volumetricQualitySetting;

    void Start()
    {
        LoadSettings();
        InitializeSetting(resolutionSetting, ApplyResolution);
        InitializeSetting(refreshRateSetting, ApplyRefreshRate);
        InitializeSetting(fullscreenSetting, ApplyFullscreen);
        InitializeSetting(vsyncSetting, ApplyVSync);
        InitializeSetting(overallQualitySetting, ApplyOverallQuality);
        InitializeSetting(motionBlurSetting, ApplyMotionBlur);
        InitializeSetting(volumetricLightingSetting, ApplyVolumetricLighting);
        InitializeSetting(volumetricQualitySetting, ApplyVolumetricQuality);
    }

    void InitializeSetting(Setting setting, System.Action applyAction)
    {
        setting.valueText.text = setting.options[setting.currentIndex];
        setting.buttonRight.onClick.AddListener(() => ChangeSetting(setting, 1, applyAction));
        setting.buttonLeft.onClick.AddListener(() => ChangeSetting(setting, -1, applyAction));
    }

    void ChangeSetting(Setting setting, int direction, System.Action applyAction)
    {
        setting.currentIndex = (setting.currentIndex + direction + setting.options.Length) % setting.options.Length;
        setting.valueText.text = setting.options[setting.currentIndex];
        applyAction?.Invoke();
    }

    void ApplyResolution()
    {
        string[] resolution = resolutionSetting.options[resolutionSetting.currentIndex].Split('x');
        int width = int.Parse(resolution[0]);
        int height = int.Parse(resolution[1]);
        Screen.SetResolution(width, height, Screen.fullScreen);
    }

    void ApplyRefreshRate()
    {
        int refreshRate = int.Parse(refreshRateSetting.options[refreshRateSetting.currentIndex]);
        Application.targetFrameRate = refreshRate;
    }

    void ApplyFullscreen()
    {
        switch (fullscreenSetting.currentIndex)
        {
            case 0: //Yes
                Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                Screen.fullScreen = true;
                break;
            case 1: //Borderless
                Screen.fullScreenMode = FullScreenMode.MaximizedWindow;
                Screen.fullScreen = false;
                break;
            case 2: //No
                Screen.fullScreenMode = FullScreenMode.Windowed;
                Screen.fullScreen = false;
                break;
        }
    }

    void ApplyVSync()
    {
        QualitySettings.vSyncCount = vsyncSetting.options[vsyncSetting.currentIndex] == "On" ? 1 : 0;
    }

    void ApplyOverallQuality()
    {
        QualitySettings.SetQualityLevel(overallQualitySetting.currentIndex);
    }

    void ApplyMotionBlur()
    {
        bool isEnabled = motionBlurSetting.options[motionBlurSetting.currentIndex] == "On";
    }

    void ApplyVolumetricLighting()
    {
        bool isEnabled = volumetricLightingSetting.options[volumetricLightingSetting.currentIndex] == "On";
    }

    void ApplyVolumetricQuality()
    {
        int qualityLevel = volumetricQualitySetting.currentIndex;
    }

    //Save settings to PlayerPrefs
    public void SaveSettings()
    {
        PlayerPrefs.SetInt("ResolutionIndex", resolutionSetting.currentIndex);
        PlayerPrefs.SetInt("RefreshRateIndex", refreshRateSetting.currentIndex);
        PlayerPrefs.SetInt("FullscreenIndex", fullscreenSetting.currentIndex);
        PlayerPrefs.SetInt("VSyncIndex", vsyncSetting.currentIndex);
        PlayerPrefs.SetInt("OverallQualityIndex", overallQualitySetting.currentIndex);
        PlayerPrefs.SetInt("MotionBlurIndex", motionBlurSetting.currentIndex);
        PlayerPrefs.SetInt("VolumetricLightingIndex", volumetricLightingSetting.currentIndex);
        PlayerPrefs.SetInt("VolumetricQualityIndex", volumetricQualitySetting.currentIndex);
        PlayerPrefs.Save();
    }

    // Load settings from PlayerPrefs
    void LoadSettings()
    {
        resolutionSetting.currentIndex = PlayerPrefs.GetInt("ResolutionIndex", 2);
        refreshRateSetting.currentIndex = PlayerPrefs.GetInt("RefreshRateIndex", 1);
        fullscreenSetting.currentIndex = PlayerPrefs.GetInt("FullscreenIndex", 0);
        vsyncSetting.currentIndex = PlayerPrefs.GetInt("VSyncIndex", 0);
        overallQualitySetting.currentIndex = PlayerPrefs.GetInt("OverallQualityIndex", 0);
        motionBlurSetting.currentIndex = PlayerPrefs.GetInt("MotionBlurIndex", 0);
        volumetricLightingSetting.currentIndex = PlayerPrefs.GetInt("VolumetricLightingIndex", 0);
        volumetricQualitySetting.currentIndex = PlayerPrefs.GetInt("VolumetricQualityIndex", 0);
    }
}
