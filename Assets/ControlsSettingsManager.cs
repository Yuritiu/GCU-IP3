using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ControlsSettingsManager : MonoBehaviour
{
    [Header("Mouse Sensitivity")]
    [SerializeField] private Slider sensitivitySlider;
    [SerializeField] private TextMeshProUGUI sensitivityValueText;
    [SerializeField] private Freelook freelookScript;

    private const string MouseSensitivityKey = "MouseSensitivity";

    private void Start()
    {
        LoadSettings();

        sensitivitySlider.minValue = 0;
        sensitivitySlider.maxValue = 100;

        sensitivitySlider.onValueChanged.AddListener(OnSensitivityChanged);
    }

    private void OnSensitivityChanged(float value)
    {
        freelookScript.mouseSensitivity = value * 10f;
        UpdateSensitivityDisplay();
        SaveSettings();
    }

    private void UpdateSensitivityDisplay()
    {
        sensitivityValueText.text = sensitivitySlider.value.ToString("F0");
    }

    private void SaveSettings()
    {
        PlayerPrefs.SetFloat(MouseSensitivityKey, freelookScript.mouseSensitivity);
        PlayerPrefs.Save();
    }

    public void LoadSettings()
    {
        float savedSensitivity = PlayerPrefs.GetFloat(MouseSensitivityKey, 350f);
        freelookScript.mouseSensitivity = savedSensitivity;
        sensitivitySlider.value = savedSensitivity / 10f;
        UpdateSensitivityDisplay();
    }
}
