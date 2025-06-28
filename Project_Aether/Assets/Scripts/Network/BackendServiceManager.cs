using System.Collections.Generic;
using System;
using Unity.Netcode.Transports.UTP;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;

public class BackendServiceManager : MonoBehaviour
{
    public static BackendServiceManager Instance { get; private set; }

    [Header("Backend API Settings")]
    [SerializeField]
    private string authApiUrl = "https://api.yourgame.com/auth"; // Your Auth API endpoint
    [SerializeField]
    private string worldApiUrl = "https://api.yourgame.com/worlds"; // Your World/Lobby API endpoint
    [SerializeField]
    private string signalRHubUrl = "https://api.yourgame.com/chathub"; // Your SignalR Chat Hub URL

    // You might store player token here after successful authentication
    private string _playerAuthToken;
    private string _playerId; // Or some identifier from your auth system

    // private HubConnection _signalRConnection; // For SignalR integration

    [Header("Game Manager Reference")]
    [SerializeField]
    private GameManager gameManager; // Assign in Inspector

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public async void Start()
    {
        if (gameManager == null) gameManager = FindObjectOfType<GameManager>();

        if (NetworkManager.Singleton == null)
        {
            Debug.LogError("BackendServiceManager: NetworkManager.Singleton is null. Ensure BootstrapScene is loaded first.");
            return;
        }

        // Initialize and authenticate with your custom backend immediately
        await AuthenticateWithBackend();
        // await ConnectToSignalR(); // Connect to SignalR after authentication
    }

