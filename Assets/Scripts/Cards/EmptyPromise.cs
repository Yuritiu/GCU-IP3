using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmptyPromise : MonoBehaviour
{
    public void PlayCardForPlayer()
    {
        //Places Card But Does Nothing
        int rand = Random.Range(0, 5);
        //\/Debuging\/
        //rand = 0;
        if (rand == 0)
        {
            //draw 2 cards
            GameManager.Instance.playerDraw2Cards = true;
            GameManager.Instance.backfire.gameObject.SetActive(true);
        }
        return;
    }
    public void PlayCardForAI()
    {
        //Places AI's Card But Does Nothing
        int rand = Random.Range(0, 5);
        if (rand == 0)
        {
            //draw 2 cards
            GameManager.Instance.aiDraw2Cards = true;
        }
        return;
    }

    //Side Effects To be added
}