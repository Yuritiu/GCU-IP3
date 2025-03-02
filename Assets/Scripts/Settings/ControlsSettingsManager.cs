using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ControlsSettingsManager : MonoBehaviour
{
    [Header("Mouse Sensitivity")]
    [SerializeField] private Slider sensitivitySlider;
    [SerializeField] private TextMeshProUGUI sensitivityValueText;
    [SerializeField] private CameraController cameraController;

    private const string MouseSensitivityKey = "MouseSensitivity";
    const int sensMultiplier = 3;

    private void Start()
    {
        LoadSettings();

        sensitivitySlider.minValue = 0;
        sensitivitySlider.maxValue = 100;

        sensitivitySlider.onValueChanged.AddListener(OnSensitivityChanged);
    }

    private void OnSensitivityChanged(float value)
    {
        if(cameraController != null)
        {
            cameraController.sensitivity = value * sensMultiplier;
            
        }

        UpdateSensitivityDisplay();
        SaveSettings();
    }

    private void UpdateSensitivityDisplay()
    {
        sensitivityValueText.text = sensitivitySlider.value.ToString("F0");
    }

    public void SaveSettings()
    {
        int savedSensitivity = Mathf.RoundToInt(sensitivitySlider.value);
        Debug.Log("SENSITIVITY: " + savedSensitivity);
        PlayerPrefs.SetInt(MouseSensitivityKey, savedSensitivity);
        PlayerPrefs.Save();
    }

    public void LoadSettings()
    {
        int savedSensitivity = PlayerPrefs.GetInt(MouseSensitivityKey, 50);
        if (cameraController != null)
        {
            cameraController.sensitivity = savedSensitivity * sensMultiplier;
            Debug.Log("LOADED SENSITIVITY: " + savedSensitivity);
        }
        sensitivitySlider.value = savedSensitivity;
        UpdateSensitivityDisplay();
    }
}
