using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardMaterialChanger : MonoBehaviour
{
    public static CardMaterialChanger Instance;

    public Material cardBackMaterial;
    public Material originalCardMaterial;

    Renderer cardRenderer;

    void Awake()
    {
        Instance = this;
        cardRenderer = GetComponent<Renderer>();
    }

    public void SetCardToCardBack()
    {
        if (cardRenderer != null && cardRenderer.materials.Length >= 2)
        {
            //Change Front Material To Card Back
            Material[] mats = cardRenderer.materials;
            mats[0] = cardBackMaterial;
            cardRenderer.materials = mats;
        }
    }

    public void SetCardToOriginalMaterial()
    {
        if (cardRenderer != null && cardRenderer.materials.Length >= 2)
        {
            //Change Card Back
            Material[] mats = cardRenderer.materials;
            mats[0] = originalCardMaterial;
            cardRenderer.materials = mats;
        }
    }
}
