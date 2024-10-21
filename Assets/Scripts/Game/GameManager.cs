using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    [HideInInspector] public bool playerSkipNextTurn;
    
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
        //aiSkipNextTurn = false;
        //playerSkipNextTurn = false;

        //Add Cards For Player And AI
        CardDrawSystem.Instance.AddCardAfterTurn();
        AICardDrawSystem.Instance.AddCardAfterTurn();

        if (playerSkipNextTurn)
        {
            CardDrawSystem.Instance.isPlayersTurn = false;

            playerSkipNextTurn = false;

            Debug.Log("PLAYER SKIP");
            //Debug
            CardDrawSystem.Instance.debugCurrentTurnText.text = ("Skipped Players Turn");

            PlayHand();
        }
        else
        {
            CardDrawSystem.Instance.isPlayersTurn = true;

            //Debug
            CardDrawSystem.Instance.debugCurrentTurnText.text = ("Play Time");
        }
    }

    public void PlayHand()
    {
        Debug.Log("Played Hand");

        if (!aiSkipNextTurn)
        {
            StartCoroutine(AIPlaceCards());
        }
        else
        {
            ShowCards();
        }
    }

    public void ShowCards()
    {
        aiSkipNextTurn = false;

        //Debug
        CardDrawSystem.Instance.debugCurrentTurnText.text = ("Showing Cards");

        //IMPORTANT Make Sure The Cards Logic Is Executed Before This Is Called!
        //Could Maybe Add The Destroy To The Card GameObject
        if (CardDrawSystem.Instance.selectedPosition1.childCount > 0 && !playerSkipNextTurn)
        {
            //For This To Work, Please Make Sure Card's Logic Is Executed In A Public Function Called PlayCard
            //And The Card's Hierarchy Mathches The 'Skip Next Turn' Card
            cardsOnTable1 = CardDrawSystem.Instance.selectedPosition1.GetChild(0).gameObject.GetComponentAtIndex(1);

            cardsOnTable1.SendMessage("PlayCardForPlayer");

            CardDrawSystem.Instance.selectedCardCount--;
        }
        if (CardDrawSystem.Instance.selectedPosition2.childCount > 0 && !playerSkipNextTurn)
        {
            //For This To Work, Please Make Sure Card's Logic Is Executed In A Public Function Called PlayCard
            //And The Card's Hierarchy Mathches The 'Skip Next Turn' Card
            cardsOnTable2 = CardDrawSystem.Instance.selectedPosition2.GetChild(0).gameObject.GetComponentAtIndex(1);
            cardsOnTable2.SendMessage("PlayCardForPlayer");

            CardDrawSystem.Instance.selectedCardCount--;
        }
        if (cardsOnTable3 != null)
        {
            //For This To Work, Please Make Sure Card's Logic Is Executed In A Public Function Called PlayCard
            //And The Card's Hierarchy Mathches The 'Skip Next Turn' Card
            cardsOnTable3.SendMessage("PlayCardForAI");

            AICardDrawSystem.Instance.selectedCardCount--;
        }
        if (cardsOnTable4 != null)
        {
            //For This To Work, Please Make Sure Card's Logic Is Executed In A Public Function Called PlayCard
            //And The Card's Hierarchy Mathches The 'Skip Next Turn' Card
            cardsOnTable4.SendMessage("PlayCardForAI");

            AICardDrawSystem.Instance.selectedCardCount--;
        }

        CardDrawSystem.Instance.isPlayersTurn = false;
        StartCoroutine(WaitSoCardsCanReveal());
    }

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
        //Debug
        CardDrawSystem.Instance.debugCurrentTurnText.text = ("Revealing Cards");

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

