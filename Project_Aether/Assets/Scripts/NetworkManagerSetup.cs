using System;
using System.Collections;
using System.Collections.Generic;
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
        // Add DontDestroyOnLoad to the NetworkManager_Setup GameObject itself
        // This ensures the NetworkManager and this setup script persist while Bootstrap Scene unloads,
        // and only the NetworkManager component (and its children/settings) will truly live on
        // as a DontDestroyOnLoad component within the NetworkManager's internal persistence.
        // It's primarily for this script's Awake/Start to complete before scene changes.
        DontDestroyOnLoad(this.gameObject); // Important for the client path especially

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
            Debug.Log("Client: Starting scene load sequence.");
            StartCoroutine(LoadClientCoreScenes()); // Start the new client loading routine
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
            // For clients, you might want to subscribe to OnClientStarted
            NetworkManager.Singleton.OnClientStarted += OnNetcodeClientStarted;
        }
    }

    void OnDisable()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnServerStarted -= OnNetcodeServerStarted;
            NetworkManager.Singleton.OnClientStarted -= OnNetcodeClientStarted;
        }
    }

    private void OnNetcodeServerStarted()
    {
        Debug.Log("Server: Netcode server fully started!");
        // Additional server-side initialization after Netcode is fully up.
        // ServerZoneManager might kick off its full zone loading here by talking to your backend.
    }

    private void OnNetcodeClientStarted()
    {
        Debug.Log("Client: Netcode client fully started!");
        // This is called when the client successfully connects to a server.
        // You might use this to trigger UI changes or initial client-side network setup.
    }

    private IEnumerator LoadClientCoreScenes()
    {
        Debug.Log("Bootstrap: Starting core scene loading...");
        // 1. Load PersistentScene additively 
        // This ensures all global managers are initialized and ready. 
        // We use LoadSceneAsync for smoother loading.
        AsyncOperation loadPersistent = SceneManager.LoadSceneAsync(GameConstants.PERSISTENT_SCENE_NAME, LoadSceneMode.Additive);
        while (!loadPersistent.isDone)
        {
            // You could update a simple loading bar or splash screen here
            Debug.Log($"Loading Persistent Scene: {loadPersistent.progress * 100}%"); yield return null;
        }
        Debug.Log("Bootstrap: Persistent Scene loaded. Activating it.");
        // Make the persistent scene active if needed, although often not strictly required 
        // unless you're doing operations directly on its root objects immediately. 
        //SceneManager.SetActiveScene(SceneManager.GetSceneByName(GameConstants.PERSISTENT_SCENE_NAME));


        // 2. Unload the Bootstrap Scene (optional but good for memory) 
        // This scene has done its job and can be removed. 
        // We do this AFTER the persistent scene is loaded, so its managers take over.
        //AsyncOperation unloadBootstrap = SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().buildIndex);
        //while (!unloadBootstrap.isDone)
        //{
        //    yield return null;
        //}
        //Debug.Log("Bootstrap: Bootstrap Scene unloaded.");

        // 3. Load the Main Menu Scene (single mode, as it will replace the current active scene)
        // This will automatically unload the 00_BootstrapScene, but PersistentScene remains.
        Debug.Log("Bootstrap: Loading Main Menu Scene...");
        AsyncOperation loadMainMenu = SceneManager.LoadSceneAsync(GameConstants.MAIN_MENU_SCENE_NAME, LoadSceneMode.Single);
        // LoadSceneMode.Single will automatically unload all other scenes EXCEPT those marked with DontDestroyOnLoad 
        // which includes our PersistentScene if its objects are setup correctly.
        while (!loadMainMenu.isDone)
        {
            // This is where you'd manage a more complex loading screen, 
            // perhaps shown *between* the Bootstrap and Main Menu.
            yield return null;
        }
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(GameConstants.MAIN_MENU_SCENE_NAME));
        Debug.Log("Bootstrap: Main Menu Scene loaded and active.");
    }

}
