using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;

public class SettingsMenu : MonoBehaviour
{
    //!-Coded By Charlie-!

    public static SettingsMenu Instance;

    [SerializeField] GameObject settingsCanvas;

    [Header("Menu Button References")]
    [SerializeField] Button videoMenuButton;
    [SerializeField] Button audioMenuButton;
    [SerializeField] Button controlsMenuButton;
    [SerializeField] Button rulesMenuButton;
    [SerializeField] Button returnToMenuButton;

    [Header("Menu Screen References")]
    [SerializeField] GameObject videoMenu;
    [SerializeField] GameObject audioMenu;
    [SerializeField] GameObject controlsMenu;
    [SerializeField] GameObject rulesMenu;

    [Header("Video Menu References")]
    [SerializeField] Dropdown resolutionDropdown;
    List<Resolution> availableResolutions;
    [SerializeField] Toggle fullscreenToggle;
    bool isFullscreen = true;

    [Header("Controls Menu References")]
    [SerializeField] Toggle assistsToggle;
    [HideInInspector] public bool assistsEnabled = true;

    public bool settingsMenuOpen = false;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        settingsCanvas.SetActive(false);

        if (videoMenuButton != null)
        {
            //Add Listener To Call The OnVideoMenuButtonClick Function When The Button Is Clicked
            videoMenuButton.onClick.AddListener(OnVideoMenuButtonClick);
        }
        if (controlsMenuButton != null)
        {
            //Add Listener To Call The OnControlsMenuButtonClick Function When The Button Is Clicked
            controlsMenuButton.onClick.AddListener(OnControlsMenuButtonClick);
        }
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !settingsMenuOpen)
        {
            settingsCanvas.SetActive(true);
            settingsMenuOpen = true;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;
            //Default To Video Menu When The Settings Menu Opens
            OnVideoMenuButtonClick();
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            settingsCanvas.SetActive(false);
            settingsMenuOpen = false;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    void OnVideoMenuButtonClick()
    {
        //Disable All Other Menus
        controlsMenu.SetActive(false);
        audioMenu.SetActive(false);
        rulesMenu.SetActive(false);
        //Enable Video Menu
        videoMenu.SetActive(true);

        //Resolution Dropdown Initialisation
        availableResolutions = new List<Resolution>(Screen.resolutions);
        resolutionDropdown.ClearOptions();

        //Hold The Resolution Options As A String
        List<string> options = new List<string>();

        //Populate The Dropdown With The Available Resolutions
        int currentResolutionIndex = 0;
        for (int i = 0; i < availableResolutions.Count; i++)
        {
            Resolution resolution = availableResolutions[i];
            string option = resolution.width + " x " + resolution.height + " @ " + resolution.refreshRate + "Hz";
            options.Add(option);

            //Set Default Dropdown Value To The Current Resolution - Ignore "Obsolete", Works Fine
            if (resolution.width == Screen.currentResolution.width &&
                resolution.height == Screen.currentResolution.height &&
                resolution.refreshRate == Screen.currentResolution.refreshRate)
            {
                currentResolutionIndex = i;
            }
        }

        //Add Available Resolutions To The Dropdown
        resolutionDropdown.AddOptions(options);

        //Show The Current Resolution In The Dropdown Menu
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();

        //Handle The Dropdown Value When Changed
        resolutionDropdown.onValueChanged.AddListener(OnResolutionChanged);

        //Resolution Scale Slider
        //TODO: IMPLEMENT SLIDER LOGIC

        //Fullscreen Toggle
        fullscreenToggle.onValueChanged.AddListener(OnFullscreenToggle);
    }

    //Function Called When A New Resolution Is Selected
    void OnResolutionChanged(int resolutionIndex)
    {
        Resolution selectedResolution = availableResolutions[resolutionIndex];
        if (isFullscreen)
        {
            //If Fullscreen, Set Resolution Then Keep Fullscreen Mode
            Screen.SetResolution(selectedResolution.width, selectedResolution.height, Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen, selectedResolution.refreshRate);
        }
        else
        {
            //If Windowed, Set Resolution Then Keep Windowed Mode
            Screen.SetResolution(selectedResolution.width, selectedResolution.height, Screen.fullScreenMode = FullScreenMode.Windowed, selectedResolution.refreshRate);
        }
    }

    //Function Called When Fullscreen Toggle Is Changed
    void OnFullscreenToggle(bool toggled)
    {
        if (toggled)
        {
            //Set Fullscreen Resolution
            Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
            isFullscreen = true;
        }
        else
        {
            //Set Windowed Resolution
            Screen.fullScreenMode = FullScreenMode.Windowed;
            isFullscreen = false;
        }
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene("Main Menu");
    }

    void OnControlsMenuButtonClick()
    {
        //Disable All Other Menus
        videoMenu.SetActive(false);
        audioMenu.SetActive(false);
        rulesMenu.SetActive(false);
        //Enable Controls Menu
        controlsMenu.SetActive(true);

        //Assists Toggle
        assistsToggle.onValueChanged.AddListener(OnAssistsToggle);
    }

    //Function Called When Assists Toggle Is Changed
    void OnAssistsToggle(bool toggled)
    {
        if (toggled)
        {
            assistsEnabled = true;
        }
        else
        {
            assistsEnabled = false;
        }
    }
}
