using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project_Aether_Backend.Data;
using ProjectAether.Objects.Models;
using static Project_Aether_Backend.Controllers.PlayerCharacterController;

namespace Project_Aether_Backend.Controllers
{
    [Route("api/[controller]")]
    public class InventoryController : AuthorizedControllerBase
    {
        public InventoryController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
            : base(context, userManager) { }



        // GET api/Inventory/{playerCharacterId}
        [HttpGet("{playerCharacterId}")]
        public async Task<IActionResult> GetPlayerInventory(int playerCharacterId)
        {
            Inventory? inventory = await GetInventoryByPlayerCharacterId(playerCharacterId);

            if (inventory == null)
            {
                return NotFound(new { Message = "Inventory not found." });
            }
            return Ok(inventory);
        }

        /// <summary>
        /// POST api/Inventory/{id}/add
        /// 
        /// This is a simplified example.Real game would have robust validation.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("{playerCharacterId}/add")]
        public async Task<IActionResult> AddItemToInventory(int playerCharacterId, [FromBody] AddItemRequestDto model)
        {
            Inventory? inventory = await GetInventoryByPlayerCharacterId(playerCharacterId);

            if (inventory == null)
            {
                return NotFound(new { Message = "Inventory not found." });
            }

            var existingItem = inventory.Items
                .FirstOrDefault(ii => ii.Id == model.ItemId);
            if (existingItem != null && model.IsStackable) // Example: items are stackable
            {
                existingItem.Quantity += model.Quantity;
                _context.InventoryItems.Update(existingItem);
            }
            else if (existingItem != null && !model.IsStackable)
            {
                // TODO: add some validation to ensure that items cannot be illegally transferred
                // between inventories without the consent of the original inventory.
                // Maybe make it illegal to transfer from on PC inventory to another,
                // and it must be put into an escrow inventory first, then the new PC can take it.
                existingItem.InventoryId = inventory.Id; // Ensure it belongs to the correct inventory
                _context.InventoryItems.Update(existingItem);
            }
            else
            {
                var newItem = new InventoryItem
                {
                    InventoryId = inventory.Id,
                    Id = model.ItemId,
                    Quantity = model.Quantity,
                    ItemType = model.ItemType
                    // Or derive from ItemId lookup
                };
                inventory.Items.Add(newItem);
                // Add to navigation property
            }
            await _context.SaveChangesAsync();
            return Ok(new { Message = $"{model.Quantity}x {model.ItemId} added to inventory." });
        }

        /// <summary>
        /// DELETE api/PlayerCharacter/inventory/remove/pc={playerCharacterId}&amp;item={itemId}
        /// </summary>
        /// <param name="playerCharacterId"></param>
        /// <param name="itemId"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        [HttpDelete("{playerCharacterId}/remove")]
        public async Task<IActionResult> RemoveItemFromInventory(int playerCharacterId, [FromQuery] int itemId, [FromQuery] int quantity = 1)
        {
            if (itemId <= 0)
            {
                return BadRequest(new { Message = "Invalid item ID." });
            }
            if (quantity < 0)
            {
                return BadRequest(new { Message = "Quantity must be greater than or equal to zero." });
            }

            Inventory? inventory = await GetInventoryByPlayerCharacterId(playerCharacterId);

            if (inventory == null)
            {
                return NotFound(new { Message = "Inventory not found." });
            }

            var itemToRemove = inventory.Items
                .FirstOrDefault(ii => ii.Id == itemId);
            if (itemToRemove == null)
            {
                return NotFound(new { Message = "Item not found in inventory." });
            }
            string message;
            if (itemToRemove.IsStackable && itemToRemove.Quantity > quantity)
            {
                itemToRemove.Quantity -= quantity;
                _context.InventoryItems.Update(itemToRemove);
                message = $"{quantity}x of Item ID: {itemId}, removed from inventory.";
            }
            else
            {
                _context.InventoryItems.Remove(itemToRemove);
                message = $"Item Id: {itemId}, removed from inventory.";
            }
            await _context.SaveChangesAsync();
            return Ok(new { Message = message });
        }

        private async Task<Inventory?> GetInventoryByPlayerCharacterId(int playerCharacterId)
        {
            var inventory = await _context.Inventories
                .Include(pc => pc.Items)
                .Join(_context.PlayerCharacters,
                    i => i.Id,
                    pc => pc.InventoryId,
                    (i, pc) => new { Inventory = i, PlayerCharacter = pc })
                .FirstOrDefaultAsync(i => i.PlayerCharacter.Player.UserId == this.UserId
                && i.PlayerCharacter.Id == playerCharacterId);
            Inventory? result = inventory.Inventory;

            return result;
        }


        // TODO: GET api/Player/inventory
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

        // TODO: POST api/Player/inventory/add
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

        // TODO: DELETE api/Player/inventory/{itemId}
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


    }
}
