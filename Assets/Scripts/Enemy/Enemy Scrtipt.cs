using UnityEngine;
using UnityEngine.UI; // Required for UI components

public class Target : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 50f;
    public float currentHealth;

    [Header("UI Settings")]
    public Slider healthSlider; // Assign in Inspector
    public Canvas healthCanvas; // Assign in Inspector (optional)
    public Vector3 healthBarOffset = new Vector3(0, 2f, 0); // Adjust position above enemy

    void Start()
    {
        currentHealth = maxHealth;

        // Initialize health slider
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }

        // Disable health bar by default (optional)
        if (healthCanvas != null)
        {
            healthCanvas.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        // Make health bar face the camera (if using World Space UI)
        if (healthCanvas != null && Camera.main != null)
        {
            healthCanvas.transform.LookAt(Camera.main.transform);
            healthCanvas.transform.position = transform.position + healthBarOffset;
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        Debug.Log($"Enemy Hit! Health: {currentHealth}");

        // Update health slider
        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
        }

        // Show health bar when hit (optional)
        if (healthCanvas != null)
        {
            healthCanvas.gameObject.SetActive(true);
        }

        // Check for death
        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Enemy Died!");
        Destroy(gameObject);
    }
}