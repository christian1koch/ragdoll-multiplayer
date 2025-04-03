using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    [Tooltip("Time in seconds before the projectile is destroyed.")]
    public float lifetime = 5f;

    void Start()
    {
        // Automatically destroy the projectile after the set lifetime
        Destroy(gameObject, lifetime);
    }

    // Optional: If you need collision logic, you can handle it here.
    // void OnCollisionEnter(Collision other)
    // {
    //     // Example: Destroy the projectile on impact
    //     Destroy(gameObject);
    // }
}