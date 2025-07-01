using System.Text.Json.Serialization;

namespace Project_Aether_Backend.Models
{
    [Serializable]
    public class GameCharacter : GameObject
    {
        public override int Id { get; set; } // Unique identifier for the character
        public override string Name { get; set; } // Character's name
        public override string Description { get; set; } // Brief description of the character
        public string CharacterClass { get; set; } // e.g., "Warrior", "Mage", "Rogue"
        public int Level { get; set; } // Character's level
        public long Experience { get; set; } // Total experience points
        public int Health { get; set; } // Current health points
        public int Mana { get; set; } // Current mana points


        public int InventoryId { get; set; } // Foreign key to Inventory
        [JsonIgnore]
        public Inventory Inventory { get; set; } // Navigation property to Inventory


        public GameCharacter() : base()
        {
            this.Id = 0; // Default value for Id
            this.Name = string.Empty;
            this.Description = string.Empty;
            this.CharacterClass = string.Empty;
            this.Level = 1; // Default level
            this.Experience = 0; // Default experience
            this.Health = 100; // Default health
            this.Mana = 50; // Default mana
            this.InventoryId = 0;   
            this.Inventory = new Inventory();
        }
    }
}
