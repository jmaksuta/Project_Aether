using ProjectAether.Objects.Net._2._1.Standard.Models;

namespace Project_Aether_Backend.Models
{
    [Serializable]
    public class PlayerProfile
    {
        public int Id { get; set; }

        public string ApplicationUserId { get; set; } // Foreign key to ApplicationUser.Id
        public ApplicationUser ApplicationUser { get; set; } // Navigation property

        public string PlayerName { get; set; }

        public ICollection<PlayerCharacter> Characters { get; set; } = new List<PlayerCharacter>();

        public PlayerProfile() : base()
        {
            Id = 0;
            ApplicationUserId = string.Empty;
            ApplicationUser = new ApplicationUser();
            PlayerName = string.Empty;
            Characters = new List<PlayerCharacter>();
        }

    }
}
