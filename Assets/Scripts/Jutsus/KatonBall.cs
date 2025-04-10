using UnityEngine;

public class KatonBall : MonoBehaviour, IJutsu
{
    [Header("Projectile Settings")]
    [Tooltip("Drag the Projectile prefab here.")]
    public GameObject projectilePrefab;

    [Tooltip("Force applied to the projectile when fired.")]
    public float shootForce = 20f;

    [Header("Scene References")]
    [Tooltip("Reference to the camera transform.")]
    public Transform cam;

    public string Name => "KatonBall";
    public int JutsuId => 2;

    public int manaValue;

    public int ManaValue => manaValue;

    public void CastJutsu()
    {
        // 1. Determine spawn position (at the player’s position, or offset if desired).
        Vector3 spawnPos = transform.position;

        // 2. Determine spawn rotation so that the projectile faces the same direction as the camera.
        Quaternion spawnRot = Quaternion.LookRotation(cam.forward);

        // 3. Instantiate the projectile at the player’s position, oriented forward from the camera.
        GameObject newProjectile = Instantiate(projectilePrefab, spawnPos, spawnRot);

        // 4. If the projectile has a Rigidbody, apply force in the camera’s forward direction.
        Rigidbody rb = newProjectile.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(cam.forward * shootForce, ForceMode.Impulse);
        }
    }
}
