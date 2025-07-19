using System.Collections.Generic;
using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System.Collections;
using Assets.Scripts.Models;
using Assets.Scripts;
using static ProjectAetherBackendApi;
using ProjectAether.Objects.Net._2._1.Standard.Models;
using System.Linq;

public class ServerZoneManager : NetworkBehaviour
{
    [SerializeField]
    public ConfigLoader configLoader; // Reference to ConfigLoader to get API Key

    public static ServerZoneManager Instance { get; private set; }

    [Header("Zone Scenes")]
    public List<string> allZoneSceneNames; // Assign in Inspector (e.g., "02_ForestZone", "03_DungeonZone")

    // Server-side tracking: Which client is in which scene
    private Dictionary<ulong, string> clientCurrentZone = new Dictionary<ulong, string>();

    // Backend API URL for world persistence
    [SerializeField]
    private string worldPersistenceApiUrl = ApiSettings.GetApiUrl(ApiSettings.ApiDomains.WorldZone);// "https://api.yourgame.com/worldpersistence";

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

                yield return NetworkManager.Singleton.SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);

                Debug.Log($"Server: Finished loading {sceneName}.");

                //// After a scene is loaded on the server, load its persistent data from your Backend.
                //LoadZonePersistentData(sceneName);
            }
        }
        Debug.Log("Server: All zone scenes loaded.");
        // TODO: fix this. added return null; to avoid compiler error
        yield return null;
    }

    private void EnsureConfigLoader()
    {
        if (configLoader == null)
        {
            configLoader = this.GetComponent<ConfigLoader>();
            if (configLoader == null)
            {
                // Try to find ConfigLoader in parent hierarchy
                configLoader = this.GetComponentInParent<ConfigLoader>();
            }
            if (configLoader == null)
            {
                // Try to find ConfigLoader in the scene
                configLoader = FindObjectOfType<ConfigLoader>();
            }
            if (configLoader == null)
            {
                Debug.LogError("ConfigLoader component not found! Please ensure it is present in the scene or attached to this GameObject.");
                return;
            }
        }
    }

    // --- World Persistence: Load Data from ASP.NET Core API ---
    private async void LoadZonePersistentData(string sceneName)
    {
        Debug.Log($"Server: Loading persistent data for zone: {sceneName} from backend.");
        try
        {
            EnsureConfigLoader();
            string apiKey = configLoader.GetApiKey();
            // Make an HTTP GET request to your ASP.NET Core API for this zone's data
            WorldZone worldZone = await ProjectAetherBackendApi.GetWorldZoneBySceneName(apiKey, sceneName);

            List<ProjectAether.Objects.Net._2._1.Standard.Models.GameObject> loadedGameObjects = await ProjectAetherBackendApi.GetGameObjectsInWorldZone(apiKey, worldZone);

            if (loadedGameObjects == null || loadedGameObjects.Count == 0)
            {
                Debug.LogWarning($"No persistent data found for zone: {sceneName}");
                return;
            }

            // Find all InteractableObjects in the newly loaded scene
            Scene loadedScene = SceneManager.GetSceneByName(sceneName);
            if (loadedScene.IsValid())
            {
                List<InteractableObject> interactables = GetInteractablesInScene(loadedScene);

                CreateAndUpdateInteractablesInScene(loadedGameObjects, interactables);

                Debug.Log($"Server: Successfully applied persistent data to {interactables.Count} interactable objects in {sceneName}.");
            }

            Debug.Log($"Server: Successfully loaded persistent data for zone: {sceneName}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Exception during loading persistent data for {sceneName}: {e.Message}");
        }
    }

    private List<InteractableObject> GetInteractablesInScene(Scene loadedScene)
    {
        List<InteractableObject> interactables = new List<InteractableObject>();
        foreach (UnityEngine.GameObject rootObj in loadedScene.GetRootGameObjects())
        {
            interactables.AddRange(rootObj.GetComponentsInChildren<InteractableObject>(true));
        }
        return interactables;
    }

    private void CreateAndUpdateInteractablesInScene(List<ProjectAether.Objects.Net._2._1.Standard.Models.GameObject> loadedGameObjects, List<InteractableObject> interactables)
    {
        UpdateInteractablesInScene(loadedGameObjects, interactables);
        // Create new objects that should exist in scene but do not currently exist.
        CreateNewObjectsInScene(loadedGameObjects, interactables);
    }

    private void UpdateInteractablesInScene(List<ProjectAether.Objects.Net._2._1.Standard.Models.GameObject> loadedGameObjects, List<InteractableObject> interactables)
    {
        foreach (var interactable in interactables)
        {
            // TODO: add this for real game.
            // In a real game, each interactable would have a unique ID that matches backend data
            // Find matching data in loadedObjects and apply its state (color, interacted status, position, etc.)
            // Example: var data = loadedObjects.FirstOrDefault(o => o.UniqueID == interactable.uniqueID);
            // if (data != null) interactable.ApplyStateFromData(data); 
            // You'd need to add ApplyStateFromData to InteractableObject

            var data = loadedGameObjects.FirstOrDefault(go => go.Id.ToString() == interactable.uniqueID);
            if (data != null)
            {
                interactable.ApplyStateFromData(data);
            }
        }
    }

    private void CreateNewObjectsInScene(List<ProjectAether.Objects.Net._2._1.Standard.Models.GameObject> loadedGameObjects, List<InteractableObject> interactables)
    {
        foreach (var gameObjectData in loadedGameObjects)
        {
            // Check if the object already exists in the scene
            var existingObject = interactables.FirstOrDefault(i => i.uniqueID == gameObjectData.Id.ToString());
            if (existingObject == null)
            {
                // Create a new InteractableObject instance and apply the data
                UnityEngine.GameObject newObject = NetworkPrefabManager.Instance.GetNetworkPrefabByName(gameObjectData.PrefabName);
                Vector3 newObjectPosition = new Vector3(gameObjectData.xPosition, gameObjectData.yPosition, gameObjectData.zPosition);
                // TODO: add the network rotation here.
                UnityEngine.GameObject newObj = Instantiate(newObject, newObjectPosition, Quaternion.identity);
                InteractableObject newInteractable = newObj.GetComponent<InteractableObject>();
                if (newInteractable != null)
                {
                    newInteractable.ApplyStateFromData(gameObjectData);
                    Debug.Log($"Server: Created new InteractableObject {newInteractable.name} with ID {gameObjectData.Id}");
                }
            }
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
        // TODO: Save player data to backend here (e.g., position, inventory, etc.)
    }

    // --- Scene Load Completion & Visibility Management ---
    private void OnServerSceneLoadComplete(ulong clientId, string sceneName, LoadSceneMode loadSceneMode)
    {
        //if (!IsServer) return; // This callback is also triggered on client for IsHost case
        //if (loadSceneMode == LoadSceneMode.Single && sceneName == GameConstants.PERSISTENT_SCENE_NAME)
        //{
        //    Debug.Log($"Server: Client {clientId} finished loading PersistentScene.");
        //    return;
        //}
        //if (allZoneSceneNames.Contains(sceneName))
        //{
        //    Debug.Log($"Server: Client {clientId} completed loading zone scene {sceneName}. Updating visibility.");
        //    StartCoroutine(UpdateVisibilityForClientAfterLoad(clientId, sceneName));
        //}
        //--------------------------------------------
        if (!IsServer) return; // This callback is also triggered on client for IsHost case

        // If the PersistentScene just loaded for the server (initial load)
        if (loadSceneMode == LoadSceneMode.Additive && sceneName == GameConstants.PERSISTENT_SCENE_NAME)
        {
            Debug.Log($"Server: PersistentScene loaded additively for server.");
            // No specific action needed here beyond what LoadAllZoneScenesRoutine handles
            return;
        }

        // If a zone scene just loaded (either initially or for a client transition)
        if (allZoneSceneNames.Contains(sceneName))
        {
            Debug.Log($"Server: Scene {sceneName} completed loading for client {clientId} (or server itself).");
            // Only load persistent data if it's the server loading the scene initially or for a client.
            // Ensure this doesn't double-load if already loaded by LoadAllZoneScenesRoutine.
            // A simple check is if the scene is already valid and loaded for the server.
            if (SceneManager.GetSceneByName(sceneName).isLoaded && SceneManager.GetSceneByName(sceneName).IsValid())
            {
                // This is the correct place to load persistent data for the zone
                LoadZonePersistentData(sceneName);
            }
            ServerCameraManagement(sceneName);
            StartCoroutine(UpdateVisibilityForClientAfterLoad(clientId, sceneName));
        }

    }

    private void ServerCameraManagement(string sceneName)
    {
        // --- Camera Management on Server ---
        // If the server is running with a display (e.g., for debugging), ensure only the relevant camera is active.
        // For a truly headless server, this block is irrelevant.
        if (IsServer) // Ensure this logic only runs on the server instance
        {
            // Disable all cameras in all loaded scenes first
            foreach (string loadedSceneName in allZoneSceneNames)
            {
                Scene currentLoadedScene = SceneManager.GetSceneByName(loadedSceneName);
                if (currentLoadedScene.IsValid() && currentLoadedScene.isLoaded)
                {
                    foreach (UnityEngine.GameObject rootObj in currentLoadedScene.GetRootGameObjects())
                    {
                        Camera[] camerasInScene = rootObj.GetComponentsInChildren<Camera>(true);
                        foreach (Camera cam in camerasInScene)
                        {
                            if (cam.gameObject.activeInHierarchy) // Only if it's currently active
                            {
                                cam.enabled = false;
                                Debug.Log($"Server: Disabled camera in scene {loadedSceneName}: {cam.name}");
                            }
                        }
                    }
                }
            }

            // Now, enable the camera in the newly loaded scene (if it's a zone scene)
            Scene newlyLoadedZoneScene = SceneManager.GetSceneByName(sceneName);
            if (newlyLoadedZoneScene.IsValid() && newlyLoadedZoneScene.isLoaded)
            {
                foreach (UnityEngine.GameObject rootObj in newlyLoadedZoneScene.GetRootGameObjects())
                {
                    Camera[] camerasInScene = rootObj.GetComponentsInChildren<Camera>(true);
                    foreach (Camera cam in camerasInScene)
                    {
                        // We assume the zone scene's camera is the one we want active
                        // You might have a specific tag or name for the "main" camera in each zone
                        if (!cam.enabled)
                        {
                            cam.enabled = true;
                            Debug.Log($"Server: Enabled camera in scene {sceneName}: {cam.name}");
                        }
                    }
                }
            }
            // Also ensure the PersistentScene's camera (if any) is disabled
            Scene persistentScene = SceneManager.GetSceneByName(GameConstants.PERSISTENT_SCENE_NAME);
            if (persistentScene.IsValid() && persistentScene.isLoaded)
            {
                foreach (UnityEngine.GameObject rootObj in persistentScene.GetRootGameObjects())
                {
                    Camera[] camerasInScene = rootObj.GetComponentsInChildren<Camera>(true);
                    foreach (Camera cam in camerasInScene)
                    {
                        if (cam.enabled)
                        {
                            cam.enabled = false;
                            Debug.Log($"Server: Disabled camera in PersistentScene: {cam.name}");
                        }
                    }
                }
            }
        }
        // --- End Camera Management ---

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
                foreach (UnityEngine.GameObject rootObj in zoneUnityScene.GetRootGameObjects())
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
