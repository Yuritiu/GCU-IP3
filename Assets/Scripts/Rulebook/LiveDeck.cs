using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LiveDeck : MonoBehaviour
{
    [Header("UI Card Prefab")]
    [SerializeField] private Sprite[] cardFaces = new Sprite[7];
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] public int cardCount = 52;

    [Header("Live Deck Grid")]
    [SerializeField] public LiveDeckGrid grid;
    [SerializeField] private Transform targetGrid;

    void Start()
    {
        SpawnCards();
    }

    void SpawnCards()
    {
        for (int i = 0; i < cardCount; i++)
        {
            GameObject newCard = Instantiate(cardPrefab, targetGrid);
            Image cardImage = newCard.GetComponent<Image>();

            if (cardImage != null && cardFaces.Length > 0)
            {
                Sprite selectedSprite = cardFaces[Random.Range(0, cardFaces.Length)];
                cardImage.sprite = selectedSprite;
                newCard.name = selectedSprite.name;
            }
        }

        grid.SortCards();
    }
}
