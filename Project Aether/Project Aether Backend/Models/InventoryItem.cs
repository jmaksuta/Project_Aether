using System.Text.Json.Serialization;

namespace Project_Aether_Backend.Models
{
    public class InventoryItem : GameObject
    {
        public override int Id { get; set; }
        
        public int Quantity { get; set; }
        public string ItemType { get; set; } // e.g., "Weapon", "Potion", "Material"


        public int InventoryId { get; set; } // Foreign key to Inventory, if you have a separate Inventory entity   
        [JsonIgnore]
        public Inventory Inventory { get; set; } // Navigation property to Inventory


        //public int GameObjectId { get; set; } // e.g., "SWORD_IRON", "POTION_HP_SMALL"
        //public GameObject Item { get; set; } // Reference to the GameObject representing the item   

    }
}
