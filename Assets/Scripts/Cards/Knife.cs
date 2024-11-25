using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Knife : MonoBehaviour
{
    //Called From GameManager
    public void PlayCardForPlayer()
    {
        StartCoroutine(PlayPlayerKnife());
    }
    public void PlayCardForAI()
    {
        StartCoroutine(PlayAiKnife());
    }
    
    //ADDS a delay to the second knife to give time for the first knife to work first
    IEnumerator WaitToStart(int character, int type)
    {
        //waits for cards to reveal
        yield return new WaitForSeconds(1f);
        StartCoroutine(GameManager.Instance.WaitToCompareCards(character, type));
    }
    
    IEnumerator PlayAiKnife()
    {
        GameManager.Instance.inKnifeAction = true;
        yield return new WaitForSeconds(0.5f);

        //Check If Its The Tutorial First
        if (!GameManager.Instance.isTutorial)
        {
            int rand = Random.Range(0, 5);
            //\/Debuging\/
            //rand = 0;
            //print(rand);
            if (rand == 0)
            {
                //makes 1 card not usable for 1 turn
                AICardDrawSystem.Instance.StopOneCard();
            }

            if (AICardDrawSystem.Instance.selectedPosition1.childCount > 0)
            {
                //print("checking knife 1");
                if (AICardDrawSystem.Instance.selectedPosition1.GetChild(0).name.Contains("knife") && !GameManager.Instance.knife1used && GameManager.Instance.playerArmour == 0)
                {
                    GameManager.Instance.canCutFinger = true;
                    GameManager.Instance.knife1used = true;
                    GameManager.Instance.numberOfKnifeCards++;
                    StartCoroutine(GameManager.Instance.WaitToCompareCards(2, 1));
                }
            }
            if (AICardDrawSystem.Instance.selectedPosition2.childCount > 0)
            {
                //print("checking knife 2");
                if (AICardDrawSystem.Instance.selectedPosition2.GetChild(0).name.Contains("knife") && !GameManager.Instance.knife2used && GameManager.Instance.playerArmour == 0) 
                {
                    GameManager.Instance.canCutFinger = true;
                    GameManager.Instance.knife2used = true;
                    GameManager.Instance.numberOfKnifeCards++;
                    if (!GameManager.Instance.knife1used)
                    {
                        StartCoroutine(GameManager.Instance.WaitToCompareCards(2, 1));
                    }
                }
            }
        }
    }

    IEnumerator PlayPlayerKnife()
    {
        yield return new WaitForSeconds(0.5f);

        //Check If Its The Tutorial First
        if (!GameManager.Instance.isTutorial)
        {
            int rand = Random.Range(0, 5);
            //\/Debuging\/
            //rand = 0;
            //print(rand);
            if (rand == 0)
            {
                //makes 1 card not usable for 1 turn
                CardDrawSystem.Instance.StopOneCard();
                GameManager.Instance.knifeBackfire.gameObject.SetActive(true);
            }

            if (CardDrawSystem.Instance.selectedPosition1.childCount > 0 && CardDrawSystem.Instance.selectedPosition2.childCount > 0)
            {
                //print("made it");
                Component card1 = CardDrawSystem.Instance.selectedPosition1.GetChild(0);
                Component card2 = CardDrawSystem.Instance.selectedPosition2.GetChild(0);
                //Damage opponent 
                //takes 1 finger away
                if (card1.gameObject.name.Contains("Knife") && card2.gameObject.name.Contains("Knife"))
                {
                    if (card2.gameObject == this.gameObject)
                    {
                        StartCoroutine(WaitToStart(1, 1));
                        yield return null;
                    }
                }
            }
            StartCoroutine(GameManager.Instance.WaitToCompareCards(1, 1));
        }
        else
        {
            if (TutorialCardDraw.Instance.selectedPosition1.childCount > 0 && TutorialCardDraw.Instance.selectedPosition2.childCount > 0)
            {
                //print("made it");
                Component card1 = TutorialCardDraw.Instance.selectedPosition1.GetChild(0);
                Component card2 = TutorialCardDraw.Instance.selectedPosition2.GetChild(0);
                //Damage opponent 
                //takes 1 finger away
                if (card1.gameObject.name.Contains("Knife") && card2.gameObject.name.Contains("Knife"))
                {
                    if (card2.gameObject == this.gameObject)
                    {
                        StartCoroutine(WaitToStart(1, 1));
                        yield return null;
                    }
                }
            }
            StartCoroutine(GameManager.Instance.WaitToCompareCards(1, 1));
        }
    }
}
