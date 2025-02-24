using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeIn : MonoBehaviour
{

    [SerializeField] Image loadingFade;
    [SerializeField] float fadeDuration = 1f;

    // Start is called before the first frame update
    private void Awake()
    {
        Color fadeColor = loadingFade.color;
        fadeColor.a = 1f;
    }

    void Start()
    {
        StartCoroutine(FadeOutBlackScreen());
    }

    IEnumerator FadeOutBlackScreen()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        float timer = 0f;
        Color fadeColor = loadingFade.color;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            fadeColor.a = Mathf.Lerp(1, 0f, timer / fadeDuration);
            loadingFade.color = fadeColor;
            yield return null;
        }

        fadeColor.a = 0f;
        loadingFade.color = fadeColor;
        Destroy(gameObject);
    }
}
