using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SwingAwayCard : MonoBehaviour
{
    GameManager gameManager;
    Freelook freelook;

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

    bool coroutineCalled = false;
    bool playCardForPlayerCalled = false;
    bool isLerping = false;
    bool isReturning = false;
    bool canUseBat = false;
    bool isLerpingToStartPosition = false;
    float lerpSpeed = 10f;

    void Start()
    {
        gameManager = FindAnyObjectByType<GameManager>();
        freelook = FindAnyObjectByType<Freelook>();
    }

    public void PlayCardForPlayer()
    {
        //Check To Only Swing Bat Once Even If 2 Bat's Played
        if (gameManager.inBatAction)
        {
            gameManager.inBatAction = true;

            // Skip AI Turn
            GameManager.Instance.aiSkippedTurns++;

            playCardForPlayerCalled = false;
        }
        else
        {
            gameManager.inBatAction = true;

            // Skip AI Turn
            GameManager.Instance.aiSkippedTurns++;

            playCardForPlayerCalled = true;
        }
    }

    void Update()
    {
        //TODO: LOCK CURSOR AGAIN, SET IN BAT ACTION FLAG TO FALSE
        //MAKE WORK WITH MAIN CAMERA, MAYBE ADD CUSTOM CAM POSITION
        //ADD AI BAT SWING AND PLAYER HEAD ON TABLE AFTER FOR X SKIPPED TURNS
        //ADD EXTRA SKIP CHANCE WITH TEXT SAYING, BASED ON CLICKED POSITION
        //FEEL: ADD SFX, RAGDOLL ENEMY, BLOOD OUT MOUTH

        //Start The Swing If The Card Is Played And No Other Actions Are Happening
        if (!isSwinging && playCardForPlayerCalled && !gameManager.inKnifeAction && !gameManager.inGunAction)
        {
            StartSwing();
        }

        if (isSwinging)
        {
            //Unlock Mouse To Swipe Bat
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = false;

            gameManager.inBatAction = true;

            HandleSwing();
        }
    }

    void StartSwing()
    {
        if (!coroutineCalled)
        {
            coroutineCalled = true;
            StartCoroutine(DelayBatStart());
        }
    }

    IEnumerator DelayBatStart()
    {
        yield return new WaitForSeconds(1f);

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

    void HandleSwing()
    {
        //Visual Swing While Waiting For Click
        if (!isLerping && canUseBat)
        {
            clickToSwingText.text = "< CLICK TO SWING >";

            //Oscillate The Bat From Left To Right Based On Time
            swingProgress += Time.deltaTime * swingSpeedMultiplier;

            float swingAngle = Mathf.Sin(swingProgress) * maxVisualSwingAngle;

            bat.position = swingStartPosition.position;
            bat.rotation = swingStartPosition.rotation;

            Quaternion swingRotation = Quaternion.AngleAxis(swingAngle, Vector3.up);
            bat.rotation = initialBatSwingRotation * swingRotation;
        }

        //Check For Click
        if (Input.GetMouseButtonDown(0) && !isLerping && !isReturning && canUseBat)
        {
            clickToSwingText.text = "";

            canUseBat = false;
            isLerping = true;
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
            }
        }

        //Lerp Back To The Side Of Table
        if (isReturning)
        {
            bat.position = Vector3.Lerp(bat.position, initialPosition, Time.deltaTime * lerpSpeed);
            bat.rotation = Quaternion.Lerp(bat.rotation, initialRotation, Time.deltaTime * lerpSpeed);

            if (Vector3.Distance(bat.position, initialPosition) < 0.01f && Quaternion.Angle(bat.rotation, initialRotation) < 1f)
            {
                isReturning = false;
                FinishSwing();
            }
        }
    }

    void FinishSwing()
    {
        Debug.Log("Swing Finished");

        //Lock Mouse
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        isSwinging = false;
        gameManager.inBatAction = false;
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
            // Skip Player's Turn
            GameManager.Instance.playerSkippedTurnsText.enabled = true;
            GameManager.Instance.playerSkippedTurns++;
            GameManager.Instance.playerSkippedTurnsText.text = "Skipped Players Turn";
        }
    }
}