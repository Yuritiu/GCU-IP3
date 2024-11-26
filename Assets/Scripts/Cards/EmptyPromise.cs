using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EmptyPromise : MonoBehaviour
{
    private GameManager gameManager;

    void Start()
    {
        gameManager = FindAnyObjectByType<GameManager>();
    }

    public void PlayCardForPlayer()
    {
        float chance = gameManager.statusPercent;
        float roll = UnityEngine.Random.Range(0f, 100f);

        if (roll <= chance)
        {
            //draw 2 cards
            GameManager.Instance.playerDraw2Cards = true;
            GameManager.Instance.emptyPromiseBackfire.gameObject.SetActive(true);
        }
        return;
    }
    public void PlayCardForAI()
    {
        float chance = gameManager.statusPercent;
        float roll = UnityEngine.Random.Range(0f, 100f);

        if (roll <= chance)
        {
            //draw 2 cards
            GameManager.Instance.aiDraw2Cards = true;
        }
        return;
    }

    //Side Effects To be added
}