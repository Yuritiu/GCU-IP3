using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEngine.UI.Image;

public class CardDrawSystem : MonoBehaviour
{
    //!-Coded By Charlie & Ben-!

    public static CardDrawSystem Instance;

    //DEBUG VARIABLES -> REMOVE FROM FINAL BUILD
    [Header("Debug Variables")]
    [SerializeField] public TextMeshProUGUI debugCurrentTurnText;

    [Header("Card Variables")]
    //Tag For The Card Objects
    [SerializeField] string cardTag = "Card";
    [SerializeField] string opponentTag = "Opponent";

    [Header("Card Slot References")]
    //Original Positions For The Cards
    [SerializeField] Transform[] originalPositions;
    //Array For Actual Card GameObjects
    [SerializeField] GameObject[] cardsInHand;
    [SerializeField] public GameObject currentCardInHand;
    //Selected Positions For The Cards
    [SerializeField] public Transform selectedPosition1;
    [SerializeField] public Transform selectedPosition2;

    [Header("Deck Location References")]
    [SerializeField] GameObject discardDeckLocation;
    [SerializeField] GameObject playingDeckLocation;

    [Header("Discard Deck Variables")]
    GameObject[] cardsToDiscard;
    Transform discardBasePosition;
    //Distance Between Cards
    float stackHeightIncrement = 0.002f;
    float currentDiscardStackHeight;

    [Header("Playing Deck Variables")]
    GameObject[] cardsToPlay;
    [HideInInspector] public Transform playingBasePosition;
    //Distance Between Cards
    float currentPlayingStackHeight;

    [Header("Cards to check bans")]
    bool card1 = true;
    bool card2 = true;
    bool card3 = true;
    bool card4 = true;

    [Header("Cards that cannot be used")]
    private int bannedCard = -1;
    private int bannedCard2 = -1;

    //Current Number Of Selected Cards - Max Of 2
    public int selectedCardCount = 0;

    //Hidden Variables
    [HideInInspector] public bool isPlayersTurn = true;
    [HideInInspector] bool cardAdded = false;
    [HideInInspector] bool cardMovingToTable = false;
    [HideInInspector] public bool cardMoving;
    [HideInInspector] public bool canPlay = true;
    [HideInInspector] CardSelection cardSelection;

    private IntroTutorial introTutorial;
    private bool stepCompleted2 = false;
    private bool stepCompleted3 = false;
    private bool stepCompleted4 = false;

    private PauseMenu pauseMenu;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if(discardDeckLocation != null)
        {
            discardBasePosition = discardDeckLocation.transform;
        }
        else
        {
            Debug.LogError("No Discard Deck Location Assigned");
        }

        if (playingDeckLocation != null)
        {
            playingBasePosition = playingDeckLocation.transform;
        }
        else
        {
            Debug.LogError("No Playing Deck Location Assigned");
        }

