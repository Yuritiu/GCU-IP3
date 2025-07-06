using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AutoGitVersionText : MonoBehaviour
{
    [SerializeField] public TextMeshProUGUI versionText;

    void Start()
    {
        TextAsset versionAsset = Resources.Load<TextAsset>("version");
        if (versionAsset != null)
        {
            versionText.text = versionAsset.text;
        }
        else
        {
            versionText.text = "Version: Unknown";
        }
    }
}