using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwingAwayCard : MonoBehaviour
{
    //Called From GameManager
    public void PlayCardForPlayer()
    {
        //Skip AI Turn
        GameManager.Instance.aiSkippedTurns ++;

        int rand = Random.Range(0, 5);
        //\/Debuging\/
        //rand = 0;
        //print(rand);
        if (rand == 0)
        {
            //shuffles cards and blurs them
            GameManager.Instance.blur.SetActive(true);
            CardDrawSystem.Instance.ShuffleHand();
            GameManager.Instance.batBackfire.gameObject.SetActive(true);
        }
    }
    public void PlayCardForAI()
    {
        //Skip Players Turn
        GameManager.Instance.playerSkippedTurns++;
    }
}
