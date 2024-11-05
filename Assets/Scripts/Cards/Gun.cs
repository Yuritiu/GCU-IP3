using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Gun : MonoBehaviour
{
    System.Random rnd = new System.Random();
    int RandomNumber;

    public void PlayCardForPlayer()
    {
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
        RandomNumber = rnd.Next(1, 6);
        //print(RandomNumber);
        GameManager.Instance.PlayerRoulette();

        if(RandomNumber >= GameManager.Instance.bullets)
        {
            
            StartCoroutine(WaitToStart(1, 1));
            StartCoroutine(GameManager.Instance.WaitToCompareCards(1, 1));
            GameManager.Instance.EndGameWin();
        }
    }

    public void PlayCardForAI()
    {
        RandomNumber = rnd.Next(1, 6);
        print(RandomNumber);
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
        if (RandomNumber >= GameManager.Instance.bullets)
        {
            StartCoroutine(WaitToStart(2, 1));
            StartCoroutine(GameManager.Instance.WaitToCompareCards(1, 1));
            GameManager.Instance.EndGameLose();
        }
    }


    IEnumerator WaitToStart(int character, int type)
    {
        //waits for cards to reveal
        yield return new WaitForSeconds(3f);
        StartCoroutine(GameManager.Instance.WaitToCompareCards(character, type));
    }
}
