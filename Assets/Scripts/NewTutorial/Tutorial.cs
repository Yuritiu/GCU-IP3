using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Tutorial : MonoBehaviour
{
    public bool tutorialEnabled = false;
    public CameraController cameraController;
    public DialogueManager dialogueManager;
    public ControlsSettingsManager controlsSettingsManager;
    private CardDeck cardDeck;

    private float orgSensitivity;
    public float tutSensitivity = 0f;
    public bool sensitivityLoaded = false;

    public bool introCalled = false;
    private IntroTutorial introTutorial;


    [SerializeField] private DialogueData[] dialogueDatas;

    void Start()
    {
        introCalled = false;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;

    }

    void Update()
    {
        if (SceneManager.GetActiveScene().name == "Intro Scene")
        {
            tutorialEnabled = true;
        }

        if (SceneManager.GetActiveScene().name == "Main Menu")
        {
            tutorialEnabled = false;
            introCalled = false;
        }

        if (tutorialEnabled == true)
        {

        }
        else
        {
            if (SceneManager.GetActiveScene().name == "Game Scene")
            {
                if (introCalled == false)
                {
                    StartIntro();
                    introCalled = true;
                }
            }
        }
    }

    public void StartIntro()
    {
        cardDeck.CallIntro();
        introTutorial.EnableTutorialUI();
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindReferences();

        if (scene.name == "Game Scene")
        {
            if (introCalled == true)
            {
                introCalled = false;
            }
        }
    }

    void FindReferences()
    {
        cameraController = FindObjectOfType<CameraController>();
        dialogueManager = FindObjectOfType<DialogueManager>();
        controlsSettingsManager = FindFirstObjectByType<ControlsSettingsManager>();
        cardDeck = FindFirstObjectByType<CardDeck>();
        introTutorial = FindFirstObjectByType<IntroTutorial>();

        if (cameraController == null)
        {
            Debug.LogWarning("CameraController not found in the scene.");
        }

        if (dialogueManager == null)
        {
            Debug.LogWarning("DialogueManager not found in the scene.");
        }

        if (controlsSettingsManager == null)
        {
            Debug.LogWarning("controlsSettingsManager not found in the scene.");
        }

        if (controlsSettingsManager == null)
        {
            Debug.LogWarning("controlsSettingsManager not found in the scene.");
        }

        StartCoroutine(StartTutorial());
    }

    private IEnumerator StartTutorial()
    {
        if (tutorialEnabled && dialogueManager != null)
        {
            yield return new WaitForSeconds(2f);
            StartDialogue(0);
        }
    }

    private void StartDialogue(int index)
    {
        if (index >= 0 && index < dialogueDatas.Length && dialogueManager != null)
        {
            dialogueManager.TriggerDialogue(index);
        }
    }
}