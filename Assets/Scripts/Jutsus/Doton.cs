using Unity.Netcode;
using UnityEngine;

public class Doton : NetworkBehaviour, IJutsu
{
    public bool isSelected;

    public GameObject dotonShieldObject;

    public Transform cameraTransform;

    public string Name => "Doton";
    public int JutsuId => 1;

    public int manaValue;

    public int ManaValue => manaValue;


    [ServerRpc]
    private void CastJutsuServerRpc()
    {
        // spawns the doton sheild in front of the caster
        GameObject dotonShield = Instantiate(dotonShieldObject, transform.position, Quaternion.Euler(0f, cameraTransform.eulerAngles.y, 0f));
        var instanceNetworkObject = dotonShield.GetComponent<NetworkObject>();
        instanceNetworkObject.Spawn();
    }

    void IJutsu.CastJutsu()
    {
        CastJutsuServerRpc();
    }
}