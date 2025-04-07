using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class WinLoss : MonoBehaviour
{
    [Header("Buttons")]
    public Button[] restartButtons;
    public Tutorial tutorialManager;
   // public Button[] feedbackButtons;
   // public Button[] mainMenuButtons;

    //[Header("FeedbackURL")]
    //public string feedbackURL = "https://forms.gle/4rPB2aM3a4HumPxD8";

    private void Start()
    {
        foreach (Button restartButton in restartButtons)
        {
            restartButton.onClick.AddListener(RestartGame);
        }

        tutorialManager = FindFirstObjectByType<Tutorial>();

        //foreach (Button feedbackButton in feedbackButtons)
        //{
        //    feedbackButton.onClick.AddListener(OpenFeedbackLink);
        //}

        //foreach (Button mainMenuButton in mainMenuButtons)
        //{
        //    mainMenuButton.onClick.AddListener(GoToMainMenu);
        //}
    }

    private void RestartGame()
    {
        Time.timeScale = 1f;
        tutorialManager.introCalled = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

    }

    //private void OpenFeedbackLink()
    //{
    //    Application.OpenURL(feedbackURL);
    //}

    //private void GoToMainMenu()
    //{
    //    Time.timeScale = 1f;
    //    SceneManager.LoadScene("Main Menu");
    //}
}
