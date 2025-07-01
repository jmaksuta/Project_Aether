namespace Project_Aether_Backend.Models
{
    public class PlayerProfile
    {
        public int Id { get; set; }

        public string UserId { get; set; } // Foreign key to ApplicationUser.Id
        public ApplicationUser User { get; set; } // Navigation property

        public string PlayerName { get; set; }
        
        public ICollection<PlayerCharacter> Characters { get; set; } = new List<PlayerCharacter>(); 

    }
}
