using System.Collections.Generic;
using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System.Collections;
using Assets.Scripts.Models;

public class ServerZoneManager : NetworkBehaviour
{
    public static ServerZoneManager Instance { get; private set; }

    [Header("Zone Scenes")]
    public List<string> allZoneSceneNames; // Assign in Inspector (e.g., "02_ForestZone", "03_DungeonZone")

    // Server-side tracking: Which client is in which scene
    private Dictionary<ulong, string> clientCurrentZone = new Dictionary<ulong, string>();

    // Backend API URL for world persistence
    [SerializeField] private string worldPersistenceApiUrl = "https://api.yourgame.com/worldpersistence";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public override void OnNetworkSpawn()
    {
        if (!IsServer)
        {
            // Only server executes the core logic of this manager
            gameObject.SetActive(false); // Disable for clients to save resources
            return;
        }
        Debug.Log("ServerZoneManager: Initializing on server.");

        // Subscribe to relevant NetworkManager events
        NetworkManager.Singleton.OnClientConnectedCallback += OnServerClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnServerClientDisconnected;
        NetworkManager.Singleton.SceneManager.OnLoadComplete += OnServerSceneLoadComplete;

        // Load all defined zone scenes additively on the server
        StartCoroutine(LoadAllZoneScenesRoutine());
    }

    public override void OnNetworkDespawn()
    {
        if (!IsServer) return; // Only server runs this logic
        NetworkManager.Singleton.OnClientConnectedCallback -= OnServerClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback -= OnServerClientDisconnected;
        NetworkManager.Singleton.SceneManager.OnLoadComplete -= OnServerSceneLoadComplete;
    }

    private IEnumerator LoadAllZoneScenesRoutine()
    {
        foreach (string sceneName in allZoneSceneNames)
        {
            if (!SceneManager.GetSceneByName(sceneName).isLoaded)
            {
                Debug.Log($"Server: Additively loading zone scene: {sceneName}");
                // TODO: Fix this.
                //yield return NetworkManager.Singleton.SceneManager.LoadScene(sceneName,
                //    LoadSceneMode.Additive).AsIEnumerator();
                Debug.Log($"Server: Finished loading {sceneName}.");

                // After a scene is loaded on the server, load its persistent data from your Backend.
                LoadZonePersistentData(sceneName);
            }
        }
        Debug.Log("Server: All zone scenes loaded.");
        // TODO: fix this. added return null; to avoid compiler error
        return null;
    }

