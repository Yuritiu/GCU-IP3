using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("Main Menu Buttons")]
    public Button startGameButton;
    public Button settingsButton;
    public Button creditsButton;
    public Button quitGameButton;

    [Header("Expanded Menu Buttons")]
    public Button tutorialButton;
    public Button easyButton;
    public Button hardButton;
    public Button closeButton;

    [Header("Menu Items to Toggle")]
    public GameObject[] objectsToToggle;

    [Header("Menus")]
    public GameObject mainMenuParent;
    public GameObject settingsMenuParent;

    private void Start()
    {
        //Listeners for interactions
        startGameButton.onClick.AddListener(StartGame);
        settingsButton.onClick.AddListener(OpenSettings);
        creditsButton.onClick.AddListener(OpenCredits);
        quitGameButton.onClick.AddListener(QuitGame);

        tutorialButton.onClick.AddListener(StartTutorial);
        easyButton.onClick.AddListener(StartEasyMode);
        hardButton.onClick.AddListener(StartHardMode);
        closeButton.onClick.AddListener(CloseExpandedMenu);
    }

    private void StartGame()
    {
        ExpandMainMenu();
    }

    private void OpenSettings()
    {
        Debug.Log("Settings menu");

        mainMenuParent.SetActive(false);

        settingsMenuParent.SetActive(true);

    }

    private void OpenCredits()
    {
        Debug.Log("Credits screen");
    }

    private void QuitGame()
    {
        Application.Quit();
    }

    //Expanded menu functions

    private void StartTutorial()
    {
        Debug.Log("Tutorial");
    }

    private void StartEasyMode()
    {
        Debug.Log("Easy");
    }

    private void StartHardMode()
    {
        Debug.Log("Hard mode started");
    }

    private void CloseExpandedMenu()
    {
        ExpandMainMenu();
    }

    private void ExpandMainMenu()
    {
        foreach (GameObject obj in objectsToToggle)
        {
            obj.SetActive(!obj.activeSelf);
        }
    }
}
