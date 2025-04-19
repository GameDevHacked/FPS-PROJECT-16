using UnityEngine;

public class WallRunHandler : MonoBehaviour
{
    public LayerMask wallLayer;
    public float wallCheckDistance = 0.7f;
    public float wallRunSpeed = 8f;
    public float wallRunDuration = 1.5f;
    public Transform cam;
    public float cameraTiltAmount = 10f; // Final Tilt amount
    public float tiltSmooth = 5f;

    private CharacterController controller;
    private float wallRunTimer;
    private bool isWallRunning = false;
    private Vector3 wallNormal;
    private float currentTilt = 0f;
    private float targetTilt = 0f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        WallCheck();

        if (isWallRunning)
            WallRunMove();

        TiltCamera();
    }

    void WallCheck()
    {
        RaycastHit hit;
        bool isMoving = Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0;

        if (Physics.Raycast(transform.position, transform.right, out hit, wallCheckDistance, wallLayer))
        {
            if (isMoving) StartWallRun(hit.normal, "right");
            return;
        }
        else if (Physics.Raycast(transform.position, -transform.right, out hit, wallCheckDistance, wallLayer))
        {
            if (isMoving) StartWallRun(hit.normal, "left");
            return;
        }

        StopWallRun();
    }

    void StartWallRun(Vector3 hitNormal, string side)
    {
        isWallRunning = true;
        wallNormal = hitNormal;
        wallRunTimer = wallRunDuration;

        // Set tilt direction
        targetTilt = (side == "left") ? cameraTiltAmount : -cameraTiltAmount;

        // Optional: Lock gravity, movement override, etc.
    }

    void WallRunMove()
    {
        wallRunTimer -= Time.deltaTime;
        if (wallRunTimer <= 0)
        {
            StopWallRun();
            return;
        }

        Vector3 wallDirection = Vector3.Cross(wallNormal, Vector3.up);
        Vector3 move = wallDirection * wallRunSpeed;

        controller.Move(move * Time.deltaTime);
    }

    void StopWallRun()
    {
        if (isWallRunning)
        {
            isWallRunning = false;
            targetTilt = 0f;
        }
    }

    void TiltCamera()
    {
        currentTilt = Mathf.Lerp(currentTilt, targetTilt, Time.deltaTime * tiltSmooth);
        cam.localRotation = Quaternion.Euler(0, 0, currentTilt);
    }
}
