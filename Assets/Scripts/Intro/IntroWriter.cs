using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;

public class TextWriter : MonoBehaviour
{
    [Header("Text and Typing Settings")]
    public TextMeshProUGUI textDisplay;
    public string[] textLines;
    public float initialPauseTime = 1f;
    public float preFadePauseTime = 1f;
    public float[] pauseTimes;
    public float typingSpeed = 0.05f;
    public float fadeDuration = 0.5f;

    [Header("Audio Settings")]
    public AudioClip typewriterSound;
    public float minPitch = 0.8f;
    public float maxPitch = 1.2f;
    public float volume = 0.5f;

    [Header("Image Fade Settings")]
    public Image uiImage;  // Reference to the Image component you want to fade
    public float imageFadeDelay = 1f;  // Delay before starting the fade
    public float imageFadeDuration = 1f;  // Duration of the fade

    public IntroCamera introCamera;

    private int index = 0;
    private Coroutine typingCoroutine;
    private CanvasGroup textCanvasGroup;
    private AudioSource audioSource;
    private bool hasStarted = false;

    void Start()
    {
        textCanvasGroup = textDisplay.GetComponent<CanvasGroup>();
        if (textCanvasGroup == null)
        {
            textCanvasGroup = textDisplay.gameObject.AddComponent<CanvasGroup>();
        }

        textCanvasGroup.alpha = 0;

        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.clip = typewriterSound;
        audioSource.volume = volume;

        if(introCamera == null)
        {
            FindFirstObjectByType<IntroCamera>();
        }

        StartCoroutine(StartTypingWithDelay());
    }

    IEnumerator StartTypingWithDelay()
    {
        yield return new WaitForSeconds(initialPauseTime);
        hasStarted = true;
        StartTyping();
    }

    void StartTyping()
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeText(textLines[index]));
    }

    IEnumerator TypeText(string line)
    {
        yield return StartCoroutine(FadeText(1));

        textDisplay.text = "";
        foreach (char letter in line.ToCharArray())
        {
            textDisplay.text += letter;
            PlayTypewriterSound();
            yield return new WaitForSeconds(typingSpeed);
        }

        yield return new WaitForSeconds(preFadePauseTime);
        yield return StartCoroutine(FadeText(0));
        textDisplay.text = "";

        yield return new WaitForSeconds(GetPauseTime());

        NextLine();
    }

    void NextLine()
    {
        if (index < textLines.Length - 1)
        {
            index++;
            StartTyping();
        }
        else
        {
            FadeUIImage();
        }
    }


    float GetPauseTime()
    {
        return (pauseTimes.Length > index) ? pauseTimes[index] : 1.5f;
    }

    IEnumerator FadeText(float targetAlpha)
    {
        float startAlpha = textCanvasGroup.alpha;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            textCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / fadeDuration);
            yield return null;
        }

        textCanvasGroup.alpha = targetAlpha;
    }

    void PlayTypewriterSound()
    {
        if (audioSource && typewriterSound)
        {
            audioSource.pitch = Random.Range(minPitch, maxPitch);
            audioSource.PlayOneShot(typewriterSound);
        }
    }

    public void FadeUIImage()
    {
        StartCoroutine(FadeImageFrom1To0());

        introCamera.cameraLocked = false;
    }

    IEnumerator FadeImageFrom1To0()
    {
        yield return new WaitForSeconds(imageFadeDelay); // Delay before starting the fade

        float startAlpha = uiImage.color.a;
        float elapsedTime = 0f;

        while (elapsedTime < imageFadeDuration)
        {
            elapsedTime += Time.deltaTime;
            Color newColor = uiImage.color;
            newColor.a = Mathf.Lerp(startAlpha, 0f, elapsedTime / imageFadeDuration);
            uiImage.color = newColor;
            yield return null;
        }

        Color finalColor = uiImage.color;
        finalColor.a = 0f;
        uiImage.color = finalColor;
    }
}
