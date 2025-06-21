using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project_Aether_Backend.Data;
using Project_Aether_Backend.Models;
using System.Security.Claims; // For accessing User.FindFirstValue

namespace Project_Aether_Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Requires authentication for all endpoints in this controller.
    public class PlayerController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public PlayerController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET api/Player/profile
        [HttpGet("profile")]
        public async Task<IActionResult> GetPlayerProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get UserId from JWT Token

            var profile = await _context.PlayerProfiles
                .Include(p => p.Inventory) // Include inventory items   
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (profile == null)
            {
                // If no profile exists, create a default one
                var newProfile = new PlayerProfile
                {
                    UserId = userId,
                    DisplayName = User.Identity.Name, // Use username as default display name
                    Level = 1,
                    Experience = 0,
                    Health = 100,
                    Mana = 50
                };
                _context.PlayerProfiles.Add(newProfile);
                await _context.SaveChangesAsync();
                return Ok(newProfile);
            }
            return Ok(profile);
        }

        // POST api/Player/profile
        [HttpPost("profile")]
        public async Task<IActionResult> UpdatePlayerProfile([FromBody] PlayerProfileUpdateDto model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get UserId from JWT Token

            var profile = await _context.PlayerProfiles
                .FirstOrDefaultAsync(p => p.UserId == userId);
            if (profile == null)
            {
                return NotFound(new { Message = "Player profile not found." });
            }
            profile.DisplayName = model.DisplayName ?? profile.DisplayName;
            // Only allow specific updates from client, e.g., cosmetic changes, not stats directly 
            // For stats, you'd have game logic servers handling updates 
            // profile.Level = model.Level; 
            // DON'T allow client to update sensitive stats directly 
            // profile.Experience = model.Experience;
            _context.PlayerProfiles.Update(profile);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Player profile updated successfully." });
        }

        // GET api/Player/inventory
        [HttpGet("inventory")]
        public async Task<IActionResult> GetPlayerInventory()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var profile = await _context.PlayerProfiles
                .Include(p => p.Inventory)
                .FirstOrDefaultAsync(p => p.UserId == userId);
            if (profile == null)
            {
                return NotFound(new { Message = "Player profile not found." });
            }
            return Ok(profile.Inventory);
        }

        // POST api/Player/inventory/add
        // This is a simplified example. Real game would have robust validation.
        [HttpPost("inventory/add")]
        public async Task<IActionResult> AddItemToInventory([FromBody] AddItemRequestDto model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var profile = await _context.PlayerProfiles
                .Include(p => p.Inventory)
                .FirstOrDefaultAsync(p => p.UserId == userId);
            if (profile == null)
            {
                return NotFound(new { Message = "Player profile not found." });
            }
            var existingItem = profile.Inventory
                .FirstOrDefault(i => i.ItemId == model.ItemId);
            if (existingItem != null && model.IsStackable) // Example: items are stackable
            {
                existingItem.Quantity += model.Quantity;
                _context.InventoryItems.Update(existingItem);
            }
            else
            {
                var newItem = new InventoryItem
                {
                    PlayerProfileId = profile.Id,
                    ItemId = model.ItemId,
                    Quantity = model.Quantity,
                    ItemType = model.ItemType
                    // Or derive from ItemId lookup
                };
                profile.Inventory.Add(newItem);
                // Add to navigation property
            }
            await _context.SaveChangesAsync();
            return Ok(new { Message = $"{model.Quantity}x {model.ItemId} added to inventory." });
        }

        // DELETE api/Player/inventory/{itemId}
        [HttpDelete("inventory/remove/{itemId}")]
        public async Task<IActionResult> RemoveItemFromInventory(string itemId, [FromQuery] int quantity = 1)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var profile = await _context.PlayerProfiles
                .Include(p => p.Inventory)
                .FirstOrDefaultAsync(p => p.UserId == userId);
            if (profile == null)
            {
                return NotFound(new { Message = "Player profile not found." });
            }
            var itemToRemove = profile.Inventory
                .FirstOrDefault(i => i.ItemId == itemId);
            if (itemToRemove == null)
            {
                return NotFound(new { Message = "Item not found in inventory." });
            }
            if (itemToRemove.Quantity > quantity)
            {
                itemToRemove.Quantity -= quantity;
                _context.InventoryItems.Update(itemToRemove);
            }
            else
            {
                _context.InventoryItems.Remove(itemToRemove);
            }
            await _context.SaveChangesAsync();
            return Ok(new { Message = $"{quantity}x {itemId} removed from inventory." });
        }

        // --- DTOs (Data Transfer Objects) ---
        public class PlayerProfileUpdateDto
        {
            public string DisplayName { get; set; }
            // Add other updateable properties
        }
        public class AddItemRequestDto
        {
            public string ItemId { get; set; }
            public int Quantity { get; set; }
            public string ItemType { get; set; }
            // Useful for initial item categorization
            public bool IsStackable { get; set; } = true;
            // Example property
        }
    }
}