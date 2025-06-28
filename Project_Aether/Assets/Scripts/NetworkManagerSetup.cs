using System;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkManagerSetup : MonoBehaviour
{
    // These fields will now hold the direct IP and Port for the dedicated server
    private string _serverIpAddress = GameConstants.GAME_SERVER_IP_ADDRESS;
    private ushort _serverPort = GameConstants.GAME_SERVER_PORT; // Default from GameConstants

    async void Awake()
    {
        // Check command-line arguments for dedicated server build
        string[] args = Environment.GetCommandLineArgs();
        bool isDedicatedServer = false;
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == "-dedicatedServer")
            {
                isDedicatedServer = true;
            }
            // Dedicated server will receive its public IP and port via command-line arguments
            else if (args[i] == "-serverIp" && i + 1 < args.Length)
            {
                _serverIpAddress = args[i + 1];
            }
            else if (args[i] == "-serverPort" && i + 1 < args.Length)
            {
                if (ushort.TryParse(args[i + 1], out ushort parsedPort))
                {
                    _serverPort = parsedPort;
                }
                else
                {
                    Debug.LogWarning($"Invalid serverPort argument: {args[i + 1]}. Using default port {GameConstants.GAME_SERVER_PORT}.");
                }
            }
        }

        // NO Unity Services initialization here. All handled by your custom backend.

        if (isDedicatedServer)
        {
            await StartDedicatedServer();
        }
        else
        {
            // For clients (Editor or built client)
            // Your custom BackendServiceManager will handle authentication and getting server connection details.
            SceneManager.LoadScene(GameConstants.PERSISTENT_SCENE_NAME); // Load main menu/lobby scene
        }
    }

    private async Task StartDedicatedServer()
    {
        Debug.Log("Server: Starting dedicated server setup...");
        try
        {
            // Direct IP/Port configuration based on command-line arguments or fallback
            string currentIp = !string.IsNullOrEmpty(_serverIpAddress) ? _serverIpAddress : GameConstants.FALLBACK_LOCAL_IP_ADDRESS;
            ushort currentPort = _serverPort;

            Debug.Log($"Server: Configuring connection data to IP: {currentIp}, Port: {currentPort}");
            NetworkManager.Singleton.GetComponent<UnityTransport>()
                .SetConnectionData(currentIp, currentPort);

            // Start the NetworkManager as a server
            NetworkManager.Singleton.StartServer();
            Debug.Log("Server: NetworkManager started as server.");

            // Load the PersistentScene additively for the server.
            // ServerZoneManager will then load all other necessary zones by querying your backend.
            SceneManager.LoadScene(GameConstants.PERSISTENT_SCENE_NAME, LoadSceneMode.Additive);
        }
        catch (Exception e)
        {
            Debug.LogError($"Server startup failed: {e.Message}");
            Application.Quit(); // Ensure server exits on critical failure
        }
    }

    void OnEnable()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnServerStarted += OnNetcodeServerStarted;
        }
    }

    void OnDisable()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnServerStarted -= OnNetcodeServerStarted;
        }
    }

    private void OnNetcodeServerStarted()
    {
        Debug.Log("Server: Netcode server fully started!");
        // Additional server-side initialization after Netcode is fully up.
        // ServerZoneManager might kick off its full zone loading here by talking to your backend.
    }

}
