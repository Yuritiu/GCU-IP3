using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //!-Coded By Charlie & Ben!

    //DEBUG VARIABLES -> REMOVE FROM FINAL BUILD
    [Header("Debug Variables")]
    [SerializeField] TextMeshProUGUI aiFingersText;
    [SerializeField] TextMeshProUGUI playerFingersText;
    
    [Header("Hand")]
    [SerializeField] Hand aiHand;
    [SerializeField] Hand playerHand;

    public static GameManager Instance;

    [Header("Health Variables")]
    [HideInInspector] public int aiFingers;
    [HideInInspector] public int playerFingers;

    [Header("Skip Turn Variables")]
    [HideInInspector] public bool aiSkipNextTurn;
    [HideInInspector] public bool playerSkipTurn;
    
    [Header("Current Players Turn")]
    [HideInInspector] public bool aiTurn;
    [HideInInspector] public bool playerTurn;

    private void Awake()
    {
        Instance = this;

        //Set Fingers To 5
        aiFingers = 5;
        playerFingers = 5;

        //Set Fingers Debug Text
        UpdateHealth(0);
    }

    public void NextTurn()
    {
        aiTurn = false;
        playerTurn = false;

        if (CardDrawSystem.Instance.isPlayersTurn && !playerSkipTurn)
        {
            playerTurn = true;
            CardDrawSystem.Instance.debugCurrentTurnText.text = ("Player Turn");
            CardDrawSystem.Instance.AddCardAfterTurn();
        }
        else if (!aiSkipNextTurn)
        {
            aiTurn = true;
            CardDrawSystem.Instance.debugCurrentTurnText.text = ("AI Turn");
            
            //TEMP - REMOVE ONCE AI IMPLEMENTED
            StartCoroutine(switchToPlayersTurnTEMP());
        }
        else
        {
            //Delay Before Giving Card So That The Card Slot Is Null
            StartCoroutine(GiveCard());
            playerSkipTurn = false;
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

    public void UpdateHealth(int character)
    {
        if (character == 1)
        {
            aiFingers--;
            aiHand.RemoveFinger(aiFingers);
        }
        else if (character == 2)
        {
            playerFingers--;
            playerHand.RemoveFinger(playerFingers);
        }

        if (aiFingers <= 0)
        {
            //YOU WIN!!
        }
        else if (playerFingers <= 0)
        {
            //YOU LOSE
        }
        
        playerFingersText.text = ("Player Fingers: " + playerFingers).ToString();
        aiFingersText.text = ("AI Fingers: " + aiFingers).ToString();
    }
}

