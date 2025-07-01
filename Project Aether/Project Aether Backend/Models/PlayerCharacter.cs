using System.Text.Json.Serialization;

namespace Project_Aether_Backend.Models
{
    public class PlayerCharacter : GameCharacter
    {
        public override int Id { get; set; }

        public string profilePictureId { get; set; } // ID of the profile picture associated with the player character  
        public string DisplayName { get; set; }

        public int PlayerProfileId { get; set; }
        [JsonIgnore]
        public PlayerProfile Player { get; set; } // Navigation property to PlayerProfile
        // ... add more character stats, equipped items IDs etc.


        public PlayerCharacter() : base()
        {
            this.Id = 0;
            this.profilePictureId = string.Empty; // Default value for profile picture ID   
            this.DisplayName = string.Empty; // Default value for display name  
            this.ObjectType = GameObjectType.PlayerCharacter; // Set the object type to PlayerCharacter
        }

    }
}
