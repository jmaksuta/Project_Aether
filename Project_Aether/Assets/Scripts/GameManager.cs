using System;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("UI References")]
    [SerializeField]
    private Button createWorldButton;
    [SerializeField]
    private Button listWorldsButton;
    [SerializeField]
    private Button joinWorldButton;
    [SerializeField]
    private TMP_InputField ipPortInputField; // Changed to take "IP:Port"
    [SerializeField]
    private Button leaveNetworkButton;
    [SerializeField]
    public TextMeshProUGUI statusText; // Public for BackendServiceManager access

    [Header("Manager References")]
    [SerializeField] private BackendServiceManager backendServiceManager; // Assign BackendServiceManager GO

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject); // Important for GameManager and BackendServiceManager

        // Ensure NetworkManager is present (from BootstrapScene)
        if (NetworkManager.Singleton == null)
        {
            Debug.LogError("GameManager: NetworkManager.Singleton is null. Please ensure BootstrapScene is correctly configured.");
            SetStatusText("Error: NetworkManager missing!");
            return;
        }

        // Ensure backendServiceManager is assigned
        if (backendServiceManager == null)
        {
            backendServiceManager = FindObjectOfType<BackendServiceManager>();
            if (backendServiceManager == null)
            {
                Debug.LogError("GameManager: BackendServiceManager is null. Please assign it in the Inspector or ensure it exists in the scene.");
                SetStatusText("Error: BackendServiceManager missing!");
                return;
            }
        }

        // Wire up UI buttons to the new BackendServiceManager
        if (createWorldButton != null)
        {
            createWorldButton.onClick.AddListener(backendServiceManager.CreateWorld);
        }
        if (listWorldsButton != null)
        {
            listWorldsButton.onClick.AddListener(backendServiceManager.ListWorlds);
        }
        if (joinWorldButton != null)
        {
            joinWorldButton.onClick.AddListener(() =>
                backendServiceManager.JoinWorld(ipPortInputField.text)); // Pass the IP:Port string
        }
        if (leaveNetworkButton != null)
        {
            leaveNetworkButton.onClick.AddListener(backendServiceManager.LeaveNetworkAndWorld);
        }

        UpdateUI(); // Initial UI state

        // Subscribe to Netcode events for UI updates
        NetworkManager.Singleton.OnClientStarted += UpdateUI;
        NetworkManager.Singleton.OnServerStarted += UpdateUI;
        NetworkManager.Singleton.OnClientStopped += UpdateUI;
        NetworkManager.Singleton.OnServerStopped += UpdateUI;
    }

    private void OnDestroy()
    {
        // Clean up event subscriptions
        if (createWorldButton != null) createWorldButton.onClick.RemoveListener(backendServiceManager.CreateWorld);
        if (listWorldsButton != null) listWorldsButton.onClick.RemoveListener(backendServiceManager.ListWorlds);
        if (joinWorldButton != null) joinWorldButton.onClick.RemoveListener(() =>
            backendServiceManager.JoinWorld(ipPortInputField.text));
        if (leaveNetworkButton != null) leaveNetworkButton.onClick.RemoveListener(backendServiceManager.LeaveNetworkAndWorld);

        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientStarted -= UpdateUI;
            NetworkManager.Singleton.OnServerStarted -= UpdateUI;
            NetworkManager.Singleton.OnClientStopped -= UpdateUI;
            NetworkManager.Singleton.OnServerStopped -= UpdateUI;
        }

        if (Instance == this)
        {
            Instance = null;
        }
    }

    private void UpdateUI(bool obj)
    {
        UpdateUI();
    }

    private void UpdateUI()
    {
        bool networkActive = NetworkManager.Singleton.IsClient || NetworkManager.Singleton.IsServer;

        if (createWorldButton != null) createWorldButton.interactable = !networkActive;
        if (listWorldsButton != null) listWorldsButton.interactable = !networkActive;
        if (joinWorldButton != null) joinWorldButton.interactable = !networkActive;
        if (ipPortInputField != null) ipPortInputField.interactable = !networkActive;
        if (leaveNetworkButton != null) leaveNetworkButton.interactable = networkActive;

        if (networkActive)
        {
            SetStatusText(NetworkManager.Singleton.IsHost ? "Running as Host (for local testing)" :
                          NetworkManager.Singleton.IsServer ? "Running as Dedicated Server" :
                          "Connected as Client");
        }
        else
        {
            SetStatusText("Idle. Use backend to find/create a world.");
        }
    }

    public void SetStatusText(string message)
    {
        if (statusText != null)
        {
            statusText.text = message;
        }
    }
}
