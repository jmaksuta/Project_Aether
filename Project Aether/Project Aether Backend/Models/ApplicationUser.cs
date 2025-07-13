using Microsoft.AspNetCore.Identity;
using ProjectAether.Objects.Net._2._1.Standard.Models;

namespace Project_Aether_Backend.Models
{
    public class ApplicationUser : IdentityUser
    {
        public DateTime DateRegistered { get; internal set; }

        public ApplicationUser() : base()
        {
            DateRegistered = DateTime.UtcNow; // Set the registration date to the current UTC time
        }
    }
}
