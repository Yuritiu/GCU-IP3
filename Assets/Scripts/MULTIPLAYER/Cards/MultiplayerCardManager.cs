using System.Collections.Generic;
using UnityEngine;

public class MultiplayerCardManager : MonoBehaviour
{
    [Header("Deck")]
    [SerializeField] private List<CardType> baseDeck;

    public List<CardType> GetShuffledDeck()
    {
        List<CardType> deck = new List<CardType>(baseDeck);
        for (int i = 0; i < deck.Count; i++)
        {
            int j = Random.Range(i, deck.Count);
            CardType temp = deck[i];
            deck[i] = deck[j];
            deck[j] = temp;
        }
        return deck;
    }
}