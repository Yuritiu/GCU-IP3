using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackfireFlash : MonoBehaviour
{
    
    public CanvasGroup flashGroup; //controls transparency
    private bool flash = false;

    private float flashBrightness = 1f; // how long screen stays white
    private float flashDuration = 5f; // how long it takes to fade out
    private float flashTimer = 0f; //tracks the time

    void Update()
    {
        if (flash)
        {
            flashTimer -= Time.deltaTime;

            if (flashTimer > flashDuration) 
            {
                flashGroup.alpha = 1; 
            }
            else if (flashTimer > 0) 
            {
                flashGroup.alpha = flashTimer / flashDuration; 
            }
            else
            {
                flashGroup.alpha = 0;
                flash = false; 
            }
        }
    }

    public void BackfireActive()
    {
        StartCoroutine(StartFlash());
    }

    private IEnumerator StartFlash()
    {
        yield return new WaitForSeconds(0.2f); // 0.5-second delay

        flash = true;
        flashTimer = flashDuration + flashBrightness;
        flashGroup.alpha = 1;
    }

}
