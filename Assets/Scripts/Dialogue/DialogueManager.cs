using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class DialogueManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image characterPortrait;
    [SerializeField] private RectTransform characterNameContainer;
    [SerializeField] private TextMeshProUGUI characterNameText;
    [SerializeField] private TextMeshProUGUI textBox;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private TextMeshProUGUI continueText;

    [Header("Dialogue Data")]
    [SerializeField] private DialogueData[] dialogueDatas;

    [Header("Animation Settings")]
    [SerializeField] private float animationSpeed = 2f;
    [SerializeField] private float animationDuration = 2f;
    [SerializeField] private float continueTextFadeDuration = 0.5f;
    [SerializeField] private float nameStartOffset = -400f;
    [SerializeField] private float portraitStartOffset = -400f;

    [Header("Text Settings")]
    [SerializeField] private bool isTyping = false;
    public float typingSpeed = 0.05f;

    private string[] currentTextLines;
    private Sprite[] currentLinePortraits;
    private int currentLineIndex = 0;

    private DialogueData currentDialogueData;

    private Vector2 nameStartPos;
    private Vector2 portraitStartPos;
    private Vector2 nameTargetPos;
    private Vector2 portraitTargetPos;

    private void Awake()
    {
        nameTargetPos = characterNameContainer.anchoredPosition;
        portraitTargetPos = characterPortrait.rectTransform.anchoredPosition;

        nameStartPos = nameTargetPos + new Vector2(0, nameStartOffset);
        portraitStartPos = portraitTargetPos + new Vector2(portraitStartOffset, 0);

        characterNameContainer.anchoredPosition = nameStartPos;
        characterPortrait.rectTransform.anchoredPosition = portraitStartPos;

        textBox.text = "";
    }

    public void TriggerDialogue(int dialogueIndex)
    {
        StartCoroutine(AnimateDialogueUI(dialogueIndex));
    }

    private IEnumerator AnimateDialogueUI(int dialogueIndex)
    {
        float elapsedTime = 0f;
        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.deltaTime * animationSpeed;
            float t = Mathf.Clamp01(elapsedTime / animationDuration);
            float easeOutT = 1 - Mathf.Pow(1 - t, 3);

            characterNameContainer.anchoredPosition = Vector2.Lerp(nameStartPos, nameTargetPos, easeOutT);
            characterPortrait.rectTransform.anchoredPosition = Vector2.Lerp(portraitStartPos, portraitTargetPos, easeOutT);

            yield return null;
        }

        characterNameContainer.anchoredPosition = nameTargetPos;
        characterPortrait.rectTransform.anchoredPosition = portraitTargetPos;

        StartDialogue(dialogueIndex);
    }

    public void StartDialogue(int dialogueIndex)
    {
        if (dialogueIndex < 0 || dialogueIndex >= dialogueDatas.Length)
        {
            Debug.LogError("Dialogue index out of range!");
            return;
        }

        currentDialogueData = dialogueDatas[dialogueIndex];
        characterNameText.text = currentDialogueData.characterName;
        currentTextLines = currentDialogueData.textLines;
        currentLinePortraits = currentDialogueData.linePortraits;
        currentLineIndex = 0;

        continueText.gameObject.SetActive(false);
        continueText.alpha = 0f;

        DisplayNextLine();
    }

    public void DisplayNextLine()
    {
        if (currentTextLines == null || currentLineIndex >= currentTextLines.Length)
        {
            StartCoroutine(ReverseAnimateDialogueUI());
            textBox.text = "";
            return;
        }

        StartCoroutine(TypeLine(currentTextLines[currentLineIndex]));

        if (currentLinePortraits != null && currentLinePortraits.Length > currentLineIndex)
        {
            characterPortrait.sprite = currentLinePortraits[currentLineIndex];
        }

        currentLineIndex++;
    }

    private IEnumerator TypeLine(string line)
    {
        textBox.text = "";
        isTyping = true;

        foreach (char letter in line.ToCharArray())
        {
            textBox.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(FadeInContinueText());
    }

    private IEnumerator FadeInContinueText()
    {
        continueText.gameObject.SetActive(true);
        float elapsedTime = 0f;

        while (elapsedTime < continueTextFadeDuration)
        {
            elapsedTime += Time.deltaTime;
            continueText.alpha = Mathf.Clamp01(elapsedTime / continueTextFadeDuration);
            yield return null;
        }

        continueText.alpha = 1f;
    }

    private void Update()
    {
        if (!isTyping && continueText.gameObject.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
            {
                continueText.gameObject.SetActive(false);
                continueText.alpha = 0f;
                DisplayNextLine();
            }
        }
    }

    private IEnumerator ReverseAnimateDialogueUI()
    {
        float elapsedTime = 0f;
        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.deltaTime * animationSpeed;
            float t = Mathf.Clamp01(elapsedTime / animationDuration);
            float easeInT = Mathf.Pow(t, 3);

            characterNameContainer.anchoredPosition = Vector2.Lerp(nameTargetPos, nameStartPos, easeInT);
            characterPortrait.rectTransform.anchoredPosition = Vector2.Lerp(portraitTargetPos, portraitStartPos, easeInT);

            yield return null;
        }

        characterNameContainer.anchoredPosition = nameStartPos;
        characterPortrait.rectTransform.anchoredPosition = portraitStartPos;
    }

    public void SetTypingSpeed(float speed)
    {
        typingSpeed = speed;
    }
}
