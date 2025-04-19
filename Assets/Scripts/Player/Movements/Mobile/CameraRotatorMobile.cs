using UnityEngine;
using UnityEngine.UI; // Required for UI components

public class CameraRotation : MonoBehaviour
{
    [Header("Rotation Settings")]
    [Range(10f, 500f)] 
    public float mouseSensitivity = 100f;
    public Transform playerGameObject;

    [Header("Touch Input Reference")]
    public CameraTouchInput touchInput;

    [Header("UI References")]
    public Slider sensitivitySlider; // Reference to your UI slider
    public Text sensitivityValueText; // Optional: to display current value

    private float xRotation = 0f;

    private void Start()
    {
        // Initialize slider if it exists
        if (sensitivitySlider != null)
        {
            // Set slider range to match our sensitivity range
            sensitivitySlider.minValue = 10f;
            sensitivitySlider.maxValue = 500f;
            
            // Set current value
            sensitivitySlider.value = mouseSensitivity;
            
            // Add listener for when slider changes
            sensitivitySlider.onValueChanged.AddListener(UpdateSensitivityFromSlider);
            
            // Update display text if available
            UpdateSensitivityText();
        }
        
        // Lock and hide cursor for testing (optional)
        // Cursor.lockState = CursorLockMode.Locked;
    }

    // Public method to update sensitivity from UI slider
    public void SetSensitivity(float newSensitivity)
    {
        mouseSensitivity = newSensitivity;
        UpdateSensitivityText();
    }

    // Method called when slider value changes
    private void UpdateSensitivityFromSlider(float value)
    {
        SetSensitivity(value);
    }

    // Optional: Update UI text to show current sensitivity
    private void UpdateSensitivityText()
    {
        if (sensitivityValueText != null)
        {
            sensitivityValueText.text = mouseSensitivity.ToString("F0"); // Display as whole number
        }
    }

    private void Update()
    {
        if (touchInput == null || !touchInput.pressed)
            return;

        // Get touch input delta
        Vector2 touchDelta = touchInput.touchDelta;

        // Calculate rotation
        float mouseX = touchDelta.x * mouseSensitivity * Time.deltaTime;
        float mouseY = touchDelta.y * mouseSensitivity * Time.deltaTime;

        // Vertical rotation (up/down)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // Apply rotations
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerGameObject.Rotate(Vector3.up * mouseX);
    }
}