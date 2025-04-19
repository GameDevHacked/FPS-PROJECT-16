using UnityEngine;
using UnityEngine.UI;

public class SensitivitySlider : MonoBehaviour
{
    [Header("References")]
    public Slider sensitivitySlider;
    public CameraRotation cameraRotation;

    [Header("Settings")]
    public float minSensitivity = 10f;
    public float maxSensitivity = 500f;
    public float defaultValue = 100f;

    private void Start()
    {
        // Setup slider values
        sensitivitySlider.minValue = minSensitivity;
        sensitivitySlider.maxValue = maxSensitivity;
        sensitivitySlider.value = defaultValue;

        // Add listener for value changes
        sensitivitySlider.onValueChanged.AddListener(OnSensitivityChanged);

        // Set initial sensitivity
        if (cameraRotation != null)
        {
            cameraRotation.SetSensitivity(defaultValue);
        }
    }

    public void OnSensitivityChanged(float value)
    {
        if (cameraRotation != null)
        {
            cameraRotation.SetSensitivity(value);
        }
    }
}
