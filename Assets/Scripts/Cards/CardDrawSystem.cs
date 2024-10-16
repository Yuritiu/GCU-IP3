using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardDrawSystem : MonoBehaviour
{
    //!-Coded By Charlie-!

    [Header("Card Variables")]
    //Tag For The Card Objects
    [SerializeField] private string cardTag = "Card";

    [Header("Card Slot References")]
    //Original Positions For The Cards
    [SerializeField] private Transform[] originalPositions;
    //Array For Actual Card GameObjects
    [SerializeField] private GameObject[] cardObjects;
    [SerializeField] private Transform selectedPosition1;
    [SerializeField] private Transform selectedPosition2;

    //Current Number Of Selected Cards - Max Of 2
    private int selectedCardCount = 0;

    void Update()
    {
        //Check For Left Mouse Click
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
                    //Find The Index Of The Card In The cardObjects Array
                    int cardIndex = System.Array.IndexOf(cardObjects, hit.transform.gameObject);
                    if (cardIndex >= 0 && cardIndex < originalPositions.Length)
                    {
                        //Toggle The Card's Selection State
                        ToggleCardSelection(cardIndex);
                    }
                }
            }
        }
    }

    void ToggleCardSelection(int index)
    {
        //Check If The Card Is Currently In The Selected Position
        bool isSelected = IsCardInSelectedPosition(cardObjects[index]);

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
        //Move The Card To The First Available Selected Position, Check If Position 2 Is Populated So That It Doesn't Fill It
        if ((selectedCardCount == 0) || (selectedCardCount == 1 && selectedPosition2.childCount >= 1))
        {
            //Move To Selected Position 1
            MoveCardToPosition(index, selectedPosition1);
        }
        else if (selectedCardCount == 1 && (selectedPosition2.childCount <= 0))
        {
            //Move To Selected Position 2
            MoveCardToPosition(index, selectedPosition2);
        }
    }

    void MoveCardToPosition(int index, Transform selectedPosition)
    {
        //Move The Card To The Selected Position
        cardObjects[index].transform.position = selectedPosition.position;
        //Set Parent Else It Doesn't Return To The Original Position
        cardObjects[index].transform.SetParent(selectedPosition);
        selectedCardCount++;
    }

    void DeselectCard(int index)
    {
        //Move The Card Back To It's Original Position
        cardObjects[index].transform.position = originalPositions[index].position;
        //Transform currentParent = cardObjects[index].transform.parent;

        //Check If The Current Parent Is A Selected Position
        //if (currentParent != null && (currentParent == selectedPosition1 || currentParent == selectedPosition2))
        //{
            //Reset The Parent To Null
            cardObjects[index].transform.SetParent(null);
            selectedCardCount--;
        //}
    }

    bool IsCardInSelectedPosition(GameObject card)
    {
        //Check If The Card Is Currently In One Of The Selected Positions
        return card.transform.parent != null && (card.transform.parent == selectedPosition1 || card.transform.parent == selectedPosition2);
    }
}
