using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TooltipManager : MonoBehaviour
{
    public static TooltipManager Instance;

    [Header("References")]
    [SerializeField] public TextMeshProUGUI clickToPlayHandText = null;
    [SerializeField] public TextMeshProUGUI valueText = null;

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
        if(!assistsOn)
        {
            PlayerPrefs.SetInt("TipsEnabled", 1);
            PlayerPrefs.Save();
            assistsOn = true;
            valueText.text = "ON";
            //Debug.Log("AS;SISTS TOGGLED ON");
        }
        else
        {
            PlayerPrefs.SetInt("TipsEnabled", 0);
            PlayerPrefs.Save();
            assistsOn = false;
            valueText.text = "OFF";
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
