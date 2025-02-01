using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [Header("Buttons")]
    public Button resumeButton;
    public Button settingsButton;
    public Button quitButton;

    [Header("Canvas")]
    public GameObject pauseCanvas;
    public GameObject settingsMenu;

    public bool isPaused = false;

    public GameManager gameManager;
    private void Start()
    {
        resumeButton.onClick.AddListener(ResumeGame);
        settingsButton.onClick.AddListener(OpenSettingsMenu);
        quitButton.onClick.AddListener(QuitToMainMenu);

        gameManager = FindAnyObjectByType<GameManager>();

        pauseCanvas.SetActive(false);
        settingsMenu.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused && !settingsMenu.activeSelf)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    private void PauseGame()
    {
        //Editted by Kyle McN (1/2/25, 15:10)
        //If the game has eneded the player won't be able to pause anymore (Fixing bug that resumed the game)
        if (!gameManager.gameEnded)
        {
            isPaused = true;
            Time.timeScale = 0f;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            pauseCanvas.SetActive(true);
        }
    }


    private void ResumeGame()
    {
        //Editted by Kyle McN (1/2/25, 15:10)
        if (!gameManager.gameEnded)
        {
            isPaused = false;
            Time.timeScale = 1f;
            pauseCanvas.SetActive(false);
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    private void OpenSettingsMenu()
    {
        pauseCanvas.SetActive(false);
        settingsMenu.SetActive(true);
    }

    private void QuitToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Main Menu");
    }

    public void ShowPauseMenu()
    {
        pauseCanvas.SetActive(true);
        isPaused = true;
        Time.timeScale = 0f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
}
