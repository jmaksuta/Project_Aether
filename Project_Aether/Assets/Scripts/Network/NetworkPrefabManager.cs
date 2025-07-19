using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetworkPrefabManager : MonoBehaviour
{
    // Assign your NetworkPrefabsList ScriptableObject in the Inspector
    [SerializeField]
    private NetworkPrefabsList myNetworkPrefabsList;

    public static NetworkPrefabManager Instance { get; private set; }

    private Dictionary<string, GameObject> _prefabLookup; // Add this for efficient lookup

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Optional: if you want this to persist across scenes
        }

        // Initialize the lookup dictionary
        _prefabLookup = new Dictionary<string, GameObject>();
        if (myNetworkPrefabsList != null)
        {
            foreach (var entry in myNetworkPrefabsList.PrefabList)
            {
                if (entry.Prefab != null)
                {
                    if (!_prefabLookup.ContainsKey(entry.Prefab.name))
                    {
                        _prefabLookup.Add(entry.Prefab.name, entry.Prefab);
                    }
                    else
                    {
                        Debug.LogWarning($"Duplicate network prefab name detected in NetworkPrefabsList: {entry.Prefab.name}. Only the first one will be accessible by name via NetworkPrefabManager.");
                    }
                }
            }
        }
        else
        {
            Debug.LogError("NetworkPrefabsList is not assigned to NetworkPrefabManager! Prefab lookup by name will not work.");
        }
    }

    /// <summary>
    /// Finds a registered network prefab by its GameObject name.
    /// </summary>
    /// <param name="prefabName">The name of the GameObject that is the root of the prefab.</param>
    /// <returns>The GameObject reference to the network prefab, or null if not found.</returns>
    public GameObject GetNetworkPrefabByName(string prefabName)
    {
        if (_prefabLookup == null || _prefabLookup.Count == 0)
        {
            Debug.LogError("NetworkPrefabManager's prefab lookup dictionary is not initialized or empty. Check if NetworkPrefabsList is assigned and populated.");
            return null;
        }

        if (_prefabLookup.TryGetValue(prefabName, out GameObject prefab))
        {
            return prefab;
        }

        Debug.LogWarning($"Network Prefab with name '{prefabName}' not found in the registered list. " +
                         "Ensure it's added to the NetworkPrefabsList asset and that asset is assigned to the NetworkPrefabManager.");
        return null;
    }

    // The example SpawnPrefabExample method is not strictly necessary for fixing "NetworkPrefabManager does not exist"
    // but it shows how GetNetworkPrefabByName would be used.
    /*
    public void SpawnPrefabExample(string prefabNameToSpawn, Vector3 position, Quaternion rotation, ulong? ownerClientId = null)
    {
        if (!NetworkManager.Singleton.IsServer)
        {
            Debug.LogWarning("Only the server can spawn network objects.");
            return;
        }

        GameObject prefab = GetNetworkPrefabByName(prefabNameToSpawn);
        if (prefab != null)
        {
            GameObject instance = Instantiate(prefab, position, rotation);
            NetworkObject networkObject = instance.GetComponent<NetworkObject>();

            if (networkObject != null)
            {
                networkObject.Spawn(ownerClientId);
                Debug.Log($"Spawned {prefabNameToSpawn} with Network ID: {networkObject.NetworkObjectId}");
            }
            else
            {
                Debug.LogError($"Prefab '{prefabNameToSpawn}' does not have a NetworkObject component!");
                Destroy(instance); // Destroy the local instance if it can't be networked
            }
        }
        else
        {
            Debug.LogError($"Could not find network prefab named: {prefabNameToSpawn}");
        }
    }
    */
}
