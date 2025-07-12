using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project_Aether_Backend.Data;
using Project_Aether_Backend.Models;
using ProjectAether.Objects.Net._2._1.Standard.Models;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace Project_Aether_Backend.Controllers
{
    [Route("api/[controller]")]
    public class PlayerCharacterController : AuthorizedControllerBase
    {
        public PlayerCharacterController(ApplicationDbContext context, UserManager<ApplicationUser> userManager) : base(context, userManager) { }

        // GET api/PlayerCharacter/
        [HttpGet("")]
        public async Task<IActionResult> GetPlayerCharacters()
        {
            var all = await _context.PlayerCharacters
                .Include(p => p.Inventory) // Include inventory items
                .Where(pc => pc.Player.User.Id == this.UserId).ToListAsync<PlayerCharacter>();

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

        // POST api/PlayerCharacter/{id}
        [HttpPost("{id}")]
        public async Task<IActionResult> UpdatePlayerCharacter(int id, [FromBody] PlayerCharacterUpdateDto model)
        {
            var profile = await _context.PlayerCharacters
                .FirstOrDefaultAsync(pc => pc.Player.UserId == this.UserId && pc.Id == id);
            if (profile == null)
            {
                return NotFound(new { Message = "Player character not found." });
            }
            // Only allow specific updates from client, e.g., cosmetic changes, not stats directly
            profile.profilePictureId = model.profilePictureId ?? profile.profilePictureId;
            profile.DisplayName = model.DisplayName ?? profile.DisplayName;
            profile.Name = model.Name ?? profile.Name;
            profile.Description = model.Description ?? profile.Description;

            _context.PlayerCharacters.Update(profile);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Player character updated successfully." });
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
                        // TODO: get starting inventory by class here.
                    };
                    _context.Inventories.Add(newInventory);

                    PlayerCharacter newCharacter = new PlayerCharacter
                    {
                        DisplayName = model.DisplayName,
                        Description = model.Description,
                        archetypeDefinitionId = model.ArchetypeDefinitionId,
                        profilePictureId = model.ProfilePictureId ?? string.Empty, // Set profile picture ID
                        IsActive = true,
                        Health = GameCharacter.STARTING_HEALTH,
                        Experience = GameCharacter.STARTING_EXPERIENCE,
                        Level = GameCharacter.STARTING_LEVEL,
                        Mana = GameCharacter.STARTING_MANA,
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

        /// <summary>
        /// DELETE api/PlayerCharacter/{id}
        /// </summary>
        /// <param name="id">The id of the player character.</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveItemFromInventory(int id)
        {
            var playerCharacter = await _context.PlayerCharacters
                .Include(p => p.Inventory)
                .FirstOrDefaultAsync(pc => pc.Player.UserId == this.UserId && pc.Id == id);

            if (playerCharacter == null)
            {
                return NotFound(new { Message = "Player character not found." });
            }

            _context.PlayerCharacters.Remove(playerCharacter);
            await _context.SaveChangesAsync();
            return Ok(new { Message = "Player character deleted successfully." });
        }


        // GET api/PlayerCharacter/archetypes/
        [HttpGet("Archetypes")]
        public async Task<IActionResult> GetCharacterArchetypeDefinitions()
        {
            var archetypes = await _context.Archetypes
                .Where(
                ad => ad.StoreItemId == 0 ||
                _context.StoreItems
                .Where(si => si.Id == ad.StoreItemId)
                .Join(
                    _context.StoreTransactionItems,
                    si => si.Id,
                    sti => sti.StoreItemId,
                    (si, sti) => new { StoreItem = si, StoreTransaction = sti })
                .Join(
                    _context.StoreTransactions,
                    combined => combined.StoreTransaction.StoreTransactionId,
                    st => st.Id,
                    (combined, st) => new { combined.StoreItem, StoreTransaction = st })
                .Any(joined =>
                            joined.StoreTransaction.UserId == this.UserId &&
                            joined.StoreItem.Id == ad.StoreItemId // Redundant but explicit for clarity
                        )
                ).ToListAsync();

            if (archetypes == null || archetypes.Count() == 0)
            {
                return Ok(new List<PlayerCharacter>()); // Return empty list if no characters found
            }
            return Ok(archetypes);
        }



        // --- DTOs (Data Transfer Objects) ---
        public class PlayerCharacterUpdateDto
        {
            /// <summary>
            /// ID of the profile picture associated with the player character
            /// </summary>
            public string profilePictureId { get; set; } = string.Empty;

            /// <summary>
            /// Display name for the player character, shown in-game
            /// </summary>
            public string DisplayName { get; set; } = string.Empty;
            // Add other updateable properties

            /// <summary>
            /// Character's name
            /// </summary>
            public string Name { get; set; } = string.Empty;
            /// <summary>
            /// Brief description of the character
            /// </summary>
            public string Description { get; set; } = string.Empty;

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
            public string Name { get; set; } = string.Empty;

            public string? ProfilePictureId { get; set; } // Optional in DTO

            // This is the DisplayName specific to PlayerCharacter. It's what's shown in-game.
            [Required(ErrorMessage = "Display name is required.")]
            [StringLength(100, ErrorMessage = "Display name cannot exceed 100 characters.")]
            public string DisplayName { get; set; } = string.Empty;

            [Required(ErrorMessage = "Description is required.")]
            [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
            public string Description { get; set; } = string.Empty;

            [Required(ErrorMessage = "Character class is required.")]
            public int ArchetypeDefinitionId { get; set; } = 0;
        }

        public class AddItemRequestDto
        {
            public int playerCharacterId { get; set; }
            public int ItemId { get; set; }
            public int Quantity { get; set; }
            public string ItemType { get; set; } = string.Empty;
            // Useful for initial item categorization
            public bool IsStackable { get; set; } = true;
            // Example property
        }
    }
}
