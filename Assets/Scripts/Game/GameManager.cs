using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.SceneManagement;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //!-Coded By Charlie & Ben-!

    //DEBUG VARIABLES -> REMOVE FROM FINAL BUILD
    public static GameManager Instance;

    [Header("Debug Variables")]
    [SerializeField] TextMeshProUGUI aiFingersText;
    [SerializeField] TextMeshProUGUI playerFingersText;
    
    [Header("Hand")]
    [SerializeField] Hand aiHand;
    [SerializeField] Hand playerHand;

    [Header("Health Variables")]
    [HideInInspector] public int aiFingers;
    [HideInInspector] public int playerFingers;

    [Header("Skip Turn Variables")]
    [HideInInspector] public bool aiSkipNextTurn;
    [HideInInspector] public bool playerSkipTurn;
    
    [Header("Cards on Table")]
    [HideInInspector] Component cardsOnTable1;
    [HideInInspector] Component cardsOnTable2;
    [HideInInspector] Component cardsOnTable3;
    [HideInInspector] Component cardsOnTable4;

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
        CardDrawSystem.Instance.isPlayersTurn = true;
        
        CardDrawSystem.Instance.debugCurrentTurnText.text = ("Play Time");
        
        CardDrawSystem.Instance.AddCardAfterTurn();
        AICardDrawSystem.Instance.AddCardAfterTurn();


        //    //TEMP - REMOVE ONCE AI IMPLEMENTED
        //    StartCoroutine(switchToPlayersTurnTEMP());
        //}
        //else
        //{
        //    //Delay Before Giving Card So That The Card Slot Is Null
        //    StartCoroutine(GiveCard());
        //    playerSkipTurn = false;
        //    aiSkipNextTurn = false;
        //}
    }

    public void PlayHand()
    {
        Debug.Log("Played Hand");

        StartCoroutine(AIPlaceCards());
    }


    public void ShowCards()
    {
        //IMPORTANT Make Sure The Cards Logic Is Executed Before This Is Called!
        //Could Maybe Add The Destroy To The Card GameObject
        if (CardDrawSystem.Instance.selectedPosition1.childCount > 0)
        {
            //For This To Work, Please Make Sure Card's Logic Is Executed In A Public Function Called PlayCard
            //And The Card's Hierarchy Mathches The 'Swing Away' Card
            cardsOnTable1 = CardDrawSystem.Instance.selectedPosition1.GetChild(0).gameObject.GetComponentAtIndex(1);
            cardsOnTable1.SendMessage("PlayCardForPlayer");

            CardDrawSystem.Instance.selectedCardCount--;
        }
        if (CardDrawSystem.Instance.selectedPosition2.childCount > 0)
        {
            //For This To Work, Please Make Sure Card's Logic Is Executed In A Public Function Called PlayCard
            //And The Card's Hierarchy Mathches The 'Swing Away' Card
            cardsOnTable2 = CardDrawSystem.Instance.selectedPosition2.GetChild(0).gameObject.GetComponentAtIndex(1);
            cardsOnTable2.SendMessage("PlayCardForPlayer");

            CardDrawSystem.Instance.selectedCardCount--;
        }
        if (cardsOnTable3 != null)
        {
            //For This To Work, Please Make Sure Card's Logic Is Executed In A Public Function Called PlayCard
            //And The Card's Hierarchy Mathches The 'Swing Away' Card
            cardsOnTable3.SendMessage("PlayCardForAI");

            //Destroy();
            AICardDrawSystem.Instance.selectedCardCount--;
        }
        if (cardsOnTable4 != null)
        {
            //For This To Work, Please Make Sure Card's Logic Is Executed In A Public Function Called PlayCard
            //And The Card's Hierarchy Mathches The 'Swing Away' Card
            cardsOnTable4.SendMessage("PlayCardForAI");

            AICardDrawSystem.Instance.selectedCardCount--;
        }

        //Handover Turn To AI
        //CardDrawSystem.Instance.isPlayersTurn = false;

        CardDrawSystem.Instance.isPlayersTurn = false;
        StartCoroutine(WaitSoCardsCanReveal());
    }

    /*public void SkipPlayerTurn()
    {
        Debug.Log("Skipped Turn");
        StartCoroutine(WaitSoCardsCanReveal());
        //Handover Turn To AI
        NextTurn();
    }*/

    IEnumerator AIPlaceCards()
    {
        //waits for cards to reveal
        yield return new WaitForSeconds(0.3f);
        cardsOnTable3 = AICardDrawSystem.Instance.SelectCard(0);
        yield return new WaitForSeconds(0.3f);
        cardsOnTable4 = AICardDrawSystem.Instance.SelectCard(1);
        yield return new WaitForSeconds(0.3f);
        ShowCards();    
    }

    IEnumerator WaitSoCardsCanReveal()
    {
        yield return new WaitForSeconds(5);
        
        if (CardDrawSystem.Instance.selectedPosition1.childCount > 0)
        {
            Destroy(CardDrawSystem.Instance.selectedPosition1.GetChild(0).gameObject);
        }
        if (CardDrawSystem.Instance.selectedPosition2.childCount > 0)
        {
            Destroy(CardDrawSystem.Instance.selectedPosition2.GetChild(0).gameObject);
        }
        if (AICardDrawSystem.Instance.selectedPosition1.childCount > 0)
        {
            Destroy(AICardDrawSystem.Instance.selectedPosition1.GetChild(0).gameObject);
        }
        if (AICardDrawSystem.Instance.selectedPosition2.childCount > 0)
        {
            Destroy(AICardDrawSystem.Instance.selectedPosition2.GetChild(0).gameObject);
        }

        yield return new WaitForSeconds(0.5f);

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
            SceneManager.LoadScene("Game Scene");
            //YOU WIN!!
        }
        else if (playerFingers <= 0)
        {
            SceneManager.LoadScene("Game Scene");
            //YOU LOSE
        }
        
        playerFingersText.text = ("Player Fingers: " + playerFingers).ToString();
        aiFingersText.text = ("AI Fingers: " + aiFingers).ToString();
    }
}

