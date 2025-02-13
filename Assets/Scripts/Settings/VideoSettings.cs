using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VideoSettings : MonoBehaviour
{
    [SerializeField] Slider fovSlider;
    [SerializeField] TextMeshProUGUI fovValueText;

    float savedFOV;

    void Start()
    {
        LoadFOV();
    }

    public void SaveFOV()
    {
        savedFOV = fovSlider.value;
        fovValueText.text = savedFOV.ToString();
        PlayerPrefs.SetFloat("FOV", savedFOV);
    }

    public void LoadFOV()
    {
        savedFOV = PlayerPrefs.GetFloat("FOV", 50);
        fovValueText.text = savedFOV.ToString();

        PlayerPrefs.SetFloat("FOV", savedFOV);

        fovSlider.value = savedFOV;

        Debug.Log("FOV Loaded: " + savedFOV);
    }
}
