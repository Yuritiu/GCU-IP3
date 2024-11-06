using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEngine.UI.Image;

public class CardDrawSystem : MonoBehaviour
{
    //!-Coded By Charlie-!

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


    [Header("Cards that cannot be used")]
    private int bannedCard = -1;
    private int bannedCard2 = -1;

    //Current Number Of Selected Cards - Max Of 2
    public int selectedCardCount = 0;

    //Hidden Variables
    [HideInInspector] public bool isPlayersTurn = true;
    [HideInInspector] bool cardAdded = false;
    [HideInInspector] public bool canPlay = true;
    [HideInInspector] CardSelection cardSelection;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        StartGame();
    }

    void Update()
    {
        //Ensure That Nothing Can Be Interacted With When The Settings Menu Is Open
        if (SettingsMenu.Instance.settingsMenuOpen)
        {
            //Check If Null, Else Null Reference Errors Will Not Stop
            if (cardSelection != null)
            {
                //Stop The Hovering Card Function Or Else It Gets Stuck On Screen When The Settings Menu Closes
                cardSelection.CardHovered(false);
            }

            return;
        }
        
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
                    if (Input.GetMouseButtonDown(0) && (cardIndex != bannedCard) && (cardIndex != bannedCard2))
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
                        GameManager.Instance.PlayHand();
                    }
                }
            }
        }
    }

    void StartGame()
    {
        //Initialize cardsInHand Array With 4 Slots
        cardsInHand = new GameObject[4];

        uncommonRarity += commonRarity;
        rareRarity += uncommonRarity;
        legendaryRarity += rareRarity;

        //+5 = 5% Chance For Legendary
        totalRarity = legendaryRarity + 5;

        //Add 4 Random Cards To The cardsInHand Array
        for (int i = 0; i < cardsInHand.Length; i++)
        {
            //Choose A Random Card Prefab
            GameObject card = GetRandomCard();
            //Instantiate And Store The Reference
            cardsInHand[i] = Instantiate(card, originalPositions[i].position, originalPositions[i].rotation);
            //updates what cards are banned
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

    public void AddCardAfterTurn()
    {
        GameObject card = GetRandomCard();
        //Add 1 Random Card Prefab After A Turn
        for (int i = 0; i < cardsInHand.Length; i++)
        {
            //Check If There Is An Available Slot
            if (cardsInHand[i] == null)
            {
                //updates what cards are banned
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
                cardsInHand[i] = Instantiate(card, originalPositions[i].position, originalPositions[i].rotation);
                //Mark The Card As Added
                cardAdded = true;
                //Exit When Card Is Placed
                break;
            }
        }
    }

    public GameObject GetRandomCard()
    {
        //Randomly Select A Card Based On Rarity Chance
        var randomChance = UnityEngine.Random.Range(0, totalRarity);

        if (randomChance <= commonRarity)
        {
            //Common Rarity
            var commonRandomChance = UnityEngine.Random.Range(0, commonCards.Length - 1);
            return commonCards[UnityEngine.Random.Range(0, commonCards.Length)];
        }
        else if (randomChance > commonRarity && randomChance <= uncommonRarity)
        {
            //Uncommon Rarity
            var uncommonRandomChance = UnityEngine.Random.Range(0, uncommonCards.Length - 1);
            return uncommonCards[UnityEngine.Random.Range(0, uncommonCards.Length)];
        }
        else if (randomChance > uncommonRarity && randomChance <= rareRarity)
        {
            //Rare Rarity
            var rareRandomChance = UnityEngine.Random.Range(0, rareCards.Length - 1);
            return rareCards[UnityEngine.Random.Range(0, rareCards.Length)];
        }
        else if (randomChance > rareRarity && randomChance <= legendaryRarity)
        {
            //Legendary Rarity
            var legendaryRandomChance = UnityEngine.Random.Range(0, legendaryCards.Length - 1);
            return legendaryCards[UnityEngine.Random.Range(0, legendaryCards.Length)];
        }
        else
        {
            //Should Never Be Called - But Function Needs A Default Return Type
            return commonCards[UnityEngine.Random.Range(0, commonCards.Length)];
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
                MoveCardToPosition(index, selectedPosition1);
            }
            else if (selectedCardCount == 1 && (selectedPosition2.childCount <= 0))
            {
                //Resize Card
                cardsInHand[index].gameObject.transform.rotation = selectedPosition2.transform.rotation;
                //Move To Selected Position 2
                MoveCardToPosition(index, selectedPosition2);
            }
            switch (index)
            {
                //updates what cards are banned
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
    }

    void MoveCardToPosition(int index, Transform selectedPosition)
    {
        //Move The Card To The Selected Position
        cardsInHand[index].transform.position = selectedPosition.position;
        //Set Parent Else It Doesn't Return To The Original Position
        cardsInHand[index].transform.SetParent(selectedPosition);
        selectedCardCount = CheckSelectedCards();
    }

    void DeselectCard(int index)
    {
        //Reset The Parent To Null
        cardsInHand[index].transform.SetParent(null);

        //Move The Card Back To It's Original Position And Rotation
        cardsInHand[index].transform.position = originalPositions[index].position;
        cardsInHand[index].transform.rotation = originalPositions[index].rotation;

        selectedCardCount = CheckSelectedCards();

        //updates what cards are banned
        switch (index)
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
}
