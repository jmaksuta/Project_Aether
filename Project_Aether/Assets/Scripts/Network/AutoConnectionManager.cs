using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AutoConnectionManager : MonoBehaviour // Inherit from MonoBehaviour
{
    // --- Compile-Time Constants based on Build Type ---
    private const string TargetIpAddress = GameConstants.GAME_SERVER_IP_ADDRESS; // Default IP for client to connect to server
    private const ushort TargetPort = GameConstants.GAME_SERVER_PORT; // Default server port
    private const bool IsDedicatedServerBuild = GameConstants.IsDedicatedServer;

    [Header("Scene Names")]
    [SerializeField]
    private string gameSceneName = "GameScene";

    [Header("UI Elements (Optional)")]
    [SerializeField]
    private TextMeshProUGUI statusText; // Assign a Text UI element in the Inspector

    private void Awake()
    {
        // --- Essential Checks ---
        if (NetworkManager.Singleton == null)
        {
            Debug.LogError("NetworkManager.Singleton not found in the scene! Please ensure it's in this scene.", this);
            UpdateStatus("Error: NetworkManager missing!");
            return;
        }

        UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        if (transport == null)
        {
            Debug.LogError("UnityTransport component not found on NetworkManager.Singleton!", this);
            UpdateStatus("Error: UnityTransport missing!");
            return;
        }

        // --- Set Connection Data (Always) ---
        transport.SetConnectionData(TargetIpAddress, TargetPort);

        // --- Start Network Session Based on Build Type ---
        if (IsDedicatedServerBuild)
        {
            NetworkManager.Singleton.StartServer();
            Debug.Log($"Automatically started DEDICATED SERVER on {TargetIpAddress}:{TargetPort}");
            UpdateStatus("Starting Dedicated Server...");
        }
        else // This is the Client Build
        {
            if (!NetworkManager.Singleton.IsClient)
            {
                NetworkManager.Singleton.StartClient();
                Debug.Log($"Automatically started CLIENT, attempting to connect to {TargetIpAddress}:{TargetPort}");
                UpdateStatus($"Connecting to {TargetIpAddress}:{TargetPort}...");
            }
        }
    }


    // --- NetworkManager Callbacks for Feedback and Scene Loading ---
    private void OnEnable()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
        NetworkManager.Singleton.OnServerStarted += OnServerStarted;
        NetworkManager.Singleton.OnClientStopped += OnClientStopped; // Useful for failed connections
    }

    private void OnDisable()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
            NetworkManager.Singleton.OnServerStarted -= OnServerStarted;
            NetworkManager.Singleton.OnClientStopped -= OnClientStopped;
        }
    }

    private void OnServerStarted()
    {
        Debug.Log("NetworkManager successfully started as SERVER. Loading game scene.");
        UpdateStatus("Server Started. Loading Game...");
        // Server loads the game scene
        SceneManager.LoadScene(gameSceneName);
    }

    private void OnClientConnected(ulong clientId)
    {
        if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            Debug.Log($"Client successfully connected to server! Client ID: {clientId}. Loading game scene...");
            UpdateStatus("Connected! Loading Game...");
            // Client loads the game scene upon successful connection
            SceneManager.LoadScene(gameSceneName);
        }
    }

    private void OnClientDisconnected(ulong clientId)
    {
        if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            Debug.Log($"Local client disconnected from server. Client ID: {clientId}.");
            UpdateStatus("Disconnected from server. Retrying...");
            // Optionally, try to reconnect after a delay, or show a UI for manual retry.
            // For now, let's just log and stay on the title screen for user feedback.
            // You could add: Invoke("RestartClientConnection", 5f);
        }
        else
        {
            Debug.Log($"Another client disconnected: Client ID: {clientId}");
        }
    }

    private void OnClientStopped(bool causedByDisconnect)
    {
        // This callback fires when the client connection fails or is stopped for any reason.
        // It's useful for initial connection failures (e.g., server not running).
        if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsHost && !NetworkManager.Singleton.IsServer)
        {
            Debug.LogError($"Client connection attempt failed or stopped. Caused by disconnect: {causedByDisconnect}.");
            UpdateStatus("Connection Failed. Retrying...");
            // You might want to automatically try to reconnect here after a short delay
            // Invoke("RestartClientConnection", 5f);
        }
    }

    private void RestartClientConnection()
    {
        if (!IsDedicatedServerBuild && NetworkManager.Singleton != null && !NetworkManager.Singleton.IsClient)
        {
            UpdateStatus($"Retrying connection to {TargetIpAddress}:{TargetPort}...");
            NetworkManager.Singleton.StartClient();
        }
    }

    private void UpdateStatus(string message)
    {
        if (statusText != null)
        {
            statusText.text = message;
        }
    }
}