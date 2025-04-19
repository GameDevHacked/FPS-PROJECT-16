using UnityEngine;

public class WeaponSway : MonoBehaviour
{
    [Header("Sway Settings")]
    public float swayAmount = 0.05f;
    public float maxSwayAmount = 0.06f;
    public float swaySmoothness = 6f;

    [Header("Rotation Sway Settings")]
    public float rotationSwayAmount = 4f;
    public float maxRotationSway = 5f;
    public float rotationSmoothness = 8f;

    private Vector3 initialPosition;
    private Quaternion initialRotation;

    void Start()
    {
        initialPosition = transform.localPosition;
        initialRotation = transform.localRotation;
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        // Position sway
        float moveX = Mathf.Clamp(-mouseX * swayAmount, -maxSwayAmount, maxSwayAmount);
        float moveY = Mathf.Clamp(-mouseY * swayAmount, -maxSwayAmount, maxSwayAmount);
        Vector3 swayPos = new Vector3(moveX, moveY, 0);

        transform.localPosition = Vector3.Lerp(transform.localPosition, initialPosition + swayPos, Time.deltaTime * swaySmoothness);

        // Rotation sway
        float tiltY = Mathf.Clamp(mouseX * rotationSwayAmount, -maxRotationSway, maxRotationSway);
        float tiltX = Mathf.Clamp(mouseY * rotationSwayAmount, -maxRotationSway, maxRotationSway);
        Quaternion swayRot = Quaternion.Euler(-tiltX, tiltY, 0);

        transform.localRotation = Quaternion.Slerp(transform.localRotation, initialRotation * swayRot, Time.deltaTime * rotationSmoothness);
    }
}