    // --- World Persistence: Load Data from ASP.NET Core API ---
    private async void LoadZonePersistentData(string sceneName)
    {
        Debug.Log($"Server: Loading persistent data for zone: {sceneName} from backend.");
        try
        {
            // Make an HTTP GET request to your ASP.NET Core API for this zone's data
            var request = UnityWebRequest.Get($"{worldPersistenceApiUrl}/zones/{sceneName}");
            // You might need to add an API key or internal server token for authorization
            // request.SetRequestHeader("X-API-KEY", "YOUR_INTERNAL_SERVER_API_KEY"); // Example for server-to-server auth
            await request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Failed to load persistent data for {sceneName}: {request.error} - {request.downloadHandler.text}");
                return;
            }

            // Assuming your backend returns a JSON list of InteractableObjectData
            // You'll need to define InteractableObjectData and a wrapper class if your API returns an array directly
            // TODO: add this for real game.
            // Example: var loadedObjects = JsonUtility.FromJson<InteractableObjectDataListWrapper>(jsonResponse).objects;
            string jsonResponse = request.downloadHandler.text;
            Debug.Log($"Loaded data for {sceneName}: {jsonResponse}"); // For debugging

            // Find all InteractableObjects in the newly loaded scene
            Scene loadedScene = SceneManager.GetSceneByName(sceneName);
            if (loadedScene.IsValid())
            {
                List<InteractableObject> interactables = new List<InteractableObject>();
                foreach (GameObject rootObj in loadedScene.GetRootGameObjects())
                {
                    interactables.AddRange(rootObj.GetComponentsInChildren<InteractableObject>(true));
                }

                foreach (var interactable in interactables)
                {
                    // TODO: add this for real game.
                    // In a real game, each interactable would have a unique ID that matches backend data
                    // Find matching data in loadedObjects and apply its state (color, interacted status, position, etc.)
                    // Example: var data = loadedObjects.FirstOrDefault(o => o.UniqueID == interactable.uniqueID);
                    // if (data != null) interactable.ApplyStateFromData(data); // You'd need to add ApplyStateFromData to InteractableObject
                }
            }
            Debug.Log($"Server: Successfully loaded persistent data for zone: {sceneName}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Exception during loading persistent data for {sceneName}: {e.Message}");
        }
    }

    // --- Client Connection & Disconnection Handling ---
    private void OnServerClientConnected(ulong clientId)
    {
        // When a client connects, command them to load the default starting zone (e.g., ForestZone)
        string defaultZone = GameConstants.FOREST_ZONE_SCENE_NAME;
        Debug.Log($"Server: Client {clientId} connected. Commanding to load default zone: {defaultZone}");

        // Client must load the persistent scene first, then their zone.
        //NetworkManager.Singleton.SceneManager.LoadScene(GameConstants.PERSISTENT_SCENE_NAME, LoadSceneMode.Single, new HashSet<ulong> { clientId });
        // Corrected: Use LoadSceneForClients to command a specific client to load scenes
        // Client must load the persistent scene first, then their zone.

        //TODO: Fix this. LoadSceneForClients is not defined in Unity Netcode.
        //NetworkManager.Singleton.SceneManager.LoadSceneForClients(
        //    new HashSet<ulong> { clientId },
        //    GameConstants.PERSISTENT_SCENE_NAME,
        //    LoadSceneMode.Single
        //);
        //NetworkManager.Singleton.SceneManager.LoadScene(defaultZone, LoadSceneMode.Additive, new HashSet<ulong> { clientId });
        NetworkManager.Singleton.SceneManager.LoadScene(defaultZone, LoadSceneMode.Additive);
        clientCurrentZone[clientId] = defaultZone;
    }

    private void OnServerClientDisconnected(ulong clientId)
    {
        Debug.Log($"Server: Client {clientId} disconnected. Removing from zone tracking.");
        clientCurrentZone.Remove(clientId);
        // Save player data to backend here (e.g., position, inventory, etc.)
    }

    // --- Scene Load Completion & Visibility Management ---
    private void OnServerSceneLoadComplete(ulong clientId, string sceneName, LoadSceneMode loadSceneMode)
    {
        if (!IsServer) return; // This callback is also triggered on client for IsHost case
        if (loadSceneMode == LoadSceneMode.Single && sceneName == GameConstants.PERSISTENT_SCENE_NAME)
        {
            Debug.Log($"Server: Client {clientId} finished loading PersistentScene.");
            return;
        }
        if (allZoneSceneNames.Contains(sceneName))
        {
            Debug.Log($"Server: Client {clientId} completed loading zone scene {sceneName}. Updating visibility.");
            StartCoroutine(UpdateVisibilityForClientAfterLoad(clientId, sceneName));
        }
    }

    private IEnumerator UpdateVisibilityForClientAfterLoad(ulong clientId, string loadedSceneName)
    {
        // Give a frame or two for scene objects to fully register and spawn.
        yield return null;
        yield return null;
        if (!NetworkManager.Singleton.ConnectedClients.ContainsKey(clientId)) yield break; // Client disconnected

        // Step 1: Hide objects that are *not* in the client's current zone (previous zone and other zones)
        // This is a simple implementation. For true 'single world' where players might see
        // adjacent zones, this logic needs to be more complex.
        foreach (string zoneName in allZoneSceneNames)
        {
            Scene zoneUnityScene = SceneManager.GetSceneByName(zoneName);
            if (zoneUnityScene.IsValid())
            {
                List<NetworkObject> objectsInThisZone = new List<NetworkObject>();
                foreach (GameObject rootObj in zoneUnityScene.GetRootGameObjects())
                {
                    objectsInThisZone.AddRange(rootObj.GetComponentsInChildren<NetworkObject>(true));
                }
                foreach (NetworkObject netObj in objectsInThisZone)
                {
                    // TODO: Fix this.
                    //if (netObj.IsSpawned && netObj.IsSceneOwned) // Important: Only spawned and scene-owned
                    if (netObj.IsSpawned) // replaced above because IsSceneOwned is undefined
                    {
                        if (zoneName == loadedSceneName)
                        {
                            // TODO: Fix this.
                            //netObj.Show(clientId); // Show objects in the current zone
                            // Debug.Log($"Showing {netObj.name} to {clientId} (in {zoneName})");
                        }
                        else
                        {
                            // TODO: Fix this.
                            //netObj.Hide(clientId); // Hide objects in other zones
                            // Debug.Log($"Hiding {netObj.name} from {clientId} (in {zoneName})");
                        }
                    }
                }
            }
        }
        // Ensure the player's own NetworkObject is visible to themselves
        var playerNetworkObject = NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(clientId);
        if (playerNetworkObject != null)
        {
            // TODO: fix this. NetworkObject.Show is not defined in Unity Netcode.
            //playerNetworkObject.Show(clientId); // Always show player's own object to themselves
        }
    }

    // --- Client Request to Change Zone ---
    [ServerRpc(RequireOwnership = false)]
    public void RequestZoneChangeServerRpc(ulong clientId, string targetZoneName, Vector3 spawnPosition, Quaternion spawnRotation)
    {
        if (!IsServer) return;
        if (!allZoneSceneNames.Contains(targetZoneName))
        {
            Debug.LogWarning($"Server: Client {clientId} requested invalid zone: {targetZoneName}");
            return;
        }

        string currentZone;
        clientCurrentZone.TryGetValue(clientId, out currentZone);
        if (currentZone == targetZoneName)
        {
            Debug.Log($"Server: Client {clientId} already in {targetZoneName}. Ignoring request.");
            return;
        }

        Debug.Log($"Server: Client {clientId} (from {currentZone}) requested to change to zone: {targetZoneName}");
        // 1. Command client to load the new scene
        NetworkManager.Singleton.SceneManager.LoadScene(targetZoneName, LoadSceneMode.Additive);
        //NetworkManager.Singleton.SceneManager.LoadScene(targetZoneName, LoadSceneMode.Additive, new HashSet<ulong> { clientId });
        // 2. Command client to unload the old scene (if any)
        if (!string.IsNullOrEmpty(currentZone))
        {
            // Give client time to load new scene before unloading old one.
            // A more robust solution might wait for a client confirmation.
            StartCoroutine(UnloadOldZoneForClientDelayed(clientId, currentZone));
        }
        // 3. Update server's tracking of client's current zone
        clientCurrentZone[clientId] = targetZoneName;

        // 4. Move player's NetworkObject to the new spawn point
        NetworkObject playerObject = NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(clientId);
        if (playerObject != null)
        {
            // TODO: Fix this. PlayerController is not defined.
            //playerObject.GetComponent<PlayerController>()?.TeleportPlayerClientRpc(spawnPosition, spawnRotation);
        }
        else
        {
            Debug.LogError($"Server: Player NetworkObject for Client {clientId} not found for teleport!");
        }
        // Visibility update will happen in OnServerSceneLoadComplete for the client.
    }

    private IEnumerator UnloadOldZoneForClientDelayed(ulong clientId, string oldZoneName)
    {
        yield return new WaitForSeconds(1.0f); // Arbitrary delay for demo
        if (NetworkManager.Singleton.ConnectedClients.ContainsKey(clientId) &&
            clientCurrentZone.ContainsKey(clientId) && clientCurrentZone[clientId] != oldZoneName)
        {
            Debug.Log($"Server: Commanding client {clientId} to unload old zone: {oldZoneName}");
            //NetworkManager.Singleton.SceneManager.UnloadScene(oldZoneName, new HashSet<ulong> { clientId });
            NetworkManager.Singleton.SceneManager.UnloadScene(SceneManager.GetSceneByName(oldZoneName));
            // Objects in oldZoneName would have been hidden for this client already during the visibility
            // update for the new scene.
        }
    }

    // --- World Persistence - SAVE CHANGES (SERVER ONLY) ---
    // This method is called by InteractableObject.cs
    public async void SaveInteractableObjectState(string sceneName, InteractableObjectData data)
    {
        if (!IsServer) return;
        Debug.Log($"Server: Saving state for object {data.UniqueID} in {sceneName} to backend.");
        try
        {
            // Make an HTTP POST/PUT request to your custom Backend World Persistence Service
            // Convert data to JSON for the request body
            string jsonData = JsonUtility.ToJson(data); // Assumes InteractableObjectData is serializable
            var request = new UnityWebRequest($"{worldPersistenceApiUrl}/zones/{sceneName}/objects/{data.UniqueID}", "POST");
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            // Add internal server token for authorization if needed
            // request.SetRequestHeader("X-API-KEY", "YOUR_INTERNAL_SERVER_API_KEY");

            await request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Failed to save object state for {data.UniqueID} in {sceneName}: {request.error} - {request.downloadHandler.text}");
            }
            else
            {
                Debug.Log($"Successfully saved state for object {data.UniqueID} in {sceneName}.");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Exception during saving persistent data for {data.UniqueID}: {e.Message}");
        }
    }

}
