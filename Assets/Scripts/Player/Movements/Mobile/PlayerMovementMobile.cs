using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(CharacterController))]
public class FPPControllerMobile : MonoBehaviour
{
    [Header("Mobile Controls")]
    [SerializeField] private FloatingJoystick joystick;
    [SerializeField] private Button jumpButton;
    [SerializeField] private Button crouchToggle;
    [SerializeField] private Button sprintToggle;
    [SerializeField] private Button weapon1Button;
    [SerializeField] private Button weapon2Button;
    [SerializeField] private Button weapon3Button;
    [SerializeField] private Image jumpCooldownIndicator;

    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float runSpeed = 8f;
    [SerializeField] private float crouchSpeed = 2.5f;
    [SerializeField] private float jumpHeight = 1.5f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float crouchHeight = 1f;
    [SerializeField] private float standHeight = 2f;
    [SerializeField] private float crouchTransitionSpeed = 5f;

    [Header("Camera Settings")]
    [SerializeField] private float tiltAmount = 5f;
    [SerializeField] private float tiltSmoothTime = 0.1f;
    [SerializeField] private Transform cameraHolder;
    [SerializeField] private Vector3 cameraCrouchOffset = new Vector3(0f, -0.5f, 0f);

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundDistance = 0.4f;
    [SerializeField] private LayerMask groundMask;

    [Header("Weapon System")]
    [SerializeField] private List<GameObject> weaponArms = new List<GameObject>();
    [SerializeField] private Image[] weaponUIImages;
    [SerializeField] private Color activeColor = Color.white;
    [SerializeField] private Color inactiveColor = Color.gray;

    private CharacterController controller;
    private Vector3 velocity;
    private Vector3 cameraOriginalPos;
    private bool isGrounded;
    private bool isCrouching;
    private bool isSprinting;
    private float currentHeight;
    private float currentSpeed;
    private float targetTilt;
    private float tiltVelocity;
    private int currentWeaponIndex = -1;
    private Animator currentWeaponAnimator;
    private bool canJump = true;
    private bool mobileCrouchToggle;
    private bool mobileSprintToggle;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        currentHeight = standHeight;
        currentSpeed = walkSpeed;
        cameraOriginalPos = cameraHolder.localPosition;

