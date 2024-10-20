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

    [HideInInspector] bool cardAdded = false;
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

   /* void Update()
    {
        Check For Left Mouse Click -- FIGURE OUT HOW TO GET AI TO PLAY
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            //Raycast To Mouse Position
            if (Physics.Raycast(ray, out hit))
            {
                //Check For A Card Collider
                if (hit.transform.CompareTag(cardTag))
                {
                    //Find The Index Of The Card In The cardsInHand Array
                    int cardIndex = System.Array.IndexOf(cardsInHand, hit.transform.gameObject);
                    Debug.Log("Card index: " + cardIndex);
                    //Check That There Is Cards And It's The Players Turn
                    if ((cardIndex >= 0 && cardIndex < originalPositions.Length) && isPlayersTurn)
                    {
                        //Toggle The Card's Selection State
                        ToggleCardSelection(cardIndex);
                    }
                }
                //Check For Opponent's Collider
                if (hit.transform.CompareTag(opponentTag))
                {
                    //Check If It's The Players Turn And Atleast 1 Card Is Selected
                    if (isPlayersTurn && selectedCardCount > 0)
                    {
                       //Play Selected Hand
                        GameManager.Instance.PlayHand();
                        //Handover Turn To AI
                        isPlayersTurn = false;
                    }
                }
            }
        }
    }
   */

    void StartGame()
    {
        //Initialize cardsInHand Array With 4 Slots
        cardsInHand = new GameObject[4];

        uncommonRarity += commonRarity;
        rareRarity += uncommonRarity;
        legendaryRarity += rareRarity;
        
        totalRarity = legendaryRarity;
        
        //Debug.Log("Total Rarity: " + totalRarity);

        //Add 4 Random Cards To The cardsInHand Array
        for (int i = 0; i < cardsInHand.Length; i++)
        {
            //Choose A Random Card Prefab
            GameObject card = GetRandomCard();
            //Instantiate And Store The Reference
            cardsInHand[i] = Instantiate(card, originalPositions[i].position, originalPositions[i].rotation);
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

    public Component SelectCard(int index)
    {

        if (cardsInHand[index] != null)
        {
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
            return cardsInHand[index].GetComponentAtIndex(1);
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
}
