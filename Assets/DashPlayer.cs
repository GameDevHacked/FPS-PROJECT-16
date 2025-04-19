using UnityEngine;
using UnityEngine.UI;

public class GhostRunnerMovement : MonoBehaviour
{
    [Header("Time Slow Settings")]
    [SerializeField] private float timeSlowScale = 0.3f;
    [SerializeField] private float timeSlowDuration = 4f;
    [SerializeField] private LayerMask airLayer;
    [SerializeField] private Slider timeSlowSlider;

    [Header("Dash Settings")]
    [SerializeField] private float dashDistance = 10f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    private bool isRightMouseHeld;
    private bool isTimeSlowed;
    private float timeSlowTimer;
    private bool isDashing;
    private float dashTimer;
    private Vector3 dashTargetPosition;
    private CharacterController characterController;

    private float lastSliderValue = -1f;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        if (timeSlowSlider != null)
        {
            timeSlowSlider.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        HandleInput();
        HandleTimeSlow();
        HandleDash();
    }

    private void HandleInput()
    {
        if (Input.GetMouseButtonDown(1))
        {
            isRightMouseHeld = true;
        }
        if (Input.GetMouseButtonUp(1))
        {
            isRightMouseHeld = false;
            EndTimeSlow();
        }

        if (isRightMouseHeld && !isTimeSlowed && !isDashing)
        {
            if (IsInAir())
            {
                StartTimeSlow();
            }
            else if (IsGrounded())
            {
                StartDash();
            }
        }
    }

    private bool IsInAir()
    {
        // Use airLayer for air detection instead of groundLayer
        return !Physics.Raycast(transform.position, Vector3.down, 1.5f, airLayer);
    }

    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, 1.1f, groundLayer);
    }

    private void StartTimeSlow()
    {
        isTimeSlowed = true;
        timeSlowTimer = timeSlowDuration;
        Time.timeScale = timeSlowScale;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;

        if (timeSlowSlider != null)
        {
            timeSlowSlider.gameObject.SetActive(true);
            timeSlowSlider.maxValue = timeSlowDuration;
            timeSlowSlider.value = timeSlowDuration;
            lastSliderValue = timeSlowDuration;
        }
    }

    private void HandleTimeSlow()
    {
        if (isTimeSlowed)
        {
            timeSlowTimer -= Time.unscaledDeltaTime;

            if (timeSlowSlider != null && Mathf.Abs(timeSlowSlider.value - timeSlowTimer) > 0.01f)
            {
                timeSlowSlider.value = timeSlowTimer;
                lastSliderValue = timeSlowTimer;
            }

            if (timeSlowTimer <= 0 || !isRightMouseHeld)
            {
                EndTimeSlow();
            }
        }
    }

    private void EndTimeSlow()
    {
        if (isTimeSlowed)
        {
            isTimeSlowed = false;
            Time.timeScale = 1f;
            Time.fixedDeltaTime = 0.02f;

            if (timeSlowSlider != null)
            {
                timeSlowSlider.gameObject.SetActive(false);
            }
        }
    }

    private void StartDash()
    {
        isDashing = true;
        dashTimer = dashDuration;
        dashTargetPosition = transform.position + transform.forward * dashDistance;
    }

    private void HandleDash()
    {
        if (isDashing)
        {
            dashTimer -= Time.deltaTime;
            float lerpFactor = 1 - (dashTimer / dashDuration);

            Vector3 newPosition = Vector3.Lerp(transform.position, dashTargetPosition, lerpFactor);
            characterController.Move(newPosition - transform.position);

            if (dashTimer <= 0)
            {
                isDashing = false;
            }
        }
    }

    private void OnDestroy()
    {
        if (isTimeSlowed)
        {
            Time.timeScale = 1f;
            Time.fixedDeltaTime = 0.02f;
        }
    }
}
