using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CardSelection : MonoBehaviour
{
    [Header("Info Variables")]
    [SerializeField] private TextMeshProUGUI cardInfoText;

    [HideInInspector] public bool canSelect = true;

    void Start()
    {
        if (cardInfoText != null)
            cardInfoText.gameObject.SetActive(false);
    }

    public void CardSelected()
    {
    }

    public void CardHovered(bool hovering)
    {
        if (CardDrawSystem.Instance.cardMoving || cardInfoText == null)
            return;

        cardInfoText.gameObject.SetActive(hovering);
    }
}

