using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuForce : MonoBehaviour
{
    public static MainMenuForce Instance;
    public bool forceToMainMenu = true;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

#if UNITY_EDITOR
        if (forceToMainMenu && SceneManager.GetActiveScene().name != "Main Menu")
        {
            SceneManager.LoadScene("Main Menu");
        }
#endif
    }
}
