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

    void Start()
    {
        gameManager = FindAnyObjectByType<GameManager>();
        statusDropdown = FindAnyObjectByType<StatusDropdown>();
    }

    public void PlayCardForPlayer()
    {
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
    }
}
