using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;

public class ButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("References")]
    public TMP_Text buttonText;
    public Image hoverImage;

    [Header("Variables")]
    public float transitionDuration = 0.2f;

    [Header("Colour")]
    public Color hoverColor = Color.yellow;
    private Color originalColor;


    void Start()
    {
        originalColor = buttonText.color;

        if (hoverImage != null)
        {
            Color imageColor = hoverImage.color;
            imageColor.a = 0;
            hoverImage.color = imageColor;
        }
    }

    void OnEnable()
    {
        if (hoverImage != null)
        {
            Color imageColor = hoverImage.color;
            imageColor.a = 0;
            hoverImage.color = imageColor;
        }
    }

    void OnDisable()
    {
        buttonText.color = originalColor;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        StopAllCoroutines();
        StartCoroutine(ChangeTextColor(buttonText.color, hoverColor, transitionDuration));

        if (hoverImage != null)
        {
            StartCoroutine(ChangeImageOpacity(hoverImage, 0, 1, transitionDuration));
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        StopAllCoroutines();
        StartCoroutine(ChangeTextColor(buttonText.color, originalColor, transitionDuration));

        if (hoverImage != null)
        {
            StartCoroutine(ChangeImageOpacity(hoverImage, 1, 0, transitionDuration));
        }
    }

    private IEnumerator ChangeTextColor(Color startColor, Color targetColor, float duration)
    {
        float time = 0;
        while (time < duration)
        {
            buttonText.color = Color.Lerp(startColor, targetColor, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        buttonText.color = targetColor;
    }

    private IEnumerator ChangeImageOpacity(Image image, float startAlpha, float targetAlpha, float duration)
    {
        float time = 0;
        Color imageColor = image.color;

        while (time < duration)
        {
            float alpha = Mathf.Lerp(startAlpha, targetAlpha, time / duration);
            imageColor.a = alpha;
            image.color = imageColor;
            time += Time.deltaTime;
            yield return null;
        }
        imageColor.a = targetAlpha;
        image.color = imageColor;
    }
}
