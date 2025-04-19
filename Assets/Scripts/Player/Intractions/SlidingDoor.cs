using UnityEngine;

public class DoorMover : MonoBehaviour
{
    public Transform doorTransform;        // Assign the door GameObject's transform
    public string playerTag = "Player";    // Tag of the player

    public float openY = -3.20f;           // Y position when door is open
    public float closedY = 0f;             // Y position when door is closed
    public float moveSpeed = 3f;           // Lerp speed

    private bool isPlayerInside = false;

    void Update()
    {
        float targetY = isPlayerInside ? openY : closedY;

        Vector3 currentPos = doorTransform.localPosition;
        Vector3 targetPos = new Vector3(currentPos.x, targetY, currentPos.z);

        doorTransform.localPosition = Vector3.Lerp(currentPos, targetPos, Time.deltaTime * moveSpeed);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            isPlayerInside = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            isPlayerInside = false;
        }
    }
}
