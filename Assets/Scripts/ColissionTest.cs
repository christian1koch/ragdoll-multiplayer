using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ColissionTest : NetworkBehaviour
{
    public int damage = 10;
    public bool shouldDestroyOnColission = true;

    public bool isPunching = false;

    private HashSet<AttributesManager> alreadyDamagedTargets = new HashSet<AttributesManager>();

    private void OnCollisionEnter(Collision collision)
    {
        if (!IsOwner) return; // Only trigger collision logic for the owner of the punch object

        AttributesManager target = collision.gameObject.GetComponentInParent<AttributesManager>();
        if (target != null && isPunching)
        {
            ulong targetId = target.NetworkObject.NetworkObjectId;
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
