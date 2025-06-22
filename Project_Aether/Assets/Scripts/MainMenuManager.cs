//using log4net.Util;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField]
    private Button startGameButton;
    [SerializeField]
    private Button settingsButton;
    [SerializeField]
    private Button exitGameButton;
    [SerializeField]
    private TextMeshProUGUI statusText;

    [Header("Game Settings")]
    [SerializeField]
    private string gameSceneName = "GameScene";

    private void Awake()
    {
        // ensure NetworkManager exists in the scene
        if (NetworkManager.Singleton == null)
        {
            Debug.LogError("NetworkManager.Singleton not found in the scene! Please ensure it's in this scene.", this);
            UpdateStatus("Error: NetworkManager missing!");
            return;
        }
        if (startGameButton != null)
        {
            startGameButton.onClick.AddListener(OnStartGameButtonClicked);
        }
        if (settingsButton != null)
        {
            settingsButton.onClick.AddListener(OnSettingsButtonClicked);
        }
        if (exitGameButton != null)
        {
            exitGameButton.onClick.AddListener(OnExitGameButtonClicked);
        }
        // Subscribe to Netcode events for feedbas and scene loading
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
        NetworkManager.Singleton.OnClientStopped += OnClientStopped;
        // no OnServerStarted if lcients never host/server
    }

    private void OnDestroy()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
            NetworkManager.Singleton.OnClientStopped -= OnClientStopped;
        }
    }

    public void OnStartGameButtonClicked()
    {
        // Set connection date (if not already set by AutoConnectionManager)
        // if your AutoConnectionManager already set the IPPort for the client, you don't need this here.
        // if you want the user to input the IP, you'd call SetTransportConnectionData() here.
        SetTransportConnectionData();
        // for a game with ONLY one dedicated server, the IP is hardcoded in AutoConnectionManager.
        // So this just triggers the client start if not already connected.
        if (!NetworkManager.Singleton.IsClient)
        {
            UpdateStatus("Attempting to connect to server...");
            Debug.Log("Attempting to connect as Client from Main Menu...");
            NetworkManager.Singleton.StartClient();
        }
        else
        {
            UpdateStatus("Already connected. Loading Game...");
            Debug.Log("Already Connect. proceeding to game scene.");
            SceneManager.LoadScene(gameSceneName);
        }
    }

    public void OnSettingsButtonClicked()
    {
        UpdateStatus("Settings clicked. (Not yet implemented)");
        Debug.Log("Settings button clicked. Implement settings menu here.");
        // TODO: open settings panel.
    }

    public void OnExitGameButtonClicked()
    {
        UpdateStatus("Exiting game...");
        Debug.Log("Exit Game button clicked.");
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // Stop play mode in editor
#endif
    }

    private void SetTransportConnectionData()
    {
        // TODO: set connection transport data here.
        string TargetIpAddress = GameConstants.ConnectionIP;
        ushort TargetPort = GameConstants.ConnectionPort;

        UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        if (transport == null)
        {
            Debug.LogError("UnityTransport component not found on NetworkManager.Singleton!", this);
            UpdateStatus("Error: UnityTransport missing!");
            return;
        }
        transport.SetConnectionData(TargetIpAddress, TargetPort);
    }

    private void UpdateStatus(string message)
    {
        if (statusText != null)
        {
            statusText.text = message;
        }
    }

    // --- NetworkManager Callbacks for Scene Loading ---
    private void OnClientConnected(ulong clientId)
    {
        if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            UpdateStatus("Connected. Loading Game...");
            Debug.Log($"Client connected. ClientId={clientId}.");
            SceneManager.LoadScene(gameSceneName);
        }
    }

    private void OnClientDisconnected(ulong clientId)
    {
        if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            UpdateStatus("Disconnected from server. Please Try again.");
            Debug.Log($"Local client disconneced from server. ClientId={clientId}.");
            // Handle this. perhaps show a "reconnect" button or just keep them at the main menu.
            // if we are already in the main menu, no need to reload it.
        }
    }

    private void OnClientStopped(bool causedByDisconnect)
    {
        // this is called if connection fails or is stopped.
        if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsHost && !NetworkManager.Singleton.IsServer)
        {
            UpdateStatus("Connection Failed. SErver unreachable or error.");
            Debug.LogError($"Client connection attempt failed or stopped. Caused by disconnect: {causedByDisconnect}.");
        }
    }
}