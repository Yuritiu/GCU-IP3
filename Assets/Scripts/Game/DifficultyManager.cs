using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DifficultyManager : MonoBehaviour
{
    public static DifficultyManager Instance;

    public int difficulty;

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(this);
    }

    public void SetEasyMode()
    {
        difficulty = 1;
        SceneManager.LoadScene("Game Scene");
    }

    public void SetHardMode()
    {
        difficulty = 2;
        SceneManager.LoadScene("Game Scene");
    }
}
