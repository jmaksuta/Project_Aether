using System.Text.Json.Serialization;

namespace Project_Aether_Backend.Models
{
    public class GameContainer : GameObject
    {
        public override int Id { get; set; } // Unique identifier for the game container

        public int InventoryId { get; set; } // Foreign key to Inventory
        [JsonIgnore]
        public Inventory Inventory { get; set; } // Navigation property to Inventory

        public GameContainer() : base()
        {
            this.Id = 0; // Default value for Id
            this.InventoryId = 0;
            this.Inventory = new Inventory();       
        }

        public GameContainer(int id, string ownerId, GameObject owner, ICollection<InventoryItem> items) : this()
        {
            this.Id = id;
        }
    }
}
