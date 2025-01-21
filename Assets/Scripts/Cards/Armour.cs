using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Armour : MonoBehaviour
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
            // Slow bloodloss
            BloodlossSystem.Instance.bloodlossTime -= BloodlossSystem.Instance.shieldBloodlossReduce;
            GameManager.Instance.playerArmour++;

            //GameManager.Instance.armourBackfire.gameObject.SetActive(true);
            statusDropdown.DisplayStatusEffect(0, 3);
        }
        else
        {
            GameManager.Instance.playerArmour++;
        }

    } 
           
    public void PlayCardForAI()
    {
        GameManager.Instance.aiArmour++;
    }
}
