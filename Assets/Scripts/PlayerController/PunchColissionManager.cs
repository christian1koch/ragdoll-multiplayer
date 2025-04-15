using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PunchColissionManager : NetworkBehaviour
{
    public int damage = 10;
    private HashSet<AttributesManager> alreadyDamagedTargets = new HashSet<AttributesManager>();
    private bool canDamage = false;

    public void EnableDamage()
    {
        canDamage = true;
        alreadyDamagedTargets.Clear();
    }

    public void DisableDamage()
    {
        canDamage = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!IsOwner) return;

        if (!canDamage) return;

        AttributesManager target = collision.gameObject.GetComponentInParent<AttributesManager>();

        if (target != null)
        {
            ulong targetId = target.NetworkObject.NetworkObjectId;
            alreadyDamagedTargets.Add(target);
            ApplyDamageServerRpc(targetId, damage);
        }
    }

    [ServerRpc]
    private void ApplyDamageServerRpc(ulong networkObjectId, int damageAmount)
    {
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(networkObjectId, out NetworkObject netObj))
        {
            AttributesManager target = netObj.GetComponentInParent<AttributesManager>();
            if (target != null)
            {
                target.ApplyDamageServerRpc(damageAmount);
            }
        }
    }
}
