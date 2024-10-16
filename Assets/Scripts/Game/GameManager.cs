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
            cardDrawSystem.AddCardAfterTurn();
        }
        else
        {
            cardDrawSystem.debugCurrentTurnText.text = ("AI Turn");
            //TEMP - REMOVE ONCE AI IMPLEMENTED
            StartCoroutine(switchToPlayersTurnTEMP());
        }
    }

    public void PlayHand()
    {
        Debug.Log("Played Hand");

        //IMPORTANT Make Sure The Cards Logic Is Executed Before This Is Called!
        //Could Maybe Add The Destroy To The Card GameObject
        if (cardDrawSystem.selectedPosition1.childCount > 0)
        {
            Destroy(cardDrawSystem.selectedPosition1.GetChild(0).gameObject);
            cardDrawSystem.selectedCardCount--;
        }
        if (cardDrawSystem.selectedPosition2.childCount > 0)
        {
            Destroy(cardDrawSystem.selectedPosition2.GetChild(0).gameObject);
            cardDrawSystem.selectedCardCount--;
        }

        //Handover Turn To AI
        cardDrawSystem.isPlayersTurn = false;
        NextTurn();
    }

    //THIS IS TEMPORARY UNTIL AI IS IMPLEMENTED
    IEnumerator switchToPlayersTurnTEMP()
    {
        yield return new WaitForSeconds(1);
        cardDrawSystem.isPlayersTurn = true;
        NextTurn();
    }
}
