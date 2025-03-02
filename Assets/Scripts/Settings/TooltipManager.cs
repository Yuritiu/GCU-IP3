using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TooltipManager : MonoBehaviour
{
    public static TooltipManager Instance;
    public GameSettingsManager gameSettingsManager;

    [Header("References")]
    [SerializeField] public TextMeshProUGUI clickToPlayHandText = null;

    public bool assistsOn;

    void Awake()
    {
        Instance = this;

        int assistsEnabled = PlayerPrefs.GetInt("TipsEnabled");
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

        if (!GameManager.Instance.canPlay)
        {
            clickToPlayHandText.enabled = false;
        }
        else if(GameManager.Instance.canPlay && assistsOn)
        {
            clickToPlayHandText.enabled = true;
        }
    }

    public void ToggleTooltips()
    {
        if(!assistsOn)
        {
            PlayerPrefs.SetInt("TipsEnabled", 1);
            PlayerPrefs.Save();
            assistsOn = true;
            //Debug.Log("ASSISTS TOGGLED ON");
        }
        else
        {
            PlayerPrefs.SetInt("TipsEnabled", 0);
            PlayerPrefs.Save();
            assistsOn = false;
            //Debug.Log("ASSISTS TOGGLED OFF");
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
