using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingManager : MonoBehaviour
{
    [SerializeField] GameObject loadingMenu;
    [SerializeField] Image loadingFade;
    [SerializeField] float fadeDuration = 1f;

    [SerializeField] GameObject fadePrefab;
    GameObject fadeInstance;

    public float startVolume;
    public float currentVolume;

    // Reference to the Tutorial script
    [SerializeField] Tutorial tutorialScript;

    void Start()
    {
        if (fadePrefab)
        {
            fadeInstance = Instantiate(fadePrefab);
        }

        StartCoroutine(FadeInAudio(1f));
    }

    public void LoadScene(string sceneName)
    {
        Time.timeScale = 1f;
        StartCoroutine(FadeOutAudio(1f));
        StartCoroutine(LoadingSceneCoroutine(sceneName));
    }


    IEnumerator LoadingSceneCoroutine(string sceneName)
    {
        yield return StartCoroutine(FadeInBlackScreen());

        loadingMenu.SetActive(true);

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);

        operation.allowSceneActivation = false;

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);

            if (operation.progress >= 0.9f)
            {
                //"Fake" Delay
                yield return new WaitForSeconds(Random.Range(2f, 4.5f));
                operation.allowSceneActivation = true;
            }

            yield return null;
        }
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    IEnumerator FadeInBlackScreen()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        float timer = 0f;
        Color fadeColor = loadingFade.color;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            fadeColor.a = Mathf.Lerp(0f, 1f, timer / fadeDuration);
            loadingFade.color = fadeColor;
            yield return null;
        }

        fadeColor.a = 1f;
        loadingFade.color = fadeColor;
    }

    private IEnumerator FadeOutAudio(float duration)
    {
        startVolume = AudioListener.volume;

        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            AudioListener.volume = Mathf.Lerp(startVolume, 0f, t / duration);
            yield return null;
        }

        AudioListener.volume = 0f;
    }

    private IEnumerator FadeInAudio(float duration)
    {
        currentVolume = AudioListener.volume;

        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            AudioListener.volume = Mathf.Lerp(currentVolume, 1f, t / duration);
            yield return null;
        }

        AudioListener.volume = 1f;
    }

}


