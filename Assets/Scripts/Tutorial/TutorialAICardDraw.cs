using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class TutorialAICardDraw : MonoBehaviour
{
    //!-Coded By Charlie & Ben-!

    public static TutorialAICardDraw Instance;

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

    [Header("Rarity Chance")]
    [SerializeField] int commonRarity = 45;
    [SerializeField] int uncommonRarity = 30;
    [SerializeField] int rareRarity = 15;
    [SerializeField] int legendaryRarity = 5;
    int totalRarity;

    [Header("Cards")]
    [SerializeField] GameObject[] knifeCard;
    [SerializeField] GameObject[] armourCard;
    [SerializeField] GameObject[] cigarCard;
    [SerializeField] GameObject[] skipTurnCard;

    [HideInInspector] bool cardAdded = false;
    [HideInInspector] int cardsNumber = 0;
    //Current Number Of Selected Cards - Max Of 2
    [HideInInspector] public int selectedCardCount = 0;
    [HideInInspector] public bool isPlayersTurn = true;

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
        //Initialize cardsInHand Array With 4 Slots
        cardsInHand = new GameObject[4];

        uncommonRarity += commonRarity;
        rareRarity += uncommonRarity;
        legendaryRarity += rareRarity;

        totalRarity = legendaryRarity;

        //Add 4 Random Cards To The cardsInHand Array
        for (int i = 0; i < cardsInHand.Length; i++)
        {
            //Choose A Random Card Prefab
            GameObject card = GetRandomCard();
            //Instantiate And Store The Reference
            cardsInHand[i] = Instantiate(card, originalPositions[i].position, originalPositions[i].rotation);
            //Destroy The CardSelection Script On The AI's Cards So The Player Can't Hover Them
            Destroy(cardsInHand[i].GetComponent<CardSelection>());
        }
    }

    public GameObject GetRandomCard()
    {
        //Randomly Select A Card Based On Rarity Chance
        cardsNumber++;

        if (cardsNumber <= 3)
        {
            //Give 4 Knife Cards @ Start Of Tutorial
            return knifeCard[Random.Range(0, knifeCard.Length)];
        }
        else if (cardsNumber > 3 && cardsNumber <= 5)
        {
            //Give 2 Armour Cards
            return armourCard[Random.Range(0, armourCard.Length)];
        }
        else
        {
            return knifeCard[Random.Range(0, knifeCard.Length)];
        }
    }

    public Component SelectCard()
    {
        int index;

        //picks cards AI will use
        index = Random.Range(0, 3);

        //for debugging
        index = 0;
        //print(index);


        if (cardsInHand[index] != null)
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
    }

    public void AddCardAfterTurn()
    {
        //Choose A Random Card Prefab
        GameObject card = GetRandomCard();
        //Add 1 Random Card Prefab After A Turn
        for (int i = 0; i < cardsInHand.Length; i++)
        {
            //Check If There Is An Available Slot
            if (cardsInHand[i] == null)
            {
                //Instantiate And Store The Reference
                cardsInHand[i] = Instantiate(card, originalPositions[i].position, originalPositions[i].rotation);
                //Destroy The CardSelection Script On The AI's Cards So The Player Can't Hover Them
                Destroy(cardsInHand[i].GetComponent<CardSelection>());
                //Mark The Card As Added
                cardAdded = true;
                //Exit When Card Is Placed
                break;
            }
        }
    }
}
