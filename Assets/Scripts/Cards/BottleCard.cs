using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BottleCard : MonoBehaviour
{
    GameManager gameManager;
    bool playCardForPlayerCalled = false;
    bool playCardForAiCalled = false;

    [Header("References")]
    [SerializeField] GameObject bottlePrefab;
    float timeFly = 1;
    Transform playerTarget;
    Transform aiTarget;
    Transform bottleSpawnPoint;

    [Header("Cards on Table")]
    [HideInInspector] Component cardsOnTable1;
    [HideInInspector] Component cardsOnTable2;
    [HideInInspector] Component cardsOnTable3;
    [HideInInspector] Component cardsOnTable4;

    public bool waitForPlayersThrow;
    bool checkedPlayersCards = false;

    void Awake()
    {
        gameManager = FindAnyObjectByType<GameManager>();
        playerTarget = GameObject.Find("BOTTLE POSITION PLAYER").transform;
        aiTarget = GameObject.Find("BOTTLE POSITION AI").transform;
        bottleSpawnPoint = GameObject.Find("BOTTLE POSITION BARTENDER").transform;
    }

    void Update()
    {

    }

    public void PlayCardForPlayer()
    {
        StartCoroutine(WaitForActionsAndPlayBottle(true));
    }

    public void PlayCardForAI()
    {
        StartCoroutine(WaitForActionsAndPlayBottle(false));
    }

    public void PlayDelayCardForAI()
    {
        //StartCoroutine(WaitToCheckPlayerCards());
        waitForPlayersThrow = true;
        PlayCardForAI();
    }

    IEnumerator DelayBottleThrow(float timeToDelay, Transform target, bool player)
    {
        yield return new WaitForSeconds(timeToDelay);
        //Throw Bottle At AI
        ThrowCube(target);
    }

    void ThrowCube(Transform target)
    {
        GameObject bottle = Instantiate(bottlePrefab, bottleSpawnPoint.position, Quaternion.identity);

        //Align Bottle To Target Direction
        Vector3 direction = (target.position - bottleSpawnPoint.position).normalized;
        bottle.transform.rotation = Quaternion.LookRotation(direction);

        Rigidbody rb = bottle.GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = bottle.AddComponent<Rigidbody>();
        }

        //Calculate Displacement From Spawn Point To Target
        Vector3 displacement = target.position - bottleSpawnPoint.position;
        Vector3 gravity = Physics.gravity;

        //Calculate Initial Velocity Required For Ballistic Arc
        Vector3 initialVelocity = displacement / timeFly - 0.5f * gravity * timeFly;

        //Set Bottle's Velocity For A Deterministic Trajectory
        rb.velocity = initialVelocity;

        //IF AI AND PLAYER HAVE PLAYED A BOTTLE THROW WAIT 5 SECONDS FOR THROWS TO FINISH
        if ((gameManager.increaseCard1SkipCalled || gameManager.increaseCard2SkipCalled) && (gameManager.increaseCard3SkipCalled || gameManager.increaseCard4SkipCalled))
        {
            StartCoroutine(WaitForAIThrow());
        }
        else
        {
            gameManager.FinishBottleTurn();
        }
    }

    IEnumerator WaitForAIThrow()
    {
        yield return new WaitForSeconds(5f);
        gameManager.FinishBottleTurn();
    }

    IEnumerator WaitForActionsAndPlayBottle(bool isPlayer)
    {
        //STOPS FROM AUTO ENDING TURN ONCE KNIFE/ GUN FINISHED
        if (isPlayer)
        {
            gameManager.inBottleAction = true;
        }
        else
        {
            gameManager.inAIBottleAction = true;
        }

        if (KnifeOrGunCardExists())
        {
            Debug.Log("Knife/Gun card detected! Waiting for its action to start...");

            //Wait A Short Time To Allow The Action To Start
            yield return new WaitForSeconds(3f);
        }

        //Wait Until All Knife/Gun Actions Are Finished
        while (KnifeOrGunActionInProgress())
        {
            Debug.Log("Knife/Gun action in progress! Waiting...");
            yield return null;
        }

        Debug.Log("All Knife/Gun actions are done. Proceeding with bottle action.");

        //All Actions Done -> Proceed With The Bottle Action
        if (isPlayer)
        {
            if (gameManager.inBottleAction)
            {
                //gameManager.inBottleAction = false;
                gameManager.playerSkipCount++;

                StartCoroutine(DelayBottleThrow(5, aiTarget, true));

                //Skip AI Turn
                GameManager.Instance.aiSkippedTurns++;

                playCardForPlayerCalled = false;
            }
        }
        //AI Logic
        else
        {
            float chance = gameManager.statusPercent;
            float roll = Random.Range(0f, 100f);

            if (roll <= chance)
            {
                // Apply status effect logic if necessary
            }

            if (gameManager.inAIBottleAction /*&& gameManager.aiSkipCount == 0*/)
            {
                //gameManager.inAIBottleAction = true;
                playCardForAiCalled = false;

                if (waitForPlayersThrow)
                {
                    Debug.Log("PLAYER PLAYED SKIP, WAITING");
                    StartCoroutine(DelayBottleThrow(10, playerTarget, false));
                }
                else
                {
                    Debug.Log("PLAYER DID NOT PLAY SKIP");
                    StartCoroutine(DelayBottleThrow(5, playerTarget, false));
                }

                GameManager.Instance.playerSkippedTurns++;
            }
        }
    }

    private bool KnifeOrGunCardExists()
    {
        Component[] cardComponents =
        {
            GetCardComponent(CardDrawSystem.Instance.selectedPosition1),
            GetCardComponent(CardDrawSystem.Instance.selectedPosition2),
            GetCardComponent(AICardDrawSystem.Instance.selectedPosition1),
            GetCardComponent(AICardDrawSystem.Instance.selectedPosition2)
        };

        foreach (var card in cardComponents)
        {
            if (card != null && (card.gameObject.name.Contains("knife") || card.gameObject.name.Contains("gun")))
            {
                //Found A Knife/ Gun Card
                return true;
            }
        }

        //No Knife/Gun Cards Detected
        return false;
    }

    //Get The Card Components
    private Component GetCardComponent(Transform cardPosition)
    {
        if (cardPosition.childCount > 0)
        {
            return cardPosition.GetChild(0).gameObject.GetComponentAtIndex(1);
        }
        return null;
    }

    //Check If Knife/ Gun Action Is Ongoing
    private bool KnifeOrGunActionInProgress()
    {
        return gameManager.inKnifeActionAiPlayed || gameManager.inKnifeActionPlayerPlayed || gameManager.inGunAction;
    }
}
