using UnityEngine;
using System.Collections;

public class PlayerCollisionHandler : MonoBehaviour
{
    public PlayerHealth playerHealth;
    public GameObject deadUIPanel;
    public AudioSource deathSound;
    public float deathUIPopupDelay = 0.5f;

    private bool isDead = false;

    void Start()
    {
        if (deadUIPanel != null)
        {
            deadUIPanel.SetActive(false);
        }

        isDead = false;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!isDead && collision.gameObject.CompareTag("Player"))
        {
            isDead = true;

            if (playerHealth != null)
            {
                playerHealth.SetHealthToZero();
            }

            if (deathSound != null)
            {
                deathSound.Play();
            }

            if (deadUIPanel != null)
            {
                StartCoroutine(ShowDeadUIPanelCoroutine());
            }
        }
    }

    private IEnumerator ShowDeadUIPanelCoroutine()
    {
        yield return new WaitForSeconds(deathUIPopupDelay);
        if (deadUIPanel != null)
        {
            deadUIPanel.SetActive(true);
        }
    }
}
