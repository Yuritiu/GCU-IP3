using UnityEngine;

public class CardMaterialHandler : MonoBehaviour
{
    private CardMaterialChanger materialChanger;

    private void Awake()
    {
        materialChanger = GetComponent<CardMaterialChanger>();
    }

    void FixedUpdate()
    {
        if (disableUI.Instance.uiDisabled)
            materialChanger.SetCardToCardBack();
        else
            materialChanger.SetCardToOriginalMaterial();
    }
}