using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("Detection Settings")]
    public float detectionRange = 15f;       // How far the enemy can see
    public float detectionAngle = 60f;      // Field of view (degrees)
    public int rayCount = 5;               // Number of rays (more = more precise)
    public LayerMask obstacleLayers;        // What blocks vision (walls, etc.)

    [Header("Shooting Settings")]
    public Transform firePoint;            // Where bullets spawn
    public GameObject bulletPrefab;        // Bullet object
    public float fireRate = 1f;            // Shots per second
    public float bulletSpeed = 20f;        // Bullet travel speed
    public float aimSmoothing = 5f;        // How smoothly enemy tracks player

    private Transform player;
    private float nextShotTime;
    private bool playerDetected;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (player == null) return;

        // Smoothly rotate toward player (even if not detected)
        AimAtPlayer();

        // Check if player is in sight
        playerDetected = CheckPlayerVisibility();

        // Shoot if player is detected and cooldown is over
        if (playerDetected && Time.time >= nextShotTime)
        {
            Shoot();
            nextShotTime = Time.time + 1f / fireRate;
        }
    }

    void AimAtPlayer()
    {
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, aimSmoothing * Time.deltaTime);
    }

    bool CheckPlayerVisibility()
    {
        for (int i = 0; i < rayCount; i++)
        {
            // Calculate ray direction with spread
            float angle = -detectionAngle / 2 + (detectionAngle / (rayCount - 1)) * i;
            Vector3 rayDirection = Quaternion.Euler(0, angle, 0) * transform.forward;

            // Cast ray
            if (Physics.Raycast(transform.position, rayDirection, out RaycastHit hit, detectionRange, obstacleLayers))
            {
                // If ray hits player, return true
                if (hit.transform.CompareTag("Player"))
                {
                    Debug.DrawRay(transform.position, rayDirection * hit.distance, Color.green, 0.1f);
                    return true;
                }
                // If ray hits obstacle, draw red line
                Debug.DrawRay(transform.position, rayDirection * hit.distance, Color.red, 0.1f);
            }
            else
            {
                // If ray hits nothing, draw gray line
                Debug.DrawRay(transform.position, rayDirection * detectionRange, Color.gray, 0.1f);
            }
        }
        return false;
    }

    void Shoot()
    {
        if (bulletPrefab == null || firePoint == null) return;

        // Create bullet
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        // Add velocity to bullet
        Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
        if (bulletRb != null)
        {
            bulletRb.linearVelocity = firePoint.forward * bulletSpeed;
        }
        else
        {
            Debug.LogWarning("Bullet prefab has no Rigidbody! Add one for physics movement.");
        }

        // Optional: Add muzzle flash or sound here
    }

    // Visualize detection in Unity Editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        for (int i = 0; i < rayCount; i++)
        {
            float angle = -detectionAngle / 2 + (detectionAngle / (rayCount - 1)) * i;
            Vector3 rayDirection = Quaternion.Euler(0, angle, 0) * transform.forward;
            Gizmos.DrawRay(transform.position, rayDirection * detectionRange);
        }
    }
}