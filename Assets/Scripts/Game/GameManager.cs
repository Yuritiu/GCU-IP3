using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
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

    public CameraController cameraController;

    private Vector3 tablePosition;
    private Quaternion rotation;
    public bool cameraMovement; //used for turning off W S P when using Knife


    [Header("camera")]
    [SerializeField] public Camera MainCamera;
    [SerializeField] public GameObject originalCameraPosition;

    [Header("Health Variables")]
    [HideInInspector] public int aiFingers;
    [HideInInspector] public int playerFingers;

    [Header("Status Effect % Chance")]
    [SerializeField] public float statusPercent = 20f;

    [Header("Gun")]
    
    [SerializeField] public GameObject Gun;
    public GameObject PlayerGun;
    public GameObject AiGun;
    [HideInInspector] public int bullets;
    public TextMeshProUGUI bulletText;
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
    [HideInInspector] public bool inKnifeActionAiPlayed = false;
    [HideInInspector] public bool inKnifeActionPlayerPlayed = false;
    [HideInInspector] public bool canCutFinger = false;
    [HideInInspector] public bool inGunAction = false;
    [HideInInspector] public bool inGunPlayerAction = false;
    [HideInInspector] public bool inBatAction = false;
    [HideInInspector] public bool inBottleAction = false;
    [HideInInspector] public bool inAIBottleAction = false;
    [HideInInspector] public bool inAIBatAction = false;
    [HideInInspector] public bool has2Guns = false;
    [HideInInspector] public int numberOfKnifeCardsAI = 0;
    [HideInInspector] public int numberOfKnifeCardsPlayer = 0;
    bool calledCard1 = false;
    bool calledCard2 = false;

    [HideInInspector] public bool knife1used = false;
    [HideInInspector] public bool knife2used = false;

    [Header("Bat Variables")]
    [HideInInspector] public int playerSkipCount = 0;
    [HideInInspector] public int aiSkipCount = 0;
    [HideInInspector] public int playerGunCount = 0;
    [HideInInspector] public int aiGunCount = 0;
    [HideInInspector] public bool increaseCard1SkipCalled = false;
    [HideInInspector] public bool increaseCard2SkipCalled = false;
    [HideInInspector] public bool increaseCard3SkipCalled = false;
    [HideInInspector] public bool increaseCard4SkipCalled = false;
    [HideInInspector] public bool increaseCard3GunCalled = false;
    [HideInInspector] public bool increaseCard4GunCalled = false;

    [Header("One In The Chamber Variables")]
    [HideInInspector] public bool inPlayerReloadCalled = false;
    [HideInInspector] public bool inAIReloadCalled = false;

    [Header("Camera Movement Variables")]
    [HideInInspector] public bool isActionInProgress = false;
    [HideInInspector] public bool crosshairUnlocked;
    [HideInInspector] public bool freelookEnabled;

    [SerializeField] private AudioClip[] aiScreams;
    [SerializeField] private AudioClip deathSFX;

    public bool firstStepsTutorial = false;

    public int timesToShoot = 0;

    public bool gameEnded = false;

    public bool displaySkipTurnText = false;

    async void Start()
    {
        Time.timeScale = 1f;
        originalCameraPosition.transform.position = MainCamera.transform.position;
        //playerSkippedTurnsText.enabled = false;

        await UnityServices.InitializeAsync();

        tablePosition = Gun.transform.position;
        rotation = Gun.transform.rotation;

        cameraController = FindFirstObjectByType<CameraController>();
    }

    private void Awake()
    {
        Instance = this;

        PlayerGun.SetActive(false);
        AiGun.SetActive(false);

        wPressed = false;
        sPressed = false;

        isTutorial = false;
        canPlay = true;

        //Set Fingers To 5
        aiFingers = 5;
        playerFingers = 4;

        //sets number of bullets
        bullets = 0;

        //Set Fingers Debug Text
        ReduceHealth(0, 0);
        DisableAllBackfires();

        blur.SetActive(false);
    }

    public void addBullet()
    {
        bullets++;
    }

    public void UpdateAmmoText()
    {
        bulletText.text = bullets.ToString();
    }

    public void NextTurn()
    {
        //print("NEXT TURN");
        Debug.Log("AI SKIPPED TURNS: " + aiSkippedTurns);
        Debug.Log("PLAYER SKIPPED TURNS: " + playerSkippedTurns);

        timesToShoot = 0;

        if(playerSkippedTurns> 0)
        {
            displaySkipTurnText = true;
            CardDrawSystem.Instance.FlipCards(true);
        }

        if (!canPlay)
            return;

        playerHasGun = false;
        aiHasGun = false;

        AICardDrawSystem.Instance.card1Moving = false;
        AICardDrawSystem.Instance.card2Moving = false;

        calledCard1 = false;
        calledCard2 = false;

        DisableAllBackfires();

        //Move Played Cards To Discard Pile
        CardDrawSystem.Instance.FindCardsOnTable();
        StartCoroutine(CardDrawSystem.Instance.LerpCardsToDiscardDeck(0.5f));
        CardSelection.ClearAllHovers();

        //Debug.Log("Next Turn");

        //Add Cards For Player And AI
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

        playerArmour = 0;
        aiArmour = 0;

        if (playerSkippedTurns > 0)
        {
            CardDrawSystem.Instance.isPlayersTurn = false;
            //nextTurnStarted = true;

            playerSkippedTurns--;

            PlayHand();
        }
        else
        {
            CardDrawSystem.Instance.isPlayersTurn = true;

            //Debug
            CardDrawSystem.Instance.debugCurrentTurnText.text = ("Play Time");

            //playerSkippedTurnsText.text = "";
            //playerSkippedTurnsText.enabled = false;
        }
    }

    public void PlayHand()
    {
        canPlay = false;

        CardDrawSystem.Instance.UnbanCards();
        blur.SetActive(false);

        //Debug.Log("Played Hand: " + isTutorial);

        if (aiSkippedTurns == 0)
        {
            StartCoroutine(AIPlaceCards());
        }
        else
        {
            ShowCards();
        }
    }

    public async void ShowCards()
    {
        AICardDrawSystem.Instance.UnbanCards();
        CardSelection.ClearAllHovers();
        if (aiSkippedTurns > 0)
        {
            aiSkippedTurns--;
        }

        //DELAY FOR CARDS TO HAVE TIME TO BE PLACED ON TABLE TO BE COMPARED AGAINST PROPERLY (particularly for bottle cards)
        await DelayCardsActionsBeingCalledForChecksToHappen();

        //IMPORTANT Make Sure The Cards Logic Is Executed Before This Is Called!
        //Could Maybe Add The Destroy To The Card GameObject
        if (CardDrawSystem.Instance.selectedPosition1.childCount > 0 && playerSkippedTurns == 0)
        {
            //For This To Work, Please Make Sure Card's Logic Is Executed In A Public Function Called PlayCard
            //And The Card's Hierarchy Mathches The 'Skip Next Turn' Card
            cardsOnTable1 = CardDrawSystem.Instance.selectedPosition1.GetChild(0).gameObject.GetComponentAtIndex(1);

            if (cardsOnTable1 != null && !calledCard1)
            {
                //calledCard1 = true;

                if (cardsOnTable1.name.Contains("bottle") && !increaseCard1SkipCalled)
                {
                    increaseCard1SkipCalled = true;
                    Debug.Log("BOTTLE IN CARD 1");
                    playerSkipCount++;
                }
                else if (cardsOnTable1.name.Contains("Chamber") && !inPlayerReloadCalled)
                {
                    //Wait For One In The Chamber Reload
                    inPlayerReloadCalled = true;
                    Debug.Log("One In Chamber In Slot 1");
                }
            }

            cardsOnTable1.SendMessage("PlayCardForPlayer");
            
            CardDrawSystem.Instance.selectedCardCount--;
        }
        if (CardDrawSystem.Instance.selectedPosition2.childCount > 0 && playerSkippedTurns == 0)
        {
            //For This To Work, Please Make Sure Card's Logic Is Executed In A Public Function Called PlayCard
            //And The Card's Hierarchy Mathches The 'Skip Next Turn' Card
            cardsOnTable2 = CardDrawSystem.Instance.selectedPosition2.GetChild(0).gameObject.GetComponentAtIndex(1);

            if (cardsOnTable2 != null && !calledCard2)
            {
                //calledCard2 = true;

                if (cardsOnTable2.name.Contains("bottle") && !increaseCard2SkipCalled)
                {
                    increaseCard2SkipCalled = true;
                    Debug.Log("BOTTLE IN CARD 2");
                    playerSkipCount++;
                }
                else if (cardsOnTable2.name.Contains("Chamber") && !inPlayerReloadCalled)
                {
                    //Wait For One In The Chamber Reload
                    inPlayerReloadCalled = true;
                    Debug.Log("One In Chamber In Slot 2");
                }
            }

            cardsOnTable2.SendMessage("PlayCardForPlayer");

            CardDrawSystem.Instance.selectedCardCount--;
        }
        if (cardsOnTable3 != null)
        {
            //For This To Work, Please Make Sure Card's Logic Is Executed In A Public Function Called PlayCard
            //And The Card's Hierarchy Mathches The 'Skip Next Turn' Card
            if (!cardsOnTable3.gameObject.name.Contains("Discarded"))
            {
                if (cardsOnTable3 != null)
                {
                    if (cardsOnTable3.name.Contains("gun") && !increaseCard3GunCalled)
                    {
                        increaseCard3GunCalled = true;
                        aiGunCount++;
                        cardsOnTable3.SendMessage("PlayCardForAI");
                    }
                    else if (cardsOnTable3.name.Contains("bottle") && !increaseCard3SkipCalled)
                    {
                        increaseCard3SkipCalled = true;
                        Debug.Log("BOTTLE IN CARD 3");
                        aiSkipCount++;

                        cardsOnTable3.SendMessage("PlayDelayCardForAI");
                    }
                    else if(cardsOnTable3.name.Contains("Chamber") && !inAIReloadCalled)
                    {
                        inAIReloadCalled = true;
                        Debug.Log("One In Chamber In Slot 3");
                        cardsOnTable3.SendMessage("PlayCardForAI");
                    }
                    else
                    {
                        cardsOnTable3.SendMessage("PlayCardForAI");
                    }
                }

                AICardDrawSystem.Instance.selectedCardCount--;
            }
        }
        if (cardsOnTable4 != null)
        {
            //For This To Work, Please Make Sure Card's Logic Is Executed In A Public Function Called PlayCard
            //And The Card's Hierarchy Mathches The 'Skip Next Turn' Card
            if (!cardsOnTable4.gameObject.name.Contains("Discarded"))
            {
                if (cardsOnTable4 != null)
                {
                    if (cardsOnTable4.name.Contains("gun") && !increaseCard4GunCalled)
                    {
                        increaseCard4GunCalled = true;
                        aiGunCount++;
                        cardsOnTable4.SendMessage("PlayCardForAI");
                    }
                    else if (cardsOnTable4.name.Contains("bottle") && !increaseCard4SkipCalled)
                    {
                        increaseCard4SkipCalled = true;
                        Debug.Log("BOTTLE IN CARD 4");
                        aiSkipCount++;

                        if(increaseCard1SkipCalled || increaseCard2SkipCalled)
                        {
                            cardsOnTable4.SendMessage("PlayDelayCardForAI");
                        }
                        else
                        {
                            cardsOnTable4.SendMessage("PlayCardForAI");
                        }
                    }
                    else if (cardsOnTable4.name.Contains("Chamber") && !inAIReloadCalled)
                    {
                        Debug.Log("One In Chamber In Slot 4");
                        inAIReloadCalled = true;
                        cardsOnTable4.SendMessage("PlayCardForAI");
                    }
                    else
                    {
                        cardsOnTable4.SendMessage("PlayCardForAI");
                    }
                }

                AICardDrawSystem.Instance.selectedCardCount--;
            }
        }

        canMoveOn = true;
        //displaySkipTurnText = true;

        CardDrawSystem.Instance.isPlayersTurn = false;
        StartCoroutine(MoveCamera());
    }

    private async Task DelayCardsActionsBeingCalledForChecksToHappen()
    {
        //Wait For 1 Seconds
        await Task.Delay(1000);
    }

    IEnumerator AIPlaceCards()
    {
        //If There Are Cards In Manual Queue -> Select Them
        if (AICardDrawSystem.Instance.manualMode && AICardDrawSystem.Instance.manualDrawQueue.Count > 0)
        {
            //Manually Select Card For Slot 3
            cardsOnTable3 = (Component)AICardDrawSystem.Instance.SelectCard();
        }
        else
        {
            cardsOnTable3 = (Component)AICardDrawSystem.Instance.SelectCard();
        }

        yield return new WaitForSeconds(0.3f);

        //If There Are Cards In Manual Queue -> Select Them
        if (AICardDrawSystem.Instance.manualMode && AICardDrawSystem.Instance.manualDrawQueue.Count > 0)
        {
            //Manually Select Card For Slot 4
            cardsOnTable4 = (Component)AICardDrawSystem.Instance.SelectCard();
        }
        else
        {
            cardsOnTable4 = (Component)AICardDrawSystem.Instance.SelectCard();
        }

        yield return new WaitForSeconds(0.3f);

        ShowCards();
    }

    IEnumerator MoveCamera()
    {
        cameraController.SetCameraToPositionTarget();

        yield return new WaitForSeconds(3f);

        if (cameraController.targetLookingPos != cameraController.barTarget && cameraController.targetLookingPos != cameraController.knifeTarget)
        {
            cameraController.SetCameraToOpponentTarget();
        }

        IsReadyToCompare = true;
    }


    IEnumerator WaitSoCardsCanReveal()
    {
        yield return new WaitForSeconds(1);

        IsReadyToCompare = false;
        CardDrawSystem.Instance.canPlay = true;
        canPlay = true;

        NextTurn();
    }

    public bool isPlayerGun = false;
    public bool isAiGun = false;
    public GameObject PositionForPlayer;
    public GameObject PositionForAi;

    public void PlayerRoulette()
    {
        timesToShoot++;
        inGunAction = true;
        StartCoroutine(WaitForGun(PlayerGun));
        isPlayerGun = true;
    }

    public void AiRoulette()
    {
        inGunAction = true;
        StartCoroutine(WaitForGun(AiGun));
        isAiGun = true;
    }

    IEnumerator WaitForGun(GameObject gun)
    {
        //print("scooby snack");

        //print(gun.name);

        //float t = 0.00f;
        yield return new WaitForSeconds(4.5f);

        float speed = 2f;
        bool PlayerShot = false;
        Quaternion startRotation = Gun.transform.rotation;
        Vector3 startPosition = Gun.transform.position;


        if (ShootScript.instance2 != null)
        {
            PlayerShot = ShootScript.instance2.PlayerShot;
        }

        if (gun.name == "PlayerGun" && !playerGunActive)
        {
            float t = 0.00f;
            while (t < 1.00f)
            {
                t += Time.deltaTime * (Time.timeScale * speed);
                Gun.transform.position = Vector3.Lerp(Gun.transform.position, gun.transform.position, t);
                Gun.transform.rotation = Quaternion.Slerp(Gun.transform.rotation, gun.transform.rotation, speed * Time.deltaTime);
                yield return null;
            }
            has2Guns = false;
            playerGunActive = true;
            Gun.SetActive(false);
            Gun.transform.rotation = startRotation;
            Gun.transform.position = startPosition;
            gun.SetActive(true);     
        }
        else if (gun.name == "PlayerGun" && playerGunActive)
        {


            has2Guns = true;
            PlayerRoulette();
        }

        if (gun.name == "AiGun" && !playerGunActive && !aiGunActive)
        {
            float t = 0.00f;
            while (t < 1.00f)
            {
                t += Time.deltaTime * (Time.timeScale * speed);
                Gun.transform.position = Vector3.Lerp(Gun.transform.position, gun.transform.position, t);
                Gun.transform.rotation = Quaternion.Slerp(Gun.transform.rotation, gun.transform.rotation, 6 * Time.deltaTime);
                yield return null;
            }

            has2Guns = false;
            aiGunActive = true;
            Gun.SetActive(false);
            Gun.transform.rotation = startRotation;
            Gun.transform.position = startPosition;
            gun.SetActive(true);         
        }

        else if (gun.name == "AiGun" && playerGunActive == true)
        {
            AiRoulette();
        }

        else if (gun.name == "AiGun" && !playerGunActive && aiGunActive)
        {
            has2Guns = true;
            AiRoulette();
        }
    }

    public IEnumerator WaitToCompareCards(int character, int type)
    {
        //waits for cards to reveal
        yield return new WaitForSeconds(1f);

        // type meaning // 1 is knife // 2 is cigar // 3 in gun//
        if (type == 1 || type == 3)
        {
            if (type == 1)
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
            inKnifeActionPlayerPlayed = false;
            OpponentAnimationController animController = FindAnyObjectByType<OpponentAnimationController>();
            animController.KnifeTr();
            aiFingers--;
            aiHand.RemoveFinger(aiFingers);
            SFXManager.instance.PlayRandomSFXClip(aiScreams, transform, 0.15f);
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
                //Debug.Log("Countdown Started");
                BloodlossSystem.Instance.IncreaseBloodloss();

                playerHand.RemoveFinger(playerFingers);
                playerFingers--;
            }
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
            SFXManager.instance.PlaySFXClip(deathSFX, transform, 0.5f);
        }

        playerFingersText.text = ("Player Fingers: " + playerFingers).ToString();
        aiFingersText.text = ("AI Fingers: " + aiFingers).ToString();
    }

    public void CheckArmour(int character, int type)
    {
        //this ensures the armour stops the gun instead of the knife
        if (character == 1)
        {
            if (aiArmour == 1)
            {
                if (playerHasGun && playerHasKnife)
                {
                    playerHasKnife = false;
                    inGunAction = false;
                    if (type == 1)
                    {
                        ReduceHealth(character, type);
                    }
                    return;
                }
                else
                {
                    aiArmour--;
                    inKnifeActionPlayerPlayed = false;
                    playerHasKnife = false;
                    inGunAction = false;
                }
            }
            else
            {
                if (aiArmour == 2)
                {
                    inGunAction = false;
                    inKnifeActionPlayerPlayed = false;
                    aiArmour--;
                    return;
                }
                else if (type == 1)
                {
                    ReduceHealth(character, type);
                    return;
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
                    inGunAction = false;
                    if (type == 1)
                    {
                        ReduceHealth(character, type);
                    }
                    return;
                }
                else if (type == 3)
                {
                    inGunAction = false;
                    playerArmour--;
                    return;
                }
                if (!knife1used || !knife2used)
                {
                    knife1used = false;
                    knife2used = false;
                    canCutFinger = false;
                    inKnifeActionAiPlayed = false;
                    playerArmour--;
                }
                else
                {
                    knife2used = false;
                    playerArmour--;
                    ReduceHealth(character, type);
                }
            }
            else
            {
                if (playerArmour == 2)
                {
                    knife1used = false;
                    knife2used = false;
                    canCutFinger = false;
                    inKnifeActionAiPlayed = false;
                    playerArmour = 0;
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
            SFXManager.instance.PlaySFXClip(deathSFX, transform, 0.5f);
        }
    }

    public void PlayCigarCard(int player)
    {
        //Player Functions
        if (player == 1)
        {
            if ((cardsOnTable1 != null && cardsOnTable2 != null) && !calledCard1 && !calledCard2)
            {
                calledCard1 = true;
                calledCard2 = true;

                

                var cardObject1 = cardsOnTable1.gameObject;
                var cardObject2 = cardsOnTable2.gameObject;

                //Card To Be Cloned Is In Slot 2 And Not A Bottle
                if (cardObject1.name.Contains("cigar") && (!cardObject2.name.Contains("cigar") && !cardObject2.name.Contains("bottle")))
                {
                    //Debug.Log("Called Function 1");
                    cardObject2.SendMessage("PlayCardForPlayer");

                    //Play Smoke VFX
                    //cardObject1.GetComponent<CigarCard>().PlaySmokeVFX("Player Smoke Graph 1");
                    if (cardObject2.name.Contains("knife"))
                    {
                        cardObject1.GetComponent<CigarCard>().PlaySmokeVFX("Player Smoke Graph 1", "knife");
                    }
                    else if (cardObject2.name.Contains("gun"))
                    {
                        cardObject1.GetComponent<CigarCard>().PlaySmokeVFX("Player Smoke Graph 1", "gun");
                    }
                    else if (cardObject2.name.Contains("Chamber"))
                    {
                        cardObject1.GetComponent<CigarCard>().PlaySmokeVFX("Player Smoke Graph 1", "Chamber");
                    }
                    else if (cardObject2.name.Contains("armour"))
                    {
                        cardObject1.GetComponent<CigarCard>().PlaySmokeVFX("Player Smoke Graph 1", "armour");
                    }
                }
                //If Bottle Is Played With Cigar:
                else if(cardObject1.name.Contains("cigar") && (!cardObject2.name.Contains("cigar") && cardObject2.name.Contains("bottle"))) 
                {
                    aiSkippedTurns++;

                    //Play Smoke VFX
                    cardObject1.GetComponent<CigarCard>().PlaySmokeVFX("Player Smoke Graph 1", "bottle");
                }

                //Card To Be Cloned Is In Slot 1 And Not A Bottle
                if (cardObject2.name.Contains("cigar") && (!cardObject1.name.Contains("cigar") && !cardObject1.name.Contains("bottle")))
                {
                    //Debug.Log("Called Function 2");
                    cardObject2.SendMessage("PlayCardForPlayer");

                    //Play Smoke VFX
                    if (cardObject1.name.Contains("knife"))
                    {
                        cardObject2.GetComponent<CigarCard>().PlaySmokeVFX("Player Smoke Graph 2", "knife");
                    }
                    else if (cardObject1.name.Contains("gun"))
                    {
                        cardObject2.GetComponent<CigarCard>().PlaySmokeVFX("Player Smoke Graph 2", "gun");
                    }
                    else if (cardObject1.name.Contains("Chamber"))
                    {
                        cardObject2.GetComponent<CigarCard>().PlaySmokeVFX("Player Smoke Graph 2", "Chamber");
                    }
                    else if (cardObject1.name.Contains("armour"))
                    {
                        cardObject2.GetComponent<CigarCard>().PlaySmokeVFX("Player Smoke Graph 2", "armour");
                    }
                }
                //If Bottle Is Played With Cigar:
                else if (cardObject2.name.Contains("cigar") && (!cardObject1.name.Contains("cigar") && cardObject1.name.Contains("bottle")))
                {
                    aiSkippedTurns++;
                    //Play Smoke VFX
                    cardObject2.GetComponent<CigarCard>().PlaySmokeVFX("Player Smoke Graph 2", "bottle");
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
                if (cardObject3.name.Contains("cigar") && (!cardObject4.name.Contains("cigar") && !cardObject4.name.Contains("bottle")))
                {
                    //Debug.Log("Called Function 3");
                    aiClonedCard = cardObject4.GetComponentAtIndex(1);
                    aiClonedCard.SendMessage("PlayCardForAI");

                    //Play Smoke VFX
                    if (cardObject4.name.Contains("knife"))
                    {
                        cardObject3.GetComponent<CigarCard>().PlaySmokeVFX("AI Smoke Graph 1", "knife");
                    }
                    else if (cardObject4.name.Contains("gun"))
                    {
                        cardObject3.GetComponent<CigarCard>().PlaySmokeVFX("AI Smoke Graph 1", "gun");
                    }
                    else if (cardObject4.name.Contains("Chamber"))
                    {
                        cardObject3.GetComponent<CigarCard>().PlaySmokeVFX("AI Smoke Graph 1", "Chamber");
                    }
                    else if (cardObject4.name.Contains("armour"))
                    {
                        cardObject3.GetComponent<CigarCard>().PlaySmokeVFX("AI Smoke Graph 1", "armour");
                    }
                }
                //If Bottle Is Played With Cigar:
                else if (cardObject3.name.Contains("cigar") && (!cardObject4.name.Contains("cigar") && cardObject4.name.Contains("bottle")))
                {
                    playerSkippedTurns++;

                    cardObject3.GetComponent<CigarCard>().PlaySmokeVFX("AI Smoke Graph 1", "bottle");
                }

                //Card To Be Cloned Is In Slot 3
                if (cardObject4.name.Contains("cigar") && (!cardObject3.name.Contains("cigar") && !cardObject3.name.Contains("bottle")))
                {
                    //Debug.Log("Called Function 4");
                    aiClonedCard = cardObject3.GetComponentAtIndex(1);
                    aiClonedCard.SendMessage("PlayCardForAI");

                    //Play Smoke VFX
                    if (cardObject3.name.Contains("knife"))
                    {
                        cardObject4.GetComponent<CigarCard>().PlaySmokeVFX("AI Smoke Graph 2", "knife");
                    }
                    else if (cardObject3.name.Contains("gun"))
                    {
                        cardObject4.GetComponent<CigarCard>().PlaySmokeVFX("AI Smoke Graph 2", "gun");
                    }
                    else if (cardObject3.name.Contains("Chamber"))
                    {
                        cardObject4.GetComponent<CigarCard>().PlaySmokeVFX("AI Smoke Graph 2", "Chamber");
                    }
                    else if (cardObject3.name.Contains("armour"))
                    {
                        cardObject4.GetComponent<CigarCard>().PlaySmokeVFX("AI Smoke Graph 2", "armour");
                    }
                }
                //If Bottle Is Played With Cigar:
                else if (cardObject4.name.Contains("cigar") && (!cardObject3.name.Contains("cigar") && cardObject3.name.Contains("bottle")))
                {
                    playerSkippedTurns++;

                    cardObject4.GetComponent<CigarCard>().PlaySmokeVFX("AI Smoke Graph 2", "bottle");
                }
            }
        }
    }

    private void Update()
    {
        UpdateAmmoText();
        UpdateSkipTurnText();

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            crosshairUnlocked = !crosshairUnlocked;
            if (crosshairUnlocked)
            {
                Cursor.lockState = CursorLockMode.None;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
            Cursor.visible = crosshairUnlocked;

            // Only disable camera look when unlocking the cursor
            freelookEnabled = !crosshairUnlocked;       
        }

        if (Gun.activeInHierarchy == true && ShootScript.instance1 != null && ShootScript.instance2 != null)
        {
            ShootScript.instance1.clampActivated = false;
            ShootScript.instance2.clampActivated = false;
        }

        //print(playerFingers);

        if (showddown == true && ShootScript.instance1.AiShot == false && ShootScript.instance2.PlayerShot == false)
        {
            GameManager.Instance.Showdown();
        }

        if (PlayerGun.activeInHierarchy == true)
        {
            inGunAction = true;
            ShootScript.instance1.clampActivated = true;
        }

        else if(PlayerGun.activeInHierarchy == false && ShootScript.instance1 != null)
        {
            ShootScript.instance1.clampActivated = false;
        }

        if (AiGun.activeInHierarchy == true)
        {
            inGunAction = true;
            ShootScript.instance2.clampActivated = true;
        }
        else if (AiGun.activeInHierarchy == false && ShootScript.instance2 != null)
        {
            ShootScript.instance2.clampActivated = false;
        }

        if (allActionsDone() == true)
        {
            StartCoroutine(WaitSoCardsCanReveal());
        }
    }

    void UpdateSkipTurnText()
    {
        if (playerSkippedTurns == 0 && canPlay)
        {
            displaySkipTurnText = false;
            CardDrawSystem.Instance.FlipCards(false);
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

    public void EndGameWin()
    {
        gameEnded = true;
        WinScreen.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
        Time.timeScale = 0f;
    }

    public void EndGameLose()
    {
        CameraDeathEffect.Instance.TriggerDeathSequence();
    }

    public void RestartGame()
    {
        var activeScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(activeScene);
    }

    public void FinishBottleTurn()
    {
        if(inBottleAction || inAIBottleAction)
        {
            //Called Here Because Multiple Bottle Cards Will Have Multiple Scripts
            inBottleAction = false;
            inAIBottleAction = false;
            increaseCard1SkipCalled = false;
            increaseCard2SkipCalled = false;
            increaseCard3SkipCalled = false;
            increaseCard4SkipCalled = false;
        }
    }

    public void FinishPlayerReload()
    {
        inPlayerReloadCalled = false;
    }

    public void FinishAIReload()
    {
        inAIReloadCalled = false;
    }

    private bool allActionsDone()
    {
        //print(inKnifeActionAiPlayed);
        //print(inKnifeActionPlayerPlayed);
        //print(inGunAction);
        //print(canMoveOn);

        if (canMoveOn)
        {
            //Debug.Log("Can move on");
            if (!inKnifeActionAiPlayed && !inKnifeActionPlayerPlayed)
            {
                //Debug.Log("No knife in action");
                if (!inGunAction)
                {
                    //Debug.Log("No gun in action");
                    if (!inBottleAction && !inAIBottleAction)
                    {
                        if(!inPlayerReloadCalled && !inAIReloadCalled)
                        {
                            Debug.Log("ALL ACTIONS DONE");
                            isActionInProgress = false;
                            canMoveOn = false;

                            return true;
                        }
                    }
                }
            }
        }
        return false;
    }
}