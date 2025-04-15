using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CollisionDamager : NetworkBehaviour
{
    public int damage = 10;
    public bool shouldDestroyOnColission = true;

    private HashSet<AttributesManager> alreadyDamagedTargets = new HashSet<AttributesManager>();

    private void OnCollisionEnter(Collision collision)
    {
        AttributesManager targetHealth = collision.gameObject.GetComponentInParent<AttributesManager>();

        if (targetHealth != null && !alreadyDamagedTargets.Contains(targetHealth))
        {
            alreadyDamagedTargets.Add(targetHealth);

            if (IsServer)
            {
                Debug.Log("Dealing damage: " + damage);
                targetHealth.ApplyDamageServerRpc(damage);
            }

            if (shouldDestroyOnColission && IsServer)
            {
                Destroy(gameObject);
            }
        }
    }
}
