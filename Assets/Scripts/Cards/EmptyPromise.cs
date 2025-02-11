using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class EmptyPromise : MonoBehaviour
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
            //Display BACKFIRE! Text
            TextMeshProUGUI backfireText = GetComponentInChildren<TextMeshProUGUI>();
            backfireText.enabled = true;

            //draw 2 cards
            GameManager.Instance.playerDraw2Cards = true;

            //GameManager.Instance.emptyPromiseBackfire.gameObject.SetActive(true);
            statusDropdown.DisplayStatusEffect(0, 6);
        }
        return;
    }
    public void PlayCardForAI()
    {
        float chance = gameManager.statusPercent;
        float roll = UnityEngine.Random.Range(0f, 100f);

        if (roll <= chance)
        {
            //Display BACKFIRE! Text
            TextMeshProUGUI backfireText = GetComponentInChildren<TextMeshProUGUI>();
            backfireText.enabled = true;

            //draw 2 cards
            GameManager.Instance.aiDraw2Cards = true;
            statusDropdown.DisplayStatusEffect(1, 6);
        }
        return;
    }

    //Side Effects To be added
}