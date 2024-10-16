using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //!-Coded By Charlie-!

    public static GameManager Instance;

    [Header("References")]
    [SerializeField] CardDrawSystem cardDrawSystem;

    private void Awake()
    {
        Instance = this;
    }

    public void NextTurn()
    {
        if (cardDrawSystem.isPlayersTurn)
        {
            cardDrawSystem.debugCurrentTurnText.text = ("Player Turn");
        }
        else
        {
            cardDrawSystem.debugCurrentTurnText.text = ("AI Turn");
        }
    }

    public void PlayHand()
    {
        Debug.Log("Played Hand");

        //IMPORTANT Make Sure The Cards Logic Is Executed Before This Is Called!
        if (cardDrawSystem.selectedPosition1 != null)
        {
            Destroy(cardDrawSystem.selectedPosition1.GetChild(0).gameObject);
            cardDrawSystem.selectedCardCount--;
        }
        if (cardDrawSystem.selectedPosition2 != null)
        {
            Destroy(cardDrawSystem.selectedPosition2.GetChild(0).gameObject);
            cardDrawSystem.selectedCardCount--;
        }

        //Handover Turn To AI
        cardDrawSystem.isPlayersTurn = false;
        NextTurn();
    }
}
