using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Gun : MonoBehaviour
{

    public void PlayCardForPlayer()
    {
        //GameManager.Instance.playerHasGun = true;
        int rand = UnityEngine.Random.Range(0, 5);
        //\/Debuging\/
        //rand = 0;
        //print(rand);
        if (rand == 0)
        {
            //Shoots off your own finger
            GameManager.Instance.ReduceHealth(2);
            GameManager.Instance.gunBackfire.gameObject.SetActive(true);
        }

        int randForBullet = UnityEngine.Random.Range(1, 7);
        //print(randForBullet);
        GameManager.Instance.PlayerRoulette();
        if (randForBullet <= GameManager.Instance.bullets)
        {
            StartCoroutine(WaitToStart(1, 3));
        }
        else
        {
            if(GameManager.Instance.bullets != 1)
            {
                GameManager.Instance.bullets--;
            }
        }
    }

    public void PlayCardForAI()
    {
        //GameManager.Instance.aiHasGun = true;   
        GameManager.Instance.AiRoulette();

        int rand = UnityEngine.Random.Range(0, 5);
        //\/Debuging\/
        //rand = 0;
        //print(rand);
        if (rand == 0)
        {
            //Shoots off your own finger
            GameManager.Instance.ReduceHealth(1);
        }
        int randForBullet = UnityEngine.Random.Range(1, 7);
        if (randForBullet >= GameManager.Instance.bullets)
        {
            StartCoroutine(WaitToStart(2, 3));
        }
    }


    IEnumerator WaitToStart(int character, int type)
    {
        //waits for cards to reveal
        yield return new WaitForSeconds(3f);
        StartCoroutine(GameManager.Instance.WaitToCompareCards(character, type));
    }

    IEnumerator WaitForGun()
    {
        yield return new WaitForSeconds(2f);
    }
}
