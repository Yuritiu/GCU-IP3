using System.Collections;
using UnityEngine;
using TMPro;

public class IntroLocationTime : MonoBehaviour
{
    public TextMeshProUGUI locationText;
    public TextMeshProUGUI timeText;

    private CanvasGroup locationCanvasGroup;
    private CanvasGroup timeCanvasGroup;

    public AudioClip typewriterSound;
    public float minPitch = 0.8f;
    public float maxPitch = 1.2f;
    public float volume = 0.5f;
    public float waitTime = 2f;

    string locationString = "CHICAGO 1928";
    string timeString = "TIME: 12:32";

    private AudioSource audioSource;

    private void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = typewriterSound;
        audioSource.volume = volume;

        locationCanvasGroup = locationText.GetComponent<CanvasGroup>();
        if (locationCanvasGroup == null)
            locationCanvasGroup = locationText.gameObject.AddComponent<CanvasGroup>();

        timeCanvasGroup = timeText.GetComponent<CanvasGroup>();
        if (timeCanvasGroup == null)
            timeCanvasGroup = timeText.gameObject.AddComponent<CanvasGroup>();

        StartCoroutine(TypewritingSequence());
    }

    IEnumerator TypewritingSequence()
    {
        yield return new WaitForSeconds(waitTime);

        yield return StartCoroutine(TypeText(locationText, "CHICAGO", 0.25f));

        yield return new WaitForSeconds(1f);

        yield return StartCoroutine(TypeText(locationText, " 1932", 0.5f));

        yield return new WaitForSeconds(2f);

        yield return StartCoroutine(TypeText(timeText, "TIME:", 0.25f));

        yield return new WaitForSeconds(1f);

        yield return StartCoroutine(TypeText(timeText, " 12:32", 0.5f));

        yield return new WaitForSeconds(4f);

        StartCoroutine(FadeText(locationCanvasGroup, 0f));
        StartCoroutine(FadeText(timeCanvasGroup, 0f));
    }

    IEnumerator TypeText(TextMeshProUGUI textComponent, string textToType, float typingSpeed)
    {
        foreach (char letter in textToType)
        {
            textComponent.text += letter;
            PlayTypewriterSound();
            yield return new WaitForSeconds(typingSpeed);
        }
    }

    void PlayTypewriterSound()
    {
        if (audioSource && typewriterSound)
        {
            audioSource.pitch = Random.Range(minPitch, maxPitch);
            audioSource.PlayOneShot(typewriterSound);
        }
    }

    IEnumerator FadeText(CanvasGroup canvasGroup, float targetAlpha)
    {
        float startAlpha = canvasGroup.alpha;
        float elapsedTime = 0f;
        float fadeDuration = 2f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = targetAlpha;
    }
}
