using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TooltipManager : MonoBehaviour
{
    public static TooltipManager Instance;

    [Header("References")]
    [SerializeField] public TextMeshProUGUI clickToPlayHandText;

    bool assistsOn;

    void Awake()
    {
        Instance = this;
    }

    void FixedUpdate()
    {
        if (!GameManager.Instance.canPlay && clickToPlayHandText.enabled)
        {
            clickToPlayHandText.enabled = false;
        }
        else if(GameManager.Instance.canPlay && assistsOn && !clickToPlayHandText.enabled)
        {
            clickToPlayHandText.enabled = true;
        }
    }

    public void ToggleTooltips(bool toggle)
    {
        if(toggle)
        {
            clickToPlayHandText.enabled = true;
            assistsOn = true;
        }
        else
        {
            clickToPlayHandText.enabled = false;
            assistsOn = false;
        }
    }
}
