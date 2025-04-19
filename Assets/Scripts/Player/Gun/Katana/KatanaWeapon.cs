using System.Collections;
using UnityEngine;

public class KatanaWeapon : MonoBehaviour
{
    public Animator KatanaPlayer;
    public GameObject DamageBox;
    public float damageAmount = 100f;
    public AudioClip shootSound; // Add this for sound
    private bool canAttack = true;
    private bool isAttacking = false;
    private AudioSource audioSource; // Add this for sound

    void Start()
    {
        // Initialize audio source
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    void Update()
    {
        // Left Ctrl press hone par attack disable (like stealth or block)
        if (Input.GetKey(KeyCode.LeftControl)) return;

        if (Input.GetMouseButtonDown(0) && canAttack)
        {
            isAttacking = true;
            KatanaPlayer.SetTrigger("Shoot");

            // Play shoot sound
            if (shootSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(shootSound);
            }

            // Camera shake
            if (CameraShake.instance != null)
                CameraShake.instance.Shake(0.1f, 0.15f);

            StartCoroutine(EnableDamageBox());
        }
    }

    IEnumerator EnableDamageBox()
    {
        DamageBox.SetActive(true);
        yield return new WaitForSeconds(0.3f); // small time during slash
        DamageBox.SetActive(false);
        isAttacking = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Make sure we only detect collisions on layer 1
        if (!isAttacking || other.gameObject.layer != 1) return;

        Target target = other.GetComponent<Target>();
        if (target != null)
        {
            target.TakeDamage(damageAmount);
        }
    }
}