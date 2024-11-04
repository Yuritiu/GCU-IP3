using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
// using UnityEngine.UIElements;

public class DifficultyManager : MonoBehaviour
{
    //!- Coded By Charlie -!

    public static DifficultyManager Instance;

    //DEBUG ONLY REMOVE LOGIC FROM FINAL BUILD
    [SerializeField] TMP_Dropdown sceneDropdown;

    [Header("Difficulty Buttons")]
    [SerializeField] Button tutorialButton;
    [SerializeField] Button easyButton;
    [SerializeField] Button hardButton;

    string sceneToLoad = "";

    [SerializeField] public int difficulty;

    private void Awake()
    {
        //Create singleton instance
        Instance = this;
        DontDestroyOnLoad(this);

        if (sceneDropdown != null)
        {
            //Add Listener To Check For Changed Value
            sceneDropdown.onValueChanged.AddListener(delegate { OnDropdownValueChange(); });
        }

        if (tutorialButton != null)
        {
            tutorialButton.onClick.AddListener(SetTutorialMode);
        }
        if (easyButton != null)
        {
            easyButton.onClick.AddListener(SetEasyMode);
        }
        if (hardButton != null)
        {
            hardButton.onClick.AddListener(SetHardMode);
        }
    }

    private void Start()
    {
        sceneToLoad = "";
    }

    public void OnDropdownValueChange()
    {
        if (sceneDropdown == null)
            return;

        //Get the dropdown value and load scenes based on selection
        switch (sceneDropdown.value)
        {
            case 1:
                //Load Game Scene
                sceneToLoad = "Game Scene";
                break;
            case 2:
                //Load Ben's Scene
                sceneToLoad = "Ben";
                break;
            case 3:
                //Load Charlie's Scene
                sceneToLoad = "Charlie";
                break;
            case 4:
                //Load Jack's Scene
                sceneToLoad = "Jack";
                break;
            case 5:
                //Load Josh's Scene
                sceneToLoad = "Josh";
                break;
            case 6:
                //Load Tutorial Scene
                sceneToLoad = "Tutorial";
                difficulty = 3;
                SceneManager.LoadScene(sceneToLoad);
                break;
            default:
                Debug.LogWarning("No Dropdown Option Selected");
                break;
        }
    }

    public void SetTutorialMode()
    {
        sceneToLoad = "Tutorial";
        difficulty = 3;
        SceneManager.LoadScene(sceneToLoad);
    }

    public void SetEasyMode()
    {
        sceneToLoad = "Game Scene";
        difficulty = 1;
        SceneManager.LoadScene(sceneToLoad);
    }

    public void SetHardMode()
    {
        sceneToLoad = "Game Scene";
        difficulty = 2;
        SceneManager.LoadScene(sceneToLoad);
    }
}
