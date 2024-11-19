using System.Collections;
using System.Collections.Generic;
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
            if (selectedCardCount == 0)
            {
                //Resize Card
                cardsInHand[index].gameObject.transform.rotation = selectedPosition1.transform.rotation;
                //Move To Selected Position 1
                MoveCardToPosition(index, selectedPosition1);
                return cardsInHand[index].GetComponentAtIndex(0);
            }
            else if (cardsInHand[index].gameObject.transform.parent != selectedPosition1)
            {
                //Resize Card
                cardsInHand[index].gameObject.transform.rotation = selectedPosition2.transform.rotation;
                //Move To Selected Position 2
                MoveCardToPosition(index, selectedPosition2);
                return cardsInHand[index].GetComponentAtIndex(1);
            }
        }
        return null;
    }

    void MoveCardToPosition(int index, Transform selectedPosition)
    {
        //Move The Card To The Selected Position
        cardsInHand[index].transform.position = selectedPosition.position;
        //Set Parent Else It Doesn't Return To The Original Position
        cardsInHand[index].transform.SetParent(selectedPosition);
        selectedCardCount++;
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
