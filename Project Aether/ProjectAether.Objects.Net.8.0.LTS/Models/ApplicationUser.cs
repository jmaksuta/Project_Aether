using Microsoft.AspNetCore.Identity;

namespace ProjectAether.Objects.Models
{
    public class ApplicationUser : IdentityUser
    {
        // Add custom fields for your user here.
        public DateTime DateRegistered { get; set; }
        public PlayerProfile Player { get; set; }

    }
}
