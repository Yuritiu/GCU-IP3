using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Crosshair : MonoBehaviour
{
    //!- Coded By Charlie -!

    [Header("References")]
    public RectTransform crosshairInner;
    public RectTransform crosshairOuter;

    [Header("Variables")]
    public float hoverScale = 1.25f;
    public float tweenDuration = 1f;

    private Vector3 originalOuterScale;
    private Vector3 targetScale;
    private float scaleTimer;
    private bool isHovering;

    void Start()
    {
        if (crosshairInner != null)
        {
            originalOuterScale = crosshairOuter.localScale;
            targetScale = originalOuterScale;
        }
    }

    void Update()
    {
        if (isHovering)
        {
            scaleTimer += Time.deltaTime / tweenDuration;
            crosshairOuter.localScale = Vector3.Lerp(crosshairOuter.localScale, targetScale, scaleTimer);
        }

        if (!GameManager.Instance.canPlay)
        {
            //Disables Inner Crosshair When Not Players Turn
            HoverScale(false);
            return;
        }

        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        //Raycast To Mouse Position
        if (Physics.Raycast(ray, out hit))
        {
            if ((hit.transform.CompareTag("Opponent") && GameManager.Instance.canPlay) || (hit.transform.CompareTag("Card") && GameManager.Instance.canPlay))
            {
                //Enables Inner Crosshair When Hovering
                HoverScale(true);
            }
            else
            {
                //Disables Inner Crosshair When Not Hovering
                HoverScale(false);
            }
        }
    }

    public void HoverScale(bool hovering)
    {
        isHovering = hovering;

        if(isHovering)
        {
            targetScale = originalOuterScale * hoverScale;
        }
        else
        {
            crosshairOuter.localScale = originalOuterScale;
        }
        //targetScale = isHovering ? originalInnerScale * hoverScale : originalInnerScale;
        scaleTimer = 0;
    }
}