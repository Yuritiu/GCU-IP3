using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;

public class RulebookButtonHoverOutline : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] GameObject outlineObject;
    [SerializeField] bool isHostButtonOnly;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isHostButtonOnly)
        {
            if (outlineObject != null)
                outlineObject.SetActive(true);
        }

        if (isHostButtonOnly)
        {
            if (!NetworkManager.Singleton.IsHost) return;

            if (outlineObject != null)
                outlineObject.SetActive(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isHostButtonOnly)
        {
            if (outlineObject != null)
                outlineObject.SetActive(false);
        }

        if (isHostButtonOnly)
        {
            if (!NetworkManager.Singleton.IsHost) return;

            if (outlineObject != null)
                outlineObject.SetActive(false);
        }
    }
}
