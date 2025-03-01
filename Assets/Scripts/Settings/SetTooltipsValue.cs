using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class SetTooltipsValue : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI valueText;

    void Start()
    {
        int assistsEnabled = PlayerPrefs.GetInt("TipsEnabled");

        if(assistsEnabled == 1)
        {
            valueText.text = "On";
        }
        else
        {
            valueText.text = "Off";
        }
    }
}
