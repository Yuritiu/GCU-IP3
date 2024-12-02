using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CigarCard : MonoBehaviour
{

    private GameManager gameManager;
    [SerializeField] private AudioClip PlayerCough;
    [SerializeField] private AudioClip AICough;
    void Start()
    {
        gameManager = FindAnyObjectByType<GameManager>();
    }
    public void PlayCardForPlayer()
    {
        //Clone Players Second Card
        StartCoroutine(GameManager.Instance.WaitToCompareCards(1, 2));

        float chance = gameManager.statusPercent;
        float roll = UnityEngine.Random.Range(0f, 100f);

        if (roll <= chance)
        {
            //skips players next turn
            GameManager.Instance.playerSkippedTurns++;
            GameManager.Instance.cigarBackfire.gameObject.SetActive(true);
            SFXManager.instance.PlaySFXClip(PlayerCough, transform, 1f);

        }

    }
    public void PlayCardForAI()
    {
        //Clone AI's Second Card
        StartCoroutine(GameManager.Instance.WaitToCompareCards(2, 2));

        float chance = gameManager.statusPercent;
        float roll = UnityEngine.Random.Range(0f, 100f);

        if (roll <= chance)
        {
            //skips Ais next turn
            GameManager.Instance.aiSkippedTurns++;
            SFXManager.instance.PlaySFXClip(AICough, transform, 1f);
        }
    }
}
