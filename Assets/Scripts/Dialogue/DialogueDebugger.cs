using System.Collections;
using UnityEngine;

public class DialogueDebugger : MonoBehaviour
{
    [Header("Dialogue Data")]
    [SerializeField] private DialogueData[] dialogueDatas;

    private DialogueManager dialogueManager;

    void Start()
    {
        dialogueManager = FindFirstObjectByType<DialogueManager>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            StartDialogue(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            StartDialogue(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            StartDialogue(2);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            StartDialogue(3);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            StartDialogue(4);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            StartDialogue(5);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            StartDialogue(6);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            StartDialogue(7);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            StartDialogue(8);
        }
    }

    private void StartDialogue(int index)
    {
        if (index >= 0 && index < dialogueDatas.Length)
        {
            dialogueManager.TriggerDialogue(index);
        }
    }
}
