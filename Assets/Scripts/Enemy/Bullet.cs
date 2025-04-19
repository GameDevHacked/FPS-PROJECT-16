using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float lifeTime = 5f;
    public int damage = 1;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Add your player health reduction logic here
            Debug.Log("Player hit!");
        }

        Destroy(gameObject);
    }
}
