using System.Collections;
using UnityEngine;
using TMPro;

public class IntroTutorial : MonoBehaviour
{
    [Header("Tutorial Elements")]
    public TextMeshProUGUI TutorialTitle;
    public TextMeshProUGUI StepOne;
    public TextMeshProUGUI StepTwo;
    public TextMeshProUGUI StepThree;
    public TextMeshProUGUI StepFour;

    [Header("Settings")]
    public float fadeOutDelay = 2f;
    public float fadeOutDuration = 1f;

    private CanvasGroup canvasGroup;
    private int completedSteps = 0;

    private GameManager gameManager;

    void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();

        if (gameManager.firstStepsTutorial == false)
        {
            canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        else
            return;
    }

    public void CompleteStep(int stepNumber)
    {
        TextMeshProUGUI stepText = null;
        switch (stepNumber)
        {
            case 1: stepText = StepOne; break;
            case 2: stepText = StepTwo; break;
            case 3: stepText = StepThree; break;
            case 4: stepText = StepFour; break;
            default: return;
        }

        if (stepText != null)
        {
            string originalText = stepText.text;
            if (originalText.Length > 1)
            {
                string firstChar = originalText.Substring(0, 1);
                string remainingText = originalText.Substring(1);
                stepText.color = Color.green; // Set base text color to green for strikethrough
                stepText.text = firstChar + "<s>" + remainingText + "</s>";
            }
            else
            {
                stepText.color = Color.green; // Set base text color to green for single-character text
                stepText.text = "<s>" + originalText + "</s>";
            }

            completedSteps++;
            if (completedSteps >= 4) StartCoroutine(FadeOutAndDelete());
        }
    }

    private IEnumerator FadeOutAndDelete()
    {
        yield return new WaitForSeconds(fadeOutDelay);

        float elapsedTime = 0f;
        while (elapsedTime < fadeOutDuration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeOutDuration);
            yield return null;
        }

        canvasGroup.alpha = 0f;
        gameManager.firstStepsTutorial = true;
        Destroy(gameObject);
    }
}
