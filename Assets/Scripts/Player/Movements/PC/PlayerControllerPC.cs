using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

[RequireComponent(typeof(CharacterController))]
public class FPPController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float runSpeed = 8f;
    [SerializeField] private float crouchSpeed = 2.5f;
    [SerializeField] private float jumpHeight = 1.5f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float crouchHeight = 1f;
    [SerializeField] private float standHeight = 2f;
    [SerializeField] private float crouchTransitionSpeed = 5f;

    [Header("Camera Tilt Settings")]
    [SerializeField] private float tiltAmount = 5f;
    [SerializeField] private float tiltSmoothTime = 0.1f;
    [SerializeField] private Transform cameraHolder;
    [SerializeField] private Vector3 cameraOriginalPos;
    [SerializeField] private Vector3 cameraCrouchOffset = new Vector3(0f, -0.5f, 0f);

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundDistance = 0.4f;
    [SerializeField] private LayerMask groundMask;

    [Header("Weapon System")]
    [SerializeField] private List<GameObject> weaponArms = new List<GameObject>();
    private int currentWeaponIndex = -1;
    private Animator currentWeaponAnimator;

    [Header("Weapon UI")]
    public Image[] weaponUIImages; // Set each weapon's image in inspector
    public Color activeColor = Color.white;
    public Color inactiveColor = Color.gray;

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;
    private bool isCrouching;
    private float currentHeight;
    private float currentSpeed;
    private float targetTilt;
    private float tiltVelocity;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        currentHeight = standHeight;
        currentSpeed = walkSpeed;

        if (cameraHolder == null)
        {
            Debug.LogError("Camera holder not assigned!");
        }
        else
        {
            cameraOriginalPos = cameraHolder.localPosition;
        }

        if (weaponArms.Count > 0)
        {
            SwitchWeapon(0);
        }
    }

    private void Update()
    {
        // Ground Check
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if (isGrounded && velocity.y < 0) velocity.y = -2f;

        // Movement Input
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 move = transform.right * x + transform.forward * z;
        if (move.magnitude > 1f) move.Normalize();

        // Crouching
        isCrouching = Input.GetKey(KeyCode.LeftControl);
        if (currentWeaponAnimator != null) currentWeaponAnimator.SetBool("isCrouching", isCrouching);

        float targetHeight = isCrouching ? crouchHeight : standHeight;
        currentHeight = Mathf.Lerp(currentHeight, targetHeight, crouchTransitionSpeed * Time.deltaTime);
        controller.height = currentHeight;
        controller.center = new Vector3(0, currentHeight / 2f, 0);

        if (cameraHolder != null)
        {
            Vector3 targetCameraPos = isCrouching ? cameraOriginalPos + cameraCrouchOffset : cameraOriginalPos;
            cameraHolder.localPosition = Vector3.Lerp(cameraHolder.localPosition, targetCameraPos, Time.deltaTime * crouchTransitionSpeed);
        }

        // Sprinting
        bool isSprinting = !isCrouching && Input.GetKey(KeyCode.LeftShift) && move.magnitude > 0.1f;
        currentSpeed = isCrouching ? crouchSpeed : (isSprinting ? runSpeed : walkSpeed);
        controller.Move(move * currentSpeed * Time.deltaTime);

        // Jump
        if (Input.GetButtonDown("Jump") && isGrounded && !isCrouching)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            if (currentWeaponAnimator != null) currentWeaponAnimator.SetTrigger("isJumping");
        }

        // Apply Gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // Camera Tilt
        if (cameraHolder != null)
        {
            targetTilt = (move.magnitude > 0.2f) ? -x * tiltAmount : 0f;
            float currentTilt = Mathf.SmoothDampAngle(cameraHolder.localEulerAngles.z, targetTilt, ref tiltVelocity, tiltSmoothTime);
            cameraHolder.localEulerAngles = new Vector3(cameraHolder.localEulerAngles.x, cameraHolder.localEulerAngles.y, currentTilt);
        }

        // Animator Parameters
        if (currentWeaponAnimator != null)
        {
            currentWeaponAnimator.SetBool("isMoving", move.magnitude > 0.1f);
            currentWeaponAnimator.SetBool("isSprinting", isSprinting);
            HandleSprintUpTrigger(isSprinting, move);
        }

        HandleWeaponInput();
    }

    private void HandleSprintUpTrigger(bool isSprinting, Vector3 move)
    {
        if (isSprinting && move.magnitude > 0.1f && currentWeaponAnimator != null)
        {
            currentWeaponAnimator.SetTrigger("isSprintUp");
        }
    }

    private void HandleWeaponInput()
    {
        // Number key switching
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

        // Update UI
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
}