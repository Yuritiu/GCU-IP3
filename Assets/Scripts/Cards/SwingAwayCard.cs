using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SwingAwayCard : MonoBehaviour
{
    //!- Coded By Charlie -!

    GameManager gameManager;
    Freelook freelook;
    CameraShake cameraShake;

    [Header("References")]
    Transform bat;
    Transform swingStartPosition;
    Transform swingTargetPosition;
    TextMeshProUGUI clickToSwingText;

    [Header("Swing Settings")]
    [SerializeField] float maxSwingAngle = 110f;
    [SerializeField] float maxVisualSwingAngle = 20f;
    [SerializeField] public float swingSpeedMultiplier = 5f;
    //Max Speed When Clicked At The Far Left
    [SerializeField] float maxSwingSpeed;
    bool isSwinging = false;

    Quaternion initialRotation;
    Vector3 initialPosition;
    Quaternion initialBatSwingRotation;
    Vector3 currentSwingRotation;
    float swingProgress = 0f;

    bool playerCoroutineCalled = false;
    bool aiCoroutineCalled = false;
    bool aiSwingCoroutineCalled = false;
    bool playCardForPlayerCalled = false;
    bool playCardForAiCalled = false;
    bool isLerping = false;
    bool isReturning = false;
    bool canUseBat = false;
    bool isLerpingToStartPosition = false;
    float lerpSpeed = 10f;
    bool reducedPlayerBatCount = false;
    bool reducedAIBatCount = false;

    bool playerBatsUsed = false;
    bool aiBatsUsed = false;

    void Start()
    {
        gameManager = FindAnyObjectByType<GameManager>();
        freelook = FindAnyObjectByType<Freelook>();

        cameraShake = Camera.main.GetComponent<CameraShake>();
    }

    public void PlayCardForPlayer()
    {
        //Check To Only Swing Bat Once Even If 2 Bat's Played
        if (gameManager.inBatAction)
        {
            gameManager.inBatAction = true;
            gameManager.playerBatCount++;

            // Skip AI Turn
            GameManager.Instance.aiSkippedTurns++;

            playCardForPlayerCalled = false;
        }
        else
        {
            gameManager.inBatAction = true;

            gameManager.playerBatCount++;

            // Skip AI Turn
            GameManager.Instance.aiSkippedTurns++;

            playCardForPlayerCalled = true;
            playCardForAiCalled = false;
        }
    }

    void Update()
    {
        if (gameManager.playerBatCount <= 0 && gameManager.aiBatCount <= 0)
            return;

        //TODO:
        //ADD PLAYER HEAD ON TABLE AFTER FOR X SKIPPED TURNS, FADE BLACK WHILE FALLING AND PLAY THWACK SFX
        //ADD EXTRA SKIP CHANCE WITH TEXT SAYING, BASED ON CLICKED POSITION, ADD VISUALIZER BAR FOR CURRENT SWING AMOUNT
        //FEEL: ADD SFX, RAGDOLL ENEMY, BLOOD OUT MOUTH

        //Start The Swing If The Card Is Played And No Other Actions Are Happening
        if (!isSwinging && (playCardForPlayerCalled && !playCardForAiCalled) && !gameManager.inKnifeAction && !gameManager.inGunAction)
        {
            if (playerBatsUsed)
                return;

            //True = IsPlayer
            StartSwing(true);
        }

        if (isSwinging && playerCoroutineCalled)
        {
            //Unlock Mouse To Swipe Bat
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = false;

            gameManager.inBatAction = true;

            HandleSwing(true);
        }

        if (!isSwinging && (playCardForAiCalled && !playCardForPlayerCalled) && !gameManager.inKnifeAction && !gameManager.inGunAction)
        {
            if (aiBatsUsed)
                return;

            //False = IsAI
            //Debug.Log("Called AI SWING");
            StartSwing(false);
        }

        if (isSwinging && aiCoroutineCalled)
        {
            gameManager.inBatAction = true;

            HandleSwing(false);
        }
    }

    void StartSwing(bool isPlayer)
    {
        if (!playerCoroutineCalled && isPlayer && !aiCoroutineCalled)
        {
            playerCoroutineCalled = true;

            StartCoroutine(DelayBatStart());
        }

        if(!aiCoroutineCalled && !isPlayer && gameManager.playerBatCount == 0 && !gameManager.calledAIBatSwing)
        {
            aiCoroutineCalled = true;
            gameManager.calledAIBatSwing = true;

            //Debug.Log("STARTED AI COROUTINE");
            StartCoroutine(DelayAIBatStart());
        }
    }

    IEnumerator DelayBatStart()
    {
        yield return new WaitForSeconds(3f);

        freelook.canLook = false;

        //Get All References
        bat = GameObject.FindGameObjectWithTag("Bat").GetComponent<Transform>();
        swingStartPosition = GameObject.FindGameObjectWithTag("SwingStartPosition").GetComponent<Transform>();
        swingTargetPosition = GameObject.FindGameObjectWithTag("SwingTargetPosition").GetComponent<Transform>();
        clickToSwingText = bat.GetComponentInChildren<TextMeshProUGUI>();

        //Set All References
        initialPosition = bat.transform.position;
        initialRotation = bat.rotation;
        clickToSwingText.text = "";
        initialBatSwingRotation = swingStartPosition.rotation;
        currentSwingRotation = initialBatSwingRotation.eulerAngles;

        maxSwingSpeed = 60f;

        isLerpingToStartPosition = true;

        //Lerp Bat's Position From Side Of Table To The Swing Starting Position
        while (isLerpingToStartPosition)
        {
            bat.position = Vector3.Lerp(bat.position, swingStartPosition.position, Time.deltaTime * lerpSpeed);
            bat.rotation = Quaternion.Lerp(bat.rotation, initialBatSwingRotation, Time.deltaTime * lerpSpeed);

            if (Vector3.Distance(bat.position, swingStartPosition.position) < 0.1f && Quaternion.Angle(bat.rotation, initialBatSwingRotation) < 1f)
            {
                isLerpingToStartPosition = false;
            }

            yield return null;
        }

        canUseBat = true;
        isSwinging = true;
    }

    void HandleSwing(bool isPlayer)
    {
        //Visual Swing While Waiting For Click
        if (!isLerping && canUseBat && isPlayer)
        {
            GameManager.Instance.in4thPos = true;

            clickToSwingText.text = "< CLICK TO SWING >";

            //Oscillate The Bat From Left To Right Based On Time
            swingProgress += Time.deltaTime * swingSpeedMultiplier;

            float swingAngle = Mathf.Sin(swingProgress) * maxVisualSwingAngle;

            bat.position = swingStartPosition.position;
            bat.rotation = swingStartPosition.rotation;

            Quaternion swingRotation = Quaternion.AngleAxis(swingAngle, Vector3.up);
            bat.rotation = initialBatSwingRotation * swingRotation;
        }
        else if(!isLerping && canUseBat && !isPlayer)
        {
            GameManager.Instance.in4thPos = true;

            //Oscillate The Bat From Left To Right Based On Time
            swingProgress += Time.deltaTime * swingSpeedMultiplier;

            float swingAngle = Mathf.Sin(swingProgress) * maxVisualSwingAngle;

            bat.position = swingStartPosition.position;
            bat.rotation = swingStartPosition.rotation;

            Quaternion swingRotation = Quaternion.AngleAxis(swingAngle, Vector3.up);
            bat.rotation = initialBatSwingRotation * swingRotation;
        }

        //Check For Click
        if (Input.GetMouseButtonDown(0) && !isLerping && !isReturning && canUseBat && isPlayer)
        {
            clickToSwingText.text = "";

            cameraShake.TriggerShake(0.003f, 0.001f, 0.003f);

            canUseBat = false;
            isLerping = true;
        }

        if (!isPlayer && !aiSwingCoroutineCalled)
        {
            aiSwingCoroutineCalled = true;
            StartCoroutine(AIBatSwingDelay());
        }

        //Swing Into Opponent
        if (isLerping)
        {
            Vector3 targetPos = swingTargetPosition.position;
            Quaternion targetRot = swingTargetPosition.rotation;

            //Calculate Bat's Current Position And Speed
            float currentPositionX = Mathf.Abs(bat.localEulerAngles.y - initialBatSwingRotation.y);
            float normalizedPosition = currentPositionX / maxSwingAngle;
            float moveSpeed = Mathf.Lerp(maxSwingSpeed, maxSwingSpeed / 3f, normalizedPosition);

            bat.position = Vector3.Lerp(bat.position, targetPos, moveSpeed * Time.deltaTime);
            bat.rotation = Quaternion.Lerp(bat.rotation, targetRot, moveSpeed * Time.deltaTime);

            if (Vector3.Distance(bat.position, targetPos) < 0.01f && Quaternion.Angle(bat.rotation, targetRot) < 1f)
            {
                isLerping = false;
                isReturning = true;

                //shakeCameraCalled = false;
            }
        }

        //Lerp Back To The Side Of Table
        if (isReturning)
        {
            if (gameManager.aiBatCount <= 0 && gameManager.playerBatCount <= 0)
            {
                GameManager.Instance.in4thPos = false;
            }

            bat.position = Vector3.Lerp(bat.position, initialPosition, Time.deltaTime * lerpSpeed);
            bat.rotation = Quaternion.Lerp(bat.rotation, initialRotation, Time.deltaTime * lerpSpeed);

            if (Vector3.Distance(bat.position, initialPosition) < 0.01f && Quaternion.Angle(bat.rotation, initialRotation) < 1f)
            {
                isReturning = false;

                if (isPlayer && !reducedPlayerBatCount)
                {
                    reducedPlayerBatCount = true;
                    gameManager.inBatAction = false;
                    playerBatsUsed = true;

                    gameManager.playerBatCount = 0;
                    //Debug.Log(gameManager.playerBatCount + " : Player Bats Left");
                }

                if (!isPlayer && !reducedAIBatCount)
                {
                    reducedAIBatCount = true;
                    gameManager.inAIBatAction = false;
                    aiBatsUsed = true;

                    gameManager.aiBatCount = 0;
                    //Debug.Log(gameManager.aiBatCount + " : AI Bats Left");
                }

                FinishSwing();
            }
        }
    }

    void FinishSwing()
    {
        reducedPlayerBatCount = false;
        reducedAIBatCount = false;

        if(gameManager.playerBatCount <= 0 && gameManager.aiBatCount <= 0)
        {
            //Reset Camera -> Bats Finished
            gameManager.in4thPos = false;

            //Reset Bat Variables
            gameManager.inBatAction = false;
            gameManager.inAIBatAction = false;
            gameManager.calledAIBatSwing = false;

            //Reset Card Values
            gameManager.increaseCard1BatCalled = false;
            gameManager.increaseCard2BatCalled = false;
            gameManager.increaseCard3BatCalled = false;
            gameManager.increaseCard4BatCalled = false;
        }

        //Lock Mouse
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        isSwinging = false;
    }

    public void PlayCardForAI()
    {
        float chance = gameManager.statusPercent;
        float roll = UnityEngine.Random.Range(0f, 100f);

        if (roll <= chance)
        {
            // Shuffle cards and blur them
            GameManager.Instance.blur.SetActive(true);
            CardDrawSystem.Instance.ShuffleHand();
            GameManager.Instance.batBackfire.gameObject.SetActive(true);
        }
        else
        {
            //Check To Only Swing Bat Once Even If 2 Bat's Played
            if (gameManager.inBatAction && gameManager.playerBatCount == 0)
            {
                gameManager.inAIBatAction = true;

                //Skip Player Turn
                GameManager.Instance.playerSkippedTurns++;

                playCardForAiCalled = false;
            }
            else
            {
                gameManager.inAIBatAction = true;

                //Skip Player Turn
                GameManager.Instance.playerSkippedTurns++;

                playCardForAiCalled = true;
            }
        }
    }

    IEnumerator DelayAIBatStart()
    {
        yield return new WaitForSeconds(3f);

        freelook.canLook = false;

        //Get All References
        bat = GameObject.FindGameObjectWithTag("Bat").GetComponent<Transform>();
        swingStartPosition = GameObject.FindGameObjectWithTag("AISwingStartPosition").GetComponent<Transform>();
        swingTargetPosition = GameObject.FindGameObjectWithTag("AISwingTargetPosition").GetComponent<Transform>();
        //clickToSwingText = bat.GetComponentInChildren<TextMeshProUGUI>();

        //Set All References
        initialPosition = bat.transform.position;
        initialRotation = bat.rotation;
        //clickToSwingText.text = "";
        initialBatSwingRotation = swingStartPosition.rotation;
        currentSwingRotation = initialBatSwingRotation.eulerAngles;

        maxSwingSpeed = 60f;

        isLerpingToStartPosition = true;

        //Lerp Bat's Position From Side Of Table To The Swing Starting Position
        while (isLerpingToStartPosition)
        {
            bat.position = Vector3.Lerp(bat.position, swingStartPosition.position, Time.deltaTime * lerpSpeed);
            bat.rotation = Quaternion.Lerp(bat.rotation, initialBatSwingRotation, Time.deltaTime * lerpSpeed);

            if (Vector3.Distance(bat.position, swingStartPosition.position) < 0.1f && Quaternion.Angle(bat.rotation, initialBatSwingRotation) < 1f)
            {
                isLerpingToStartPosition = false;
            }

            yield return null;
        }

        canUseBat = true;
        isSwinging = true;
    }

    IEnumerator AIBatSwingDelay()
    {
        yield return new WaitForSeconds(Random.Range(3,6));
        canUseBat = false;
        isLerping = true;
    }
}