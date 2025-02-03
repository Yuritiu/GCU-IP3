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
        //Check To Only Throw Bottle Once Even If 2 Bottle's Played
        if (gameManager.inBottleAction)
        {
            gameManager.inBottleAction = true;
            gameManager.playerSkipCount++;

            //Skip AI Turn
            GameManager.Instance.aiSkippedTurns++;

            playCardForPlayerCalled = false;
        }
        else
        {
            gameManager.inBottleAction = true;

            StartCoroutine(DelayBottleThrow(5, aiTarget, true));

            gameManager.playerSkipCount++;

            //Skip AI Turn
            GameManager.Instance.aiSkippedTurns++;

            playCardForPlayerCalled = true;
            playCardForAiCalled = false;
        }
    }

    public void PlayCardForAI()
    {
        float chance = gameManager.statusPercent;
        float roll = Random.Range(0f, 100f);

        if (roll <= chance)
        {
            //blurCalled = true;
            //statusDropdown.DisplayStatusEffect(0, 5);
            //Check To Only Swing Bat Once Even If 2 Bat's Played    
        }

        if (gameManager.inBottleAction && gameManager.playerSkipCount == 0)
        {
            gameManager.inAIBottleAction = true;
            playCardForAiCalled = false;

            //Skip PLAYER Turn
            GameManager.Instance.playerSkippedTurns++;
        }
        else
        {
            gameManager.inAIBottleAction = true;
            playCardForAiCalled = true;

            //Skip PLAYER Turn
            GameManager.Instance.playerSkippedTurns++;

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
        }
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
}
