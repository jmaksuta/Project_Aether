using System.Text.Json.Serialization;

namespace Project_Aether_Backend.Models
{
    [Serializable]
    public class Inventory
    {
        public int Id { get; set; } // Unique identifier for the inventory
        

        [JsonIgnore]
        public ICollection<InventoryItem> Items { get; set; } = new List<InventoryItem>(); // Collection of items in the inventory

        public Inventory() : base()
        {
            this.Id = 0; // Default value for Id
            this.Items = new List<InventoryItem>();
        }

        public Inventory(int id, ICollection<InventoryItem> items)
        {
            this.Id = id;
            this.Items = items ?? new List<InventoryItem>();
        }

        public Inventory(ICollection<InventoryItem> items)
        {
            this.Id = 0; // Default value for Id
            this.Items = items ?? new List<InventoryItem>();
        }
    }
}
