using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonToggle : MonoBehaviour
{
    [SerializeField] private GameObject toggledGameobject;

    Button button;

    private void Awake()
    {
        button = GetComponent<Button>();

        if (button == null)
        {
            Debug.LogError("Button Missing!");
            enabled = false;
            return;
        }

        button.onClick.AddListener(ToggleTarget);
    }

    private void ToggleTarget()
    {
        if (toggledGameobject != null)
        {
            toggledGameobject.SetActive(!toggledGameobject.activeSelf);
        }
    }
}