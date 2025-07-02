using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ControlsSettingsManager : MonoBehaviour
{
    [Header("Mouse Sensitivity")]
    [SerializeField] private Slider sensitivitySlider;
    [SerializeField] private TextMeshProUGUI sensitivityValueText;
    [SerializeField] private CameraController cameraController;
    [SerializeField] private MultiplayerCameraController multiplayerCameraController;

    private const string MouseSensitivityKey = "MouseSensitivity";
    const int sensMultiplier = 3;

    private void Start()
    {
        LoadSettings();

        cameraController = FindFirstObjectByType<CameraController>();
        multiplayerCameraController = FindFirstObjectByType<MultiplayerCameraController>();

        sensitivitySlider.minValue = 0;
        sensitivitySlider.maxValue = 100;

        sensitivitySlider.onValueChanged.AddListener(OnSensitivityChanged);
    }

    private void OnSensitivityChanged(float value)
    {
        if(cameraController != null)
        {
            cameraController.sensitivity = value * sensMultiplier;
            Debug.Log("Changed Singleplayer Camera Sens");
        }

        if(multiplayerCameraController != null)
        {
            multiplayerCameraController.sensitivity = value * sensMultiplier;
            Debug.Log("Changed Multiplayer Camera Sens");
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
        Debug.Log("Sensitivity Saved: " + savedSensitivity);
        PlayerPrefs.SetInt(MouseSensitivityKey, savedSensitivity);
        PlayerPrefs.Save();
    }

    public void LoadSettings()
    {
        int savedSensitivity = PlayerPrefs.GetInt(MouseSensitivityKey, 50);
        if (cameraController != null)
        {
            cameraController.sensitivity = savedSensitivity * sensMultiplier;
            Debug.Log("LOADED SINGLE PLAYER SENSITIVITY: " + savedSensitivity);
        }
        if (multiplayerCameraController != null)
        {
            multiplayerCameraController.sensitivity = savedSensitivity * sensMultiplier;
            Debug.Log("LOADED MULTIPLAYER SENSITIVITY: " + savedSensitivity);
        }
        sensitivitySlider.value = savedSensitivity;

        // Debugging the updated sensitivity display
        Debug.Log("Updated Sensitivity Display: " + sensitivitySlider.value);
        UpdateSensitivityDisplay();
    }
}