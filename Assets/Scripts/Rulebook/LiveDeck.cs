using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LiveDeck : MonoBehaviour
{
    [Header("UI Card Prefab")]
    [SerializeField] private Sprite[] cardFaces = new Sprite[7];
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] public int cardCount = 53;

    [Header("Card Distribution")]
    [SerializeField] private int[] cardFaceCounts = new int[7]; // Defines how many times each sprite appears in the deck

    [Header("Live Deck Grid")]
    [SerializeField] public LiveDeckGrid grid;
    [SerializeField] private Transform targetGrid;

    void Start()
    {
        SpawnCards();
    }

    void SpawnCards()
    {
        List<Sprite> deck = new List<Sprite>();

        // Add sprites to the deck based on the specified counts
        for (int i = 0; i < cardFaces.Length; i++)
        {
            for (int j = 0; j < cardFaceCounts[i]; j++)
            {
                deck.Add(cardFaces[i]);
            }
        }

        // Shuffle the deck
        ShuffleDeck(deck);

        // Spawn the cards from the shuffled deck
        foreach (var cardSprite in deck)
        {
            GameObject newCard = Instantiate(cardPrefab, targetGrid);
            Image cardImage = newCard.GetComponent<Image>();

            if (cardImage != null)
            {
                cardImage.sprite = cardSprite;
                newCard.name = cardSprite.name; // Name the object after the sprite
            }
        }

        grid.SortCards(); // Sort the cards after spawning
    }

    // Shuffle the deck using Fisher-Yates algorithm
    void ShuffleDeck(List<Sprite> deck)
    {
        for (int i = deck.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            Sprite temp = deck[i];
            deck[i] = deck[j];
            deck[j] = temp;
        }
    }
}
