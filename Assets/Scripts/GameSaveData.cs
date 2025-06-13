using System.Collections.Generic;
using System;
using UnityEngine;

[System.Serializable]
public class GameSaveData
{
    // --- Player Progress ---
    public int currentLevel = 1;
    public int score = 0;
    public float playerHealth = 100f;
    public string lastCheckpointName = "StartArea";

    // --- Inventory (Example) ---
    public List<string> inventoryItems = new List<string>();
    public Dictionary<string, int> itemCounts = new Dictionary<string, int>(); // Dictionaries need custom serialization or a wrapper

    // For dictionaries, you can use a serializable wrapper like this:
    [System.Serializable]
    public class SerializableDictionary<TKey, TValue>
    {
        public List<TKey> keys = new List<TKey>();
        public List<TValue> values = new List<TValue>();

        public Dictionary<TKey, TValue> ToDictionary()
        {
            Dictionary<TKey, TValue> dict = new Dictionary<TKey, TValue>();
            for (int i = 0; i < Math.Min(keys.Count, values.Count); i++)
            {
                dict[keys[i]] = values[i];
            }
            return dict;
        }

        public void FromDictionary(Dictionary<TKey, TValue> dict)
        {
            keys.Clear();
            values.Clear();
            foreach (var item in dict)
            {
                keys.Add(item.Key);
                values.Add(item.Value);
            }
        }
    }
    public SerializableDictionary<string, int> serializableItemCounts = new SerializableDictionary<string, int>();


    // --- Character Appearance Data (integrate your existing CharacterAppearanceData) ---
    public CharacterAppearanceData characterAppearance;

    // --- Constructor (Optional) ---
    public GameSaveData()
    {
        // Initialize default values here if needed
        inventoryItems.Add("Default Item 1");
        inventoryItems.Add("Default Item 2");
        serializableItemCounts.FromDictionary(new Dictionary<string, int> { { "Potion", 5 }, { "Coin", 100 } });
        characterAppearance = new CharacterAppearanceData(); // Initialize with default character data
    }
}