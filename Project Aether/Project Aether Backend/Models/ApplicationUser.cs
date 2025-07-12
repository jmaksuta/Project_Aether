using Microsoft.AspNetCore.Identity;
using ProjectAether.Objects.Net._2._1.Standard.Models;

namespace Project_Aether_Backend.Models
{
    public class ApplicationUser : IdentityUser
    {
        public User user { get; set; } = new User(); // Navigation property to User model
        public DateTime DateRegistered { get; internal set; }

        public PlayerProfile Player { get; set; }

        public ApplicationUser() : base()
        {
            user = new User();
            DateRegistered = DateTime.UtcNow; // Set the registration date to the current UTC time
        }
    }
}
