using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class DoubleBullets : MonoBehaviour
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
        StartCoroutine(WaitForActionsAndPlayChamber(true));
    }

    public void PlayCardForAI()
    {
        StartCoroutine(WaitForActionsAndPlayChamber(false));
    }

    IEnumerator WaitForActionsAndPlayChamber(bool isPlayer)
    {
        ////STOPS FROM AUTO ENDING TURN ONCE KNIFE/ GUN FINISHED
        //if (isPlayer)
        //{
        //    gameManager.inPlayerReloadCalled = true;
        //}
        //else
        //{
        //    gameManager.inAIReloadCalled = true;
        //}

        if (PriorityCardExists())
        {
            Debug.Log("Priority card detected! Waiting for its action to start...");

            //Wait A Short Time To Allow Any Knife/Gun Action To Start
            yield return new WaitForSeconds(3f);
        }

        //Wait Until All Knife/Gun Actions Are Finished
        while (PriorityActionInProgress())
        {
            Debug.Log("Priority action in progress! Waiting...");
            yield return null;
        }

        Debug.Log("All Priority actions are done. Proceeding with chamber action.");

        //All Actions Done -> Proceed With The Chamber Action
        if (isPlayer)
        {
            if (gameManager.inPlayerReloadCalled)
            {
                float chance = gameManager.statusPercent;
                float roll = UnityEngine.Random.Range(0f, 100f);
                //reloadScript.Instance.reloadHappened = false;

                if (reloadScript.Instance.in2ndPos == false)
                {
                    reloadScript.Instance.moveGun();
                }

                if (roll <= chance)
                {
                    //Display BACKFIRE! Text
                    TextMeshProUGUI backfireText = GetComponentInChildren<TextMeshProUGUI>();
                    backfireText.enabled = true;

                    //loads blank (does nothing)
                    statusDropdown.DisplayStatusEffect(0, 2);
                    //return;
                }

                gameManager.addBullet();
            }
        }
        //AI Logic
        else
        {
            if (gameManager.inAIReloadCalled)
            {
                Debug.Log("AI RELOAD CALLED");
                float chance = gameManager.statusPercent;
                float roll = UnityEngine.Random.Range(0f, 100f);
                //reloadScript.Instance.reloadHappened = false;

                if (reloadScript.Instance.in2ndPos == false)
                {
                    reloadScript.Instance.moveGun();
                }

                if (roll <= chance)
                {
                    //Display BACKFIRE! Text
                    TextMeshProUGUI backfireText = GetComponentInChildren<TextMeshProUGUI>();
                    backfireText.enabled = true;

                    //loads blank (does nothing)
                    statusDropdown.DisplayStatusEffect(1, 2);
                    //return;
                }

                gameManager.addBullet();
            }
        }
    }

    private bool PriorityCardExists()
    {
        Component[] cardComponents =
        {
            GetCardComponent(CardDrawSystem.Instance.selectedPosition1),
            GetCardComponent(CardDrawSystem.Instance.selectedPosition2),
            GetCardComponent(AICardDrawSystem.Instance.selectedPosition1),
            GetCardComponent(AICardDrawSystem.Instance.selectedPosition2)
        };

        foreach (var card in cardComponents)
        {
            if (card != null && (card.gameObject.name.Contains("knife") || card.gameObject.name.Contains("gun")))
            {
                //Found A Knife/ Gun Card
                return true;
            }
        }

        //No Knife/Gun Cards Detected
        return false;
    }

    //Get The Card Components
    private Component GetCardComponent(Transform cardPosition)
    {
        if (cardPosition.childCount > 0)
        {
            return cardPosition.GetChild(0).gameObject.GetComponentAtIndex(1);
        }
        return null;
    }

    //Check If Knife/ Gun Action Is Ongoing
    private bool PriorityActionInProgress()
    {
        return gameManager.inKnifeActionAiPlayed || gameManager.inKnifeActionPlayerPlayed || gameManager.inGunAction || gameManager.aiGunCount > 0 || 
            gameManager.increaseCard1SkipCalled || gameManager.increaseCard2SkipCalled || gameManager.increaseCard3SkipCalled || gameManager.increaseCard4SkipCalled;
    }
}
