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

    private BackfireGlow backfireGlow;

    void Start()
    {
        gameManager = FindAnyObjectByType<GameManager>();
        statusDropdown = FindAnyObjectByType<StatusDropdown>();

        if (backfireGlow == null)
        {
            backfireGlow = GetComponent<BackfireGlow>();
        }

    }

    public void PlayCardForPlayer()
    {
        //Save To Stat Tracker
        StatTracker.Instance.UpdateStat("EmptyPromiseCardsPlayed", 1);

        DataGathering dG = FindObjectOfType<DataGathering>();
        dG.emptyUsed = dG.emptyUsed + 1;

        float chance = gameManager.statusPercent;
        float roll = UnityEngine.Random.Range(0f, 100f);
        
        if (roll <= chance)
        {
            dG.emptyBackfire = dG.emptyBackfire + 1;


            //Display BACKFIRE! Text
            TextMeshProUGUI backfireText = GetComponentInChildren<TextMeshProUGUI>();
            backfireText.enabled = true;
            if (backfireGlow != null)
            {
                backfireGlow.GlowActive(); // Calls the method to start the glow effect
            }

            //draw 2 cards
            GameManager.Instance.playerDraw2Cards = true;

            //GameManager.Instance.emptyPromiseBackfire.gameObject.SetActive(true);
            statusDropdown.DisplayStatusEffect(0, 6);

        }
        return;
    }
    public void PlayCardForAI()
    {
        DataGathering dG = FindObjectOfType<DataGathering>();
        dG.emptyUsedAI = dG.emptyUsedAI + 1;

        float chance = gameManager.statusPercent;
        float roll = UnityEngine.Random.Range(0f, 100f);

        if (roll <= chance)
        {
            dG.emptyBackfireAI = dG.emptyBackfireAI + 1;

            //Display BACKFIRE! Text
            TextMeshProUGUI backfireText = GetComponentInChildren<TextMeshProUGUI>();
            backfireText.enabled = true;
            if (backfireGlow != null)
            {
                backfireGlow.GlowActive(); // Calls the method to start the glow effect
            }

            //draw 2 cards
            GameManager.Instance.aiDraw2Cards = true;
            statusDropdown.DisplayStatusEffect(1, 6);
        }
        return;
    }

    //Side Effects To be added
}