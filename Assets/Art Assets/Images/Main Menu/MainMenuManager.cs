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
    public Button quitGameButton;
    public Button feedBackButton;

    [Header("Expanded Menu Buttons")]
    public Button playButton;
    public Button closeButton;

    [Header("Menu Items to Toggle")]
    public GameObject[] objectsToToggle;

    [Header("Menus")]
    public GameObject mainMenuParent;
    public GameObject settingsMenuParent;

    [Header("Toggle with F4")]
    public GameObject toggleDev;

    [Header("FeedbackURL")]
    public string feedbackURL = "https://forms.gle/4rPB2aM3a4HumPxD8";

    [Header("Settings Manager References")]
    public VideoSettingsManager videoSettingsManager;
    public AudioSettingsManager audioSettingsManager;

    private void Start()
    {
        videoSettingsManager.LoadSettings();
        audioSettingsManager.LoadAudioSettings();
        //Listeners for interactions
        startGameButton.onClick.AddListener(StartGame);
        settingsButton.onClick.AddListener(OpenSettings);
        quitGameButton.onClick.AddListener(QuitGame);
        feedBackButton.onClick.AddListener(OpenFeedbackLink);

        playButton.onClick.AddListener(PlayGame);
        closeButton.onClick.AddListener(CloseExpandedMenu);
    }

    private void Update()
    {
        //Check if F4 is pressed
        if (Input.GetKeyDown(KeyCode.F4) && toggleDev != null)
        {
            toggleDev.SetActive(!toggleDev.activeSelf);
        }
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

    private void QuitGame()
    {
        Application.Quit();
    }

    private void OpenFeedbackLink()
    {
        Application.OpenURL(feedbackURL);
    }

    private void PlayGame()
    {
        //Debug.Log("Easy");
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
