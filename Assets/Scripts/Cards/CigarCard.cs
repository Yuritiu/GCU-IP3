using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.VFX;

public class CigarCard : MonoBehaviour
{
    [Header("Private References")]
    private GameManager gameManager;
    private StatusDropdown statusDropdown;

    [SerializeField] private AudioClip PlayerCough;
    [SerializeField] private AudioClip AICough;

    [Header("Smoke Effect")]
    VisualEffect smokeVFX;
    bool backfired = false;

    void Start()
    {
        gameManager = FindAnyObjectByType<GameManager>();
        statusDropdown = FindAnyObjectByType<StatusDropdown>();

        smokeVFX = GetComponent<VisualEffect>();
    }

    public void PlayCardForPlayer()
    {
        //Clone Players Second Card
        //GameManager.Instance.PlayCigarCard(1);
        StartCoroutine(GameManager.Instance.WaitToCompareCards(1, 2));

        float chance = gameManager.statusPercent;
        float roll = UnityEngine.Random.Range(0f, 100f);

        if (roll <= chance && !backfired)
        {
            backfired = true;
            //Display BACKFIRE!Text
            TextMeshProUGUI backfireText = GetComponentInChildren<TextMeshProUGUI>();
            backfireText.enabled = true;

            //skips players next turn
            GameManager.Instance.playerSkippedTurns++;
            SFXManager.instance.PlaySFXClip(PlayerCough, transform, 0.2f);

            GameManager.Instance.cigarBackfire.gameObject.SetActive(true);
            statusDropdown.DisplayStatusEffect(0, 4);

            StopAllSmokeVFX();
        }
    }
    public void PlayCardForAI()
    {
        //Clone AI's Second Card
        //GameManager.Instance.PlayCigarCard(2);
        StartCoroutine(GameManager.Instance.WaitToCompareCards(2, 2));

        float chance = gameManager.statusPercent;
        float roll = UnityEngine.Random.Range(0f, 100f);

        if (roll <= chance && !backfired)
        {
            backfired = true;
            //Display BACKFIRE! Text
            TextMeshProUGUI backfireText = GetComponentInChildren<TextMeshProUGUI>();
            backfireText.enabled = true;

            //skips Ais next turn
            GameManager.Instance.aiSkippedTurns++;
            SFXManager.instance.PlaySFXClip(AICough, transform, 0.2f);

            statusDropdown.DisplayStatusEffect(1, 4);

            StopAllSmokeVFX();
        }
    }

    public void PlaySmokeVFX(string objectName, string cardName)
    {
        GameObject targetObject = GameObject.Find(objectName);
        if (targetObject == null)
        {
            Debug.LogWarning("No GameObject With The Name: " + objectName);
            return;
        }

        VisualEffect smokeVFX = targetObject.GetComponent<VisualEffect>();
        if (smokeVFX == null)
        {
            Debug.LogWarning("No VisualEffect Found On " + objectName);
            return;
        }

        if (backfired)
            return;

        smokeVFX.GetComponent<VisualEffect>().enabled = true;
        smokeVFX.Play();

        StartCoroutine(StopVFXAfterTime(smokeVFX, 0.3f));

        //Get Card To Be Cloned From CigarManager
        GameObject cardPrefab = CigarManager.Instance.GetCardPrefab(cardName);
        if (cardPrefab == null)
        {
            return;
        }
        //Spawn Cloned Card In Place Of Cigar
        Instantiate(cardPrefab, this.transform.position, this.transform.rotation);
    }

    private IEnumerator StopVFXAfterTime(VisualEffect smokeVFX, float delay)
    {
        yield return new WaitForSeconds(delay - 0.2f);
        //Hide Cigar Card
        this.gameObject.GetComponent<MeshRenderer>().enabled = false;

        yield return new WaitForSeconds(delay);
        smokeVFX.Stop();
        smokeVFX.GetComponent<VisualEffect>().enabled = false;
    }

    void StopAllSmokeVFX()
    {
        VisualEffect[] allSmokeVFX = FindObjectsOfType<VisualEffect>();
        Debug.Log("SMOKE VFX's: " + allSmokeVFX.Length);

        foreach (VisualEffect vfx in allSmokeVFX)
        {
            if (vfx != null)
            {
                vfx.Stop();
                vfx.GetComponent<VisualEffect>().enabled = false;
            }
        }
    }
}
