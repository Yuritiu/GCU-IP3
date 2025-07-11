using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CommandMenuManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] public TMP_Text displayText;
    [SerializeField] public TMP_InputField hiddenInputField;
    [SerializeField] public TMP_Text logText;

    [Header("Settings")]
    [SerializeField] public float caretBlinkRate = 0.5f;

    private string[] validCommands = { "help", "clear", "version" };

    private bool caretVisible = true;
    private float caretTimer = 0f;
    private string rawInput = "";

    private void OnEnable()
    {
        rawInput = "";
        caretTimer = 0f;
        caretVisible = true;

        hiddenInputField.text = "";
        hiddenInputField.ActivateInputField();
    }

    void Update()
    {
        if (!hiddenInputField.isFocused)
            hiddenInputField.ActivateInputField();

        //Capture Input
        rawInput = hiddenInputField.text;

        //Caret Blinking
        caretTimer += Time.deltaTime;
        if (caretTimer >= caretBlinkRate)
        {
            caretVisible = !caretVisible;
            caretTimer = 0f;
        }

        //Add Blinking Caret to Text Input
        displayText.text = "> " + rawInput + (caretVisible ? "|" : "");

        if (Input.GetKeyDown(KeyCode.Return))
        {
            SubmitCommand(rawInput);
            hiddenInputField.text = "";
            rawInput = "";
            caretVisible = true;
            caretTimer = 0f;
        }
    }

    void SubmitCommand(string cmd)
    {
        cmd = cmd.Trim().ToLower();
        if (string.IsNullOrWhiteSpace(cmd)) return;

        //Check If Log Text > The Visible Text Area
        float logHeight = logText.preferredHeight;
        float visibleHeight = ((RectTransform)logText.transform.parent).rect.height;
        if (logHeight >= visibleHeight)
        {
            logText.text = "";
        }

        logText.text += $"\n> {cmd}";

        switch (cmd)
        {
            case "help":
                logText.text += "\navailable commands: " + string.Join(", ", validCommands);
                break;
            case "clear":
                logText.text = "";
                break;
            case "version":
                logText.text += "\ngame version: pre pre alpha";
                break;
            default:
                logText.text += $"\nInvalid command, type 'help'.";
                break;
        }
    }
}