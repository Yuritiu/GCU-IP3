using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MultiplayerPauseManager : MonoBehaviour
{
    [Header("Buttons")]
    public Button resumeButton;
    public Button settingsButton;
    public Button quitButton;

    [Header("Canvas")]
    public GameObject pauseCanvas;
    public GameObject settingsMenu;

    public bool isPaused = false;

    public MultiplayerGameManager gameManager;
    private void Start()
    {
        resumeButton.onClick.AddListener(ResumeGame);
        settingsButton.onClick.AddListener(OpenSettingsMenu);
        quitButton.onClick.AddListener(QuitToMainMenu);

        gameManager = FindAnyObjectByType<MultiplayerGameManager>();

        pauseCanvas.SetActive(false);
        settingsMenu.SetActive(false);
    }

    private void Update()
    {
        //if (gameManager.gameEnded)
        //    return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused && !settingsMenu.activeSelf)
            {
                ResumeGame();
            }
            else
            {
                OpenSettingsMenu();
            }
        }

        if (isPaused && pauseCanvas.activeSelf)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    private void ResumeGame()
    {
        isPaused = false;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        pauseCanvas.SetActive(false);
    }

    private void OpenSettingsMenu()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        pauseCanvas.SetActive(false);
        settingsMenu.SetActive(true);
    }

    private void QuitToMainMenu()
    {
        //AudioListener.volume = 0f;
    }
}