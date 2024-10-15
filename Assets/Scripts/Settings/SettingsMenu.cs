using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    //!-Coded By Charlie-!

    [SerializeField] GameObject settingsCanvas;

    [Header("Menu Button References")]
    [SerializeField] Button videoMenuButton;
    [SerializeField] Button audioMenuButton;
    [SerializeField] Button controlsMenuButton;
    [SerializeField] Button helpMenuButton;

    [Header("Menu Screen References")]
    [SerializeField] GameObject videoMenu;
    [SerializeField] GameObject audioMenu;
    [SerializeField] GameObject controlsMenu;
    [SerializeField] GameObject helpMenu;

    [Header("Video Menu References")]
    [SerializeField] Dropdown resolutionDropdown;
    List<Resolution> availableResolutions;
    [SerializeField] Toggle fullscreenToggle;
    bool isFullscreen = true;

    bool settingsMenuOpen = false;

    void Start()
    {
        settingsCanvas.SetActive(false);

        if (videoMenuButton != null)
        {
            //Add Listener To Call The OnVideoMenuButtonClick Function When The Button Is Clicked
            videoMenuButton.onClick.AddListener(OnVideoMenuButtonClick);
        }
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !settingsMenuOpen)
        {
            settingsCanvas.SetActive(true);
            settingsMenuOpen = true;

            //Default To Video Menu When The Settings Menu Opens
            OnVideoMenuButtonClick();
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            settingsCanvas.SetActive(false);
            settingsMenuOpen = false;
        }
    }

    void OnVideoMenuButtonClick()
    {
        Debug.Log("Video Menu Initialised");

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
            Screen.SetResolution(selectedResolution.width, selectedResolution.height, Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen, selectedResolution.refreshRate);
        }
        else
        {
            Screen.SetResolution(selectedResolution.width, selectedResolution.height, Screen.fullScreenMode = FullScreenMode.Windowed, selectedResolution.refreshRate);
        }
    }

    //Function Called When Fullscreen Toggle Is Changed
    void OnFullscreenToggle(bool toggled)
    {
        if (toggled)
        {
            Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
            isFullscreen = true;
        }
        else
        {
            Screen.fullScreenMode = FullScreenMode.Windowed;
            isFullscreen = false;
        }
    }
}
