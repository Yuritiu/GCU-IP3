using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RulebookButtonHoverOutline : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] GameObject outlineObject;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (outlineObject != null)
            outlineObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (outlineObject != null)
            outlineObject.SetActive(false);
    }
}
