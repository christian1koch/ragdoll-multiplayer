using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using TMPro;

public class RelayConnectionUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Button hostButton;
    [SerializeField] private Button clientButton;
    [SerializeField] private Button shutdownButton;
    [SerializeField] private TMP_InputField joinCodeInputField;
    [SerializeField] private TMP_Text infoText;

    private async void Start()
    {
        hostButton.onClick.AddListener(Host);
        clientButton.onClick.AddListener(Join);
        shutdownButton.onClick.AddListener(Shutdown);

        await InitializeUnityServices();
    }

    private async Task InitializeUnityServices()
    {
        await UnityServices.InitializeAsync();

        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
    }

    private async void Host()
    {
        if (NetworkManager.Singleton.IsListening)
        {
            infoText.text = "Already running.";
            return;
        }

        try
        {
            var allocation = await RelayService.Instance.CreateAllocationAsync(2);
            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            transport.SetRelayServerData(AllocationUtils.ToRelayServerData(allocation, "dtls"));

            infoText.text = $"Join Code: {joinCode}";
            Debug.Log($"Hosting with Join Code: {joinCode}");

            NetworkManager.Singleton.StartHost();
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            infoText.text = $"Host Error: {e.Message}";
        }
    }

    private async void Join()
    {
        if (NetworkManager.Singleton.IsListening)
        {
            infoText.text = "Already running.";
            return;
        }

        string joinCode = joinCodeInputField.text.Trim();

        if (string.IsNullOrEmpty(joinCode))
        {
            infoText.text = "Enter Join Code.";
            return;
        }

        try
        {
            var joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

            var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            transport.SetRelayServerData(AllocationUtils.ToRelayServerData(joinAllocation, "dtls"));

            infoText.text = $"Joining with Code: {joinCode}";
            Debug.Log($"Joining with Join Code: {joinCode}");

            NetworkManager.Singleton.StartClient();
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            infoText.text = $"Join Error: {e.Message}";
        }
    }

    private void Shutdown()
    {
        if (NetworkManager.Singleton.IsListening)
        {
            NetworkManager.Singleton.Shutdown();
            infoText.text = "Disconnected.";
        }
        else
        {
            infoText.text = "Not connected.";
        }
    }
}
