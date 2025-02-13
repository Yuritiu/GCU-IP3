using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiveDeckGrid : MonoBehaviour
{
    [Header("Custom Order String")]

    [SerializeField] private string[] customOrder = { "Knife", "Gun", "Chamber", "Armor", "Cigar", "Bottle", "Promise" };

    void Start()
    {
        SortCards();
    }

    public void SortCards()
    {
        List<Transform> cards = new List<Transform>();

        foreach (Transform card in transform)
        {
            cards.Add(card);
        }

        cards.Sort((a, b) => GetCustomOrderIndex(a.name).CompareTo(GetCustomOrderIndex(b.name)));

        for (int i = 0; i < cards.Count; i++)
        {
            cards[i].SetSiblingIndex(i);
        }
    }

    private int GetCustomOrderIndex(string cardName)
    {
        for (int i = 0; i < customOrder.Length; i++)
        {
            if (cardName.Contains(customOrder[i]))
            {
                return i;
            }
        }

        return customOrder.Length;
    }
}
