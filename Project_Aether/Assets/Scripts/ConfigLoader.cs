using System.Collections.Generic;
using System;
using UnityEngine;

public class ConfigLoader : MonoBehaviour
{
    public GameConfiguration CurrentConfig { get; private set; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    void LoadConfig()
    {
        TextAsset jsonTextAsset = Resources.Load<TextAsset>("GameConfig"); // "GameConfig" without extension
        if (jsonTextAsset == null)
        {
            Debug.LogError("GameConfig.json not found in Resources folder!");
            return;
        }

        string jsonContent = jsonTextAsset.text;

        try
        {
            CurrentConfig = JsonUtility.FromJson<GameConfiguration>(jsonContent);
        }
        catch (Exception e)
        {
            Debug.LogError($"Error parsing GameConfig.json: {e.Message}");
        }
    }

    public string GetApiKey()
    {
        if (CurrentConfig == null)
        {
            LoadConfig(); // Load the config if not already loaded
        }
        return CurrentConfig?.ApiKeys.GameServer; // Return the API key or null if not set
    }

}
