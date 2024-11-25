using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Gun : MonoBehaviour
{
    public void PlayCardForPlayer()
    {
        GameManager.Instance.playerHasGun = true;

        GameManager.Instance.PlayerRoulette();
        GameManager.Instance.inGunAction = true;
    }

    public void CloneGunCardForPlayer()
    {
        GameManager.Instance.playerHasGun = true;

        int rand = UnityEngine.Random.Range(0, 5);
        //\/Debuging\/
        //rand = 0;
        //print(rand);
        if (rand == 0)
        {
            //Shoots off your own finger
            GameManager.Instance.ReduceHealth(2, 3);
            GameManager.Instance.gunBackfire.gameObject.SetActive(true);
        }
        else
        {
            GameManager.Instance.PlayerRoulette();
            GameManager.Instance.inGunAction = true;
        }
    }

    public void PlayCardForAI()
    {
        //GameManager.Instance.aiHasGun = true;   
        GameManager.Instance.AiRoulette();
        GameManager.Instance.aiHasGun = true;
    }

    IEnumerator WaitToStart(int character, int type)
    {
        //waits for cards to reveal
        yield return new WaitForSeconds(3f);
        StartCoroutine(GameManager.Instance.WaitToCompareCards(character, type));
    }
}
