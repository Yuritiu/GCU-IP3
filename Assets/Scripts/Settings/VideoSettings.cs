using System.Collections;
using System.Collections.Generic;
using TMPro;
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

    [Header("Resolution Variables")]
    [SerializeField] TMP_Dropdown resolutionDropdown;
    List<Resolution> filteredResolutions = new List<Resolution>();

    void Start()
    {
        LoadFOV();
        PopulateResolutionDropdown();
        LoadResolution();
    }

    public void SaveFOV()
    {
        savedFOV = fovSlider.value;
        fovValueText.text = savedFOV.ToString();
        PlayerPrefs.SetFloat("FOV", savedFOV);
        PlayerPrefs.Save();

        if (camera != null)
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
    }

    //////////////////////////////////////RESOLUTION STUFF///////////////////////////////////////////////////////////

    void PopulateResolutionDropdown()
    {
        Resolution[] allResolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();
        filteredResolutions.Clear();

        List<string> options = new List<string>();
        int currentResolutionIndex = 0;

        for (int i = 0; i < allResolutions.Length; i++)
        {
            if (allResolutions[i].width >= 1024 && allResolutions[i].height >= 768)
            {
                filteredResolutions.Add(allResolutions[i]);

                string option = allResolutions[i].width + " x " + allResolutions[i].height + " @ " + allResolutions[i].refreshRate + "Hz";
                options.Add(option);

                if (allResolutions[i].width == Screen.currentResolution.width && allResolutions[i].height == Screen.currentResolution.height &&
                    allResolutions[i].refreshRate == Screen.currentResolution.refreshRate)
                {
                    currentResolutionIndex = filteredResolutions.Count - 1;
                }
            }
        }

        if (filteredResolutions.Count == 0)
        {
            Debug.LogWarning("No resolutions at or above 1080p found!");
            return;
        }

        resolutionDropdown.AddOptions(options);

        int savedIndex = PlayerPrefs.GetInt("Resolution");
        if (savedIndex >= 0 && savedIndex < filteredResolutions.Count)
        {
            currentResolutionIndex = savedIndex;
        }

        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
        resolutionDropdown.onValueChanged.AddListener(SetResolution);
    }


    void SetResolution(int resolutionIndex)
    {
        Resolution resolution = filteredResolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen, resolution.refreshRate);

        PlayerPrefs.SetInt("Resolution", resolutionIndex);
        PlayerPrefs.Save();
    }

    public void LoadResolution()
    {
        int savedIndex = PlayerPrefs.GetInt("Resolution");
        if (savedIndex >= 0 && savedIndex < filteredResolutions.Count)
        {
            SetResolution(savedIndex);
            resolutionDropdown.value = savedIndex;
            resolutionDropdown.RefreshShownValue();
        }
    }

    //////////////////////////////////////RESOLUTION STUFF END///////////////////////////////////////////////////////////
}
