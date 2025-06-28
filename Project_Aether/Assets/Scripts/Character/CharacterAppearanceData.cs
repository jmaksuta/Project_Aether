using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CharacterAppearanceData
{
    //public string headPartName;
    //public string torsoPartName;
    //public string legsPartName;
    //// ... other part names
    //public Dictionary<string, float> blendShapeValues; // Blend shape name -> weight
    //public string skinColorHex; // Or use RGB values
    //// ... other color values


    // --- Modular Parts ---
    // Instead of saving the GameObject prefab directly, save its identifier (e.g., name or index).
    // Using index is generally more robust as prefab names might change.
    public int headIndex = -1; // -1 can mean "not set" or default
    public int torsoIndex = -1;
    public int legsIndex = -1;
    public int handsIndex = -1;
    public int feetIndex = -1;
    // Add more indices for other CharacterPartType enums you have (e.g., hairIndex, armorIndex)

    // --- Blend Shapes (if you implement them) ---
    // A Dictionary is great for blend shapes: blend shape name -> weight value
    // However, JsonUtility cannot directly serialize Dictionaries.
    // So, we need a wrapper class for a list of key-value pairs.
    public List<BlendShapeEntry> blendShapeValues = new List<BlendShapeEntry>();

    // Helper class to make Dictionary serializable by JsonUtility
    [System.Serializable]
    public class BlendShapeEntry
    {
        public string name;
        public float weight;

        public BlendShapeEntry(string n, float w)
        {
            name = n;
            weight = w;
        }
    }

    // --- Material Colors (if you implement tinting) ---
    // Storing colors as hex strings is common and easy to save/load.
    public string skinColorHex = "#FFFFFF"; // Default to white
    public string hairColorHex = "#000000"; // Default to black
    // Add more hex strings for other customizable material colors

    // --- Constructor (Optional but good for defaults) ---
    public CharacterAppearanceData()
    {
        // Initialize default values here if needed, or let the -1 indices signify defaults
    }

}
