using System.Collections;
using UnityEngine;
using TMPro;

public class StatusDropdown : MonoBehaviour
{
    [Header("Status Effect Data")]
    [SerializeField] private string[] status = new string[7];
    [SerializeField] private string[] description = new string[7];
    [SerializeField] private Color[] statusColour = new Color[7];

    [Header("UI References")]
    [SerializeField] private GameObject parentObject;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descriptionText;

    [Header("Animation Settings")]
    [SerializeField] private float moveDuration = 1.0f;
    [SerializeField] private float typewriterSpeed = 0.05f;
    [SerializeField] private Vector2 startPos = new Vector2(0, Screen.height);
    [SerializeField] private Vector2 endPos = new Vector2(0, Screen.height - 200);
    [SerializeField] private float additionalPauseTime = 2.0f;

    //Display status effect
    public void DisplayStatusEffect(int index)
    {
        if (index < 0 || index >= status.Length) return;
        titleText.text = status[index];
        titleText.color = statusColour[index];
        descriptionText.color = Color.white;
        StartCoroutine(MoveObjectAndTypewriterEffect(index));
    }

    //Move object and show description
    private IEnumerator MoveObjectAndTypewriterEffect(int index)
    {
        RectTransform parentRect = parentObject.GetComponent<RectTransform>();
        parentRect.anchoredPosition = startPos;
        float elapsedTime = 0f;

        while (elapsedTime < moveDuration)
        {
            elapsedTime += Time.deltaTime;
            parentRect.anchoredPosition = Vector2.Lerp(startPos, endPos, elapsedTime / moveDuration);
            yield return null;
        }

        StartCoroutine(TypewriterEffect(description[index]));
        float pauseDuration = description[index].Length * typewriterSpeed + additionalPauseTime;
        yield return new WaitForSeconds(pauseDuration);
        StartCoroutine(ReverseTypewriterEffect());
        yield return new WaitForSeconds(description[index].Length * typewriterSpeed);
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(MoveOffScreenAndReset());
    }

    //Typewriter effect
    private IEnumerator TypewriterEffect(string fullText)
    {
        descriptionText.text = "";
        for (int i = 0; i < fullText.Length; i++)
        {
            descriptionText.text = fullText.Substring(0, i + 1);
            yield return new WaitForSeconds(typewriterSpeed);
        }
    }

    //Reverse typewriter effect
    private IEnumerator ReverseTypewriterEffect()
    {
        string currentText = descriptionText.text;
        float elapsedTime = 0f;
        while (elapsedTime < currentText.Length * typewriterSpeed)
        {
            elapsedTime += Time.deltaTime;
            int length = Mathf.FloorToInt(Mathf.Lerp(currentText.Length, 0, elapsedTime / (currentText.Length * typewriterSpeed)));
            descriptionText.text = currentText.Substring(0, length);
            yield return null;
        }
        descriptionText.text = "";
    }

    //Move object off screen and reset
    private IEnumerator MoveOffScreenAndReset()
    {
        RectTransform parentRect = parentObject.GetComponent<RectTransform>();
        float elapsedTime = 0f;
        Vector2 offScreenPos = new Vector2(0, -Screen.height * -2);

        while (elapsedTime < moveDuration)
        {
            elapsedTime += Time.deltaTime;
            parentRect.anchoredPosition = Vector2.Lerp(endPos, offScreenPos, elapsedTime / moveDuration);
            yield return null;
        }
        descriptionText.text = "";
    }
}
