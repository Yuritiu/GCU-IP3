using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameSettingsManager : MonoBehaviour
{
    public static GameSettingsManager Instance;

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

    int button = 0;

    [Header("Game Blocks")]
    public Setting assistsSetting;

    [Header("Game Values")]
    public bool assistsOn;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        LoadSettings();
        InitializeSetting(assistsSetting, ApplyAssists);
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

    void ApplyAssists()
    {      
        if(assistsSetting.currentIndex == 0)
        {
            assistsOn = true;
        }
        else
        {
            assistsOn = false;
        }

        TooltipManager.Instance.ToggleTooltips(assistsOn);

        SaveSettings();
    }

    //Save settings to PlayerPrefs
    public void SaveSettings()
    {
        PlayerPrefs.SetInt("AssistsIndex", assistsSetting.currentIndex);
        
        PlayerPrefs.Save();
    }

    // Load settings from PlayerPrefs
    public void LoadSettings()
    {
        assistsSetting.currentIndex = PlayerPrefs.GetInt("AssistsIndex", 1);

        if (assistsSetting.currentIndex == 0)
        {
            assistsOn = true;
        }
        else
        {
            assistsOn = false;
        }
    }
}
