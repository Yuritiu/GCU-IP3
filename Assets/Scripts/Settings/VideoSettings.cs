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
    Resolution[] resolutions;

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
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();
        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height + " @ " + resolutions[i].refreshRate + "Hz";
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height &&
                resolutions[i].refreshRate == Screen.currentResolution.refreshRate)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();

        resolutionDropdown.onValueChanged.AddListener(SetResolution);
    }

    void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen, resolution.refreshRate);

        PlayerPrefs.SetInt("Resolution", resolutionIndex);
        Debug.Log(resolutionIndex);
        PlayerPrefs.Save();
    }

    public void LoadResolution()
    {
        int savedIndex = PlayerPrefs.GetInt("Resolution");
        if (savedIndex >= 0 && savedIndex < resolutions.Length)
        {
            SetResolution(savedIndex);
            resolutionDropdown.value = savedIndex;
            resolutionDropdown.RefreshShownValue();
        }
    }

    //////////////////////////////////////RESOLUTION STUFF END///////////////////////////////////////////////////////////
}
