using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class DevConsole : MonoBehaviour
{
    public static DevConsole Instance;

    [SerializeField] GameObject consolePanel;
    [SerializeField] TMP_InputField inputField;
    [SerializeField] TextMeshProUGUI suggestionText;
    [SerializeField] public TextMeshProUGUI debugText;
    bool isConsoleOpen = false;
    CommandList commandList;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        commandList = GetComponent<CommandList>();
        consolePanel.SetActive(false);

        inputField.onEndEdit.AddListener(OnSubmit);
        inputField.onValueChanged.AddListener(delegate { OnTextChanged(); });
    }

    void Update()
    {
        //` Key
        if (Input.GetKeyDown(KeyCode.BackQuote))
        {
            ToggleConsole();
        }
    }

    void ToggleConsole()
    {
        isConsoleOpen = !isConsoleOpen;
        consolePanel.SetActive(isConsoleOpen);
        if (isConsoleOpen)
        {
            inputField.ActivateInputField();
        }
    }

    public void OnTextChanged()
    {
        string input = inputField.text;
        string suggestion = commandList.GetAutoCompleteSuggestion(input);
        suggestionText.text = suggestion;
    }

    public void OnSubmit(string input)
    {
        if (!isConsoleOpen || string.IsNullOrEmpty(input))
            return;

        if (commandList.ExecuteCommand(input))
        {
            inputField.text = "";
            suggestionText.text = "";
        }

        //Keep Focus
        inputField.ActivateInputField();
    }
}
