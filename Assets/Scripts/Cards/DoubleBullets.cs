using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DoubleBullets : MonoBehaviour
{
    [Header("Private References")]
    private GameManager gameManager;
    private StatusDropdown statusDropdown;

    void Start()
    {
        gameManager = FindAnyObjectByType<GameManager>();
        statusDropdown = FindAnyObjectByType<StatusDropdown>();
    }

    public void PlayCardForPlayer()
    {
        float chance = gameManager.statusPercent;
        float roll = UnityEngine.Random.Range(0f, 100f);

        if (roll <= chance)
        {
            //loads blank (does nothing)
            statusDropdown.DisplayStatusEffect(0, 2);
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
            statusDropdown.DisplayStatusEffect(1, 2);
            return;
        }
        GameManager.Instance.addBullet();
    }
}
