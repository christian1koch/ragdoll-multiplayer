using Unity.Netcode;
using UnityEngine;

public class KatonBall : NetworkBehaviour, IJutsu
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
        if (!IsOwner) return;

        // Client calculates the ray and sends the direction
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        Vector3 targetPoint;

        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance))
        {
            targetPoint = hit.point;
        }
        else
        {
            targetPoint = ray.origin + ray.direction * maxDistance;
        }

        Vector3 direction = (targetPoint - transform.position).normalized;

        CastJutsuServerRpc(direction);
    }

    [ServerRpc]
    private void CastJutsuServerRpc(Vector3 direction)
    {
        Vector3 spawnPos = transform.position + direction.normalized * 1.5f; // spawn in front of caster

        GameObject projectile = Instantiate(projectilePrefab, spawnPos, Quaternion.LookRotation(direction));
        var instanceNetworkObject = projectile.GetComponent<NetworkObject>();
        instanceNetworkObject.Spawn();

        Rigidbody rb = instanceNetworkObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(direction * shootForce, ForceMode.Impulse);
        }
    }
}
