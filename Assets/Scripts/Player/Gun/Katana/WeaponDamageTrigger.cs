using UnityEngine;

public class WeaponDamageTrigger : MonoBehaviour
{
    public float damageAmount = 100f;

    private void OnTriggerEnter(Collider other)
    {
        Target target = other.GetComponent<Target>();
        if (target != null)
        {
            target.TakeDamage(damageAmount);
            Debug.Log("Hit enemy with Katana");
            Debug.Log("Collided with: " + other.name);

        }
    }
}

