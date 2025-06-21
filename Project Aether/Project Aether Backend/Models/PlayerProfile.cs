namespace Project_Aether_Backend.Models
{
    public class PlayerProfile
    {
        public int Id { get; set; }
        public string UserId { get; set; } // Foreign key to ApplicationUser.Id
        public ApplicationUser User { get; set; } // Navigation property

        public string DisplayName { get; set; }
        public int Level { get; set; }
        public long Experience { get; set; }
        public int Health { get; set; }
        public int Mana { get; set; }
        // ... add more character stats, equipped items IDs etc.

        public ICollection<InventoryItem> Inventory { get; set; } = new List<InventoryItem>();
    }
}
