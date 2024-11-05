using OpenCover.Framework.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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

    private bool wPressed;
    private bool sPressed;

    [HideInInspector] public float speed = 3f;
    [SerializeField] public Transform Target1;
    [SerializeField] public Transform Target2;

    [Header("camera")]
    [SerializeField] public Camera MainCamera;

    [Header("Health Variables")]
    [HideInInspector] public int aiFingers;
    [HideInInspector] public int playerFingers;

    [Header("Gun")]
    [SerializeField] GameObject PlayerGun;
    [SerializeField] GameObject AIGun;
    [HideInInspector] public int bullets;

    [Header("Armour")]
    [HideInInspector] public int aiArmour;
    [HideInInspector] public int playerArmour;  
    
    [Header("Skip Turn Variables")]
    [HideInInspector] public int aiSkippedTurns = 0;
    [HideInInspector] public int playerSkippedTurns = 0;
    
    [Header("Draw 2 cards")]
    public GameObject backfire;
    [HideInInspector] public bool aiDraw2Cards = false;
    [HideInInspector] public bool playerDraw2Cards = false;
    
    [Header("Cards on Table")]
    [HideInInspector] Component cardsOnTable1;
    [HideInInspector] Component cardsOnTable2;
    [HideInInspector] Component cardsOnTable3;
    [HideInInspector] Component cardsOnTable4;

    [Header("Cards ready to be compared")]
    bool IsReadyToCompare;

    [HideInInspector] public bool canPlay = true;
    [HideInInspector] public bool isTutorial = false;
 
    private void Awake()
    {
        PlayerGun.SetActive(false);
        AIGun.SetActive(false);


        Instance = this;
        isTutorial = false;

        //Set Fingers To 5
        aiFingers = 5;
        playerFingers = 5;

        //Set Fingers Debug Text
        UpdateHealth(0);
        backfire.gameObject.SetActive(false);
    }

    public void addBullet()
    {
        bullets = 2;
    }

    public void NextTurn()
    {
        if (!canPlay)
            return;

        //Debug.Log("Next Turn");

        //Add Cards For Player And AI
        if (!isTutorial)
        {
            AICardDrawSystem.Instance.AddCardAfterTurn();
            AICardDrawSystem.Instance.selectedCardCount = 0;
            CardDrawSystem.Instance.AddCardAfterTurn();
            if (playerDraw2Cards == true)
            {
                playerDraw2Cards = false;
                CardDrawSystem.Instance.AddCardAfterTurn();
                backfire.gameObject.SetActive(false);
            }
            if (aiDraw2Cards == true)
            {
                aiDraw2Cards = false;
                AICardDrawSystem.Instance.AddCardAfterTurn();
            }
        }
        else
        {
            TutorialAICardDraw.Instance.AddCardAfterTurn();
            TutorialAICardDraw.Instance.selectedCardCount = 0;
            TutorialCardDraw.Instance.AddCardAfterTurn();
        }

        playerArmour = 0;
        aiArmour = 0;

        if (playerSkippedTurns > 0)
        {
            if (!isTutorial)
            {
                CardDrawSystem.Instance.isPlayersTurn = false;
            }
            else
            {
                TutorialCardDraw.Instance.isPlayersTurn = false;
            }

            playerSkippedTurns--;

            PlayHand();
        }
        else
        {
            if (!isTutorial)
            {
                CardDrawSystem.Instance.isPlayersTurn = true;

                //Debug
                CardDrawSystem.Instance.debugCurrentTurnText.text = ("Play Time");
            }
            else
            {
                TutorialCardDraw.Instance.isPlayersTurn = true;

                //Debug
                TutorialCardDraw.Instance.debugCurrentTurnText.text = ("Play Time");
            }
        }
    }

    public void PlayHand()
    {
        //Debug.Log("Played Hand: " + isTutorial);

        if (aiSkippedTurns == 0 && !isTutorial)
        {
            StartCoroutine(AIPlaceCards());
        }
        else if(aiSkippedTurns == 0 && isTutorial)
        {
            StartCoroutine(AIPlaceCardsInTutorial());
        }
        else if(!isTutorial)
        {
            ShowCards();
        }
        else if(isTutorial)
        {
            ShowCardsInTutorial();
        }
    }

    public void ShowCards()
    {
        if (aiSkippedTurns > 0)
        {
            aiSkippedTurns--;
        }

        //IMPORTANT Make Sure The Cards Logic Is Executed Before This Is Called!
        //Could Maybe Add The Destroy To The Card GameObject
        if (CardDrawSystem.Instance.selectedPosition1.childCount > 0 && playerSkippedTurns == 0)
        {
            //For This To Work, Please Make Sure Card's Logic Is Executed In A Public Function Called PlayCard
            //And The Card's Hierarchy Mathches The 'Skip Next Turn' Card
            cardsOnTable1 = CardDrawSystem.Instance.selectedPosition1.GetChild(0).gameObject.GetComponentAtIndex(1);

            cardsOnTable1.SendMessage("PlayCardForPlayer");

            CardDrawSystem.Instance.selectedCardCount--;
        }
        if (CardDrawSystem.Instance.selectedPosition2.childCount > 0 && playerSkippedTurns == 0)
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

        IsReadyToCompare = true;

        CardDrawSystem.Instance.isPlayersTurn = false;
        StartCoroutine(WaitSoCardsCanReveal());
    }

    //TUTORIAL SPECIFIC FUNCTION
    public void ShowCardsInTutorial()
    {
        if (aiSkippedTurns > 0)
        {
            aiSkippedTurns--;
        }

        //IMPORTANT Make Sure The Cards Logic Is Executed Before This Is Called!
        //Could Maybe Add The Destroy To The Card GameObject
        if (TutorialCardDraw.Instance.selectedPosition1.childCount > 0 && playerSkippedTurns == 0)
        {
            //For This To Work, Please Make Sure Card's Logic Is Executed In A Public Function Called PlayCard
            //And The Card's Hierarchy Mathches The 'Skip Next Turn' Card
            cardsOnTable1 = TutorialCardDraw.Instance.selectedPosition1.GetChild(0).gameObject.GetComponentAtIndex(1);

            cardsOnTable1.SendMessage("PlayCardForPlayer");

            TutorialCardDraw.Instance.selectedCardCount--;
        }
        if (TutorialCardDraw.Instance.selectedPosition2.childCount > 0 && playerSkippedTurns == 0)
        {
            //For This To Work, Please Make Sure Card's Logic Is Executed In A Public Function Called PlayCard
            //And The Card's Hierarchy Mathches The 'Skip Next Turn' Card
            cardsOnTable2 = TutorialCardDraw.Instance.selectedPosition2.GetChild(0).gameObject.GetComponentAtIndex(1);
            cardsOnTable2.SendMessage("PlayCardForPlayer");

            TutorialCardDraw.Instance.selectedCardCount--;
        }
        if (cardsOnTable3 != null)
        {
            //For This To Work, Please Make Sure Card's Logic Is Executed In A Public Function Called PlayCard
            //And The Card's Hierarchy Mathches The 'Skip Next Turn' Card
            cardsOnTable3.SendMessage("PlayCardForAI");

            TutorialAICardDraw.Instance.selectedCardCount--;
        }
        if (cardsOnTable4 != null)
        {
            //For This To Work, Please Make Sure Card's Logic Is Executed In A Public Function Called PlayCard
            //And The Card's Hierarchy Mathches The 'Skip Next Turn' Card
            cardsOnTable4.SendMessage("PlayCardForAI");

            TutorialAICardDraw.Instance.selectedCardCount--;
        }

        IsReadyToCompare = true;

        TutorialCardDraw.Instance.isPlayersTurn = false;
        StartCoroutine(WaitSoCardsCanRevealInTutorial());
    }

    IEnumerator AIPlaceCards()
    {
        //waits for cards to reveal
        yield return new WaitForSeconds(0.3f);
        cardsOnTable3 = AICardDrawSystem.Instance.SelectCard();
        yield return new WaitForSeconds(0.3f);
        cardsOnTable4 = AICardDrawSystem.Instance.SelectCard();
        yield return new WaitForSeconds(0.3f);
        ShowCards();
    }

    //TUTORIAL SPECIFIC FUNCTION
    IEnumerator AIPlaceCardsInTutorial()
    {
        yield return new WaitForSeconds(0.3f);
        cardsOnTable3 = TutorialAICardDraw.Instance.SelectCard();
        yield return new WaitForSeconds(0.3f);
        cardsOnTable4 = TutorialAICardDraw.Instance.SelectCard();
        yield return new WaitForSeconds(0.3f);
        ShowCardsInTutorial();
    }

    IEnumerator WaitSoCardsCanReveal()
    {
        //Debug
        CardDrawSystem.Instance.debugCurrentTurnText.text = ("Revealing Cards");

        MainCamera.transform.position = new Vector3(0f, 7.34f, -5.06f);
        MainCamera.transform.rotation = new Quaternion(77.15f, 0f, 0f, 0f);
        //waits for cards to reveal
        yield return new WaitForSeconds(2f);

        MainCamera.transform.position = new Vector3(0f, 5.45f, -11.04f);
        MainCamera.transform.rotation = new Quaternion(26.24f, 0f, 0f, 0f);

        yield return new WaitForSeconds(4);

        PlayerGun.SetActive(false);
        AIGun.SetActive(false);
        
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

        IsReadyToCompare = false;
        NextTurn();
    }

    public void PlayerRoulette()
    {
        AIGun.SetActive(true);
    }

    public void AiRoulette()
    {
        PlayerGun.SetActive(true);
    }

    IEnumerator WaitSoCardsCanRevealInTutorial()
    {
        //Debug
        TutorialCardDraw.Instance.debugCurrentTurnText.text = ("Revealing Cards");

        yield return new WaitForSeconds(4);

        if (TutorialCardDraw.Instance.selectedPosition1.childCount > 0)
        {
            Destroy(TutorialCardDraw.Instance.selectedPosition1.GetChild(0).gameObject);
        }
        if (TutorialCardDraw.Instance.selectedPosition2.childCount > 0)
        {
            Destroy(TutorialCardDraw.Instance.selectedPosition2.GetChild(0).gameObject);
        }
        if (TutorialAICardDraw.Instance.selectedPosition1.childCount > 0)
        {
            Destroy(TutorialAICardDraw.Instance.selectedPosition1.GetChild(0).gameObject);
        }
        if (TutorialAICardDraw.Instance.selectedPosition2.childCount > 0)
        {
            Destroy(TutorialAICardDraw.Instance.selectedPosition2.GetChild(0).gameObject);
        }

        yield return new WaitForSeconds(0.5f);

        IsReadyToCompare = false;
        NextTurn();
    }

    public IEnumerator WaitToCompareCards(int character, int type)
    {

        MainCamera.transform.position = new Vector3(0f, 7.34f, -5.06f);
        MainCamera.transform.rotation = new Quaternion(77.15f, 0f, 0f, 0f);
        //waits for cards to reveal
        yield return new WaitForSeconds(2f);

        MainCamera.transform.position = new Vector3(0f, 5.45f, -11.04f);
        MainCamera.transform.rotation = new Quaternion(26.24f, 0f, 0f, 0f);

        if (type == 1)
        {
            CheckArmour(character);
        }
        if(type == 2)
        {
            //Need To Wait For cardsOnTable2 Reference To Be Filled At This Point
            PlayCigarCard(character);
        }
    }

    public void UpdateHealth(int character)
    {
        //checks if character is player or ai
        //AI
        if (character == 1)
        {
            aiFingers--;
            aiHand.RemoveFinger(aiFingers);
        }
        //Player
        else if (character == 2)
        {
            playerFingers--;
            playerHand.RemoveFinger(playerFingers);

            if(playerFingers == 4)
            {
                //Debug.Log("Countdown Started");
                BloodlossSystem.Instance.StartCountdown();
            }
        }

        if (aiFingers <= 0 && !isTutorial)
        {
            var activeScene = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene(activeScene);
            //YOU WIN!!
        }
        else if (playerFingers <= 0 && !isTutorial)
        {
            var activeScene = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene(activeScene);
            //YOU LOSE :(
        }
        
        playerFingersText.text = ("Player Fingers: " + playerFingers).ToString();
        aiFingersText.text = ("AI Fingers: " + aiFingers).ToString();
    }

    public void CheckArmour(int character)
    {
        if (character == 1 && IsReadyToCompare)
        {
            if (aiArmour > 0)
            {
                aiArmour--;
                if (cardsOnTable1 != null)
                {
                    if (cardsOnTable1.gameObject.name.Contains("Gun") || cardsOnTable1.gameObject.name.Contains("Knife"))
                    { //stop gun/ knife
                        return;
                    }
                }
                if (cardsOnTable2 != null)
                {
                    if (cardsOnTable2.gameObject.name.Contains("Gun") || cardsOnTable2.gameObject.name.Contains("Knife"))
                    { //stop gun/ knife
                        return;
                    }
                }
            }
        }
        
        if (character == 2 && IsReadyToCompare)
        {
            if (playerArmour > 0)
            {
                playerArmour--;
                if (cardsOnTable3 != null)
                {
                    if (cardsOnTable3.gameObject.name.Contains("Gun") || cardsOnTable3.gameObject.name.Contains("Knife"))
                    { //stop gun/ knife
                        return;
                    }
                }
                if (cardsOnTable4 != null)
                {
                    if (cardsOnTable4.gameObject.name.Contains("Gun") || cardsOnTable4.gameObject.name.Contains("Knife"))
                    { //stop gun/ knife
                        return;
                    }
                }
            }
        }

        UpdateHealth(character);
    }

    public void PlayCigarCard(int player)
    {
        //Player Functions
        if (player == 1)
        {
            if (cardsOnTable1 != null && cardsOnTable2 != null)
            {
                var cardObject1 = cardsOnTable1.gameObject;
                var cardObject2 = cardsOnTable2.gameObject;
                Component playerClonedCard = null;

                //Card To Be Cloned Is In Slot 2
                if (cardObject1.name.Contains("Cigar") && !cardObject2.name.Contains("Cigar"))
                {
                    //Debug.Log("Called Function 1");
                    playerClonedCard = cardObject2.GetComponentAtIndex(1);
                    playerClonedCard.SendMessage("PlayCardForPlayer");
                }
                //Card To Be Cloned Is In Slot 1
                if (cardObject2.name.Contains("Cigar") && !cardObject1.name.Contains("Cigar"))
                {
                    //Debug.Log("Called Function 2");
                    playerClonedCard = cardObject1.GetComponentAtIndex(1);
                    playerClonedCard.SendMessage("PlayCardForPlayer");
                }
            }
        }
        else if (player == 2)
        {
            if (cardsOnTable3 != null && cardsOnTable4 != null)
            {
                var cardObject3 = cardsOnTable3.gameObject;
                var cardObject4 = cardsOnTable4.gameObject;
                Component aiClonedCard = null;

                //Card To Be Cloned Is In Slot 4
                if (cardObject3.name.Contains("Cigar") && !cardObject4.name.Contains("Cigar"))
                {
                    //Debug.Log("Called Function 3");
                    aiClonedCard = cardObject4.GetComponentAtIndex(1);
                    aiClonedCard.SendMessage("PlayCardForAI");
                }
                //Card To Be Cloned Is In Slot 3
                if (cardObject4.name.Contains("Cigar") && !cardObject3.name.Contains("Cigar"))
                {
                    //Debug.Log("Called Function 4");
                    aiClonedCard = cardObject3.GetComponentAtIndex(1);
                    aiClonedCard.SendMessage("PlayCardForAI");
                }
            }
        }
    }

    private void Update()
    {
        float step = Time.deltaTime * speed;
        

        if (Input.GetKey("s"))
        {
            sPressed = true;
            print("s pressed");
        }

        if (Input.GetKey("w"))
        {
            wPressed = true;
            print("w pressed");
        }

        if (sPressed == true)
        {
            

            MainCamera.transform.position = Vector3.MoveTowards(MainCamera.transform.position, Target1.transform.position, step);

            if(MainCamera.transform.position == Target1.transform.position)
            {
                sPressed = false;
                MainCamera.transform.position = MainCamera.transform.position;
            }

        }

        if(wPressed == true)
        {

            MainCamera.transform.position = Vector3.MoveTowards(MainCamera.transform.position, Target2.transform.position, step);

            if(MainCamera.transform.position == Target2.transform.position)
            {
                wPressed = false;
                MainCamera.transform.position = MainCamera.transform.position;
            }
        }

        
    }

    public void EndGameWin()
    {
        print("You Win");
        var activeScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(activeScene);
    }

    public void EndGameLose()
    {
        print("You Lose");
        var activeScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(activeScene);
    }


}

