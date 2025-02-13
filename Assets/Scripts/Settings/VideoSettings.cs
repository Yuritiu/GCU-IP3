using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class VideoSettings : MonoBehaviour
{
    [Header("FOV Variables")]
    [SerializeField] Slider fovSlider;
    [SerializeField] TextMeshProUGUI fovValueText;
    [SerializeField] Camera camera;

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

        if(camera != null)
        {
            camera.fieldOfView = savedFOV;
        }
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
