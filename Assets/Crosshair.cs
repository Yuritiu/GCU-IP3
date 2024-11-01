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
        if (crosshairOuter != null)
        {
            originalOuterScale = crosshairOuter.localScale;
            targetScale = originalOuterScale;
        }
    }

    void Update()
    {
        //Needs to be changed to find hovering from CardSelection script, can't work it out
        if (Input.GetKeyDown(KeyCode.Space))
        {
            HoverScale(true);
        }
        else if (Input.GetKeyUp(KeyCode.Space))
        {
            HoverScale(false);
        }

        if (crosshairOuter != null)
        {
            scaleTimer += Time.deltaTime / tweenDuration;
            crosshairOuter.localScale = Vector3.Lerp(crosshairOuter.localScale, targetScale, scaleTimer);
        }
    }

    public void HoverScale(bool hovering)
    {
        isHovering = hovering;
        targetScale = isHovering ? originalOuterScale * hoverScale : originalOuterScale;
        scaleTimer = 0;
    }
}