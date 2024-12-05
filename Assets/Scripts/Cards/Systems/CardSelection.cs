using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CardSelection : MonoBehaviour
{
    [Header("Info Variables")]
    [SerializeField] private TextMeshProUGUI cardInfoText;

    [HideInInspector] public bool canSelect = true;

    private static CardSelection currentlyHoveredCard;

    void Start()
    {
        if (cardInfoText != null)
            cardInfoText.gameObject.SetActive(false);
    }

    public void CardHovered(bool hovering)
    {
        if (CardDrawSystem.Instance.cardMoving || cardInfoText == null)
            return;

        if (hovering)
        {
            if (currentlyHoveredCard != null && currentlyHoveredCard != this)
            {
                currentlyHoveredCard.cardInfoText.gameObject.SetActive(false);
                currentlyHoveredCard = null;
            }
            currentlyHoveredCard = this;
        }
        else
        {
            if (currentlyHoveredCard == this)
                currentlyHoveredCard = null;
        }
        cardInfoText.gameObject.SetActive(hovering);
    }
}
