using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
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
    //Current Number Of Selected Cards - Max Of 2
    [HideInInspector] public int selectedCardCount = 0;
    [HideInInspector] public bool isPlayersTurn = true;

    [Header("Cards that cannot be used")]
    private int bannedCard = -1;
    private int bannedCard2 = -1;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        StartGame();
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
                break;
            }

            //Debug.Log("Drew Card: " + card.name + " Remaining Cards In Deck: " + CardDeck.Instance.deck.Count);

            //Instantiate And Store The Reference
            cardsInHand[i] = Instantiate(card, originalPositions[i].position, originalPositions[i].rotation);
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
        }

        //Debug.Log("Deck Count After Creating Hand: " + CardDeck.Instance.deck.Count);
    }

    public void DeleteCardsInHand()
    {
        for (int i = 0; i < cardsInHand.Length; i++)
        {
            if (cardsInHand[i] != null && cardsInHand[i].name == "Discarded Card")
            {
                cardsInHand[i] = null;
            }
        }
    }

    public Component SelectCard()
    {
        int index;

        //picks cards AI will use
        index = Random.Range(0, 3);
        
        //for debugging
        //index = 0; 
        //print(index);

        if (cardsInHand[index] != null && (index != bannedCard) && (index != bannedCard2))
        {
            selectedCardCount++;

            if (selectedCardCount == 1)
            {
                //Move To Selected Position 1
                MoveCard1ToPosition(index, selectedPosition1, cardsInHand[index].transform);
                return cardsInHand[index].GetComponentAtIndex(0);
            }
            else if (cardsInHand[index].gameObject.transform.parent != selectedPosition1 && selectedCardCount == 2)
            {
                //Move To Selected Position 2
                MoveCard2ToPosition(index, selectedPosition2, cardsInHand[index].transform);
                return cardsInHand[index].GetComponentAtIndex(1);
            }
        }
        return null;
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
        //Debug.Log("Attempting To Add An AI Card After The Turn...");

        for (int i = 0; i < cardsInHand.Length; i++)
        {
            if (cardsInHand[i] == null && CardDeck.Instance.deck.Count > 0)
            {
                GameObject card = CardDeck.Instance.DrawCard();
                if (card == null)
                {
                    return;
                }
                //Debug.Log("Adding Card " + card.name + " To Slot " + i);

                cardsInHand[i] = Instantiate(card, originalPositions[i].position, originalPositions[i].rotation);
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
}
