using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CigarCard : MonoBehaviour
{
    [Header("Private References")]
    private GameManager gameManager;
    private StatusDropdown statusDropdown;

    [SerializeField] private AudioClip PlayerCough;
    [SerializeField] private AudioClip AICough;
    void Start()
    {
        gameManager = FindAnyObjectByType<GameManager>();
        statusDropdown = FindAnyObjectByType<StatusDropdown>();
    }
    public void PlayCardForPlayer()
    {
        //Clone Players Second Card
        //GameManager.Instance.PlayCigarCard(1);
        StartCoroutine(GameManager.Instance.WaitToCompareCards(1, 2));

        float chance = gameManager.statusPercent;
        float roll = UnityEngine.Random.Range(0f, 100f);

        if (roll <= chance)
        {
            //skips players next turn
            GameManager.Instance.playerSkippedTurns++;
            SFXManager.instance.PlaySFXClip(PlayerCough, transform, 0.2f);

            //GameManager.Instance.cigarBackfire.gameObject.SetActive(true);
            statusDropdown.DisplayStatusEffect(0, 4);
        }

    }
    public void PlayCardForAI()
    {
        //Clone AI's Second Card
        //GameManager.Instance.PlayCigarCard(2);
        StartCoroutine(GameManager.Instance.WaitToCompareCards(2, 2));

        float chance = gameManager.statusPercent;
        float roll = UnityEngine.Random.Range(0f, 100f);

        if (roll <= chance)
        {
            //skips Ais next turn
            GameManager.Instance.aiSkippedTurns++;
            SFXManager.instance.PlaySFXClip(AICough, transform, 0.2f);

            statusDropdown.DisplayStatusEffect(1, 4);
        }
    }
}