    // --- Custom Authentication ---
    public async Task AuthenticateWithBackend()
    {
        gameManager.SetStatusText("Authenticating with custom backend...");
        try
        {
            // Example: Make an HTTP POST request to your ASP.NET Core Auth API
            // This is a simplified example. You'd likely send username/password or use a refresh token.
            // Replace with your actual authentication payload and endpoint.
            var request = UnityWebRequest.Post(authApiUrl + "/login", JsonUtility.ToJson(new { username = "testuser", password = "testpassword" }));
            request.SetRequestHeader("Content-Type", "application/json");
            await request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Authentication failed: {request.error} - {request.downloadHandler.text}");
                gameManager.SetStatusText($"Auth Error: {request.error}");
                return;
            }

            // Assuming your backend returns a JSON with an auth token and player ID
            var authResponse = JsonUtility.FromJson<AuthResponse>(request.downloadHandler.text);
            _playerAuthToken = authResponse.Token;
            _playerId = authResponse.PlayerId;

            Debug.Log($"Successfully authenticated with backend. Player ID: {_playerId}");
            gameManager.SetStatusText($"Signed in as {_playerId}");

        }
        catch (Exception e)
        {
            Debug.LogError($"Error during backend authentication: {e.Message}");
            gameManager.SetStatusText($"Auth Exception: {e.Message}");
        }
    }

    [System.Serializable]
    private class AuthResponse
    {
        public string Token;
        public string PlayerId;
        // Add other fields your auth response might have (e.g., player name)
    }

    // --- SignalR Integration (Conceptual) ---
    /*
    // Uncomment and implement if you integrate SignalR client library
    public async Task ConnectToSignalR()
    {
        if (string.IsNullOrEmpty(_playerAuthToken))
        {
            Debug.LogWarning("Cannot connect to SignalR: Not authenticated.");
            return;
        }

        gameManager.SetStatusText("Connecting to SignalR...");
        try
        {
            _signalRConnection = new HubConnectionBuilder()
                .WithUrl(signalRHubUrl, options =>
                {
                    // Pass your custom auth token in the header
                    options.Headers.Add("Authorization", "Bearer " + _playerAuthToken);
                })
                .Build();

            // Example: Register client-side methods that the server can call
            _signalRConnection.On<string, string>("ReceiveChatMessage", (user, message) =>
            {
                Debug.Log($"SignalR Chat: {user}: {message}");
                gameManager.SetStatusText(gameManager.statusText.text + $"\nChat: {user}: {message}");
            });

            await _signalRConnection.StartAsync();
            Debug.Log("SignalR connection established.");
            gameManager.SetStatusText(gameManager.statusText.text + "\nSignalR Connected.");

        }
        catch (Exception e)
        {
            Debug.LogError($"SignalR connection failed: {e.Message}");
            gameManager.SetStatusText($"SignalR Error: {e.Message}");
        }
    }

    public async Task SendChatMessage(string message)
    {
        if (_signalRConnection != null && _signalRConnection.State == HubConnectionState.Connected)
        {
            try
            {
                await _signalRConnection.InvokeAsync("SendMessage", _playerId, message); // Or your player name
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to send SignalR chat message: {e.Message}");
            }
        }
        else
        {
            Debug.LogWarning("SignalR not connected to send message.");
        }
    }
    */

    // --- Custom World/Lobby Management (Replacing UGS Lobby) ---

    // This data structure should match what your ASP.NET Core API returns for a world
    [System.Serializable]
    public class WorldInfo
    {
        public string WorldId;
        public string Name;
        public int CurrentPlayers;
        public int MaxPlayers;
        public string IpAddress; // Now directly provides the server IP
        public ushort Port;      // Now directly provides the server Port
        public string Region;
    }

    [System.Serializable]
    private class ListWorldsResponse
    {
        public List<WorldInfo> Worlds;
    }

    // Requesting your backend to create a new dedicated server instance.
    public async void CreateWorld()
    {
        if (string.IsNullOrEmpty(_playerAuthToken))
        {
            gameManager.SetStatusText("Error: Not authenticated to create world!");
            return;
        }

        gameManager.SetStatusText("Requesting new world from backend...");
        try
        {
            // Your backend will handle:
            // 1. Launching a dedicated server instance (using your orchestration)
            // 2. Getting the public IP and Port of the new server
            // 3. Registering this world (and its IP/Port) in its own database/world directory
            // 4. Returning the server's IP and Port to this client

            var request = UnityWebRequest.Post(worldApiUrl + "/create", "{}");
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", "Bearer " + _playerAuthToken); // Pass your auth token
            await request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Failed to create world via backend: {request.error} - {request.downloadHandler.text}");
                gameManager.SetStatusText($"Error creating world: {request.error}");
                return;
            }

            // Assuming backend returns the direct IP and Port needed for client to connect to new server
            var createWorldResponse = JsonUtility.FromJson<WorldInfo>(request.downloadHandler.text);
            string serverIp = createWorldResponse.IpAddress;
            ushort serverPort = createWorldResponse.Port;

            Debug.Log($"World creation initiated. Received server IP: {serverIp}, Port: {serverPort}. Starting client...");
            gameManager.SetStatusText($"World '{createWorldResponse.Name}' created! Connecting to {serverIp}:{serverPort}...");

            // Client now uses this IP/Port to connect directly to the dedicated server.
            var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            transport.ConnectionData.Address = serverIp;
            transport.ConnectionData.Port = serverPort;

            NetworkManager.Singleton.StartClient(); // Start Netcode as Client

        }
        catch (Exception e)
        {
            Debug.LogError($"Error during world creation/connection: {e.Message}");
            gameManager.SetStatusText($"Error connecting to world: {e.Message}");
        }
    }

    // Listing available worlds from your backend.
    public async void ListWorlds()
    {
        if (string.IsNullOrEmpty(_playerAuthToken))
        {
            gameManager.SetStatusText("Error: Not authenticated to list worlds!");
            return;
        }

        gameManager.SetStatusText("Listing worlds from backend...");
        try
        {
            // Make an HTTP GET request to your ASP.NET Core API to get available worlds
            var request = UnityWebRequest.Get(worldApiUrl + "/list");
            request.SetRequestHeader("Authorization", "Bearer " + _playerAuthToken); // Pass your auth token
            await request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Failed to list worlds via backend: {request.error} - {request.downloadHandler.text}");
                gameManager.SetStatusText($"Error listing worlds: {request.error}");
                return;
            }

            var response = JsonUtility.FromJson<ListWorldsResponse>(request.downloadHandler.text);
            gameManager.SetStatusText($"Found {response.Worlds.Count} active worlds:");

            foreach (WorldInfo world in response.Worlds)
            {
                string worldDetails = $" - {world.Name} ({world.CurrentPlayers}/{world.MaxPlayers}) IP: {world.IpAddress}:{world.Port} Region: {world.Region}";
                Debug.Log(worldDetails);
                gameManager.SetStatusText(gameManager.statusText.text + "\n" + worldDetails);
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to list worlds: {e.Message}");
            gameManager.SetStatusText($"Error listing worlds: {e.Message}");
        }
    }

    // Joining an existing world by direct IP/Port (or by WorldId, then querying backend for IP/Port)
    public async void JoinWorld(string ipAndPort) // Expecting format "IP:Port"
    {
        if (string.IsNullOrEmpty(_playerAuthToken))
        {
            gameManager.SetStatusText("Error: Not authenticated to join world!");
            return;
        }

        if (string.IsNullOrEmpty(ipAndPort) || !ipAndPort.Contains(":"))
        {
            gameManager.SetStatusText("Error: Invalid IP:Port format. Expected 'IP:Port'.");
            return;
        }

        string[] parts = ipAndPort.Split(':');
        string serverIp = parts[0];
        ushort serverPort;
        if (!ushort.TryParse(parts[1], out serverPort))
        {
            gameManager.SetStatusText("Error: Invalid port number.");
            return;
        }

        gameManager.SetStatusText($"Joining world directly: {serverIp}:{serverPort}...");
        try
        {
            // Optional: Call your backend here to validate the join request or record the player join
            // e.g., await SomeBackendCallToJoinWorld(worldId, _playerAuthToken);

            var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            transport.ConnectionData.Address = serverIp;
            transport.ConnectionData.Port = serverPort;

            NetworkManager.Singleton.StartClient(); // Start Netcode as Client
        }
        catch (Exception e)
        {
            Debug.LogError($"Error connecting directly to server: {e.Message}");
            gameManager.SetStatusText($"Error connecting to world: {e.Message}");
        }
    }

    public async void LeaveNetworkAndWorld()
    {
        // First, shutdown Netcode
        if (NetworkManager.Singleton.IsClient || NetworkManager.Singleton.IsServer)
        {
            NetworkManager.Singleton.Shutdown();
            Debug.Log("Netcode Shutdown.");
        }

        // Optional: Notify your backend that the player has left
        // if (!string.IsNullOrEmpty(_playerAuthToken))
        // {
        //    var request = UnityWebRequest.Post(worldApiUrl + "/leave", JsonUtility.ToJson(new { playerId = _playerId }));
        //    request.SetRequestHeader("Content-Type", "application/json");
        //    request.SetRequestHeader("Authorization", "Bearer " + _playerAuthToken);
        //    await request.SendWebRequest();
        //    if (request.result != UnityWebRequest.Result.Success) Debug.LogError($"Failed to notify backend of leave: {request.error}");
        // }

        // Disconnect from SignalR (if connected)
        /*
        if (_signalRConnection != null && _signalRConnection.State == HubConnectionState.Connected)
        {
            await _signalRConnection.StopAsync();
            Debug.Log("SignalR disconnected.");
        }
        */

        gameManager.SetStatusText("Disconnected from network & world.");
        SceneManager.LoadScene(GameConstants.PERSISTENT_SCENE_NAME); // Return to main menu scene
    }

}
