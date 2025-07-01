using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project_Aether_Backend.Data;
using Project_Aether_Backend.Models;
using System.ComponentModel.DataAnnotations;
using System.Numerics;
using System.Security.Claims;

namespace Project_Aether_Backend.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Requires authentication for all endpoints in this controller.
    public class PlayerCharacterController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        private string UserId
        {
            get
            {
                string result = string.Empty;
                if (User != null)
                {
                    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get UserId from JWT Token
                    if (userId != null)
                    {
                        result = userId;
                    }
                }
                return result;
            }
        }

        private string UserName
        {
            get
            {
                string result = string.Empty;
                if (User != null && User.Identity != null && User.Identity.Name != null)
                {
                    result = User.Identity.Name;
                }
                return result;
            }
        }

        public PlayerCharacterController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET api/PlayerCharacter/
        [HttpGet("")]
        public async Task<IActionResult> GetPlayerCharacters()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get UserId from JWT Token

            IQueryable<PlayerCharacter> all = _context.PlayerCharacters
                .Include(p => p.Inventory) // Include inventory items
                .Where(pc => pc.Player.User.Id == userId);
            //.FirstOrDfaultAsync(p => p.UserId == this.UserId);
            if (all == null || all.Count() == 0)
            {
                return Ok(new List<PlayerCharacter>()); // Return empty list if no characters found
            }
            return Ok(all);
        }

        // GET api/PlayerCharacter/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPlayerCharacterById(string id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get UserId from JWT Token
            var character = await _context.PlayerCharacters
                .Include(p => p.Inventory) // Include inventory items
                .FirstOrDefaultAsync(p => p.Id == Int32.Parse(id) && p.Player.User.Id == userId);
            if (character == null)
            {
                return NotFound(new { Message = "Player character not found." });
            }
            return Ok(character);
        }

        // POST api/PlayerCharacter/profile
        [HttpPost("profile")]
        public async Task<IActionResult> UpdatePlayerProfile([FromBody] PlayerCharacterUpdateDto model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get UserId from JWT Token

            var profile = await _context.PlayerProfiles
                .FirstOrDefaultAsync(p => p.UserId == userId);
            if (profile == null)
            {
                return NotFound(new { Message = "Player profile not found." });
            }
            profile.PlayerName = model.DisplayName ?? profile.PlayerName;
            // Only allow specific updates from client, e.g., cosmetic changes, not stats directly 
            // For stats, you'd have game logic servers handling updates 
            // profile.Level = model.Level; 
            // DON'T allow client to update sensitive stats directly 
            // profile.Experience = model.Experience;
            _context.PlayerProfiles.Update(profile);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Player profile updated successfully." });
        }

        // POST api/PlayerCharacter/
        [HttpPost("")]
        public async Task<IActionResult> CreatePlayerCharacter([FromBody] PlayerCharacterCreateDto model)
        {
            try
            {
                var profile = await _context.PlayerCharacters
                .FirstOrDefaultAsync(p => p.DisplayName == model.DisplayName);
                if (profile == null)
                {
                    Inventory newInventory = new Inventory
                    {
                        Items = new List<InventoryItem>() // Initialize with an empty list
                    };
                    _context.Inventories.Add(newInventory);

                    PlayerCharacter newCharacter = new PlayerCharacter
                    {
                        DisplayName = model.DisplayName,
                        Description = model.Description,
                        CharacterClass = model.CharacterClass,
                        profilePictureId = model.ProfilePictureId ?? string.Empty, // Set profile picture ID
                        IsActive = true,
                        Health = 100,
                        Experience = 0,
                        Level = 1,
                        Mana = 100,
                        Inventory = newInventory, // Associate the new invento
                                                  //ObjectType = GameObjectType.PlayerCharacter // You can set other default properties here if needed
                    };
                    _context.PlayerCharacters.Add(newCharacter);
                    await _context.SaveChangesAsync();
                    return Ok(new { Message = "Player Character updated successfully.", id = newCharacter.Id });
                }
                else
                {
                    return Conflict(new { Message = "Player character with this display name already exists." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while creating the player character.", Error = ex.Message }); 
            }
        }

        // TODO: GET api/PlayerCharacter/inventory
        //[HttpGet("inventory")]
        //public async Task<IActionResult> GetPlayerInventory()
        //{
        //    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        //    var profile = await _context.PlayerProfiles
        //        .Include(p => p.Inventory)
        //        .FirstOrDefaultAsync(p => p.UserId == userId);
        //    if (profile == null)
        //    {
        //        return NotFound(new { Message = "Player profile not found." });
        //    }
        //    return Ok(profile.Inventory);
        //}

        // TODO: POST api/PlayerCharacter/inventory/add
        // This is a simplified example. Real game would have robust validation.
        //[HttpPost("inventory/add")]
        //public async Task<IActionResult> AddItemToInventory([FromBody] AddItemRequestDto model)
        //{
        //    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        //    var profile = await _context.PlayerProfiles
        //        .Include(p => p.Inventory)
        //        .FirstOrDefaultAsync(p => p.UserId == userId);
        //    if (profile == null)
        //    {
        //        return NotFound(new { Message = "Player profile not found." });
        //    }
        //    var existingItem = profile.Inventory
        //        .FirstOrDefault(i => i.ItemId == model.ItemId);
        //    if (existingItem != null && model.IsStackable) // Example: items are stackable
        //    {
        //        existingItem.Quantity += model.Quantity;
        //        _context.InventoryItems.Update(existingItem);
        //    }
        //    else
        //    {
        //        var newItem = new InventoryItem
        //        {
        //            PlayerProfileId = profile.Id,
        //            ItemId = model.ItemId,
        //            Quantity = model.Quantity,
        //            ItemType = model.ItemType
        //            // Or derive from ItemId lookup
        //        };
        //        profile.Inventory.Add(newItem);
        //        // Add to navigation property
        //    }
        //    await _context.SaveChangesAsync();
        //    return Ok(new { Message = $"{model.Quantity}x {model.ItemId} added to inventory." });
        //}

        // TODO: DELETE api/PlayerCharacter/inventory/{itemId}
        //[HttpDelete("inventory/remove/{itemId}")]
        //public async Task<IActionResult> RemoveItemFromInventory(string itemId, [FromQuery] int quantity = 1)
        //{
        //    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        //    var profile = await _context.PlayerProfiles
        //        .Include(p => p.Inventory)
        //        .FirstOrDefaultAsync(p => p.UserId == userId);
        //    if (profile == null)
        //    {
        //        return NotFound(new { Message = "Player profile not found." });
        //    }
        //    var itemToRemove = profile.Inventory
        //        .FirstOrDefault(i => i.ItemId == itemId);
        //    if (itemToRemove == null)
        //    {
        //        return NotFound(new { Message = "Item not found in inventory." });
        //    }
        //    if (itemToRemove.Quantity > quantity)
        //    {
        //        itemToRemove.Quantity -= quantity;
        //        _context.InventoryItems.Update(itemToRemove);
        //    }
        //    else
        //    {
        //        _context.InventoryItems.Remove(itemToRemove);
        //    }
        //    await _context.SaveChangesAsync();
        //    return Ok(new { Message = $"{quantity}x {itemId} removed from inventory." });
        //}

        // --- DTOs (Data Transfer Objects) ---
        public class PlayerCharacterUpdateDto
        {
            public string DisplayName { get; set; }
            // Add other updateable properties
        }

        public class PlayerCharacterCreateDto
        {
            //public string profilePictureId { get; set; } // ID of the profile picture associated with the player character  
            //public string DisplayName { get; set; }
            //public string Description { get; set; } // Brief description of the character
            //public string CharacterClass { get; set; } // e.g., "Warrior", "Mage", "Rogue"
            //                                           // This is the Name inherited from GameObject. It's often the unique identifier for the object itself.
            [Required(ErrorMessage = "Character name (GameObject.Name) is required.")]
            [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
            public string Name { get; set; }

            public string? ProfilePictureId { get; set; } // Optional in DTO

            // This is the DisplayName specific to PlayerCharacter. It's what's shown in-game.
            [Required(ErrorMessage = "Display name is required.")]
            [StringLength(100, ErrorMessage = "Display name cannot exceed 100 characters.")]
            public string DisplayName { get; set; }

            [Required(ErrorMessage = "Description is required.")]
            [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
            public string Description { get; set; }

            [Required(ErrorMessage = "Character class is required.")]
            [StringLength(50, ErrorMessage = "Character class cannot exceed 50 characters.")]
            public string CharacterClass { get; set; }
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
