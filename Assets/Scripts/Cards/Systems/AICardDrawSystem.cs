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

    [Header("Rarity Chance")]
    [SerializeField] int commonRarity = 45;
    [SerializeField] int uncommonRarity = 30;
    [SerializeField] int rareRarity = 15;
    [SerializeField] int legendaryRarity = 5;
    int totalRarity;

    [Header("Unique Cards")]
    [SerializeField] GameObject[] commonCards;
    [SerializeField] GameObject[] uncommonCards;
    [SerializeField] GameObject[] rareCards;
    [SerializeField] GameObject[] legendaryCards;

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
            switch (i)
            {
                case 0:
                    card1 = true;
                    break;
                case 1:
                    card2 = true;
                    break;
                case 2:
                    card3 = true;
                    break;
                case 3:
                    card4 = true;
                    break;
            }
        }
    }

    public GameObject GetRandomCard()
    {
        //Randomly Select A Card Based On Rarity Chance
        var randomChance = Random.Range(0, totalRarity);

        if (randomChance <= commonRarity)
        {
            //Common Rarity
            var commonRandomChance = Random.Range(0, commonCards.Length - 1);
            return commonCards[Random.Range(0, commonCards.Length)];
        }
        else if (randomChance > commonRarity && randomChance <= uncommonRarity)
        {
            //Uncommon Rarity
            var uncommonRandomChance = Random.Range(0, uncommonCards.Length - 1);
            return uncommonCards[Random.Range(0, uncommonCards.Length)];
        }
        else if (randomChance > uncommonRarity && randomChance <= rareRarity)
        {
            //Rare Rarity
            var rareRandomChance = Random.Range(0, rareCards.Length - 1);
            return rareCards[Random.Range(0, rareCards.Length)];
        }
        else if (randomChance > rareRarity && randomChance <= legendaryRarity)
        {
            //Legendary Rarity
            var legendaryRandomChance = Random.Range(0, legendaryCards.Length - 1);
            return legendaryCards[Random.Range(0, legendaryCards.Length)];
        }
        else
        {
            //Should Never Be Called - But Function Needs A Default Return Type
            return commonCards[Random.Range(0, commonCards.Length)];
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
        print(cardsInCurrentHand);

        int rand = UnityEngine.Random.Range(0, cardsInCurrentHand);

        print(rand);
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
