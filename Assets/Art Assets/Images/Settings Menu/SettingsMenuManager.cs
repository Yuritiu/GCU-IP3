using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenuManager : MonoBehaviour
{
    [Header("Menus")]
    public GameObject mainMenuParent;
    public GameObject settingsMenuParent;

    [Header("Sub Menus")]
    public GameObject gameSubMenu;
    public GameObject controlsSubMenu;
    public GameObject videoSubMenu;
    public GameObject audioSubMenu;

    [Header("Buttons")]
    public Button gameButton;
    public Button controlsButton;
    public Button videoButton;
    public Button audioButton;

    [Header("Save Data References")]
    public VideoSettingsManager videoSettingsManager;

    void Start()
    {
        gameButton.onClick.AddListener(() => ShowSubMenu(gameSubMenu));
        controlsButton.onClick.AddListener(() => ShowSubMenu(controlsSubMenu));
        videoButton.onClick.AddListener(() => ShowSubMenu(videoSubMenu));
        audioButton.onClick.AddListener(() => ShowSubMenu(audioSubMenu));
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (settingsMenuParent.activeSelf)
            {
                ToggleSettingsMenu();
                videoSettingsManager.SaveSettings();
            }
        }
    }

    void ToggleSettingsMenu()
    {
        settingsMenuParent.SetActive(false);
        mainMenuParent.SetActive(true);
    }

    void ShowSubMenu(GameObject subMenuToShow)
    {
        gameSubMenu.SetActive(false);
        controlsSubMenu.SetActive(false);
        videoSubMenu.SetActive(false);
        audioSubMenu.SetActive(false);

        subMenuToShow.SetActive(true);
    }
}