        StartGame();
        introTutorial = FindObjectOfType<IntroTutorial>();
        pauseMenu = FindFirstObjectByType<PauseMenu>();
    }

    void Update()
    {
        if (pauseMenu.isPaused)
            return;

            if (isPlayersTurn)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            //Raycast To Mouse Position
            if (Physics.Raycast(ray, out hit))
            {
                //Check For A Card Collider
                if (hit.transform.CompareTag(cardTag) && canPlay)
                {
                    //Find The Index Of The Card In The cardsInHand Array
                    int cardIndex = System.Array.IndexOf(cardsInHand, hit.transform.gameObject);

                    //Check For Left Mouse Click && Checks card is not banned
                    if (Input.GetMouseButtonDown(0) && (cardIndex != bannedCard) && (cardIndex != bannedCard2) && !cardMoving)
                    {
                        //Check That There Is Cards And It's The Players Turn
                        if ((cardIndex >= 0 && cardIndex < originalPositions.Length) && isPlayersTurn)
                        {
                            //Toggle The Card's Selection State
                            ToggleCardSelection(cardIndex);
                        }
                    }
                    else
                    {
                        //Enable The Hovering Card Function
                        cardSelection = hit.transform.gameObject.GetComponent<CardSelection>();
                        if (cardSelection != null)
                        {
                            cardSelection.CardHovered(true);
                        }
                    }
                }
                else
                {
                    //Check If Null, Else Null Reference Errors Will Not Stop
                    if (cardSelection != null)
                    {
                        //Stop The Hovering Card Function
                        cardSelection.CardHovered(false);
                    }
                }
                //Check For Left Mouse Click
                if (Input.GetMouseButtonDown(0) && canPlay)
                {
                    //Check For Opponent's Collider
                    if (hit.transform.CompareTag(opponentTag))
                    {
                        //Check If It's The Players Turn And Atleast 1 Card Is Selected
                        canPlay = false;
                        GameManager.Instance.PlayHand();

                        if (introTutorial != null && !stepCompleted4)
                        {
                            introTutorial.CompleteStep(4);
                            stepCompleted4 = true;
                        }
                    }
                }
            }
        }
    }

    void StartGame()
    {
        //Debug.Log("Deck Count Before Creating Hand: " + CardDeck.Instance.deck.Count);

        //Initialize cardsInHand With 4 Slots
        cardsInHand = new GameObject[4];

        for (int i = 0; i < cardsInHand.Length; i++)
        {
            //Get The Next Card From The Deck
            GameObject card = CardDeck.Instance.DrawCard();

            if (card == null)
            {
                return;
            }

            //Debug.Log("Drew Card: " +  card.name + " Remaining Cards In Deck: " + CardDeck.Instance.deck.Count);

            cardsInHand[i] = Instantiate(card, originalPositions[i].position, originalPositions[i].rotation);

            //updates what cards are banned
            switch (i)
            {
                case 0: card1 = true; break;
                case 1: card2 = true; break;
                case 2: card3 = true; break;
                case 3: card4 = true; break;
            }
        }

        //Debug.Log("Deck Count After Creating Hand: " + CardDeck.Instance.deck.Count);
    }

    public void AddCardAfterTurn()
    {
        //Debug.Log("Attempting To Add A Card After The Turn...");

        for (int i = 0; i < cardsInHand.Length; i++)
        {
            if (cardsInHand[i] == null && CardDeck.Instance.deck.Count > 0)
            {
                //Get The Next Card From The Deck
                GameObject card = CardDeck.Instance.DrawCard();

                if (card == null)
                {
                    return;
                }

                // Updates what cards are banned
                switch (i)
                {
                    case 0: card1 = true; break;
                    case 1: card2 = true; break;
                    case 2: card3 = true; break;
                    case 3: card4 = true; break;
                }

                cardsInHand[i] = Instantiate(card, originalPositions[i].position, originalPositions[i].rotation);
                cardAdded = true;

                //Debug.Log("Card Added Successfully.");
                break;
            }
        }
    }

    void ToggleCardSelection(int index)
    {
        //Check If The Card Is Currently In The Selected Position
        bool isSelected = IsCardInSelectedPosition(cardsInHand[index]);

        //If The Card Is Already Selected Deselect It
        if (isSelected)
        {
            DeselectCard(index);
        }
        else
        {
            //Max Of 2 Selected Cards
            if (selectedCardCount < 2)
            {
                SelectCard(index);
            }
        }
    }

    void SelectCard(int index)
    {
        //Check If It's The Players Turn First
        if (isPlayersTurn)
        {
            //Move The Card To The First Available Selected Position, Check If Position 2 Is Populated So That It Doesn't Fill It
            if ((selectedCardCount == 0) || (selectedCardCount == 1 && selectedPosition2.childCount >= 1))
            {
                //Resize Card
                cardsInHand[index].gameObject.transform.rotation = selectedPosition1.transform.rotation;
                //Move To Selected Position 1
                cardMovingToTable = true;
                MoveCardToPosition(index, selectedPosition1, cardsInHand[index].transform);
            }
            else if (selectedCardCount == 1 && (selectedPosition2.childCount <= 0))
            {
                //Resize Card
                cardsInHand[index].gameObject.transform.rotation = selectedPosition2.transform.rotation;
                //Move To Selected Position 2
                cardMovingToTable = true;
                MoveCardToPosition(index, selectedPosition2, cardsInHand[index].transform);
            }
            // Updates what cards are banned
            switch (index)
            {
                case 0: card1 = false; break;
                case 1: card2 = false; break;
                case 2: card3 = false; break;
                case 3: card4 = false; break;
            }

            if (introTutorial != null && !stepCompleted2)
            {
                introTutorial.CompleteStep(2);
                stepCompleted2 = true;
            }
        }
    }

    void DeselectCard(int index)
    {
        //Reset The Parent To Null
        cardsInHand[index].transform.SetParent(null);

        cardMovingToTable = false;
        MoveCardToPosition(index, originalPositions[index].transform, cardsInHand[index].transform);

        selectedCardCount = CheckSelectedCards();

        //updates what cards are banned
        switch (index)
        {
            case 0: card1 = true; break;
            case 1: card2 = true; break;
            case 2: card3 = true; break;
            case 3: card4 = true; break;
        }

        if (introTutorial != null && !stepCompleted3)
        {
            introTutorial.CompleteStep(3);
            stepCompleted3 = true;
        }
    }

    public void FindCardsOnTable()
    {
        //List To Temporarily Store Found Cards
        List<GameObject> foundCards = new List<GameObject>();

        //Find All Cards In The Scene
        foreach (GameObject card in FindObjectsOfType<GameObject>())
        {
            if (card.transform.parent != null && card.transform.parent.name.Contains("Selected"))
            {
                //Clear Parent
                card.transform.parent = null;
                card.name = "Discarded Card";
                if(card.GetComponent <CardSelection>() != null && card.GetComponent<BoxCollider>() != null)
                {
                    Destroy(card.GetComponent<CardSelection>());
                    Destroy(card.GetComponent<BoxCollider>());
                }
                AICardDrawSystem.Instance.DeleteCardsInHand();
                DeleteCardsInHand();
                foundCards.Add(card.gameObject);
            }
        }

        //Convert The List To An Array
        cardsToDiscard = foundCards.ToArray();
    }

    public void DeleteCardsInHand()
    {
        for (int i = 0; i < cardsInHand.Length; i++)
        {
            if (cardsInHand[i] != null && cardsInHand[i].name == "Discarded Card")
            {
                RemoveScripts(cardsInHand[i].gameObject);
                cardsInHand[i] = null;
            }
        }
    }

    void RemoveScripts(GameObject obj)
    {
        var components = obj.GetComponents<MonoBehaviour>();

        //Destroy Each Script
        foreach (var script in components)
        {
            if (script != null)
            {
                Destroy(script);
            }
        }
    }

    void MoveCardToPosition(int index, Transform selectedPosition, Transform currentPosition)
    {
        if (cardMoving)
            return;

        //Stop Player Moving 2 Cards At Once
        cardMoving = true;

        cardSelection.canSelect = false;

        StartCoroutine(MoveCardToPosition(index, selectedPosition, 0.5f, 0.1f, currentPosition));
    }

    IEnumerator MoveCardToPosition(int index, Transform selectedPosition, float duration, float pauseDuration, Transform currentPosition)
    {
        Vector3 startPosition;
        Quaternion startRotation;

        Vector3 targetPosition;
        Quaternion targetRotation;

        //Halfway Flip
        Quaternion halfwayRotation;

        //Check If The Card Is Being Selected (true) OR Deselected (false)
        if (cardMovingToTable)
        {
            //Start Position TODO: CHECK WHAT POSITION
            startPosition = currentPosition.transform.position;
            startRotation = currentPosition.transform.rotation;

            halfwayRotation = Quaternion.Euler(-20, 180, -33);

            //Final Position
            targetPosition = selectedPosition.position;
            targetRotation = Quaternion.Euler(-90, 180, 0f);

        }
        else
        {
            //Start Position
            startPosition = currentPosition.transform.position;
            startRotation = currentPosition.transform.rotation;

            halfwayRotation = Quaternion.Euler(-20, 180, -33);

            //Final Position
            targetPosition = selectedPosition.transform.position;
            targetRotation = selectedPosition.rotation;
        }

        //Lift Position
        Vector3 liftPosition;
        Quaternion liftRotation;

        //Halfway Position
        Vector3 halfwayPosition;

        //Check If The Card Is Being Selected (true) OR Deselected (false)
        if (cardMovingToTable)
        {
            liftPosition = new Vector3(startPosition.x, startPosition.y + 0.08f, startPosition.z);
            liftRotation = Quaternion.Euler(-20, 180, -0.235f);

            halfwayPosition = (liftPosition + targetPosition) / 2 + Vector3.up * 0.03f;
        }
        else
        {
            liftPosition = new Vector3(startPosition.x, startPosition.y + 0.1f, startPosition.z);
            liftRotation = Quaternion.Euler(-20, 180, 0.235f);

            halfwayPosition = (liftPosition + targetPosition) / 2 + Vector3.up * -0.03f;
        }

        float elapsedTime = 0f;

        //Movement 1: Lift up
        while (elapsedTime < duration * 0.3f)
        {
            //Calculate Normalized Time
            float t = elapsedTime / (duration * 0.3f);
            float easedT = EaseMovementCubic(t);

            //Lerp Position And Keep Rotation The Same
            cardsInHand[index].transform.position = Vector3.Lerp(startPosition, liftPosition, easedT);
            cardsInHand[index].transform.rotation = Quaternion.Lerp(startRotation, liftRotation, easedT);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        //Verify Positions
        cardsInHand[index].transform.position = liftPosition;
        cardsInHand[index].transform.rotation = liftRotation;

        //Pause Briefly
        yield return new WaitForSeconds(pauseDuration);

        elapsedTime = 0f;

        //Movement 2 & 3: Placement Motion
        while (elapsedTime < duration * 0.7f)
        {
            //Calculate Normalized Time
            float t = elapsedTime / (duration * 0.7f);
            float easedT = EaseMovementCubic(t);

            if (easedT < 0.5f)
            {
                //Movement 2: Move Towards Halfway Position And Rotation
                //Normalize 0-0.5 Range To 0-1
                float phase2T = easedT * 2f;
                currentPosition.transform.position = Vector3.Lerp(liftPosition, halfwayPosition, phase2T);
                currentPosition.transform.rotation = Quaternion.Lerp(liftRotation, halfwayRotation, phase2T);
            }
            else
            {
                //Movement 3: Move Towards Final Position And Rotation
                //Normalize 0.5-1 Range To 0-1
                float phase3T = (easedT - 0.5f) * 2f;
                currentPosition.transform.position = Vector3.Lerp(halfwayPosition, targetPosition, phase3T);
                currentPosition.transform.rotation = Quaternion.Lerp(halfwayRotation, targetRotation, phase3T);
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        //Verify Positions
        currentPosition.transform.position = targetPosition;
        currentPosition.transform.rotation = targetRotation;

        //Set Parent After Movement
        currentPosition.transform.SetParent(selectedPosition);

        cardSelection.canSelect = true;
        cardMoving = false;

        //Recalculate Selected Card Count
        selectedCardCount = CheckSelectedCards();
    }

    public IEnumerator LerpCardsToDiscardDeck(float duration)
    {
        if (cardsToDiscard.Length == 0)
        {
            //No Cards On Table To Clear
            yield break;
        }

        //Start Position
        Vector3[] startPositions = new Vector3[cardsToDiscard.Length];
        Quaternion[] startRotations = new Quaternion[cardsToDiscard.Length];

        for (int i = 0; i < cardsToDiscard.Length; i++)
        {
            startPositions[i] = cardsToDiscard[i].transform.position;
            startRotations[i] = cardsToDiscard[i].transform.rotation;
        }

        //Final Position
        Vector3 targetPosition = discardDeckLocation.transform.position;
        Quaternion targetRotation = Quaternion.Euler(-90, 180, 0f);

        float elapsedTime = 0f;

        //Movement 1: Slide To Edge Of Table
        while (elapsedTime < duration * 0.3f)
        {
            //Calculate Normalized Time
            float t = elapsedTime / (duration * 0.3f);
            float easedT = EaseMovementCubic(t);

            for (int i = 0; i < cardsToDiscard.Length; i++)
            {
                if (cardsToDiscard[i] != null)
                {
                    Vector3 discardDeckStackPosition = discardBasePosition.position + new Vector3(0, currentDiscardStackHeight + i * stackHeightIncrement, 0);

                    //Lerp Cards To Discard Pile Location
                    cardsToDiscard[i].transform.position = Vector3.Lerp(startPositions[i], discardDeckStackPosition, t);
                    cardsToDiscard[i].transform.rotation = Quaternion.Lerp(startRotations[i], targetRotation, t);
                }
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        for (int i = 0; i < cardsToDiscard.Length; i++)
        {
            if (cardsToDiscard[i] != null)
            {
                //Set Final Position
                cardsToDiscard[i].transform.position = discardBasePosition.position + new Vector3(0, currentDiscardStackHeight + i * stackHeightIncrement, 0);
                cardsToDiscard[i].transform.rotation = targetRotation;
            }
        }

        //Update The Stack Height
        currentDiscardStackHeight += cardsToDiscard.Length * stackHeightIncrement;
    }

    //This Makes The Cards Movement Increase Over Time At The Start And Decrease Near The End
    float EaseMovementCubic(float t)
    {
        if (t < 0.5f)
        {
            return 4f * t * t * t;
        }
        else
        {
            return 1f - Mathf.Pow(-2f * t + 2f, 3f) / 2f;
        }
    }

    bool IsCardInSelectedPosition(GameObject card)
    {
        //Check If The Card Is Currently In One Of The Selected Positions
        return card.transform.parent != null && (card.transform.parent == selectedPosition1 || card.transform.parent == selectedPosition2);
    }

    int CheckSelectedCards()
    {
        if(selectedPosition1.childCount > 0 && selectedPosition2.childCount > 0)
        {
            return 2;
        }
        if (selectedPosition1.childCount > 0 || selectedPosition2.childCount > 0)
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }

    public void StopOneCard()
    {
        /*for (int i = 0; i < cardsInHand.Length; i++)
        {
            if (cardsInHand[i] != null)
            {
                if (IsCardInSelectedPosition(cardsInHand[i]) != true)
                { 
                    GameObject cardNotinUse = cardsInHand[i].gameObject;
                    cardNotinUse.transform.Rotate(0,180,0);
                    bannedCard = i;
                    return;
                }
            }
        }*/


        //says how many cards are avalible
        int cardsInCurrentHand = 4;

        for (int i = 0; i < cardsInHand.Length; i++)
        {
            if (cardsInHand[i] == null)
            {
                cardsInCurrentHand--;
            }
        }

        if (selectedPosition1.childCount > 0)
        {
            cardsInCurrentHand--;
        }
        if (selectedPosition2.childCount > 0)
        {
            cardsInCurrentHand--;
        }
        //print(cardsInCurrentHand);

        int rand = UnityEngine.Random.Range(0, cardsInCurrentHand);

        //print(rand);
        //print(card1);
        //print(card2);
        //print(card3);
        //print(card4);
        //checks what card to ban
        if(card1 == true)
        {
            if(rand == 0)
            {
                GameObject cardNotinUse = cardsInHand[0].gameObject;
                cardNotinUse.transform.Rotate(0,180,0);
                if(bannedCard != -1)
                {
                    bannedCard2 = 0;
                    return;
                }
                bannedCard = 0;
                return;
            }
            rand--;
        }
        if(card2 == true)
        {
            if(rand == 0)
            {
                GameObject cardNotinUse = cardsInHand[1].gameObject;
                cardNotinUse.transform.Rotate(0,180,0);
                if (bannedCard != -1)
                {
                    bannedCard2 = 1;
                    return;
                }
                bannedCard = 1;
                return;
            }
            rand--;
        }
        if(card3 == true)
        {
            if(rand == 0)
            {
                GameObject cardNotinUse = cardsInHand[2].gameObject;
                cardNotinUse.transform.Rotate(0,180,0);
                if (bannedCard != -1)
                {
                    bannedCard2 = 2;
                    return;
                }
                bannedCard = 2;
                return;
            }
            rand--;
        }
        if(card4 == true)
        {
            if(rand == 0)
            {
                GameObject cardNotinUse = cardsInHand[3].gameObject;
                cardNotinUse.transform.Rotate(0,180,0);
                if (bannedCard != -1)
                {
                    bannedCard2 = 3;
                    return;
                }
                bannedCard = 3;
                return;
            }
            rand--;
        }
    }

    public void UnbanCards()
    {
        if (bannedCard != -1)
        {
            if (cardsInHand[bannedCard] != null)
            {
                cardsInHand[bannedCard].gameObject.transform.Rotate(0, 180, 0);
            }
            bannedCard = -1;
        }
        if (bannedCard2 != -1)
        {
            if (cardsInHand[bannedCard2] != null)
            {
                cardsInHand[bannedCard2].gameObject.transform.Rotate(0, 180, 0);
            }
            bannedCard2 = -1;
        }
    }

    public void ShuffleHand()
    {
        //TODO add several array options then pick one of them at random
        GameObject[] array = {cardsInHand[3], cardsInHand[0], cardsInHand[1], cardsInHand[2]};
        cardsInHand = array;

        for (int i = 0; i < cardsInHand.Length; i++)
        {
            if (cardsInHand[i] != null)
            {
                if (cardsInHand[i].transform.position != selectedPosition1.position && cardsInHand[i].transform.position != selectedPosition2.position)
                {
                    cardsInHand[i].transform.position = originalPositions[i].position;
                    cardsInHand[i].transform.rotation = originalPositions[i].rotation;
                }
            } 
        }
    }
}
