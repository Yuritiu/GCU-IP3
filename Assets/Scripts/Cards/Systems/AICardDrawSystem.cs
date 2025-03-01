using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class AICardDrawSystem : MonoBehaviour
{
    //!-Coded By Charlie & Ben-!

    public static AICardDrawSystem Instance;

    [Header("Card Variables")]
    //Tag For The Card Objects
    [SerializeField] string cardTag = "Card";
    [SerializeField] string opponentTag = "Player";

    [Header("Card Slot References")]
    //Original Positions For The Cards
    [SerializeField] Transform[] originalPositions;
    //Array For Actual Card GameObjects
    [SerializeField] public GameObject[] cardsInHand;
    //Selected Positions For The Cards
    [SerializeField] public Transform selectedPosition1;
    [SerializeField] public Transform selectedPosition2;

    [Header("Cards to check bans")]
    bool card1 = true;
    bool card2 = true;
    bool card3 = true;
    bool card4 = true;

    [HideInInspector] bool cardAdded = false;
    [HideInInspector] public bool card1Moving = false;
    [HideInInspector] public bool card2Moving = false;
    //Track Each Card's Movement Status From Deck To Hand
    bool[] cardMovementStatus = new bool[4];
    //Current Number Of Selected Cards - Max Of 2
    [HideInInspector] public int selectedCardCount = 0;
    [HideInInspector] public bool isPlayersTurn = true;

    [Header("Cards that cannot be used")]
    private int bannedCard = -1;
    private int bannedCard2 = -1;

    [Header("cards that have been used")]
    private int selectedCard1Index = 0;

    [Header("Dev Console Variables")]
    //If True -> AI Waits For Manual Draw Input
    [HideInInspector] public bool manualMode = false;
    public Queue<GameObject> manualDrawQueue = new Queue<GameObject>();

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        StartCoroutine(WaitForDeckToFinishFanning());
    }

    IEnumerator WaitForDeckToFinishFanning()
    {
        while (!CardDeck.Instance.fanAnimationComplete)
        {
            yield return null;
        }

        StartGame();
    }

    void StartGame()
    {
        //Debug.Log("Deck Count Before Creating Hand: " + CardDeck.Instance.deck.Count);

        var playingDeckLocation = CardDrawSystem.Instance.playingDeckLocation;
        float currentTopDeckPosition = CardDeck.Instance.currentDeckStackHeight + playingDeckLocation.transform.position.y;
        Vector3 playingDeckTopLocation = new Vector3(playingDeckLocation.transform.position.x, currentTopDeckPosition, playingDeckLocation.transform.position.z);

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

            //Debug.Log("Drew Card: " + card.name + " Remaining Cards In Deck: " + CardDeck.Instance.deck.Count);

            //Instantiate And Store The Reference
            //cardsInHand[i] = Instantiate(card, originalPositions[i].position, originalPositions[i].rotation);
            cardsInHand[i] = Instantiate(card, playingDeckTopLocation, Quaternion.Euler(90, 0, 0));

            //Destroy The CardSelection Script On The AI's Cards So The Player Can't Hover Them
            Destroy(cardsInHand[i].GetComponent<CardSelection>());
            Destroy(cardsInHand[i].GetComponent<BoxCollider>());

            // Updates what cards are banned
            switch (i)
            {
                case 0: card1 = true; break;
                case 1: card2 = true; break;
                case 2: card3 = true; break;
                case 3: card4 = true; break;
            }

            StartCoroutine(MoveCardToSlot(cardsInHand[i], originalPositions[i].position, originalPositions[i].rotation, 0.5f, i * 0.3f, i));
        }
        
        //Debug.Log("Deck Count After Creating Hand: " + CardDeck.Instance.deck.Count);
    }

    IEnumerator MoveCardToSlot(GameObject card, Vector3 targetPosition, Quaternion targetRotation, float duration, float delay, int cardIndex)
    {
        cardMovementStatus[cardIndex] = true;

        Vector3 startPosition = card.transform.position;
        Vector3 endPosition = targetPosition;
        float elapsedTime = 0f;

        yield return new WaitForSeconds(delay);

        //Move Card With A Parabolic Arc (Throw)
        while (elapsedTime < duration)
        {
            float progress = elapsedTime / duration;

            //Throwing Arc Height
            float arcHeight = Mathf.Sin(progress * Mathf.PI) * 0.1f;
            Vector3 currentPosition = Vector3.Lerp(startPosition, endPosition, progress);
            currentPosition.y += arcHeight;

            Quaternion currentRotation = Quaternion.Lerp(card.transform.rotation, targetRotation, progress);

            card.transform.position = currentPosition;
            card.transform.rotation = currentRotation;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        card.transform.position = endPosition;
        card.transform.rotation = targetRotation;

        cardMovementStatus[cardIndex] = false;
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
            if(script!= null)
            {
                Destroy(script);
            }
        }
    }

    public Component SelectCard()
    {
        if (manualMode && manualDrawQueue.Count > 0)
        {
            //Dequeue The Next Card Prefab From Manual Draw Queue
            GameObject cardPrefab = manualDrawQueue.Dequeue();

            if (cardPrefab != null)
            {
                for (int i = 0; i < cardsInHand.Length; i++)
                {
                    //Only Select Empty Slots And Check There Are Cards In The Deck
                    if (cardsInHand[i] == null && CardDeck.Instance.deck.Count > 0)
                    {
                        if (selectedCardCount == 0)
                        {
                            selectedCardCount++;

                            //Get Top Of The Deck For Second Card
                            var playingDeckLocation = CardDrawSystem.Instance.playingDeckLocation;
                            float currentTopDeckPosition = CardDeck.Instance.currentDeckStackHeight + playingDeckLocation.transform.position.y;
                            Vector3 playingDeckTopLocation = new Vector3(playingDeckLocation.transform.position.x, currentTopDeckPosition, playingDeckLocation.transform.position.z);

                            cardsInHand[i] = Instantiate(cardPrefab, playingDeckTopLocation, Quaternion.Euler(90, 0, 0));
                            MoveCard1ToPosition(i, selectedPosition1, cardsInHand[i].transform);

                            return cardsInHand[i].GetComponent<Component>();
                        }
                        else if (selectedCardCount == 1)
                        {
                            selectedCardCount++;

                            //Get Top Of The Deck For Second Card
                            var playingDeckLocation = CardDrawSystem.Instance.playingDeckLocation;
                            float currentTopDeckPosition = CardDeck.Instance.currentDeckStackHeight + playingDeckLocation.transform.position.y;
                            Vector3 playingDeckTopLocation = new Vector3(playingDeckLocation.transform.position.x, currentTopDeckPosition, playingDeckLocation.transform.position.z);

                            cardsInHand[i] = Instantiate(cardPrefab, playingDeckTopLocation, Quaternion.Euler(90, 0, 0));
                            MoveCard2ToPosition(i, selectedPosition2, cardsInHand[i].transform);

                            return cardsInHand[i].GetComponent<Component>();
                        }
                    }
                }
            }
            else
            {
                Debug.LogWarning("Card Prefab Is Null");
                return null;
            }
        }

        //If Not In Manual Mode -> Proceed With AI's Random Selection
        int index;

        if (IsAnyCardMoving())
        {
            return null;
        }

        //Pick a random card index (for AI)
        index = Random.Range(0, 4);

        //Debugging purposes
        //index = 0; // For debugging, force it to always pick the first card

        //Check if all cards are not null
        if (cardsInHand[0] != null && cardsInHand[1] != null && cardsInHand[2] != null && cardsInHand[3] != null)
        {
            //If all cards contain the "cigar" in their name, handle them specially
            if (cardsInHand[0].name.Contains("cigar") && cardsInHand[1].name.Contains("cigar") &&
                cardsInHand[2].name.Contains("cigar") && cardsInHand[3].name.Contains("cigar"))
            {
                selectedCardCount++;
                //Move the selected card to the first selected position
                MoveCard1ToPosition(index, selectedPosition1, cardsInHand[index].transform);
                selectedCard1Index = index;
                return cardsInHand[index].GetComponentAtIndex(0);
            }
        }

        //Ensure the selected card isn't banned
        if (cardsInHand[index] != null && index != bannedCard && index != bannedCard2)
        {
            if (selectedCardCount == 0 && !cardsInHand[index].name.Contains("cigar"))
            {
                selectedCardCount++;
                //Move the selected card to the first selected position
                MoveCard1ToPosition(index, selectedPosition1, cardsInHand[index].transform);
                selectedCard1Index = index;
                return cardsInHand[index].GetComponentAtIndex(0);
            }
            else if (selectedCardCount == 1 && index != selectedCard1Index)
            {
                selectedCardCount++;
                //Move the second selected card to the second selected position
                MoveCard2ToPosition(index, selectedPosition2, cardsInHand[index].transform);
                return cardsInHand[index].GetComponentAtIndex(1);
            }
        }

        return null;
    }


    bool IsAnyCardMoving()
    {
        foreach (bool isMoving in cardMovementStatus)
        {
            if (isMoving)
            {
                return true;
            }
        }
        return false;
    }

    void MoveCard1ToPosition(int index, Transform selectedPosition, Transform currentPosition)
    {
        if (card1Moving)
            return;

        card1Moving = true;

        switch (index)
        {
            case 0:
                card1 = false;
                break;
            case 1:
                card2 = false;
                break;
            case 2:
                card3 = false;
                break;
            case 3:
                card4 = false;
                break;
        }

        StartCoroutine(MoveCardToPosition(index, selectedPosition, 0.5f, 0.1f, currentPosition));
    }

    void MoveCard2ToPosition(int index, Transform selectedPosition, Transform currentPosition)
    {
        if (card2Moving)
            return;

        //Stop Player Moving 2 Cards At Once
        card2Moving = true;

        switch (index)
        {
            case 0:
                card1 = false;
                break;
            case 1:
                card2 = false;
                break;
            case 2:
                card3 = false;
                break;
            case 3:
                card4 = false;
                break;
        }

        StartCoroutine(MoveCardToPosition(index, selectedPosition, 0.5f, 0.1f, currentPosition));
    }

    IEnumerator MoveCardToPosition(int index, Transform selectedPosition, float duration, float pauseDuration, Transform currentPosition)
    {
        //TODO: ADD ANIMATIONS TO AI CARD DRAW

        //Start Position
        Vector3 startPosition = currentPosition.transform.position;
        Quaternion startRotation = currentPosition.transform.rotation;

        //Lift Position
        Vector3 liftPosition = new Vector3(startPosition.x, startPosition.y + 0.08f, startPosition.z);
        Quaternion liftRotation = Quaternion.Euler(-20, 0, 0.235f);

        //Final Position
        Vector3 targetPosition = selectedPosition.position;
        Quaternion targetRotation = Quaternion.Euler(-90, 0, 0f);

        //Halfway Position
        Vector3 halfwayPosition = (liftPosition + targetPosition) / 2 + Vector3.up * 0.03f;
        Quaternion halfwayRotation = Quaternion.Euler(-20, 0, -33);

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

    public void AddCardAfterTurn()
    {
        if (manualMode)
        {
            Debug.Log("Manual Mode is ON. Waiting For Player To Select Card(s) Using 'set_ai_draw'");

            return;
        }

        DrawAndAddCard();
    }

    void DrawAndAddCard() 
    {
        //Debug.Log("Attempting To Add An AI Card After The Turn...");

        var playingDeckLocation = CardDrawSystem.Instance.playingDeckLocation;
        float currentTopDeckPosition = CardDeck.Instance.currentDeckStackHeight + playingDeckLocation.transform.position.y;
        Vector3 playingDeckTopLocation = new Vector3(playingDeckLocation.transform.position.x, currentTopDeckPosition, playingDeckLocation.transform.position.z);

        for (int i = 0; i < cardsInHand.Length; i++)
        {
            if (cardsInHand[i] == null && CardDeck.Instance.deck.Count > 0)
            {
                GameObject card = CardDeck.Instance.DrawCard();

                if (card == null)
                {
                    return;
                }

                //cardsInHand[i] = Instantiate(card, originalPositions[i].position, originalPositions[i].rotation);
                cardsInHand[i] = Instantiate(card, playingDeckTopLocation, Quaternion.Euler(90, 0, 0));
                StartCoroutine(MoveCardToSlot(cardsInHand[i], originalPositions[i].position, originalPositions[i].rotation, 0.5f, i * 0.3f, i));
                cardAdded = true;

                //Debug.Log("Card Added Successfully.");

                break;
            }
        }
    }
    
    bool IsCardInSelectedPosition(GameObject card)
    {
        //Check If The Card Is Currently In One Of The Selected Positions
        return card.transform.parent != null && (card.transform.parent == selectedPosition1 || card.transform.parent == selectedPosition2);
    }

    public void StopOneCard()
    {
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
        if (card1 == true)
        {
            if (rand == 0)
            {
                GameObject cardNotinUse = cardsInHand[0].gameObject;
                cardNotinUse.transform.Rotate(0, 180, 0);
                if (bannedCard != -1)
                {
                    bannedCard2 = 0;
                    return;
                }
                bannedCard = 0;
                return;
            }
            rand--;
        }
        if (card2 == true)
        {
            if (rand == 0)
            {
                GameObject cardNotinUse = cardsInHand[1].gameObject;
                cardNotinUse.transform.Rotate(0, 180, 0);
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
        if (card3 == true)
        {
            if (rand == 0)
            {
                GameObject cardNotinUse = cardsInHand[2].gameObject;
                cardNotinUse.transform.Rotate(0, 180, 0);
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
        if (card4 == true)
        {
            if (rand == 0)
            {
                GameObject cardNotinUse = cardsInHand[3].gameObject;
                cardNotinUse.transform.Rotate(0, 180, 0);
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

    //-----------------------------------------------DEV CONSOLE FUNCTIONS-----------------------------------------------
    public void ToggleManualDrawMode()
    {
        manualMode = !manualMode;
        //Remove All Cards In Current AI Hand
        ManualDeleteCardsInHand();

        if (manualMode)
        {
            DevConsole.Instance.debugText.text = "Manual Draw Mode Enabled";
        }
        else
        {
            DevConsole.Instance.debugText.text = "Manual Draw Mode Disabled";
        }
    }

    public void ManualDeleteCardsInHand()
    {
        for (int i = 0; i < cardsInHand.Length; i++)
        {
            if (cardsInHand[i] != null)
            {
                Destroy(cardsInHand[i].gameObject);
                cardsInHand[i] = null;
            }
        }
    }

    public void ListAvailableCards()
    {
        if (CardDeck.Instance.deck.Count == 0)
        {
            DevConsole.Instance.debugText.text = "No Cards Available In Deck";
            return;
        }

        List<string> cardNames = CardDeck.Instance.deck
            .Select(card => card.name)
            .Distinct()
            .ToList();

        DevConsole.Instance.debugText.text = ("Available Cards: " + string.Join(", ", cardNames));
    }

    public void SetNextDrawCards(string[] cardNames)
    {
        if (!manualMode)
        {
            DevConsole.Instance.debugText.text = "Enable Manual Mode first: Use 'manualdraw'";
            return;
        }

        manualDrawQueue.Clear();

        foreach (string name in cardNames)
        {
            GameObject cardPrefab = CardDeck.Instance.deck.FirstOrDefault(c => c.name.ToLower().Contains(name.ToLower()));
            if (cardPrefab != null)
            {
                //If A Card Matching The Keyword Is Found -> Enqueue The Card Prefab
                manualDrawQueue.Enqueue(cardPrefab);
            }
            else
            {
                DevConsole.Instance.debugText.text = ($"No Card Found With Keyword: {name}");
            }
        }

        DevConsole.Instance.debugText.text = ($"Queued {manualDrawQueue.Count} Cards For AI Draw.");
    }
}
