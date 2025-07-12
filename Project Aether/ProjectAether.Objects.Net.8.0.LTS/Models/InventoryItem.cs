using System.Text.Json.Serialization;

namespace ProjectAether.Objects.Models
{
    public class InventoryItem : GameObject
    {
        public override int Id { get; set; }

        public int Quantity { get; set; }
        public string ItemType { get; set; } = string.Empty; // e.g., "Weapon", "Potion", "Material"

        public bool IsStackable { get; set; }

        public int InventoryId { get; set; } // Foreign key to Inventory, if you have a separate Inventory entity   
        [JsonIgnore]
        public Inventory Inventory { get; set; } // Navigation property to Inventory


        //public int GameObjectId { get; set; } // e.g., "SWORD_IRON", "POTION_HP_SMALL"
        //public GameObject Item { get; set; } // Reference to the GameObject representing the item   

        public InventoryItem() : base()
        {
            Id = 0;
            Quantity = 0;
            ItemType = string.Empty;
            IsStackable = false;
            InventoryId = 0;   
            Inventory = new Inventory();
        }

    }
}
