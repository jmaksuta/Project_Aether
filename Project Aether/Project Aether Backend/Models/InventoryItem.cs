using System.Text.Json.Serialization;

namespace Project_Aether_Backend.Models
{
    public class InventoryItem
    {
        public int Id { get; set; }
        public int PlayerProfileId { get; set; } // Foreign key
        [JsonIgnore]
        public PlayerProfile PlayerProfile { get; set; } // Navigation property

        public string ItemId { get; set; } // e.g., "SWORD_IRON", "POTION_HP_SMALL"
        public int Quantity { get; set; }
        public string ItemType { get; set; } // e.g., "Weapon", "Potion", "Material"
    }
}
