using Microsoft.AspNetCore.Identity;

namespace Project_Aether_Backend.Models
{
    public class ApplicationUser : IdentityUser
    {
        // Add custom fields for your user here.
        public DateTime DateRegistered { get; set; }
    }
}
