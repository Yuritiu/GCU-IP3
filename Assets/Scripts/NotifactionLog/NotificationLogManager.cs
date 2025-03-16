using UnityEngine;
using TMPro;
using System.Collections;

public class NotificationLogManager : MonoBehaviour
{
    public GameObject notificationPrefab;
    public Transform logContainer;
    public float displayDuration = 3f;

    public Color customBlue;
    public Color customRed;
    public Color customYellow;

    void Awake()
    {
        ColorUtility.TryParseHtmlString("#2680bf", out customBlue);
        ColorUtility.TryParseHtmlString("#bf4026", out customRed);
        ColorUtility.TryParseHtmlString("#bfa626", out customYellow);
    }

    public void SendFormattedNotification(string message, string highlightedText1, Color color1, string highlightedText2, Color color2)
    {
        string formattedMessage = $"<color=#{ColorUtility.ToHtmlStringRGB(color1)}>{highlightedText1}</color> {message} <color=#{ColorUtility.ToHtmlStringRGB(color2)}>{highlightedText2}</color>";
        AddNotification(formattedMessage);
    }

    public void AddNotification(string message)
    {
        GameObject newNotification = Instantiate(notificationPrefab, logContainer);
        newNotification.GetComponent<TextMeshProUGUI>().text = message;

        CanvasGroup canvasGroup = newNotification.GetComponent<CanvasGroup>() ?? newNotification.AddComponent<CanvasGroup>();

        StartCoroutine(FadeOut(newNotification, canvasGroup, displayDuration));
    }

    private IEnumerator FadeOut(GameObject notification, CanvasGroup canvasGroup, float delay)
    {
        yield return new WaitForSeconds(delay - 0.5f);

        float fadeDuration = 0.5f;
        float elapsedTime = 0;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1, 0, elapsedTime / fadeDuration);
            yield return null;
        }

        Destroy(notification);
    }
}
