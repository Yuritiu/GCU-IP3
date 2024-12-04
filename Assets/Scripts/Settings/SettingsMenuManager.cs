using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SettingsMenuManager : MonoBehaviour
{
    [Header("Menus")]
    public GameObject mainMenuParent;
    public GameObject settingsMenuParent;

    [Header("Pause Manager")]
    public PauseMenu pauseMenu;

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
    public AudioSettingsManager audioSettingsManager;
    public ControlsSettingsManager controlsSettingsManager;

    private string mainMenuSceneName = "Main Menu";
    private string gameSceneName = "Game Scene";

    void Start()
    {
        ShowSubMenu(gameSubMenu);

        gameButton.onClick.AddListener(() => ShowSubMenu(gameSubMenu));
        controlsButton.onClick.AddListener(() => ShowSubMenu(controlsSubMenu));
        videoButton.onClick.AddListener(() => ShowSubMenu(videoSubMenu));
        audioButton.onClick.AddListener(() => ShowSubMenu(audioSubMenu));
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            string currentScene = SceneManager.GetActiveScene().name;

            if (currentScene == mainMenuSceneName && settingsMenuParent.activeSelf)
            {
                ToggleSettingsMenuFromMainMenu();
                videoSettingsManager.SaveSettings();
                audioSettingsManager.SaveAudioSettings();
                controlsSettingsManager.SaveSettings();
            }
            else if (currentScene == mainMenuSceneName && !settingsMenuParent.activeSelf)
            {
                ToggleSettingsMenuFromMainMenu();
            }
            else if (currentScene == gameSceneName && settingsMenuParent.activeSelf)
            {
                CloseSettingsAndOpenPauseMenu();
                videoSettingsManager.SaveSettings();
                audioSettingsManager.SaveAudioSettings();
                //controlsSettingsManager.SaveSettings();
            }
        }
    }

    void ToggleSettingsMenuFromMainMenu()
    {
        settingsMenuParent.SetActive(!settingsMenuParent.activeSelf);
        mainMenuParent.SetActive(!settingsMenuParent.activeSelf);
    }

    void CloseSettingsAndOpenPauseMenu()
    {
        settingsMenuParent.SetActive(false);
        if (pauseMenu != null)
        {
            pauseMenu.ShowPauseMenu();
        }
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
