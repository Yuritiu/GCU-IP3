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

        float assistsEnabled = PlayerPrefs.GetFloat("TipsEnabled");
        if (assistsEnabled == 1)
        {
            assistsOn = true;
        }
        else
        {
            assistsOn = false;
        }
    }

    void FixedUpdate()
    {
        if (!GameManager.Instance.canPlay && clickToPlayHandText.enabled)
        {
            clickToPlayHandText.gameObject.SetActive(false);
        }
        else if(GameManager.Instance.canPlay && assistsOn && !clickToPlayHandText.enabled)
        {
            clickToPlayHandText.gameObject.SetActive(true);
        }
    }

    public void ToggleTooltips(bool toggle)
    {
        if(toggle)
        {
            clickToPlayHandText.enabled = true;
            PlayerPrefs.SetFloat("TipsEnabled", 1);
            assistsOn = true;
        }
        else
        {
            clickToPlayHandText.enabled = false;
            PlayerPrefs.SetFloat("TipsEnabled", 0);
            assistsOn = false;
        }
    }
}
