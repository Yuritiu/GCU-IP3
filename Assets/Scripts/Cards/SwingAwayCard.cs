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
    Transform camera;

    [Header("References")]
    Transform bat;
    Transform swingStartPosition;
    Transform swingTargetPosition;
    TrailRenderer trailRenderer;
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
    bool canSwingBat = false;
    bool canSwingBatCoroutineCalled = false;
    bool isLerpingToStartPosition = false;
    float lerpSpeed = 10f;
    bool reducedPlayerBatCount = false;
    bool reducedAIBatCount = false;
    bool inFreezeCam = false;

    bool playerBatsUsed = false;
    bool aiBatsUsed = false;

    bool blurCalled = false;

    void Start()
    {
        gameManager = FindAnyObjectByType<GameManager>();
        freelook = FindAnyObjectByType<Freelook>();

        cameraShake = Camera.main.GetComponent<CameraShake>();
        camera = Camera.main.GetComponent<Transform>();

        trailRenderer = FindObjectOfType<BatRagdoll>().GetComponentInChildren<TrailRenderer>();
        trailRenderer.enabled = false;
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

        if (inFreezeCam)
        {
            freelook.currentXRotation = 0;
            freelook.currentYRotation = 0;
            freelook.canLook = false;
        }
        //TODO:
        //ADD PLAYER HEAD ON TABLE AFTER FOR X SKIPPED TURNS, FADE BLACK WHILE FALLING AND PLAY THWACK SFX
        //ADD EXTRA SKIP CHANCE WITH TEXT SAYING, BASED ON CLICKED POSITION, ADD VISUALIZER BAR FOR CURRENT SWING AMOUNT
        //FEEL: ADD SFX, RAGDOLL ENEMY, BLOOD OUT MOUTH

        //Start The Swing If The Card Is Played And No Other Actions Are Happening
        if (!isSwinging && (playCardForPlayerCalled && !playCardForAiCalled) && !gameManager.inKnifeAction && !gameManager.inGunAction)
        {
            if (playerBatsUsed)
            {
                clickToSwingText = bat.GetComponentInChildren<TextMeshProUGUI>();
                clickToSwingText.text = "";
                return;
            }

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

        if (!aiCoroutineCalled && !isPlayer && gameManager.playerBatCount == 0 && !gameManager.calledAIBatSwing)
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

            //Oscillate The Bat From Left To Right Based On Time
            swingProgress += Time.deltaTime * swingSpeedMultiplier;

            float swingAngle = Mathf.Sin(swingProgress) * maxVisualSwingAngle;

            bat.position = swingStartPosition.position;
            bat.rotation = swingStartPosition.rotation;

            Quaternion swingRotation = Quaternion.AngleAxis(swingAngle, Vector3.up);
            bat.rotation = initialBatSwingRotation * swingRotation;
        }
        else if (!isLerping && canUseBat && !isPlayer)
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

        if(canUseBat && isPlayer && !canSwingBatCoroutineCalled)
        {
            canSwingBatCoroutineCalled = true;
            StartCoroutine(BatSwingDelay());
        }

        //Check For Click
        if (Input.GetMouseButtonDown(0) && !isLerping && !isReturning && canUseBat && isPlayer && canSwingBat)
        {
            canSwingBat = false;

            clickToSwingText.text = "";

            trailRenderer.enabled = true;

            GameManager.Instance.in4thPos = false;

            //Freeze Player Camera
            Freelook.Instance.inBatSwing = true;

            ThwackSFX.Instance.PlayThwackSFX();

            //Player Hitting AI Small Shake
            cameraShake.TriggerShake(0.08f, 0.1f, 0.15f);

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
                if (isPlayer)
                {
                    //gameManager.in4thPos = false;
                    //gameManager.in5thPos = false;

                    //freelook.currentXRotation = 0;
                    //freelook.currentYRotation = 0;

                    //freelook.canLook = false;
                }

                isLerping = false;
                isReturning = true;
            }
        }

        //Lerp Back To The Side Of Table
        if (isReturning)
        {
            trailRenderer.enabled = false;

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
                }

                if (!isPlayer && !reducedAIBatCount)
                {
                    reducedAIBatCount = true;
                    gameManager.inAIBatAction = false;
                    aiBatsUsed = true;

                    gameManager.aiBatCount = 0;
                }

                FinishSwing();
            }
        }
    }

    void FinishSwing()
    {
        reducedPlayerBatCount = false;
        reducedAIBatCount = false;

        if (gameManager.playerBatCount <= 0 && gameManager.aiBatCount <= 0)
        {
            //Reset Bat Variables
            gameManager.inBatAction = false;
            gameManager.inAIBatAction = false;
            gameManager.calledAIBatSwing = false;

            //Reset Card Values
            gameManager.increaseCard1BatCalled = false;
            gameManager.increaseCard2BatCalled = false;
            gameManager.increaseCard3BatCalled = false;
            gameManager.increaseCard4BatCalled = false;

            freelook.canLook = true;
        }

        //Lock Mouse
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        isSwinging = false;
    }

    public void PlayCardForAI()
    {
        float chance = gameManager.statusPercent;
        float roll = Random.Range(0f, 100f);

        if (roll <= chance)
        {
            blurCalled = true;

            //Check To Only Swing Bat Once Even If 2 Bat's Played
            if (gameManager.inBatAction && gameManager.playerBatCount == 0)
            {
                gameManager.inAIBatAction = true;

                playCardForAiCalled = false;
            }
            else
            {
                gameManager.inAIBatAction = true;

                playCardForAiCalled = true;
            }
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

    IEnumerator BatSwingDelay()
    {
        yield return new WaitForSeconds(1.5f);

        clickToSwingText.text = "< CLICK TO SWING >";
        canSwingBat = true;
        canSwingBatCoroutineCalled = false;
    }

    IEnumerator DelayAIBatStart()
    {
        yield return new WaitForSeconds(3f);

        GameManager.Instance.isActionInProgress = false;
        //GameManager.Instance.in5thPos = true;

        //Get All References
        bat = GameObject.FindGameObjectWithTag("Bat").GetComponent<Transform>();
        swingStartPosition = GameObject.FindGameObjectWithTag("AISwingStartPosition").GetComponent<Transform>();
        swingTargetPosition = GameObject.FindGameObjectWithTag("AISwingTargetPosition").GetComponent<Transform>();
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

    IEnumerator AIBatSwingDelay()
    {
        //Set Camera To Focus Opponent

        inFreezeCam = true;

        yield return new WaitForSeconds(Random.Range(3, 6));

        camera.transform.rotation = Quaternion.Euler(0,0,0);
        GameManager.Instance.in4thPos = false;
        GameManager.Instance.in5thPos = false;

        if (blurCalled)
        {
            //Shuffle cards and blur them
            GameManager.Instance.blur.SetActive(true);
            CardDrawSystem.Instance.ShuffleHand();
            GameManager.Instance.batBackfire.gameObject.SetActive(true);
        }

        ThwackSFX.Instance.PlayThwackSFX();

        cameraShake.TriggerShake(0.075f, 0.5f, 0.2f);

        canUseBat = false;
        isLerping = true;

        yield return new WaitForSeconds(0.5f);

        blurCalled = false;
        inFreezeCam = false;
    }
}