        if (weaponArms.Count > 0) SwitchWeapon(0);
    }

    private void Start()
    {
        // Mobile button setup
        if (jumpButton != null) jumpButton.onClick.AddListener(OnJumpButtonPressed);
        if (crouchToggle != null) crouchToggle.onClick.AddListener(ToggleCrouch);
        if (sprintToggle != null) sprintToggle.onClick.AddListener(ToggleSprint);

        // Weapon buttons
        if (weapon1Button != null) weapon1Button.onClick.AddListener(() => SwitchWeapon(0));
        if (weapon2Button != null) weapon2Button.onClick.AddListener(() => SwitchWeapon(1));
        if (weapon3Button != null) weapon3Button.onClick.AddListener(() => SwitchWeapon(2));

        // Hide cooldown indicator if exists
        if (jumpCooldownIndicator != null) jumpCooldownIndicator.gameObject.SetActive(false);
    }

    private void Update()
    {
        HandleGroundCheck();
        HandleMovement();
        HandleCrouching();
        HandleSprinting();
        HandleJump();
        HandleGravity();
        HandleCameraTilt();
        HandleWeaponInput();
        UpdateAnimatorParameters();
    }

    #region Movement Systems
    private void HandleGroundCheck()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if (isGrounded && velocity.y < 0) velocity.y = -2f;
    }

    private void HandleMovement()
    {
        // Combined PC and mobile input
        float x = Input.GetAxis("Horizontal") + (joystick != null ? joystick.Horizontal : 0f);
        float z = Input.GetAxis("Vertical") + (joystick != null ? joystick.Vertical : 0f);

        Vector3 move = transform.right * x + transform.forward * z;
        if (move.magnitude > 1f) move.Normalize();

        controller.Move(move * currentSpeed * Time.deltaTime);
    }

    private void HandleCrouching()
    {
        // PC input or mobile toggle
        bool shouldCrouch = Input.GetKey(KeyCode.LeftControl) || mobileCrouchToggle;

        if (isCrouching != shouldCrouch)
        {
            isCrouching = shouldCrouch;
            if (currentWeaponAnimator != null)
                currentWeaponAnimator.SetBool("isCrouching", isCrouching);
        }

        float targetHeight = isCrouching ? crouchHeight : standHeight;
        currentHeight = Mathf.Lerp(currentHeight, targetHeight, crouchTransitionSpeed * Time.deltaTime);
        controller.height = currentHeight;
        controller.center = new Vector3(0, currentHeight / 2f, 0);

        // Camera position adjustment
        Vector3 targetCameraPos = isCrouching ? cameraOriginalPos + cameraCrouchOffset : cameraOriginalPos;
        cameraHolder.localPosition = Vector3.Lerp(cameraHolder.localPosition, targetCameraPos, Time.deltaTime * crouchTransitionSpeed);
    }

    private void HandleSprinting()
    {
        // PC input or mobile toggle
        bool shouldSprint = (!isCrouching && (Input.GetKey(KeyCode.LeftShift) || mobileSprintToggle));

        // Get movement input to check if player is moving
        float x = Input.GetAxis("Horizontal") + (joystick != null ? joystick.Horizontal : 0f);
        float z = Input.GetAxis("Vertical") + (joystick != null ? joystick.Vertical : 0f);
        Vector3 move = transform.right * x + transform.forward * z;

        // Only sprint if moving forward
        shouldSprint &= z > 0.1f && move.magnitude > 0.1f;

        currentSpeed = isCrouching ? crouchSpeed : (shouldSprint ? runSpeed : walkSpeed);
    }

    private void HandleJump()
    {
        // PC input
        if (Input.GetButtonDown("Jump") && CanJump())
        {
            PerformJump();
        }
    }

    private void HandleGravity()
    {
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
    #endregion

    #region Camera Systems
    private void HandleCameraTilt()
    {
        float x = Input.GetAxis("Horizontal") + (joystick != null ? joystick.Horizontal : 0f);
        float z = Input.GetAxis("Vertical") + (joystick != null ? joystick.Vertical : 0f);
        Vector3 move = transform.right * x + transform.forward * z;

        targetTilt = (move.magnitude > 0.2f) ? -x * tiltAmount : 0f;
        float currentTilt = Mathf.SmoothDampAngle(cameraHolder.localEulerAngles.z, targetTilt, ref tiltVelocity, tiltSmoothTime);
        cameraHolder.localEulerAngles = new Vector3(cameraHolder.localEulerAngles.x, cameraHolder.localEulerAngles.y, currentTilt);
    }
    #endregion

    #region Weapon Systems
    private void HandleWeaponInput()
    {
        // PC input
        if (Input.GetKeyDown(KeyCode.Alpha1)) SwitchWeapon(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SwitchWeapon(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) SwitchWeapon(2);

        // Scroll wheel switching
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0f)
        {
            int newIndex = currentWeaponIndex + (scroll > 0f ? 1 : -1);
            if (newIndex >= weaponArms.Count) newIndex = 0;
            if (newIndex < 0) newIndex = weaponArms.Count - 1;
            SwitchWeapon(newIndex);
        }
    }

    public void SwitchWeapon(int index)
    {
        if (index < 0 || index >= weaponArms.Count) return;

        // Disable current weapon
        if (currentWeaponIndex >= 0 && currentWeaponIndex < weaponArms.Count)
        {
            weaponArms[currentWeaponIndex].SetActive(false);
        }

        // Enable new weapon
        currentWeaponIndex = index;
        weaponArms[currentWeaponIndex].SetActive(true);
        currentWeaponAnimator = weaponArms[currentWeaponIndex].GetComponent<Animator>();

        UpdateWeaponUI();
    }

    private void UpdateWeaponUI()
    {
        if (weaponUIImages == null || weaponUIImages.Length == 0) return;

        for (int i = 0; i < weaponUIImages.Length; i++)
        {
            if (i < weaponArms.Count)
                weaponUIImages[i].color = (i == currentWeaponIndex) ? activeColor : inactiveColor;
        }
    }
    #endregion

    #region Mobile Input Handlers
    public void OnJumpButtonPressed()
    {
        if (CanJump())
        {
            PerformJump();

            // Optional cooldown for mobile
            if (jumpCooldownIndicator != null)
            {
                StartCoroutine(JumpCooldownRoutine());
            }
        }
    }

    public void ToggleCrouch()
    {
        mobileCrouchToggle = !mobileCrouchToggle;
    }

    public void ToggleSprint()
    {
        mobileSprintToggle = !mobileSprintToggle;
    }
    #endregion

    #region Utility Methods
    private bool CanJump()
    {
        return isGrounded && !isCrouching && canJump;
    }

    private void PerformJump()
    {
        velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        if (currentWeaponAnimator != null) currentWeaponAnimator.SetTrigger("isJumping");
    }

    private void UpdateAnimatorParameters()
    {
        if (currentWeaponAnimator == null) return;

        float x = Input.GetAxis("Horizontal") + (joystick != null ? joystick.Horizontal : 0f);
        float z = Input.GetAxis("Vertical") + (joystick != null ? joystick.Vertical : 0f);
        Vector3 move = transform.right * x + transform.forward * z;

        currentWeaponAnimator.SetBool("isMoving", move.magnitude > 0.1f);
        currentWeaponAnimator.SetBool("isSprinting", isSprinting);

        // Sprint trigger
        if (isSprinting && move.magnitude > 0.1f)
        {
            currentWeaponAnimator.SetTrigger("isSprintUp");
        }
    }

    private IEnumerator JumpCooldownRoutine()
    {
        canJump = false;
        jumpCooldownIndicator.gameObject.SetActive(true);
        float timer = 0;

        while (timer < 0.5f) // Half second cooldown
        {
            timer += Time.deltaTime;
            jumpCooldownIndicator.fillAmount = 1 - (timer / 0.5f);
            yield return null;
        }

        jumpCooldownIndicator.gameObject.SetActive(false);
        canJump = true;
    }
    #endregion

    #region Public Methods
    public void AddWeapon(GameObject weaponArm)
    {
        weaponArm.SetActive(false);
        weaponArms.Add(weaponArm);
    }

    public void RemoveWeapon(int index)
    {
        if (index < 0 || index >= weaponArms.Count) return;
        if (index == currentWeaponIndex) SwitchWeapon(0);
        Destroy(weaponArms[index]);
        weaponArms.RemoveAt(index);
    }
    #endregion
}