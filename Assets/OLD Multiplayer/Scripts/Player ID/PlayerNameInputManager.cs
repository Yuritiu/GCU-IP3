using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerNameInputManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] TMP_InputField nameInputField;
    public static string RawPlayerName { get; private set; }
    [SerializeField] TMP_Text errorText;

    private const int MaxNameLength = 16;

    void Awake()
    {
        //Load Saved Name Into Input Field If Available
        if (PlayerPrefs.HasKey("PlayerDisplayName"))
        {
            string savedName = PlayerPrefs.GetString("PlayerDisplayName");
            nameInputField.text = savedName;
            RawPlayerName = savedName;
            Debug.Log("Name Loaded as " + savedName);
        }

        errorText.gameObject.SetActive(false);
    }

    public bool ValidateAndSaveInput()
    {
        string inputName = nameInputField.text.Trim();

        if (string.IsNullOrEmpty(inputName))
        {
            ShowError("ERROR: Enter your name.");
            return false;
        }

        if (inputName.Length > MaxNameLength)
        {
            ShowError("ERROR: Name must be 16 characters or fewer.");
            return false;
        }

        string upper = inputName.ToUpperInvariant();
        if (upper.Contains("[") || upper.Contains("]") || upper.Contains("DEV"))
        {
            ShowError("ERROR: Name cannot contain '[', ']', or 'DEV'.");
            return false;
        }

        //Save Valid Name
        RawPlayerName = inputName;
        PlayerPrefs.SetString("PlayerDisplayName", inputName);
        PlayerPrefs.Save();

        Debug.Log("Name Saved as " + inputName);

        errorText.text = "";
        return true;
    }

    private void ShowError(string message)
    {
        errorText.text = message;
        errorText.gameObject.SetActive(true);
    }

    public string GetCurrentName() => nameInputField.text.Trim();
}