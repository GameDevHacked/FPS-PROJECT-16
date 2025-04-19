using UnityEngine;
using System.Collections;
using TMPro; // ← TMP namespace

public class PistolShoot : MonoBehaviour
{
    [Header("Gun Settings")]
    public float damage = 20f;
    public float range = 100f;
    public float fireRate = 0.5f;
    public int maxAmmo = 12;
    public float reloadTime = 1.5f;

    [Header("References")]
    public Camera fpsCam;
    public ParticleSystem muzzleFlash;
    public GameObject impactEffect;
    public Animator gunAnimator;
    public AudioSource shootSound;
    public AudioSource reloadSound;
    public TextMeshPro ammoDisplay; // ← TMP reference

    private int currentAmmo;
    private float nextTimeToFire = 0f;
    private bool isReloading = false;

    private void Start()
    {
        currentAmmo = maxAmmo;
        UpdateAmmoUI(); // ← Set initial ammo display
    }

    private void OnEnable()
    {
        isReloading = false;
        UpdateAmmoUI(); // ← Update on weapon enable
    }

    private void Update()
    {
        if (isReloading)
            return;

        if (currentAmmo <= 0 || Input.GetKeyDown(KeyCode.R))
        {
            StartCoroutine(Reload());
            return;
        }

        // 🔒 Prevent shooting if Left Ctrl is held
        if (Input.GetButtonDown("Fire1") && Time.time >= nextTimeToFire && !Input.GetKey(KeyCode.LeftControl))
        {
            nextTimeToFire = Time.time + fireRate;
            Shoot();
        }
    }

    void Shoot()
    {
        muzzleFlash.Play();
        if (shootSound != null) shootSound.Play();

        gunAnimator.ResetTrigger("Shoot");
        gunAnimator.SetTrigger("Shoot");

        currentAmmo--;
        UpdateAmmoUI(); // ← Update UI on shot

        RaycastHit hit;
        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range))
        {
            Debug.Log("Hit: " + hit.transform.name);

            Target target = hit.transform.GetComponent<Target>();
            if (target != null)
            {
                target.TakeDamage(damage);
            }

            if (impactEffect != null)
            {
                GameObject impactGO = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(impactGO, 1f);
            }
        }
    }

    IEnumerator Reload()
    {
        isReloading = true;

        if (reloadSound != null) reloadSound.Play();
        gunAnimator.SetTrigger("Reload");

        yield return new WaitForSeconds(reloadTime);

        currentAmmo = maxAmmo;
        isReloading = false;
        UpdateAmmoUI(); // ← Update after reload
    }

    void UpdateAmmoUI()
    {
        if (ammoDisplay != null)
        {
            ammoDisplay.text = currentAmmo + " / " + maxAmmo;
        }
    }
}
