using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwingAwayCard : MonoBehaviour
{
    private GameManager gameManager;

    void Start()
    {
        gameManager = FindAnyObjectByType<GameManager>();
    }

    public void PlayCardForAI()
    {
        float chance = gameManager.statusPercent;
        float roll = UnityEngine.Random.Range(0f, 100f);

        if (roll <= chance)
        {
            //shuffles cards and blurs them
            GameManager.Instance.blur.SetActive(true);
            CardDrawSystem.Instance.ShuffleHand();
            GameManager.Instance.batBackfire.gameObject.SetActive(true);
        }

        else
        {
            //Skip Players Turn
            GameManager.Instance.playerSkippedTurnsText.enabled = true;
            GameManager.Instance.playerSkippedTurns++;
            GameManager.Instance.playerSkippedTurnsText.text = "Skipped Players Turn";
        }
    }
}
