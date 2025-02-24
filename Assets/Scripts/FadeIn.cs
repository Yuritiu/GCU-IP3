using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FadeIn : MonoBehaviour
{
    [SerializeField] Image loadingFade;
    [SerializeField] float fadeDuration = 1f;

    private void Awake()
    {
        if (loadingFade)
        {
            Color fadeColor = loadingFade.color;
            fadeColor.a = 1f;
            loadingFade.color = fadeColor;
        }
    }

    void Start()
    {
        StartCoroutine(FadeOutBlackScreen());
    }

    IEnumerator FadeOutBlackScreen()
    {
        float timer = 0f;
        Color fadeColor = loadingFade.color;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            fadeColor.a = Mathf.Lerp(1f, 0f, timer / fadeDuration);
            loadingFade.color = fadeColor;
            yield return null;
        }

        fadeColor.a = 0f;
        loadingFade.color = fadeColor;

        if (SceneManager.GetActiveScene().name == "Main Menu")
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        gameObject.SetActive(false);
    }
}
