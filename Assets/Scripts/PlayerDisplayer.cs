using TMPro;
using Unity.Collections;
using Unity.Netcode;

public class PlayerDisplayer : NetworkBehaviour
{
    public TMP_Text playerNameText;

    private NetworkVariable<FixedString32Bytes> playerName = new NetworkVariable<FixedString32Bytes>(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public override void OnNetworkSpawn()
    {
        playerName.OnValueChanged += (oldName, newName) =>
        {
            playerNameText.text = newName.ToString();
        };



        if (IsOwner)
        {
            SetName(PlayerData.playerName);
        }

        else
        {
            playerNameText.text = playerName.Value.ToString();
        }

    }
    void SetName(string newName)
    {
        playerName.Value = newName;
    }
}

