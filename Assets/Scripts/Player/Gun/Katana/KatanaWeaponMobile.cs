using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
public class KatanaWeaponMobile : MonoBehaviour
{
    public Animator KatanaPlayer;
    public GameObject DamageBox;
    public float damageEnableTime = 0.2f;
    public Button attackButton; // Mobile attack button

    private void Start()
    {
        // Set up mobile button
        if (attackButton != null)
        {
            // Using EventTrigger for better control
            var trigger = attackButton.gameObject.GetComponent<EventTrigger>() ??
                          attackButton.gameObject.AddComponent<EventTrigger>();

            // Pointer Down (attack)
            var pointerDown = new EventTrigger.Entry();
            pointerDown.eventID = EventTriggerType.PointerDown;
            pointerDown.callback.AddListener((e) => Attack());
            trigger.triggers.Add(pointerDown);
        }
    }

    public void Attack()
    {
        KatanaPlayer.SetTrigger("Shoot");

        // Shake camera
        if (CameraShake.instance != null)
            CameraShake.instance.Shake(0.1f, 0.2f);

        StartCoroutine(EnableDamage());
    }

    IEnumerator EnableDamage()
    {
        DamageBox.SetActive(true);
        yield return new WaitForSeconds(damageEnableTime);
        DamageBox.SetActive(false);
    }
}