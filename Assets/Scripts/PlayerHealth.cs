using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;
    public Slider healthSlider;
    public Text healthText;  // Optional: To show health percentage in UI
    public float regenerationRate = 1f;  // Health regeneration rate (health per second)
    public float invincibilityDuration = 1f;  // Time in seconds the player is invincible after taking damage
    private bool isInvincible = false;

    private float regenerationTimer = 0f;
    private int lastHealthValue;

    void Start()
    {
        currentHealth = maxHealth;
        lastHealthValue = currentHealth;
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }

        if (healthText != null)
        {
            healthText.text = $"{currentHealth}/{maxHealth}";
        }
    }

    void Update()
    {
        if (currentHealth < maxHealth)
        {
            RegenerateHealth();
        }
    }

    public void TakeDamage(int damage)
    {
        if (isInvincible) return;  // Prevent damage during invincibility

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthUI();

        if (currentHealth <= 0)
        {
            Die();
        }

        // Start invincibility period after taking damage
        StopAllCoroutines();
        StartCoroutine(InvincibilityCoroutine());
    }

    public void SetHealthToZero()
    {
        currentHealth = 0;
        UpdateHealthUI();
        Die();
    }

    void RegenerateHealth()
    {
        regenerationTimer += Time.deltaTime;
        if (regenerationTimer >= 1f)
        {
            int regenAmount = Mathf.FloorToInt(regenerationRate * regenerationTimer);
            currentHealth = Mathf.Clamp(currentHealth + regenAmount, 0, maxHealth);
            regenerationTimer = 0f;
            UpdateHealthUI();
        }
    }

    void UpdateHealthUI()
    {
        if (currentHealth != lastHealthValue)
        {
            if (healthSlider != null)
            {
                healthSlider.value = currentHealth;
            }

            if (healthText != null)
            {
                healthText.text = $"{currentHealth}/{maxHealth} ({(currentHealth * 100 / maxHealth)}%)";
            }
            lastHealthValue = currentHealth;
        }
    }

    void Die()
    {
        // Additional death logic can be added here if needed
        Debug.Log("Player is dead.");
    }

    // Coroutine to handle invincibility after taking damage
    private IEnumerator InvincibilityCoroutine()
    {
        isInvincible = true;
        yield return new WaitForSeconds(invincibilityDuration);
        isInvincible = false;
    }
}
