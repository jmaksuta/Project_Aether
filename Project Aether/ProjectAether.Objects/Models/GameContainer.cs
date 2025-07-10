using System.Text.Json.Serialization;

namespace ProjectAether.Objects.Models
{
    public class GameContainer : GameObject
    {
        public override int Id { get; set; } // Unique identifier for the game container

        public int InventoryId { get; set; } // Foreign key to Inventory
        [JsonIgnore]
        public Inventory Inventory { get; set; } // Navigation property to Inventory

        public GameContainer() : base()
        {
            Id = 0; // Default value for Id
            InventoryId = 0;
            Inventory = new Inventory();       
        }

    }
}
