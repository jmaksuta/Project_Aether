using System;
using System.Text.Json.Serialization;

namespace ProjectAether.Objects.Net._2._1.Standard.Models
{
    [Serializable]
    public class GameCharacter : GameObject
    {
        public const int STARTING_HEALTH = 100;
        public const int STARTING_MANA = 50;
        public const int STARTING_LEVEL = 1;
        public const long STARTING_EXPERIENCE = 0;
        public const string DEFAULT_CHARACTER_CLASS = "Warrior"; // Default character class
        public const string DEFAULT_NAME = "Adventurer"; // Default name for a new character


        public override int Id { get; set; } // Unique identifier for the character
        public override string Name { get; set; } // Character's name
        public override string Description { get; set; } // Brief description of the character
        //public string CharacterClass { get; set; } // e.g., "Warrior", "Mage", "Rogue"
        public int archetypeDefinitionId { get; set; } // Foreign key to ArchetypeDefinition
        public ArchetypeDefinition ArchetypeDefinition { get; set; } // Navigation property to ArchetypeDefinition
        public int Level { get; set; } // Character's level
        public long Experience { get; set; } // Total experience points
        public int Health { get; set; } // Current health points
        public int Mana { get; set; } // Current mana points


        public int InventoryId { get; set; } // Foreign key to Inventory
        [JsonIgnore]
        public Inventory Inventory { get; set; } // Navigation property to Inventory


        public GameCharacter() : base()
        {
            Id = 0; // Default value for Id
            Name = string.Empty;
            Description = string.Empty;
            archetypeDefinitionId = 0;
            Level = 1; // Default level
            Experience = 0; // Default experience
            Health = 100; // Default health
            Mana = 50; // Default mana
            InventoryId = 0;
            Inventory = new Inventory();
        }
    }
}
