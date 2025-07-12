using System;
using System.Collections.Generic;

namespace ProjectAether.Objects.Net._2._1.Standard.Models
{
    [Serializable]
    public class PlayerProfile
    {
        public int Id { get; set; }

        public string UserId { get; set; } // Foreign key to ApplicationUser.Id
        public User User { get; set; } // Navigation property

        public string PlayerName { get; set; }

        public ICollection<PlayerCharacter> Characters { get; set; } = new List<PlayerCharacter>();

        public PlayerProfile() : base()
        {
            Id = 0;
            UserId = string.Empty;
            User = new User();
            PlayerName = string.Empty;
            Characters = new List<PlayerCharacter>();
        }

    }
}
