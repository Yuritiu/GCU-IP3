using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class disableUI : MonoBehaviour
{
    public static disableUI Instance;
    [SerializeField] GameObject bottleBackfireText;

    TextMeshProUGUI[] allText;
    public bool uiDisabled;

    void Awake()
    {
        Instance = this;
        uiDisabled = false;
    }

    void Start()
    {
        allText = FindObjectsOfType<TextMeshProUGUI>(true);
    }

    public void DisableAllText()
    {
        StartCoroutine(bottleTextHide());

        foreach (var tmpText in allText)
        {
            if (tmpText != null)
            {
                tmpText.enabled = false;
                uiDisabled = true;
            }
        }
    }

    public void EnableAllText()
    {
        foreach (var tmpText in allText)
        {
            if(tmpText != null)
            {
                tmpText.enabled = true;
                uiDisabled = false;
            }
        }
    }

    IEnumerator bottleTextHide()
    {
        bottleBackfireText.SetActive(true);
        yield return new WaitForSeconds(5f);
        bottleBackfireText.SetActive(false);
    }
}
