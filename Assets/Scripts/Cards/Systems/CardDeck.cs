using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CardDeck : MonoBehaviour
{
    //!- Coded By Charlie -!
    public static CardDeck Instance;

    [System.Serializable]
    public class CardPrefab
    {
        [SerializeField] public GameObject cardPrefab;
        [SerializeField] public int quantity;
    }

    [Header("All Card Prefabs")]
    [SerializeField] List<CardPrefab> cardPrefabs;
    [SerializeField] public List<GameObject> deck = new List<GameObject>();

    [Header("Playing Card Pile")]
    [SerializeField] GameObject emptyCardPrefab;
    [SerializeField] Transform deckPosition;
    //Distance Between Cards
    [SerializeField] float cardStackOffset = 0.002f;
    [SerializeField] float currentDeckStackHeight;

    [Header("Card Count Display")]
    private TextMeshProUGUI cardCount;
    [SerializeField] Camera playerCamera;

    private List<GameObject> visualDeck = new List<GameObject>();

    private bool reshuffling;

    private void Start()
    {
        cardCount = GameObject.Find("CardCount").GetComponent<TextMeshProUGUI>();
        UpdateCardCount();
    }
    void Awake()
    {
        Instance = this;

        if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        InitializeDeck();
    }


    void Update()
    {
        if (deck.Count == 0 && !reshuffling)
        {
            reshuffling = true;
            GameManager.Instance.Showdown();
        }

        if (cardCount != null && playerCamera != null)
        {
            cardCount.transform.LookAt(playerCamera.transform);
            cardCount.transform.Rotate(0, 180, 0);
        }
    }

    void InitializeDeck()
    {
        if (deck.Count == 0 && reshuffling == true)
        {
            GameManager.Instance.Showdown();
        }

        deck.Clear();
        foreach (CardPrefab card in cardPrefabs)
        {
            for (int i = 0; i < card.quantity; i++)
            {
                if (card != null)
                {
                    //For Each Card In The Deck Instantiate A Visual Card Prefab At The Proper Position
                    Vector3 playingDeckStackPosition = deckPosition.position + new Vector3(90, currentDeckStackHeight + cardStackOffset, 0);
                    GameObject emptyCard = Instantiate(emptyCardPrefab, playingDeckStackPosition, Quaternion.identity);

                    emptyCard.transform.localPosition = new Vector3(deckPosition.position.x, playingDeckStackPosition.y, deckPosition.position.z);
                    emptyCard.transform.localRotation = Quaternion.Euler(90, 0, 0);

                    // Add to the list of visual cards
                    visualDeck.Add(emptyCard);

                    // Update the stack height
                    currentDeckStackHeight += cardStackOffset;

                    deck.Add(card.cardPrefab);
                    UpdateCardCount();
                }
            }
        }

        ShuffleDeck();
        UpdateCardCount();
        //Debug.Log("Deck Created With " + deck.Count + " Cards.");
    }

    void ShuffleDeck()
    {
        //Debug.Log("Deck Shuffling...");
        for (int i = 0; i < deck.Count; i++)
        {
            int randomIndex = Random.Range(0, deck.Count);
            GameObject temp = deck[i];
            deck[i] = deck[randomIndex];
            deck[randomIndex] = temp;
        }
        //Debug.Log("Deck Shuffled.");
    }

    void ReshuffleDeck()
    {
        InitializeDeck();
        //Debug.Log("Deck Reshuffled. New deck count: " + deck.Count);
        //Might Be A Problem Just Adding A Card
        DrawCard();
    }

    public GameObject DrawCard()
    {
        if (deck.Count == 0)
        {
            ReshuffleDeck();
        }

        //Get First Card From The Deck
        GameObject card = deck[0];

        if (card == null)
        {
            //Debug.LogError("Drawn Card Is Null.");
            return null;
        }

        //Remove The Drawn Card From The Deck
        deck.RemoveAt(0);
        RemoveCards(1);
        UpdateCardCount();

        return card;
    }

    void RemoveCards(int numberOfCardsToRemove)
    {
        //Ensure The Number To Remove Doesn't Exceed The Deck Size
        int cardsToRemove = Mathf.Min(numberOfCardsToRemove, deck.Count);

        for (int i = 0; i < cardsToRemove; i++)
        {
            //Get The Top Visual Card Prefab From Playing Deck
            GameObject visualCard = visualDeck[visualDeck.Count - 1];

            Destroy(visualCard);

            //Remove The Destroyed Card From The List Of Visual Cards
            visualDeck.RemoveAt(visualDeck.Count - 1);
        }
    }

    void UpdateCardCount()
    {
        if (cardCount != null)
        {
            cardCount.text = visualDeck.Count.ToString();
        }
    }
}
