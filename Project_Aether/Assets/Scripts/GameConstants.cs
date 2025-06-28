using UnityEngine;

public static class GameConstants
{
    // Define a constant that changes based on the build type
#if UNITY_SERVER
    public const int MaxPlayers = 64; // Value for server build
    public const string ConnectionIP = "192.168.1.147"; //"0.0.0.0"; // Listen on all interfaces
    public const ushort ConnectionPort = 55000;// 7777; // Default port for server connections   
    public const bool IsDedicatedServer = true;
#else
    public const int MaxPlayers = 8; // Value for client/host build
    public const string ConnectionIP = "192.168.1.147"; // "127.0.0.1"; // Default for client to connect to local host
    public const ushort ConnectionPort = 55000; // 7777; // Default port for server connections   
    public const bool IsDedicatedServer = false;
#endif

    public const string BOOTSTRAP_SCENE_NAME = "00_BootstrapScene";
    public const string PERSISTENT_SCENE_NAME = "01_PersistentScene";
    public const string FOREST_ZONE_SCENE_NAME = "02_ForestZone";
    public const string DUNGEON_ZONE_SCENE_NAME = "03_DungeonZone";

    // Default connection port for Netcode's Unity Transport for direct connections
    public const string GAME_SERVER_IP_ADDRESS = "192.168.1.21"; // Ensure this matches your dedicated server's open port
    public const ushort GAME_SERVER_PORT = 7777; // Ensure this matches your dedicated server's open port
    
    public const string FALLBACK_LOCAL_IP_ADDRESS = "127.0.0.1"; // For local testing of direct connect
    public const ushort FALLBACK_CONNECTION_PORT = 7777;


    // Other shared constants
    public const float PlayerSpeed = 5.0f;

    public static void LogBuildType()
    {
#if UNITY_SERVER
        Debug.Log("This is a SERVER build!");
#else
        Debug.Log("This is a CLIENT/HOST build!");
#endif
        Debug.Log($"Max Players: {MaxPlayers}");
        Debug.Log($"Connection IP: {ConnectionIP}");
    }
}
