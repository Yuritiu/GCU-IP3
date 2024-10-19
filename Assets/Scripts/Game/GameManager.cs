using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //!-Coded By Charlie-!

    public static GameManager Instance;

    [Header("Skip Turn Variables")]
    [HideInInspector] public bool aiSkipNextTurn;
    [HideInInspector] public bool playerSkipTurn;

    private void Awake()
    {
        Instance = this;
    }

    public void NextTurn()
    {
        if (CardDrawSystem.Instance.isPlayersTurn && !playerSkipTurn)
        {
            CardDrawSystem.Instance.debugCurrentTurnText.text = ("Player Turn");
            CardDrawSystem.Instance.AddCardAfterTurn();
        }
        else if (!aiSkipNextTurn)
        {
            CardDrawSystem.Instance.debugCurrentTurnText.text = ("AI Turn");
            //TEMP - REMOVE ONCE AI IMPLEMENTED
            StartCoroutine(switchToPlayersTurnTEMP());
        }
        else if (aiSkipNextTurn)
        {
            //Called After Turn Has Been Skipped So That Player Can Play Again
            CardDrawSystem.Instance.isPlayersTurn = true;
            //Delay Before Giving Card So That The Card Slot Is Null
            StartCoroutine(GiveCard());
            aiSkipNextTurn = false;
        }
    }

    IEnumerator GiveCard()
    {
        yield return new WaitForSeconds(0.01f);
        CardDrawSystem.Instance.AddCardAfterTurn();
    }

    public void PlayHand()
    {
        Debug.Log("Played Hand");

        //IMPORTANT Make Sure The Cards Logic Is Executed Before This Is Called!
        //Could Maybe Add The Destroy To The Card GameObject
        if (CardDrawSystem.Instance.selectedPosition1.childCount > 0)
        {
            //For This To Work, Please Make Sure Card's Logic Is Executed In A Public Function Called PlayCard
            //And The Card's Hierarchy Mathches The 'Swing Away' Card
            var card1 = CardDrawSystem.Instance.selectedPosition1.GetChild(0).gameObject.GetComponentAtIndex(1);
            card1.SendMessage("PlayCard");

            Destroy(CardDrawSystem.Instance.selectedPosition1.GetChild(0).gameObject);
            CardDrawSystem.Instance.selectedCardCount--;
        }
        if (CardDrawSystem.Instance.selectedPosition2.childCount > 0)
        {
            //For This To Work, Please Make Sure Card's Logic Is Executed In A Public Function Called PlayCard
            //And The Card's Hierarchy Mathches The 'Swing Away' Card
            var card2 = CardDrawSystem.Instance.selectedPosition2.GetChild(0).gameObject.GetComponentAtIndex(1);
            card2.SendMessage("PlayCard");

            Destroy(CardDrawSystem.Instance.selectedPosition2.GetChild(0).gameObject);
            CardDrawSystem.Instance.selectedCardCount--;
        }

        //Handover Turn To AI
        CardDrawSystem.Instance.isPlayersTurn = false;
        NextTurn();
    }

    public void SkipPlayerTurn()
    {
        Debug.Log("Skipped Turn");

        //Handover Turn To AI
        CardDrawSystem.Instance.isPlayersTurn = false;
        NextTurn();
    }

    //THIS IS TEMPORARY UNTIL AI IS IMPLEMENTED
    IEnumerator switchToPlayersTurnTEMP()
    {
        yield return new WaitForSeconds(1);
        CardDrawSystem.Instance.isPlayersTurn = true;
        NextTurn();
    }
}
