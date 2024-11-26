using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Armour : MonoBehaviour
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
            // Slow bloodloss
            BloodlossSystem.Instance.bloodlossTime -= BloodlossSystem.Instance.shieldBloodlossReduce;
            GameManager.Instance.armourBackfire.gameObject.SetActive(true);
            GameManager.Instance.playerArmour++;
        }
        else
        {
            // Damage opponent 
            // Takes 1 finger away 
            GameManager.Instance.playerArmour++;
        }

    } 
           
    public void PlayCardForAI()
    {
        //Damage opponent 
        //takes 1 finger away 
        GameManager.Instance.aiArmour++;
    }
}
