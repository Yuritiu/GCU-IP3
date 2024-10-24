using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEngine.UI.Image;

public class TutorialCardDraw : MonoBehaviour
{
    //!-Coded By Charlie-!

    public static TutorialCardDraw Instance;

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

    [HideInInspector]
    //Current Number Of Selected Cards - Max Of 2
    public int selectedCardCount = 0;
    int cardsNumber = 0;

    //Hidden Variables
    [HideInInspector] public bool isPlayersTurn = true;
    [HideInInspector] bool cardAdded = false;
    [HideInInspector] public bool canPlay = true;
    [HideInInspector] public bool canSelectCards = true;
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
                    if (cardsInHand == null)
                        return;

                    //Find The Index Of The Card In The cardsInHand Array
                    int cardIndex = System.Array.IndexOf(cardsInHand, hit.transform.gameObject);

                    //Check For Left Mouse Click
                    if (Input.GetMouseButtonDown(0) && canSelectCards)
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
                if (Input.GetMouseButtonDown(0) && canPlay && TutorialManager.Instance.tutorialPhase == 1)
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

        //Add 4 Random Cards To The cardsInHand Array
        for (int i = 0; i < cardsInHand.Length; i++)
        {
            //Choose A Random Card Prefab
            GameObject card = GetRandomCard();
            //Instantiate And Store The Reference
            cardsInHand[i] = Instantiate(card, originalPositions[i].position, originalPositions[i].rotation);
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
        cardsNumber++;

        if (cardsNumber <= 1)
        {
            //Give 2 Knife Cards @ Start Of Tutorial
            return knifeCard[Random.Range(0, knifeCard.Length)];
        }
        else if (cardsNumber > 1 && cardsNumber <= 3)
        {
            //Give 2 Armour Cards @ Start Of Tutorial
            return armourCard[Random.Range(0, armourCard.Length)];
        }
        //else if (randomChance > uncommonRarity && randomChance <= rareRarity)
        //{
        //    //Rare Rarity
        //    var rareRandomChance = Random.Range(0, rareCards.Length - 1);
        //    return rareCards[Random.Range(0, rareCards.Length)];
        //}
        //else if (randomChance > rareRarity && randomChance <= legendaryRarity)
        //{
        //    //Legendary Rarity
        //    var legendaryRandomChance = Random.Range(0, legendaryCards.Length - 1);
        //    return legendaryCards[Random.Range(0, legendaryCards.Length)];
        //}
        else
        {
            //Should Never Be Called - But Function Needs A Default Return Type
            return knifeCard[Random.Range(0, knifeCard.Length)];
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
        }
    }

    void MoveCardToPosition(int index, Transform selectedPosition)
    {
        //Move The Card To The Selected Position
        cardsInHand[index].transform.position = selectedPosition.position;
        //Set Parent Else It Doesn't Return To The Original Position
        cardsInHand[index].transform.SetParent(selectedPosition);
        selectedCardCount++;
    }

    void DeselectCard(int index)
    {
        //Reset The Parent To Null
        cardsInHand[index].transform.SetParent(null);

        //Move The Card Back To It's Original Position And Rotation
        cardsInHand[index].transform.position = originalPositions[index].position;
        cardsInHand[index].transform.rotation = originalPositions[index].rotation;

        selectedCardCount--;
    }

    bool IsCardInSelectedPosition(GameObject card)
    {
        //Check If The Card Is Currently In One Of The Selected Positions
        return card.transform.parent != null && (card.transform.parent == selectedPosition1 || card.transform.parent == selectedPosition2);
    }
}
