using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DoubleBullets : MonoBehaviour
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
            //loads blank (does nothing)
            GameManager.Instance.twoInChamberBackfire.gameObject.SetActive(true);
            return;
        }
        GameManager.Instance.addBullet();
    }

    public void PlayCardForAI()
    {
        float chance = gameManager.statusPercent;
        float roll = UnityEngine.Random.Range(0f, 100f);

        if (roll <= chance)
        {
            //loads blank (does nothing)
            return;
        }
        GameManager.Instance.addBullet();
    }
}
