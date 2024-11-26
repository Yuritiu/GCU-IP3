using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.Burst.CompilerServices;
using Unity.Services.Analytics;
using Unity.Services.Core;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    //!-Coded By Charlie & Ben-!

    //DEBUG VARIABLES -> REMOVE FROM FINAL BUILD
    public static GameManager Instance;

    [HideInInspector] public bool playerGunActive = false;
    [HideInInspector] public bool aiGunActive = false;

    [Header("Debug Variables")]
    [SerializeField] TextMeshProUGUI aiFingersText;
    [SerializeField] TextMeshProUGUI playerFingersText;

    [Header("Hand")]
    [SerializeField] Hand aiHand;
    [SerializeField] Hand playerHand;

    private bool wPressed;
    private bool sPressed;
    private bool pPressed;
    


    [SerializeField] public float speed;
    [SerializeField] public Transform Target1;
    [SerializeField] public Transform Target2;
    [SerializeField] public Transform Target3;
    [SerializeField] public Transform Target4;
    [SerializeField] public Transform Target5;
    [SerializeField] public Transform Target6;
    [HideInInspector] public bool in2ndPos;
    [HideInInspector] public bool in3rdPos;


    public bool cameraMovement; //used for turning off W S P when using Knife


    [Header("camera")]
    [SerializeField] public Camera MainCamera;
    [SerializeField] public GameObject originalCameraPosition;

    [Header("Health Variables")]
    [HideInInspector] public int aiFingers;
    [HideInInspector] public int playerFingers;

    [Header("Gun")]
    [SerializeField] GameObject PlayerGun;
    [SerializeField] GameObject AIGun;
    [SerializeField] public GameObject Gun;
    [HideInInspector] public int bullets;
    public bool showddown = false;

    [Header("Armour")]
    [HideInInspector] public int aiArmour;
    [HideInInspector] public int playerArmour;

    [Header("Skip Turn Variables")]
    [HideInInspector] public int aiSkippedTurns = 0;
    [HideInInspector] public int playerSkippedTurns = 0;
    [SerializeField] public TextMeshProUGUI playerSkippedTurnsText;

    [Header("check what weapons plays have")]
    [HideInInspector] public bool aiHasGun = false;
    [HideInInspector] public bool playerHasGun = false;
    [HideInInspector] public bool playerHasKnife = false;
    [HideInInspector] public bool aiHasKnife = false;

    [Header("Backfire Text")]
    public GameObject emptyPromiseBackfire;
    public GameObject knifeBackfire;
    public GameObject armourBackfire;
    public GameObject gunBackfire;
    public GameObject batBackfire;
    public GameObject twoInChamberBackfire;
    public GameObject cigarBackfire;


    [Header("Draw 2 cards")]
    [HideInInspector] public bool aiDraw2Cards = false;
    [HideInInspector] public bool playerDraw2Cards = false;

    [Header("Cards on Table")]
    [HideInInspector] Component cardsOnTable1;
    [HideInInspector] Component cardsOnTable2;
    [HideInInspector] Component cardsOnTable3;
    [HideInInspector] Component cardsOnTable4;

    [Header("Cards ready to be compared")]
    bool IsReadyToCompare;

    [Header("Blur")]
    [SerializeField] public GameObject blur;

    [HideInInspector] public bool canPlay = true;
    [HideInInspector] public bool isTutorial = false;


    [Header("Win And Lose Screens")]
    [SerializeField] public GameObject WinScreen;
    [SerializeField] public GameObject LoseScreen;

    [Header("Action")]
    private bool canMoveOn;
    [HideInInspector] public bool inKnifeAction = false;
    [HideInInspector] public bool canCutFinger = false;
    [HideInInspector] public bool inGunAction = false;
    [HideInInspector] public bool has2Guns = false;
    [HideInInspector] public int numberOfKnifeCards = 0;
    
    [HideInInspector] public bool knife1used = false;
    [HideInInspector] public bool knife2used = false;

    [Header("Camera Movement Variables")]
    [HideInInspector] public bool isActionInProgress = false;

    [SerializeField] private AudioClip musictest;

    public int timesToShoot = 0;

    async void Start()
    {
        Time.timeScale = 1f;
        originalCameraPosition.transform.position = MainCamera.transform.position;
        playerSkippedTurnsText.enabled = false;
        //SFXManager.instance.PlayMusicClip(musictest, transform, 1f);

        await UnityServices.InitializeAsync();
        AnalyticsService.Instance.StartDataCollection();
    }

    private void Awake()
    {
        Instance = this;

        PlayerGun.SetActive(false);
        AIGun.SetActive(false);

        wPressed = false;
        sPressed = false;
        in2ndPos = false;
        in3rdPos = false;

        isTutorial = false;
        canPlay = true;

        //Set Fingers To 5
        aiFingers = 5;
        playerFingers = 4;

        //sets number of bullets
        bullets = 1;

        //Set Fingers Debug Text
        ReduceHealth(0, 0);
        DisableAllBackfires();

        blur.SetActive(false);
    }

    public void addBullet()
    {
        bullets++;
    }

    public void NextTurn()
    {
        //print("NEXT TURN");

        timesToShoot = 0;

        if (!canPlay)
            return;

        playerHasGun = false;
        aiHasGun = false;

        AICardDrawSystem.Instance.card1Moving = false;
        AICardDrawSystem.Instance.card2Moving = false;

        DisableAllBackfires();

        //Move Played Cards To Discard Pile
        CardDrawSystem.Instance.FindCardsOnTable();
        StartCoroutine(CardDrawSystem.Instance.LerpCardsToDiscardDeck(0.5f));

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
                playerSkippedTurnsText.text = "";
                playerSkippedTurnsText.enabled = false;
                TutorialCardDraw.Instance.isPlayersTurn = true;

                //Debug
                TutorialCardDraw.Instance.debugCurrentTurnText.text = ("Play Time");
            }
        }
    }

    public void PlayHand()
    {
        canPlay = false;

        CardDrawSystem.Instance.UnbanCards();
        blur.SetActive(false);

        //Debug.Log("Played Hand: " + isTutorial);

        if (aiSkippedTurns == 0 && !isTutorial)
        {
            StartCoroutine(AIPlaceCards());
        }
        else if (aiSkippedTurns == 0 && isTutorial)
        {
            StartCoroutine(AIPlaceCardsInTutorial());
        }
        else if (!isTutorial)
        {
            ShowCards();
        }
        else if (isTutorial)
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
            if (!cardsOnTable3.gameObject.name.Contains("Discarded"))
            {
                cardsOnTable3.SendMessage("PlayCardForAI");
                AICardDrawSystem.Instance.selectedCardCount--;
            }
        }
        if (cardsOnTable4 != null)
        {
            //For This To Work, Please Make Sure Card's Logic Is Executed In A Public Function Called PlayCard
            //And The Card's Hierarchy Mathches The 'Skip Next Turn' Card
            if (!cardsOnTable4.gameObject.name.Contains("Discarded"))
            {
                cardsOnTable4.SendMessage("PlayCardForAI");
                AICardDrawSystem.Instance.selectedCardCount--;
            }
        }

        canMoveOn = true;


        CardDrawSystem.Instance.isPlayersTurn = false;
        StartCoroutine(MoveCamera());
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

    IEnumerator MoveCamera()
    {
        in2ndPos = true;
        StartCoroutine(CameraTransitionIEnum(Target2));

        //waits for cards to reveal
        yield return new WaitForSeconds(3f);
        in2ndPos = false;

        if (!in3rdPos)
        {
            StartCoroutine(CameraTransitionIEnum(Target1));
        }

        IsReadyToCompare = true;
    }

    IEnumerator WaitSoCardsCanReveal()
    {
        //CHANGED FROM 5.5 TO 4 - Felt Too Long
        yield return new WaitForSeconds(4);

        IsReadyToCompare = false;
        CardDrawSystem.Instance.canPlay = true;
        canPlay = true;
        NextTurn();
    }

    public void PlayerRoulette()
    {
        if (ShootScript.instance2 != null)
        {
            timesToShoot++;
            ShootScript.instance2.PlayerShot = true;
            inGunAction = true;
            StartCoroutine(WaitForGun(AIGun));
        }
    }

    public void AiRoulette()
    {
        if(ShootScript.instance1 != null)
        {
            inGunAction = true;
            ShootScript.instance1.AiShot = true;
            StartCoroutine(WaitForGun(PlayerGun));
        }
    }

    IEnumerator WaitForGun(GameObject gun)
    {
        yield return new WaitForSeconds(1f);
        Gun.SetActive(false);

        if (gun.name == "Player Gun" && !playerGunActive)
        {
            has2Guns = false;
            playerGunActive = true;
            gun.SetActive(true);
        }
        else if (gun.name == "Player Gun" && playerGunActive)
        {
            has2Guns = true;
            PlayerRoulette();
        }

        else if (gun.name == "Ai Gun" && !playerGunActive && !aiGunActive)
        {
            has2Guns = false;
            aiGunActive = true;
            gun.SetActive(true);
        }

        else if (gun.name == "Ai Gun" && playerGunActive)
        {
            AiRoulette();
        }
        
        else if (gun.name == "Ai Gun" && !playerGunActive && aiGunActive)
        {
            has2Guns = true;
            AiRoulette();
        }
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
        CardDrawSystem.Instance.canPlay = true;
        NextTurn();
    }

    public IEnumerator WaitToCompareCards(int character, int type)
    {
        //waits for cards to reveal
        yield return new WaitForSeconds(1f);

        // type meaning // 1 is knife // 2 is cigar // 3 in gun//

        if (type == 1 || type == 3)
        {
            if(type == 1)
            {
                CheckArmour(character, type);
            }
        }
        if (type == 2)
        {
            //Need To Wait For cardsOnTable2 Reference To Be Filled At This Point
            PlayCigarCard(character);
        }
    }

    public void ReduceHealth(int character, int type)
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
            if (type == 1)
            {
                playerHand.StartOfAction();
            }
            if (type == 3)
            {
                playerHand.RemoveFinger(playerFingers);
                playerFingers--;
            }
            //Debug.Log("Countdown Started");
            BloodlossSystem.Instance.IncreaseBloodloss();
        }
        CheckFingers();
    }

    public void CheckFingers()
    {

        if (aiFingers <= 0 && !isTutorial)
        {
            EndGameWin();
        }
        else if (playerFingers < 0 && !isTutorial)
        {
            EndGameLose();
        }

        playerFingersText.text = ("Player Fingers: " + playerFingers).ToString();
        aiFingersText.text = ("AI Fingers: " + aiFingers).ToString();

    }

    public void CheckArmour(int character, int type)
    {
        //print(playerArmour);

        //this ensures the armour stops the gun instead of the knife
        if (character == 1)
        {
            if (aiArmour == 1)
            {
                if (playerHasGun && playerHasKnife)
                {
                    if (type == 1)
                    {
                        ReduceHealth(character, type);
                    }
                    return;
                }
                else if (aiArmour > 0)
                {
                    aiArmour--;
                    return;
                }
                else if (type == 1)
                {
                    ReduceHealth(character, type);
                }
                else if (type == 3)
                {
                    FireGun(character);
                }
            }
            else
            {
                if (aiArmour > 0)
                {
                    aiArmour--;
                    return;
                }
                else if (type == 1)
                {
                    ReduceHealth(character, type);
                }
                else if (type == 3)
                {
                    FireGun(character);
                }
            }
        }
        if (character == 2)
        {
            if (playerArmour == 1)
            {
                if (aiHasGun && aiHasKnife)
                {
                    print("made it");
                    if (type == 1)
                    {
                        ReduceHealth(character, type);
                    }
                    return;
                }
                else if (type == 3)
                {
                    playerArmour--;
                    return;
                }
                if (!knife1used || !knife2used)
                {
                    knife1used = false;
                    knife2used = false;
                    canCutFinger = false;
                    inKnifeAction = false;
                    playerArmour--;
                }
                else
                {
                    numberOfKnifeCards--;
                    knife2used = false;
                    playerArmour--;
                    ReduceHealth(character, type);
                }
            }
            else
            {
                if (playerArmour == 2 && (knife1used && knife2used))
                {
                    knife1used = false;
                    knife2used = false;
                    canCutFinger = false;
                    inKnifeAction = false;
                    playerArmour = 0;
                }
                else if (playerArmour > 0)
                {
                    playerArmour--;
                    return;
                }
                else if (type == 1)
                {
                    ReduceHealth(character, type);
                }
                else if (type == 3)
                {
                    FireGun(character);
                }
            }
        }
    }

    private void FireGun(int character)
    {
        //GUN WINS GAME
        if (character == 1)
        {
            EndGameWin();
        }
        if (character == 2)
        {
            EndGameLose();
        }
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

                //Card To Be Cloned Is In Slot 2
                if (cardObject1.name.Contains("cigar") && !cardObject2.name.Contains("cigar"))
                {
                    Debug.Log("Called Function 1");
                    cardObject2.SendMessage("PlayCardForPlayer");
                }
                //Card To Be Cloned Is In Slot 1
                if (cardObject2.name.Contains("cigar") && !cardObject1.name.Contains("cigar"))
                {
                    Debug.Log("Called Function 2");
                    cardObject1.SendMessage("PlayCardForPlayer");
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
                    Debug.Log("Called Function 3");
                    aiClonedCard = cardObject4.GetComponentAtIndex(1);
                    aiClonedCard.SendMessage("PlayCardForAI");
                }
                //Card To Be Cloned Is In Slot 3
                if (cardObject4.name.Contains("Cigar") && !cardObject3.name.Contains("Cigar"))
                {
                    Debug.Log("Called Function 4");
                    aiClonedCard = cardObject3.GetComponentAtIndex(1);
                    aiClonedCard.SendMessage("PlayCardForAI");
                }
            }
        }
    }

    private void Update()
    {
        //print(playerFingers);

        if(Input.GetKeyDown(KeyCode.Space))
        {
            EndGameLose();
            EndGameWin();
        }

        if (showddown == true && ShootScript.instance1.AiShot == false && ShootScript.instance2.PlayerShot == false)
        {
            GameManager.Instance.Showdown();
        }

        if (PlayerGun.activeInHierarchy == true)
        {
            inGunAction = true;
        }

        if(AIGun.activeInHierarchy == true)
        {
            inGunAction = true;
        }

        //Input for "s" key
        if (Input.GetKey("s") && !isActionInProgress)
        {
            isActionInProgress = true;
            in2ndPos = false;
            in3rdPos = false;

            StartCoroutine(HandleCameraTransition(Target1));
        }

        //Input for "w" key
        if (Input.GetKey("w") && !isActionInProgress)
        {
            isActionInProgress = true;
            in2ndPos = true;
            in3rdPos = false;

            StartCoroutine(HandleCameraTransition(Target2));
        }

        //Input for "p" key
        if (Input.GetKey("p") && !isActionInProgress)
        {
            isActionInProgress = true;
            in2ndPos = false;
            in3rdPos = true;

            StartCoroutine(HandleCameraTransition(Target3));
        }

        if (allActionsDone() == true)
        {
            StartCoroutine(WaitSoCardsCanReveal());
        }
    }

    public void Showdown()
    {
        int randForBullet = UnityEngine.Random.Range(1, 7);
        ShootScript.instance2.PRandom = randForBullet;
        randForBullet = UnityEngine.Random.Range(1, 7);
        ShootScript.instance1.AiRandom = randForBullet;
        showddown = true;
        AiRoulette();
        PlayerRoulette();
    }
    private void DisableAllBackfires()
    {
        emptyPromiseBackfire.gameObject.SetActive(false);
        knifeBackfire.gameObject.SetActive(false);
        armourBackfire.gameObject.SetActive(false);
        batBackfire.gameObject.SetActive(false);
        gunBackfire.gameObject.SetActive(false);
        twoInChamberBackfire.gameObject.SetActive(false);
        cigarBackfire.gameObject.SetActive(false);
    }

    IEnumerator HandleCameraTransition(Transform target)
    {
        yield return CameraTransitionIEnum(target);
    }

    public IEnumerator CameraTransitionIEnum(Transform Target)
    {
        float t = 0.00f;
        Vector3 startingpos = MainCamera.transform.position;

        // Transition when not in 2nd or 3rd positions
        while (t < 1.0f && !in2ndPos && !in3rdPos)
        {
            t += Time.deltaTime * (Time.timeScale * speed);
            MainCamera.transform.position = Vector3.Lerp(startingpos, Target.position, t);

            if (t >= 1.0f)
            isActionInProgress = false;
            yield return null;
        }

        //Transition to 2nd position
        while (in2ndPos && t < 1.0f)
        {
            t += Time.deltaTime * (Time.timeScale * speed);
            MainCamera.transform.position = Vector3.Lerp(startingpos, Target.position, t);
            MainCamera.transform.rotation = Quaternion.Slerp(MainCamera.transform.rotation, Quaternion.LookRotation(Target5.position - MainCamera.transform.position), speed * Time.deltaTime);

            if (t >= 1.0f)
                isActionInProgress = false;

            yield return null;
        }

        //Maintain 2nd position rotation
        while (in2ndPos)
        {
            MainCamera.transform.rotation = Quaternion.Slerp(MainCamera.transform.rotation, Quaternion.LookRotation(Target5.position - MainCamera.transform.position), speed * Time.deltaTime);

            yield return null;
        }

        //Transition to 3rd position
        while (in3rdPos && t < 1.0f)
        {
            t += Time.deltaTime * (Time.timeScale * speed);
            MainCamera.transform.position = Vector3.Lerp(startingpos, Target.position, t);
            MainCamera.transform.rotation = Quaternion.Slerp(MainCamera.transform.rotation, Quaternion.LookRotation(Target6.position - MainCamera.transform.position), speed * Time.deltaTime);

            yield return null;
        }

        //Maintain 3rd position rotation
        while (in3rdPos)
        {
            isActionInProgress = true;
            MainCamera.transform.rotation = Quaternion.Slerp(MainCamera.transform.rotation, Quaternion.LookRotation(Target6.position - MainCamera.transform.position), speed * Time.deltaTime);

            yield return null;
        }
    }

    public void EndGameWin()
    {
        print("You Win");

        Unity.Services.Analytics.CustomEvent timesWon = new Unity.Services.Analytics.CustomEvent("TimesWon")
        {
            { "timesWon", 1 }
        };

        Debug.Log("Analytic Recorded: " + "Times Won");

        WinScreen.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
        Time.timeScale = 0f;
    }

    public void EndGameLose()
    {
        print("You Lose");

        Unity.Services.Analytics.CustomEvent timesDied = new Unity.Services.Analytics.CustomEvent("TimesDied")
        {
            { "timesDied", 1 }
        };

        Debug.Log("Analytic Recorded: " + "Times Died");

        LoseScreen.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
        Time.timeScale = 0f;
    }

    public void RestartGame()
    {
        var activeScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(activeScene);
    }
    
    private bool allActionsDone()
    {
        //print(inKnifeAction);
        //print(inGunAction);
        //print(canMoveOn);

        if (canMoveOn)
        {
            if (!inKnifeAction)
            {
                if (!inGunAction)
                {
                    isActionInProgress = false;
                    canMoveOn = false;
                    return true;
                }
            }
        }
        return false;
    }
}