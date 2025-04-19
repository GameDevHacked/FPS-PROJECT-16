using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;

public class PistolShootMobile : MonoBehaviour
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
    public TextMeshPro ammoDisplay;

    [Header("Mobile Controls")]
    public Button shootButton;
    public Button reloadButton;

    private int currentAmmo;
    private float nextTimeToFire = 0f;
    private bool isReloading = false;

    private void Start()
    {
        currentAmmo = maxAmmo;
        UpdateAmmoUI();

        // Set up mobile button listeners
        if (shootButton != null)
        {
            shootButton.onClick.AddListener(OnShootButtonClick);
        }

        if (reloadButton != null)
        {
            reloadButton.onClick.AddListener(OnReloadButtonPressed);
        }
    }

    private void OnEnable()
    {
        isReloading = false;
        UpdateAmmoUI();
    }

    private void Update()
    {
        if (isReloading)
            return;
    }

    // Modified to handle single shot on click
    public void OnShootButtonClick()
    {
        if (!isReloading && currentAmmo > 0 && Time.time >= nextTimeToFire)
        {
            nextTimeToFire = Time.time + fireRate;
            Shoot();
        }
    }

    public void OnReloadButtonPressed()
    {
        if (!isReloading && currentAmmo < maxAmmo)
        {
            StartCoroutine(Reload());
        }
    }

    void Shoot()
    {
        muzzleFlash.Play();
        if (shootSound != null) shootSound.Play();

        gunAnimator.ResetTrigger("Shoot");
        gunAnimator.SetTrigger("Shoot");

        currentAmmo--;
        UpdateAmmoUI();

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
        UpdateAmmoUI();
    }

    void UpdateAmmoUI()
    {
        if (ammoDisplay != null)
        {
            ammoDisplay.text = currentAmmo + " / " + maxAmmo;
        }
    }
}