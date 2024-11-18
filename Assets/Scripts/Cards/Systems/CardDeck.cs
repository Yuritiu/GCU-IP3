using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] private List<CardPrefab> cardPrefabs;
    [SerializeField] public List<GameObject> deck = new List<GameObject>();

    [Header("Card Piles")]
    [SerializeField] GameObject cardPrefab;
    //[SerializeField] public Transform deckPosition;
    //[SerializeField] public Transform discardPilePosition;

    bool reshuffling = false;

    void Awake()
    {
        Instance = this;

        if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        //DontDestroyOnLoad(gameObject);

        InitializeDeck();
    }

    void Update()
    {
        if (deck.Count == 0 && !reshuffling)
        {
            reshuffling = true;
            ReshuffleDeck();
        }
    }

    void InitializeDeck()
    {
        Debug.Log("Card Deck Initialized");

        deck.Clear();
        foreach (CardPrefab card in cardPrefabs)
        {
            for (int i = 0; i < card.quantity; i++)
            {
                if (card.cardPrefab != null)
                {
                    deck.Add(card.cardPrefab);
                }
            }
        }

        ShuffleDeck();
        Debug.Log("Deck Created With " + deck.Count + " Cards.");
    }

    void ShuffleDeck()
    {
        Debug.Log("Deck Shuffling...");
        for (int i = 0; i < deck.Count; i++)
        {
            int randomIndex = Random.Range(0, deck.Count);
            GameObject temp = deck[i];
            deck[i] = deck[randomIndex];
            deck[randomIndex] = temp;
        }
        Debug.Log("Deck Shuffled.");
    }

    void ReshuffleDeck()
    {
        InitializeDeck();
        Debug.Log("Deck Reshuffled. New deck count: " + deck.Count);
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
            Debug.LogError("Drawn Card Is Null.");
            return null;
        }

        //Remove The Drawn Card From The Deck
        deck.RemoveAt(0);
        Debug.Log($"Drew Card: " +  card.name + " Remaining Cards In Deck: " + deck.Count);
        return card;
    }

    public void DiscardCard(GameObject card)
    {
        if (card != null)
        {
            Destroy(card);
        }
    }
}
