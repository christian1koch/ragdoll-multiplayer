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
    public Camera cam;

    public string Name => "KatonBall";
    public int JutsuId => 2;

    public int manaValue;

    public int ManaValue => manaValue;

    public float maxDistance = 100f;

    public void CastJutsu()
    {
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        Vector3 targetPoint;

        // 2. Raycast to see what we hit
        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance))
        {
            targetPoint = hit.point;
        }
        else
        {
            targetPoint = ray.origin + ray.direction * maxDistance;
        }

        // 3. Get direction from spawn to target point
        Vector3 direction = (targetPoint - transform.position).normalized;

        // 4. Instantiate and shoot
        GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.LookRotation(direction));

        // 4. If the projectile has a Rigidbody, apply force in the camera’s forward direction.
        Rigidbody rb = projectile.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.AddForce(direction * shootForce, ForceMode.Impulse);
        }
    }
}
