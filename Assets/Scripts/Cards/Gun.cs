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
       

        //int randForBullet = UnityEngine.Random.Range(1, 7);
        //print("player " + randForBullet);

        //ShootScript.instance2.PRandom = randForBullet;
        
        //if (randForBullet <= GameManager.Instance.bullets)
        //{
        //    StartCoroutine(WaitToStart(1, 3));
        //}
        //else
        //{
        //    if(GameManager.Instance.bullets != 1)
        //    {
        //        GameManager.Instance.bullets--;
        //    }
        //}
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
