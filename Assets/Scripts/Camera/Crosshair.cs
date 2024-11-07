using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Crosshair : MonoBehaviour
{
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