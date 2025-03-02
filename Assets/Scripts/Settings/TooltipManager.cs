using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TooltipManager : MonoBehaviour
{
    public static TooltipManager Instance;

    [Header("References")]
    [SerializeField] public TextMeshProUGUI clickToPlayHandText = null;
    [SerializeField] public TextMeshProUGUI valueText = null;
    [SerializeField] public GameObject tutorialCanvas = null;
    string gameSceneName = "Game Scene";

    public bool assistsOn;
    bool loadedSettings = false;

    void Awake()
    {
        Instance = this;

        int assistsEnabled = PlayerPrefs.GetInt("TipsEnabled", 1);
        if (assistsEnabled == 1)
        {
            assistsOn = true;
        }
        else
        {
            assistsOn = false;
        }
    }

    void Start()
    {
        LoadSettings();
    }

    void FixedUpdate()
    {
        if (GameManager.Instance == null)
            return;

        if (!loadedSettings)
        {
            loadedSettings = true;
            LoadSettings();
        }

        if (!GameManager.Instance.canPlay || !assistsOn)
        {
            clickToPlayHandText.enabled = false;
        }
        else if(assistsOn && GameManager.Instance.canPlay)
        {
            clickToPlayHandText.enabled = true;
        }
    }

    public void ToggleTooltips()
    {
        string currentScene = SceneManager.GetActiveScene().name;

        if (currentScene == gameSceneName)
        {
            if (assistsOn == true)
            {
                assistsOn = false;
                PlayerPrefs.SetInt("TipsEnabled", 0);
                PlayerPrefs.Save();
                clickToPlayHandText.gameObject.SetActive(false);
                tutorialCanvas.gameObject.SetActive(false);
                valueText.text = "OFF";
            }
            else
            {
                assistsOn = true;
                PlayerPrefs.SetInt("TipsEnabled", 1);
                PlayerPrefs.Save();
                clickToPlayHandText.gameObject.SetActive(true);
                tutorialCanvas.gameObject.SetActive(true);
                valueText.text = "ON";
            }
        }
        else if(currentScene != gameSceneName)
        {
            if (assistsOn == true)
            {
                assistsOn = false;
                PlayerPrefs.SetInt("TipsEnabled", 0);
                PlayerPrefs.Save();

                valueText.text = "OFF";
            }
            else
            {
                assistsOn = true;
                PlayerPrefs.SetInt("TipsEnabled", 1);
                PlayerPrefs.Save();

                valueText.text = "ON";
            }
        }
    }

    void LoadSettings()
    {
        var tipsOn = PlayerPrefs.GetInt("TipsEnabled", 1);
        if(tipsOn == 1)
        {
            assistsOn = true;
        }
        else
        {
            assistsOn = false;
        }
    }
}
