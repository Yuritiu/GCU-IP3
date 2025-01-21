using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Crosshair : MonoBehaviour
{
    //!- Coded By Charlie -!

    [Header("References")]
    [SerializeField] RectTransform crosshairInner;
    [SerializeField] RectTransform crosshairOuter;

    [Header("Variables")]
    [SerializeField] float hoverScale = 1.25f;
    [SerializeField]  float tweenDuration = 0.25f;

    Vector3 originalOuterScale;
    Vector3 targetScale;
    float scaleTimer;
    bool isHovering;

    void Start()
    {
        if (crosshairOuter != null)
        {
            originalOuterScale = new Vector3(1,1,1);
            targetScale = originalOuterScale;
        }
    }

    void Update()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        //Raycast To Mouse Position
        if (Physics.Raycast(ray, out hit))
        {
            if ((hit.transform.CompareTag("Opponent") && GameManager.Instance.canPlay) ||
                (hit.transform.CompareTag("Card") && GameManager.Instance.canPlay) ||
                (hit.transform.CompareTag("Rulebook") && GameManager.Instance.canPlay))
            {
                HoverScale(true);
            }
            else
            {
                HoverScale(false);
            }
        }
        else
        {
            HoverScale(false);
        }

        scaleTimer += Time.deltaTime / tweenDuration;
        crosshairOuter.localScale = Vector3.Lerp(crosshairOuter.localScale, targetScale, scaleTimer);

        if (!GameManager.Instance.canPlay)
        {
            //Disables Inner Crosshair When Not Players Turn
            HoverScale(false);
            return;
        }
    }

    public void HoverScale(bool hovering)
    {
        isHovering = hovering;

        if (isHovering)
        {
            targetScale = originalOuterScale * hoverScale;
        }
        else
        {
            targetScale = originalOuterScale;
        }
        scaleTimer = 0;
    }
}