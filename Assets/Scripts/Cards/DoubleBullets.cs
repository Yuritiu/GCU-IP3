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
        StartCoroutine(WaitForActionsAndPlayChamber(true));
    }

    public void PlayCardForAI()
    {
        StartCoroutine(WaitForActionsAndPlayChamber(false));
    }

    IEnumerator WaitForActionsAndPlayChamber(bool isPlayer)
    {
        if (PriorityCardExists())
        {
            //Debug.Log("Priority card detected! Waiting for its action to start...");

            //Wait A Short Time To Allow Any Knife/Gun Action To Start
            yield return new WaitForSeconds(3f);
        }

        //Wait Until All Knife/Gun Actions Are Finished
        while (PriorityActionInProgress())
        {
            //Debug.Log("Priority action in progress! Waiting...");
            yield return null;
        }

        //Debug.Log("All Priority actions are done. Proceeding with chamber action.");

        //All Actions Done -> Proceed With The Chamber Action
        if (isPlayer)
        {
            if (gameManager.inPlayerReloadCalled)
            {
                DataGathering dG = FindObjectOfType<DataGathering>();
                dG.oneChamberUsed = dG.oneChamberUsed + 1;

                float chance = gameManager.statusPercent;
                float roll = UnityEngine.Random.Range(0f, 100f);
                //reloadScript.Instance.reloadHappened = false;

                if (reloadScript.Instance.in2ndPos == false)
                {
                    reloadScript.Instance.gameObject.SetActive(true);
                    reloadScript.Instance.moveGun();
                }

                if (roll <= chance)
                {
                    dG.oneChamberBackfire = dG.oneChamberBackfire + 1;
                    //Display BACKFIRE! Text
                    TextMeshProUGUI backfireText = GetComponentInChildren<TextMeshProUGUI>();
                    backfireText.enabled = true;

                    if (backfireGlow != null)
                    {
                        backfireGlow.GlowActive(); // Calls the method to start the glow effect
                    }


                    //loads blank (does nothing)
                    statusDropdown.DisplayStatusEffect(0, 2);
                    //return;
                }
                else
                {
                    gameManager.addBullet();
                }
            }
        }

        //AI Logic
        else
        {
            if (gameManager.inAIReloadCalled)
            {
                DataGathering dG = FindObjectOfType<DataGathering>();
                dG.oneChamberUsedAI = dG.oneChamberUsedAI + 1;
                Debug.Log("AI RELOAD CALLED");
                float chance = gameManager.statusPercent;
                float roll = UnityEngine.Random.Range(0f, 100f);
                //reloadScript.Instance.reloadHappened = false;

                if (reloadScript.Instance.in2ndPos == false  && reloadScript.Instance.gameObject.activeInHierarchy == true)
                {
                    reloadScript.Instance.gameObject.SetActive(true);
                    reloadScript.Instance.moveGun();
                }
                
                if (roll <= chance)
                {
                    dG.oneChamberBackfireAI = dG.oneChamberBackfireAI + 1;
                    //Display BACKFIRE! Text
                    TextMeshProUGUI backfireText = GetComponentInChildren<TextMeshProUGUI>();
                    backfireText.enabled = true;

                    if (backfireGlow != null)
                    {
                        backfireGlow = GetComponent<BackfireGlow>();
                    }

                    //loads blank (does nothing)
                    statusDropdown.DisplayStatusEffect(1, 2);
                    //return;
                }
                else
                {
                    gameManager.addBullet();
                }
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
