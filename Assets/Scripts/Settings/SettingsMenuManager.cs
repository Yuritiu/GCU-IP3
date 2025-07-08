using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SettingsMenuManager : MonoBehaviour
{
    [Header("References")]
    MultiplayerCameraController multiplayerCameraController = null;

    [Header("Menus")]
    public GameObject mainMenuParent;
    public GameObject settingsMenuParent;
    public GameObject statsMenuParent;

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
    public GameSettingsManager gameSettingsManager;

    private string mainMenuSceneName = "Main Menu";
    private string gameSceneName = "Game Scene";
    private string multiplayerGameSceneName = "MultiplayerMatrixGameScene";

    bool canUseMenu = false;

    void Start()
    {
        ShowSubMenu(gameSubMenu);

        gameButton.onClick.AddListener(() => ShowSubMenu(gameSubMenu));
        controlsButton.onClick.AddListener(() => ShowSubMenu(controlsSubMenu));
        videoButton.onClick.AddListener(() => ShowSubMenu(videoSubMenu));
        audioButton.onClick.AddListener(() => ShowSubMenu(audioSubMenu));

        string currentScene = SceneManager.GetActiveScene().name;

        if (currentScene == "Game Scene")
        {
            canUseMenu = true;
        }
        else if(currentScene == multiplayerGameSceneName)
        {
            canUseMenu = true;
            //Retrieve Camera
            Transform cameraHolder = transform.root.Find("Camera");
            if (cameraHolder != null)
            {
                multiplayerCameraController = cameraHolder.GetComponent<MultiplayerCameraController>();
            }
            Debug.Log("[SETTINGS MENU MANAGER] Retrieved Client's Camera Controller: " + multiplayerCameraController);
        }
    }

    void Update()
    {
        //Stop Leaving By Esc Key Once Connected To Network
        if (!canUseMenu)
        {
            if (LobbyState.InLobby) return;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            string currentScene = SceneManager.GetActiveScene().name;

            if (currentScene == multiplayerGameSceneName)
            {
                if (settingsMenuParent.activeSelf)
                {
                    //Close Menu & Save Settings
                    multiplayerCameraController.cameraLocked = false;
                    Cursor.visible = false;
                    Cursor.lockState = CursorLockMode.Locked;

                    ToggleSettingsMenuFromMultiplayer();
                    videoSettingsManager.SaveSettings();
                    audioSettingsManager.SaveAudioSettings();
                    controlsSettingsManager.SaveSettings();
                    gameSettingsManager.SaveSettings();
                }
                else
                {
                    //Open Menu
                    multiplayerCameraController.cameraLocked = true;
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.None;

                    ToggleSettingsMenuFromMultiplayer();
                }
            }
            else if (currentScene == gameSceneName)
            {
                if (settingsMenuParent.activeSelf)
                {
                    videoSettingsManager.SaveSettings();
                    audioSettingsManager.SaveAudioSettings();
                    controlsSettingsManager.SaveSettings();
                    gameSettingsManager.SaveSettings();

                    CloseSettingsAndOpenPauseMenu();
                }
            }
            else if(currentScene == mainMenuSceneName)
            {
                if ((MenuCameraController.Instance != null && (MenuCameraController.Instance.currentMode == MenuCameraController.CameraMode.Rulebook || MenuCameraController.Instance.currentMode == MenuCameraController.CameraMode.Customise || MenuCameraController.Instance.isTransitioning)))
                {
                    return;
                }
                else if (settingsMenuParent.activeSelf)
                {
                    ToggleSettingsMenuFromMainMenu();
                    videoSettingsManager.SaveSettings();
                    audioSettingsManager.SaveAudioSettings();
                    controlsSettingsManager.SaveSettings();
                    gameSettingsManager.SaveSettings();
                }
                else if (!settingsMenuParent.activeSelf && !statsMenuParent.activeSelf)
                {
                    ToggleSettingsMenuFromMainMenu();
                }
                else if (!settingsMenuParent.activeSelf && statsMenuParent.activeSelf)
                {
                    CloseStatsAndOpenMenu();
                }
            }
        }
    }

    void ToggleSettingsMenuFromMainMenu()
    {
        settingsMenuParent.SetActive(!settingsMenuParent.activeSelf);
        mainMenuParent.SetActive(!settingsMenuParent.activeSelf);
    }

    void ToggleSettingsMenuFromMultiplayer()
    {
        settingsMenuParent.SetActive(!settingsMenuParent.activeSelf);
    }

    void CloseSettingsAndOpenPauseMenu()
    {
        settingsMenuParent.SetActive(false);
        if (pauseMenu != null)
        {
            pauseMenu.ShowPauseMenu();
        }
    }

    void CloseSettingsAndOpenPauseMenuMultiplayer()
    {
        settingsMenuParent.SetActive(false);
    }

    void CloseStatsAndOpenMenu()
    {
        mainMenuParent.SetActive(true);
        statsMenuParent.SetActive(false);
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
