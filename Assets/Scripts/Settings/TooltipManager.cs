using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TooltipManager : MonoBehaviour
{
    public static TooltipManager Instance;
    public GameSettingsManager gameSettingsManager;

    [Header("References")]
    [SerializeField] public TextMeshProUGUI clickToPlayHandText;

    public bool assistsOn;

    void Awake()
    {
        Instance = this;

        float assistsEnabled = PlayerPrefs.GetInt("TipsEnabled");
        if (assistsEnabled == 1)
        {
            assistsOn = true;
            Debug.Log("ASSISTS ON");
        }
        else
        {
            assistsOn = false;
            Debug.Log("ASSISTS OFF");
        }
    }

    void FixedUpdate()
    {
        if (!GameManager.Instance.canPlay)
        {
            clickToPlayHandText.enabled = false;
            Debug.Log("CANT PLAY");
        }
        else if(GameManager.Instance.canPlay && assistsOn)
        {
            clickToPlayHandText.enabled = true;
            Debug.Log("CAN PLAY");
        }
    }

    public void ToggleTooltips(bool toggle)
    {
        if(toggle)
        {
            PlayerPrefs.SetInt("TipsEnabled", 1);
            assistsOn = true;
        }
        else
        {
            PlayerPrefs.SetInt("TipsEnabled", 0);
            assistsOn = false;
        }
    }
}
