using TMPro;
using Unity.Collections;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class PlayerDisplayer : NetworkBehaviour
{
    public TMP_Text playerNameText;

    private NetworkVariable<FixedString32Bytes> playerName = new NetworkVariable<FixedString32Bytes>();
    public override void OnNetworkSpawn()
    {
        if (IsOwner && IsClient)
        {
            SetNameServerRpc(PlayerData.playerName);
        }

        playerName.OnValueChanged += (oldName, newName) =>
        {
            playerNameText.text = newName.ToString();
        };
    }

    [ServerRpc]
    void SetNameServerRpc(string newName)
    {
        playerName.Value = newName;
    }
}

