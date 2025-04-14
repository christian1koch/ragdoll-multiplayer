using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using TMPro;

public class ConnectIpScript : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Button hostButton;
    [SerializeField] private Button clientButton;

    [SerializeField] private Button shutdownButton;
    [SerializeField] private TMP_InputField ipInputField;

    [Header("Settings")]
    [SerializeField] private ushort port = 7777;

    private void Start()
    {
        hostButton.onClick.AddListener(HostButtonOnClick);
        clientButton.onClick.AddListener(ClientButtonOnClick);
        shutdownButton.onClick.AddListener(ShutdownButtonOnClick);
    }

    private void HostButtonOnClick()
    {
        if (NetworkManager.Singleton.IsListening)
        {
            Debug.LogWarning("Already running as Host/Client/Server.");
            return;
        }

        var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        transport.ConnectionData.Address = "0.0.0.0";  // Accept all IPs
        transport.ConnectionData.Port = port;

        Debug.Log($"Starting Host on port {port}...");
        NetworkManager.Singleton.StartHost();
    }

    private void ClientButtonOnClick()
    {
        if (NetworkManager.Singleton.IsListening)
        {
            Debug.LogWarning("Already running as Host/Client/Server.");
            return;
        }

        var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        string ipAddress = ipInputField.text.Trim();

        if (string.IsNullOrEmpty(ipAddress))
        {
            Debug.LogWarning("Please enter the host's IP address.");
            return;
        }

        transport.ConnectionData.Address = ipAddress;
        transport.ConnectionData.Port = port;

        Debug.Log($"Starting Client connecting to {ipAddress}:{port}...");
        NetworkManager.Singleton.StartClient();
    }

    private void ShutdownButtonOnClick()
    {
        if (NetworkManager.Singleton.IsListening)
        {
            Debug.Log("Shutting down NetworkManager...");
            NetworkManager.Singleton.Shutdown();
        }
        else
        {
            Debug.LogWarning("NetworkManager is not running.");
        }
    }
}
