using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project_Aether_Backend.Data;
using Project_Aether_Backend.Filters;
using Project_Aether_Backend.Models;
using System.Security.Claims;

namespace Project_Aether_Backend.Controllers
{
    [Route("api/[controller]")]
    public class WorldZoneController : ApiKeyAuthorizedControllerBase
    {
        public WorldZoneController(ApplicationDbContext context, UserManager<ApplicationUser> userManager) : base(context, userManager)
        {
        }

        // GET api/WorldZone/ByName?sceneName={sceneName}
        [HttpGet("ByName")]
        [ServiceFilter(typeof(ApiKeyAuthFilter))] // You can use ServiceFilter directly too
        public async Task<IActionResult> GetZoneByName([FromQuery] string sceneName)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get UserId from JWT Token
            var worldZone = await _context.WorldZones
                .FirstOrDefaultAsync(wz => wz.SceneName == sceneName);
            if (worldZone == null)
            {
                return NotFound(new { Message = "Player character not found." });
            }
            return Ok(worldZone);
        }
    }
}
