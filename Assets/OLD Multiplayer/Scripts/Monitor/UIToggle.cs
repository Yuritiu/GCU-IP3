using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIToggle : MonoBehaviour
{
    [SerializeField] private GameObject target;
    private bool isVisible = false;

    public void Toggle()
    {
        isVisible = !isVisible;
        target.SetActive(isVisible);
    }

    public void Set(bool visible)
    {
        isVisible = visible;
        target.SetActive(isVisible);
    }

    public bool IsVisible() => isVisible;
}