using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System;
using TMPro;

public class Knife : MonoBehaviour
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
        GameManager.Instance.numberOfKnifeCardsPlayer++;
        GameManager.Instance.inKnifeActionPlayerPlayed = true;
        StartCoroutine(PlayPlayerKnife());
    }
    public void PlayCardForAI()
    {
        //print("playing knife card");
        GameManager.Instance.numberOfKnifeCardsAI++;
        GameManager.Instance.inKnifeActionAiPlayed = true;
        StartCoroutine(PlayAiKnife());
    }

    //ADDS a delay to the second knife to give time for the first knife to work first
    IEnumerator WaitToStart(int character, int type)
    {
        //waits for cards to reveal
        yield return new WaitForSeconds(1f);
        StartCoroutine(GameManager.Instance.WaitToCompareCards(character, type));
    }

    IEnumerator PlayAiKnife()
    {

        DataGathering dG = FindObjectOfType<DataGathering>();
        dG.knifeUsedAI = dG.knifeUsedAI + 1;

        GameManager.Instance.aiHasKnife = true;
        yield return new WaitForSeconds(0.5f);

        //Check If Its The Tutorial First
        if (!GameManager.Instance.isTutorial)
        {
            float chance = gameManager.statusPercent;
            float roll = UnityEngine.Random.Range(0f, 100f);

            if (roll <= chance)
            {
                dG.kinfebackfireAI = dG.kinfebackfireAI + 1;
                //Display BACKFIRE! Text
                TextMeshProUGUI backfireText = GetComponentInChildren<TextMeshProUGUI>();
                backfireText.enabled = true;

                if (backfireGlow != null)
                {
                    backfireGlow.GlowActive(); // Calls the method to start the glow effect
                }

                //makes 1 card not usable for 1 turn
                AICardDrawSystem.Instance.StopOneCard();
                statusDropdown.DisplayStatusEffect(1, 0);
            }

            if (AICardDrawSystem.Instance.selectedPosition1.childCount > 0)
            {
                //print("checking knife 1");
                if ((AICardDrawSystem.Instance.selectedPosition1.GetChild(0).name.Contains("knife") || 
                    AICardDrawSystem.Instance.selectedPosition1.GetChild(0).name.Contains("cigar")) && !GameManager.Instance.knife1used)
                {
                    GameManager.Instance.knife1used = true;
                    StartCoroutine(GameManager.Instance.WaitToCompareCards(2, 1));
                }
            }
            if (AICardDrawSystem.Instance.selectedPosition2.childCount > 0)
            {
                //print("checking knife 2");
                if ((AICardDrawSystem.Instance.selectedPosition2.GetChild(0).name.Contains("knife") ||
                    AICardDrawSystem.Instance.selectedPosition2.GetChild(0).name.Contains("cigar")) && !GameManager.Instance.knife2used)
                {
                    GameManager.Instance.knife2used = true;
                    if (!GameManager.Instance.knife1used)
                    {
                        StartCoroutine(GameManager.Instance.WaitToCompareCards(2, 1));
                    }
                }
            }
        }
    }

    IEnumerator PlayPlayerKnife()
    {
        DataGathering dG = FindObjectOfType<DataGathering>();
        dG.knifeUsed = dG.knifeUsed + 1;
        //AI knife animation is in Game Manager :3

        GameManager.Instance.playerHasKnife = true;
        yield return new WaitForSeconds(0.5f);

        float chance = gameManager.statusPercent;
        float roll = UnityEngine.Random.Range(0f, 100f);
         
        if (roll <= chance)
        {
            dG.kinfebackfire = dG.kinfebackfire + 1;

            //Display BACKFIRE! Text
            TextMeshProUGUI backfireText = GetComponentInChildren<TextMeshProUGUI>();
            backfireText.enabled = true;

            if (backfireGlow != null)
            {
                backfireGlow.GlowActive(); // Calls the method to start the glow effect
            }

            //makes 1 card not usable for 1 turn
            CardDrawSystem.Instance.StopOneCard();
            statusDropdown.DisplayStatusEffect(0, 0);
        }

        if (CardDrawSystem.Instance.selectedPosition1.childCount > 0 && CardDrawSystem.Instance.selectedPosition2.childCount > 0)
        {
            //print("made it");
            Component card1 = CardDrawSystem.Instance.selectedPosition1.GetChild(0);
            Component card2 = CardDrawSystem.Instance.selectedPosition2.GetChild(0);
            //Damage opponent 
            //takes 1 finger away
            if ((card1.gameObject.name.Contains("knife") || card1.gameObject.name.Contains("cigar")) && (card2.gameObject.name.Contains("knife") || card2.gameObject.name.Contains("cigar")))
            {
                if (card2.gameObject == this.gameObject)
                {
                    StartCoroutine(WaitToStart(1, 1));
                    yield return null;
                }
                else
                {
                    //print(1);
                    StartCoroutine(GameManager.Instance.WaitToCompareCards(1, 1));
                }
            }
            else
            {
                //print(2);
                StartCoroutine(GameManager.Instance.WaitToCompareCards(1, 1));
            }

            //else if (card1.gameObject.name.Contains("knife") || card2.gameObject.name.Contains("knife"))
            //{
            //    //Avoids Softlock When inKnifeAction Is Still True
            //    GameManager.Instance.inKnifeAiAction = false;
            //}
        }
        else
        {
            //print(3);
            StartCoroutine(GameManager.Instance.WaitToCompareCards(1, 1));
        }
    }
}
