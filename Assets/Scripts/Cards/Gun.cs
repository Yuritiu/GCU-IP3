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
        RandomNumber = rnd.Next(1, 6);
        print(RandomNumber);
        GameManager.Instance.PlayerRoulette();

        if(RandomNumber == 1)
        {
            
            StartCoroutine(WaitToStart(1, 1));
            StartCoroutine(GameManager.Instance.WaitToCompareCards(1, 1));
            GameManager.Instance.EndGameWin();
        }

        if(RandomNumber == 2 && GameManager.Instance.bullets == 2)
        {
            StartCoroutine(WaitToStart(1, 1));
            StartCoroutine(GameManager.Instance.WaitToCompareCards(1, 1));
            GameManager.Instance.EndGameWin();
            GameManager.Instance.bullets = 1;
        }
    }

    public void PlayCardForAI()
    {
        RandomNumber = rnd.Next(1, 6);
        print(RandomNumber);
        GameManager.Instance.AiRoulette();

        if (RandomNumber == 1)
        {
            StartCoroutine(WaitToStart(2, 1));
            StartCoroutine(GameManager.Instance.WaitToCompareCards(1, 1));
            GameManager.Instance.EndGameLose();
        }

        if (RandomNumber == 2 && GameManager.Instance.bullets == 2)
        {
            StartCoroutine(WaitToStart(2, 1));
            StartCoroutine(GameManager.Instance.WaitToCompareCards(1, 1));
            GameManager.Instance.EndGameLose();
            GameManager.Instance.bullets = 1;
        }
    }


    IEnumerator WaitToStart(int character, int type)
    {
        //waits for cards to reveal
        yield return new WaitForSeconds(3f);
        StartCoroutine(GameManager.Instance.WaitToCompareCards(character, type));
    }
}
