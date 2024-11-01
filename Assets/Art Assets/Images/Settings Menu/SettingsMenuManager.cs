using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsMenuManager : MonoBehaviour
{
    [Header("Menus")]
    public GameObject mainMenuParent;
    public GameObject settingsMenuParent;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (settingsMenuParent.activeSelf)
            {
                ToggleSettingsMenu();
            }
        }
    }

    void ToggleSettingsMenu()
    {
        settingsMenuParent.SetActive(false);
        mainMenuParent.SetActive(true);
    }
}
