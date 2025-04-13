using Unity.Netcode;
using UnityEngine;

public class CollisionDamager : NetworkBehaviour
{
    public int damage = 10;
    public bool shouldDestroyOnColission = true;

    private void OnCollisionEnter(Collision collision)
    {
        // Look in parents too
        AttributesManager targetHealth = collision.gameObject.GetComponentInParent<AttributesManager>();

        if (targetHealth != null)
        {
            // Apply damage on the server side
            if (IsServer) // Ensures this runs only on the server
            {
                Debug.Log("taking damage " + damage);
                targetHealth.ApplyDamageServerRpc(damage);
            }

            if (shouldDestroyOnColission)
            {
                // Only destroy on the server (using NetworkObject.Destroy for sync)
                if (IsServer)
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}
