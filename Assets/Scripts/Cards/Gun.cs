using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Gun : MonoBehaviour
{
    public void PlayCardForPlayer()
    {
        GameManager.Instance.inGunAction = true;

        GameManager.Instance.playerHasGun = true;

        GameManager.Instance.PlayerRoulette();
    }

    public void CloneGunCardForPlayer()
    {
        GameManager.Instance.inGunAction = true;

        GameManager.Instance.playerHasGun = true;

        GameManager.Instance.PlayerRoulette();
    }

    public void PlayCardForAI()
    {
        GameManager.Instance.inGunAction = true;

        GameManager.Instance.aiHasGun = true;

        GameManager.Instance.AiRoulette();
    }

    public void CloneGunCardForAI()
    {
        GameManager.Instance.inGunAction = true;

        GameManager.Instance.aiHasGun = true;

        GameManager.Instance.AiRoulette();
    }

    IEnumerator WaitToStart(int character, int type)
    {
        //waits for cards to reveal
        yield return new WaitForSeconds(3f);
        StartCoroutine(GameManager.Instance.WaitToCompareCards(character, type));
    }
}
