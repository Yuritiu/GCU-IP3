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

    [SerializeField] private AudioClip armourEquip;

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

    void FixedUpdate()
    {
        if (disableUI.Instance.uiDisabled)
        {
            var cardMaterial = GetComponent<CardMaterialChanger>();
            cardMaterial.SetCardToCardBack();
        }
        else
        {
            var cardMaterial = GetComponent<CardMaterialChanger>();
            cardMaterial.SetCardToOriginalMaterial();
        }
    }

    public void PlayCardForPlayer()
    {
        //Save To Stat Tracker
        StatTracker.Instance.UpdateStat("ArmourCardsPlayed", 1);

        DataGathering dG = FindObjectOfType<DataGathering>();
        dG.armorUsed = dG.armorUsed + 1;

        float chance = gameManager.statusPercent;
        float roll = UnityEngine.Random.Range(0f, 100f);

        GameManager.Instance.playerArmour++;
        SFXManager.instance.PlaySFXClip(armourEquip, transform, 0.2f);
        if (roll <= chance)
        {
            dG.armorBackfire = dG.armorBackfire + 1;

            //Display BACKFIRE! Text
            TextMeshProUGUI backfireText = GetComponentInChildren<TextMeshProUGUI>();
            backfireText.enabled = true;

            if (backfireGlow != null)
            {
                backfireGlow.GlowActive(); // Calls the method to start the glow effect
            }


            // Slow bloodloss
            BloodlossSystem.Instance.bloodlossTime -= BloodlossSystem.Instance.shieldBloodlossReduce;

            //GameManager.Instance.armourBackfire.gameObject.SetActive(true);
            statusDropdown.DisplayStatusEffect(0, 3);
        }
    }

    public void PlayCardForAI()
    {
        DataGathering dG = FindObjectOfType<DataGathering>();
        dG.armorUsedAI = dG.armorUsedAI + 1;

        GameManager.Instance.aiArmour++;
        BloodlossSystemAI.Instance.AIbloodlossTime -= BloodlossSystemAI.Instance.shieldAIBloodlossReduce;
    }
}
