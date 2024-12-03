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

    [HideInInspector] public bool canSelect = true;

    void Start()
    {

    }

    public void CardSelected()
    {
    
    }

    public void CardHovered(bool hovering)
    {
        if (CardDrawSystem.Instance.cardMoving)
            return;

        //!-MUST SET CardInfoImage & CardInfoText IN THE UI CANVAS TAGS TO THE TAGS DEFINED IN THE REFERENCES ABOVE-!
        var cardImage = GameObject.FindGameObjectWithTag(cardInfoImageTag).GetComponent<Image>();
        var cardText = GameObject.FindGameObjectWithTag(cardInfoTextTag).GetComponent<TextMeshProUGUI>();
    }
}
