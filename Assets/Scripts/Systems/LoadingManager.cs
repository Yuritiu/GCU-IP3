using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingManager : MonoBehaviour
{
    [SerializeField] GameObject loadingMenu;

    public void LoadScene(string sceneName)
    {
        Time.timeScale = 1f;

        loadingMenu.SetActive(true);
        StartCoroutine(LoadingSceneCoroutine(sceneName));
    }

    IEnumerator LoadingSceneCoroutine(string sceneName)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);

        operation.allowSceneActivation = false;

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);

            if (operation.progress >= 0.9f)
            {
                //"Fake" Delay
                yield return new WaitForSeconds(2f);
                operation.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}
