using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TooltipManager : MonoBehaviour
{
    public static TooltipManager Instance;

    [Header("References")]
    [SerializeField] public TextMeshProUGUI clickToPlayHandText;

    void Awake()
    {
        Instance = this;
    }

    public void ToggleTooltips(bool toggle)
    {
        if(toggle)
        {
            clickToPlayHandText.enabled = true;
        }
        else
        {
            clickToPlayHandText.enabled = false;
        }
    }
}
