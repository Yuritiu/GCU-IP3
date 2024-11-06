using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CardSelection : MonoBehaviour
{
    //!-Coded By Charlie-!

    [Header("References")]
    [SerializeField] string cardInfoImageTag = "CardInfoImage";
    [SerializeField] string cardInfoTextTag = "CardInfoText";

    [Header("Info Variables")]
    [SerializeField] Image cardInfoImage = null;
    [SerializeField] string cardInfoText = "*Insert Card Information*";

    void Start()
    {

    }

    public void CardSelected()
    {

    }

    public void CardHovered(bool hovering)
    {
        //!-MUST SET CardInfoImage & CardInfoText IN THE UI CANVAS TAGS TO THE TAGS DEFINED IN THE REFERENCES ABOVE-!
        var cardImage = GameObject.FindGameObjectWithTag(cardInfoImageTag).GetComponent<Image>();
        var cardText = GameObject.FindGameObjectWithTag(cardInfoTextTag).GetComponent<TextMeshProUGUI>();
        var crosshair = GameObject.FindGameObjectWithTag("Crosshair").GetComponent<Crosshair>();

        if (hovering)
        {
            //Enables Inner Crosshair When Hovering
            crosshair.HoverScale(true);
        }
        else
        {
            //Enables Inner Crosshair When Hovering
            crosshair.HoverScale(false);
        }

        if(SettingsMenu.Instance.assistsEnabled)
        {         
            if (hovering)
            {
                //Set Info Image And Text
                if(cardInfoImage != null)
                {
                    cardImage.color = cardInfoImage.color;
                    cardText.text = cardInfoText;
                }
            }
            else
            {
                //Reset Info Image And Text
                cardImage.color = new Color32(0,0,0,0);
                cardText.text = null;
            }
        }
    }
}
