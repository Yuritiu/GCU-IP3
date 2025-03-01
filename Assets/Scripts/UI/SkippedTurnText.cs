using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SkippedTurnText : MonoBehaviour
{
    [Header("References")]
    [SerializeField] TextMeshProUGUI skippedTurnText;
    [SerializeField] Camera playerCamera;

    void Start()
    {
        skippedTurnText.enabled = false;
    }

    void FixedUpdate()
    {
        if(GameManager.Instance.displaySkipTurnText)
        {
            if (skippedTurnText != null && playerCamera != null)
            {
                skippedTurnText.enabled = true;

                skippedTurnText.transform.LookAt(playerCamera.transform);
                skippedTurnText.transform.Rotate(0, 180, 0);
            }
        }
        else
        {
            skippedTurnText.enabled = false;
        }
    }
}
