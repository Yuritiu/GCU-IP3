using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CigarCard : MonoBehaviour
{
    //!- Coded By Charlie -!

    //Called From GameManager
    public void PlayCardForPlayer()
    {
        //Clone Players Second Card
        StartCoroutine(GameManager.Instance.WaitToCompareCards(1, 2));

        int rand = Random.Range(0, 5);
        //\/Debuging\/
        //rand = 0;
        //print(rand);
        if (rand == 0)
        {
            
            //skips players next turn
            GameManager.Instance.playerSkippedTurns++;
            GameManager.Instance.cigarBackfire.gameObject.SetActive(true);
        }

    }
    public void PlayCardForAI()
    {
        //Clone AI's Second Card
        StartCoroutine(GameManager.Instance.WaitToCompareCards(2, 2));
       
        int rand = Random.Range(0, 5);
        //\/Debuging\/
        //rand = 0;
        //print(rand);
        if (rand == 0)
        {
            //skips players next turn
            GameManager.Instance.aiSkippedTurns++;
        }
    }
}
