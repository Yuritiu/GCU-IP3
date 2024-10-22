using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knife : MonoBehaviour
{
    //Called From GameManager



    public void PlayCardForPlayer()
    {
        if (CardDrawSystem.Instance.selectedPosition1.childCount > 0 && CardDrawSystem.Instance.selectedPosition2.childCount > 0)
        {
            print("made it");
            Component card1 = CardDrawSystem.Instance.selectedPosition1.GetChild(0);
            Component card2 = CardDrawSystem.Instance.selectedPosition2.GetChild(0);
            //Damage opponent 
            //takes 1 finger away
            if (card1.gameObject.name.Contains("Knife") && card2.gameObject.name.Contains("Knife"))
            {
                if (card2.gameObject == this.gameObject)
                {
                    StartCoroutine(WaitToStart(1, 1));
                }
                else
                {
                    StartCoroutine(GameManager.Instance.WaitToCompareCards(1, 1));
                }
            }
            else
            {
                StartCoroutine(GameManager.Instance.WaitToCompareCards(1, 1));
            }
        }
        else
        {
            StartCoroutine(GameManager.Instance.WaitToCompareCards(1, 1));
        }
    }
    public void PlayCardForAI()
    {

        if (AICardDrawSystem.Instance.selectedPosition1.childCount > 0 && AICardDrawSystem.Instance.selectedPosition2.childCount > 0)
        {
            Component card3 = AICardDrawSystem.Instance.selectedPosition1.GetChild(0);
            Component card4 = AICardDrawSystem.Instance.selectedPosition2.GetChild(0);


            //Damage opponent 
            //takes 1 finger away
            if (card3.gameObject.name.Contains("Knife") && card4.gameObject.name.Contains("Knife"))
            {
                if (card4.gameObject == this.gameObject)
                {
                    StartCoroutine(WaitToStart(2, 1));
                }
                else
                {
                    StartCoroutine(GameManager.Instance.WaitToCompareCards(2, 1));
                }
            }
            else
            {
                StartCoroutine(GameManager.Instance.WaitToCompareCards(2, 1));
            }
        }
        else
        {
            StartCoroutine(GameManager.Instance.WaitToCompareCards(2, 1));
        }
    }

    //ADDS a delay to the second knife to give time for the first knife to work first
    IEnumerator WaitToStart(int character, int type)
    {
        //waits for cards to reveal
        yield return new WaitForSeconds(1f);
        StartCoroutine(GameManager.Instance.WaitToCompareCards(character, type));
    }
}
