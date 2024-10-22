using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class DifficultyManager : MonoBehaviour
{
    //!- Coded By Charlie -!

    public static DifficultyManager Instance;

    //DEBUG ONLY REMOVE LOGIC FROM FINAL BUILD
    [SerializeField] TMP_Dropdown sceneDropdown;
    string sceneToLoad = "";

    [SerializeField] public int difficulty;

    private void Awake()
    {
        // Create singleton instance
        Instance = this;
        DontDestroyOnLoad(this);

        if (sceneDropdown != null)
        {
            //Add Listener To Check For Changed Value
            sceneDropdown.onValueChanged.AddListener(delegate { OnDropdownValueChange(); });
        }
    }

    public void OnDropdownValueChange()
    {
        if (sceneDropdown == null)
            return;

        //Get The Dropdown Value And Load Scenes Based On Selection
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
            default:
                Debug.LogWarning("No Dropdown Option Selected");
                break;
        }
    }

    public void SetEasyMode()
    {
        if (sceneDropdown == null)
            return;

        difficulty = 1;
        SceneManager.LoadScene(sceneToLoad);
    }

    public void SetHardMode()
    {
        if (sceneDropdown == null)
            return;

        difficulty = 2;
        SceneManager.LoadScene(sceneToLoad);
    }
}
