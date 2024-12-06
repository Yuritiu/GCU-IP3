using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StatusDropdown : MonoBehaviour
{
    [Header("Status Effect Data")]
    [SerializeField] private string[] status = new string[7];
    [SerializeField] private string[] description = new string[7];
    [SerializeField] private Color[] statusColour = new Color[7];

    [Header("Player Status Data")]
    [SerializeField] private string[] playerStatusNames = new string[2];
    [SerializeField] private Color[] playerStatusColours = new Color[2];

    [Header("UI References")]
    [SerializeField] private GameObject parentObject;
    [SerializeField] private TextMeshProUGUI playerText;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descriptionText;

    [Header("Animation Settings")]
    [SerializeField] private float moveDuration = 1.0f;
    [SerializeField] private float typewriterSpeed = 0.05f;
    [SerializeField] private Vector2 startPos = new Vector2(0, Screen.height);
    [SerializeField] private Vector2 endPos = new Vector2(0, Screen.height - 200);
    [SerializeField] private float additionalPauseTime = 2.0f;

    private Queue<(int playerIndex, int effectIndex)> effectQueue = new Queue<(int, int)>();
    private bool isProcessing = false;

    public void DisplayStatusEffect(int playerIndex, int effectIndex)
    {
        if (playerIndex < 0 || playerIndex >= playerStatusNames.Length) return;
        if (effectIndex < 0 || effectIndex >= status.Length) return;

        effectQueue.Enqueue((playerIndex, effectIndex));
        if (!isProcessing)
        {
            StartCoroutine(ProcessQueue());
        }
    }

    private IEnumerator ProcessQueue()
    {
        isProcessing = true;

        while (effectQueue.Count > 0)
        {
            var (playerIndex, effectIndex) = effectQueue.Dequeue();

            playerText.text = playerStatusNames[playerIndex];
            playerText.color = playerStatusColours[playerIndex];

            titleText.text = status[effectIndex];
            titleText.color = statusColour[effectIndex];
            descriptionText.color = Color.white;

            yield return StartCoroutine(MoveObjectAndTypewriterEffect(effectIndex));
        }

        isProcessing = false;
    }

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

        yield return StartCoroutine(TypewriterEffect(description[index]));
        float pauseDuration = description[index].Length * typewriterSpeed + additionalPauseTime;
        yield return new WaitForSeconds(pauseDuration);
        yield return StartCoroutine(ReverseTypewriterEffect());
        yield return new WaitForSeconds(description[index].Length * typewriterSpeed);
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(MoveOffScreenAndReset());
    }

    private IEnumerator TypewriterEffect(string fullText)
    {
        descriptionText.text = "";
        for (int i = 0; i < fullText.Length; i++)
        {
            descriptionText.text = fullText.Substring(0, i + 1);
            yield return new WaitForSeconds(typewriterSpeed);
        }
    }

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
