using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project_Aether_Backend.Data;
using Project_Aether_Backend.Models;
using ProjectAether.Objects.Net._2._1.Standard.Models;
using System.Security.Claims; // For accessing User.FindFirstValue

namespace Project_Aether_Backend.Controllers
{
    [Route("api/[controller]")]
    public class PlayerController : AuthorizedControllerBase
    {
        public PlayerController(ApplicationDbContext context, UserManager<ApplicationUser> userManager) : base(context, userManager) { }

        //TODO: GET api/Player/profile
        [HttpGet("profile")]
        public async Task<IActionResult> GetPlayerProfile()
        {
            var profile = await _context.PlayerProfiles
                .Include(p => p.Characters) // Include Player Characters
                .FirstOrDefaultAsync(p => p.UserId == this.UserId);

            if (profile == null)
            {
                // If no profile exists, create a default one
                var newProfile = await createNewPlayerProfile();

                return Ok(newProfile);
            }
            return Ok(profile);
        }

        //TODO: POST api/Player/profile
        [HttpPost("profile")]
        public async Task<IActionResult> UpdatePlayerProfile([FromBody] PlayerProfileUpdateDto model)
        {
            var profile = await _context.PlayerProfiles
                .Include(p => p.User) // Include ApplicationUser for profile updates
                .FirstOrDefaultAsync(p => p.UserId == this.UserId);
            if (profile == null)
            {
                var newProfile = await createNewPlayerProfile();
                
                return Ok(new { Message = "Player profile created successfully." });
            }
            profile.PlayerName = model.PlayerName ?? profile.PlayerName;
            // Only allow specific updates from client, e.g., cosmetic changes, not stats directly 
            // For stats, you'd have game logic servers handling updates 
            // profile.Level = model.Level; 
            // DON'T allow client to update sensitive stats directly 
            _context.PlayerProfiles.Update(profile);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Player profile updated successfully." });
        }

        private async Task<PlayerProfile> createNewPlayerProfile()
        {
            // Create new PlayerProfile
            PlayerProfile playerProfile = new PlayerProfile
            {
                UserId = this.UserId,
                User = new User(),
                PlayerName = this.UserName, // Use username as default display name
                Characters = new List<PlayerCharacter>()
            };

            await _context.PlayerProfiles.AddAsync(playerProfile);
            await _context.SaveChangesAsync();

            return playerProfile;
        }

        // --- DTOs (Data Transfer Objects) ---
        public class PlayerProfileUpdateDto
        {
            public string PlayerName { get; set; } = string.Empty;
            // Add other updateable properties
        }

        //public class AddItemRequestDto
        //{
        //    public string ItemId { get; set; } = string.Empty;
        //    public int Quantity { get; set; }
        //    public string ItemType { get; set; } = string.Empty;
        //    // Useful for initial item categorization
        //    public bool IsStackable { get; set; } = true;
        //    // Example property
        //}
    }
}