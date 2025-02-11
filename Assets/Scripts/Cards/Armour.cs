using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

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

        GameManager.Instance.playerArmour++;

        if (roll <= chance)
        {
            //Display BACKFIRE! Text
            TextMeshProUGUI backfireText = GetComponentInChildren<TextMeshProUGUI>();
            backfireText.enabled = true;

            // Slow bloodloss
            BloodlossSystem.Instance.bloodlossTime -= BloodlossSystem.Instance.shieldBloodlossReduce;

            //GameManager.Instance.armourBackfire.gameObject.SetActive(true);
            statusDropdown.DisplayStatusEffect(0, 3);
        }
    } 
           
    public void PlayCardForAI()
    {
        GameManager.Instance.aiArmour++;
    }
}
