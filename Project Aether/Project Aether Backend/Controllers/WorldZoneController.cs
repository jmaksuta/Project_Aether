using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project_Aether_Backend.Data;
using Project_Aether_Backend.Filters;
using Project_Aether_Backend.Models;
using ProjectAether.Objects.Net._2._1.Standard.Models;
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
            //var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get UserId from JWT Token
            var worldZone = await _context.WorldZones
                .FirstOrDefaultAsync(wz => wz.SceneName == sceneName);
            if (worldZone == null)
            {
                return NotFound(new { Message = "World Zone not found." });
            }
            return Ok(worldZone);
        }

        [HttpGet("Objects/ByZone/{zoneId}")]
        [ServiceFilter(typeof(ApiKeyAuthFilter))] // You can use ServiceFilter directly too
        public async Task<ActionResult<IEnumerable<GameObject>>> GetObjectsInZone(int zoneId)
        {
            if (zoneId <= 0)
            {
                return BadRequest(new { Message = "Invalid zoneId parameter." });
            }

            var objectsInZone = await this._context.GameObjects
                .Where(go => go.WorldZoneId == zoneId).ToListAsync();

            if (!objectsInZone.Any())
            {
                return NotFound(new { Message = $"No objects found for Zone ID: {zoneId}." });
            }
            return Ok(objectsInZone);
        }

    }
}
