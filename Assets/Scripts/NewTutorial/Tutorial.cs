using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Tutorial : MonoBehaviour
{
    public bool tutorialEnabled = false;
    public CameraController cameraController;
    public DialogueManager dialogueManager;
    private float orgSensitivity;
    private float tutSensitivity = 0f;

    [SerializeField] private DialogueData[] dialogueDatas;

    void Start()
    {
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void Update()
    {
        if (tutorialEnabled && cameraController != null)
        {
            cameraController.sensitivity = tutSensitivity;
        }
        else
        {
            cameraController.sensitivity = orgSensitivity;
        }
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindReferences();
    }

    void FindReferences()
    {
        cameraController = FindObjectOfType<CameraController>();
        dialogueManager = FindObjectOfType<DialogueManager>();

        if (cameraController == null)
        {
            Debug.LogWarning("CameraController not found in the scene.");
        }

        if (dialogueManager == null)
        {
            Debug.LogWarning("DialogueManager not found in the scene.");
        }

        orgSensitivity = cameraController.sensitivity;


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