using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LiveDeckGrid : MonoBehaviour
{
    [Header("Custom Order String")]
    [SerializeField] private string[] customOrder = { "Knife", "Gun", "Chamber", "Armor", "Cigar", "Bottle", "Promise" };
    [SerializeField] private string[] matReceiver = { "KnifeMat", "GunMat", "ChamberMat", "ArmorMat", "CigarMat", "BottleMat", "PromiseMat" };

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

    public void CheckMaterialMatch(string materialName)
    {
        for (int i = 0; i < matReceiver.Length; i++)
        {
            if (matReceiver[i].Equals(materialName))
            {
                Debug.Log("Match found: " + materialName);

                //Remove "Mat" from the material name to get the card name
                string cardName = materialName.Replace("Mat", "");
                Debug.Log("Looking for card: " + cardName);
                FindAndSetCardOpacity(cardName);
                return;
            }
        }
        Debug.Log("No match found for: " + materialName);
    }

    private void FindAndSetCardOpacity(string cardName)
    {
        //Debug.Log("Searching for card with name: " + cardName);
        bool opacityChanged = false;

        foreach (Transform card in transform)
        {
            //Debug.Log("Checking card: " + card.name);
            if (card.name.Contains(cardName))
            {
                //Find image comp on object
                Image image = card.GetComponent<Image>();
                if (image != null)
                {
                    //Check if already has opacity
                    Color currentColor = image.color;
                    if (currentColor.a != 0.25f)
                    {
                        currentColor.a = 0.25f;
                        image.color = currentColor;
                        //Debug.Log($"Set opacity of {cardName} to 0.25");
                        opacityChanged = true;
                        break; //Exit after change
                    }
                    else
                    {
                        Debug.Log($"Card {cardName} already has 0.25 opacity, skipping.");
                    }
                }
                else
                {
                    Debug.LogWarning("No Image component found on card: " + card.name);
                }
            }

            //if the card is already changed break
            if (opacityChanged)
            {
                break;
            }
        }

        if (!opacityChanged)
        {
            Debug.Log("No matching card found for " + cardName);
        }
    }

}
