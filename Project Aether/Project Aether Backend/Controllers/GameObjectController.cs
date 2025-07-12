using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project_Aether_Backend.Data;
using Project_Aether_Backend.Models;
using ProjectAether.Objects.Net._2._1.Standard.Models;

namespace Project_Aether_Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Requires authentication for all endpoints in this controller.
    public class GameObjectController : AuthorizedControllerBase
    {

        public GameObjectController(ApplicationDbContext context, UserManager<ApplicationUser> userManager) : base(context, userManager)
        {

        }

        // GET api/GameObject/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetGameObjects(int id)
        {
            if (UserId == null)
            {
                return Unauthorized(new { Message = "User is not authenticated." });
            }
            var gameObject = await this._context.GameObjects
                .FirstOrDefaultAsync(go => go.Id == id);

            if (gameObject == null)
            {
                return NotFound(new { Message = "GameObject not found." });
            }
            return Ok(gameObject);
        }

        [HttpPost("{id}")]
        public async Task<IActionResult> SaveGameObjects(int id, [FromBody] GameObjectDTO gameObjectDTO)
        {
            if (UserId == null)
            {
                return Unauthorized(new { Message = "User is not authenticated." });
            }
            var gameObject = await this._context.GameObjects
                .FirstOrDefaultAsync(go => go.Id == id);

            if (gameObject == null)
            {
                GameObject newGameObject = new GameObject
                {
                    Id = gameObjectDTO.Id,
                    Name = gameObjectDTO.Name,
                    Description = gameObjectDTO.Description,
                    ObjectType = (GameObjectType)Enum.Parse(typeof(GameObjectType), gameObjectDTO.ObjectType),
                    IsActive = gameObjectDTO.IsActive,
                    IsDeleted = gameObjectDTO.IsDeleted,
                    WorldZoneId = gameObjectDTO.WorldZoneId,
                    xPosition = gameObjectDTO.x,
                    yPosition = gameObjectDTO.y,
                    zPosition = gameObjectDTO.z
                };

                SetWorldZoneIdIfMissing(ref newGameObject, gameObjectDTO, gameObjectDTO.sceneName);

                _context.GameObjects.Add(newGameObject);
            }
            else
            {
                gameObject.Id = gameObjectDTO.Id;
                gameObject.Name = gameObjectDTO.Name;
                gameObject.Description = gameObjectDTO.Description;
                gameObject.ObjectType = (GameObjectType)Enum.Parse(typeof(GameObjectType), gameObjectDTO.ObjectType);
                gameObject.IsActive = gameObjectDTO.IsActive;
                gameObject.IsDeleted = gameObjectDTO.IsDeleted;
                gameObject.WorldZoneId = gameObjectDTO.WorldZoneId;
                gameObject.xPosition = gameObjectDTO.x;
                gameObject.yPosition = gameObjectDTO.y;
                gameObject.zPosition = gameObjectDTO.z;

                SetWorldZoneIdIfMissing(ref gameObject, gameObjectDTO, gameObjectDTO.sceneName);

                _context.GameObjects.Update(gameObject);
            }
            await _context.SaveChangesAsync();
            return Ok(gameObject);
        }

        /// <summary>
        /// Find Zone by Scene Name if WorldZoneId is not provided
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="gameObjectDTO"></param>
        /// <param name="sceneName"></param>
        private void SetWorldZoneIdIfMissing(ref GameObject gameObject, GameObjectDTO gameObjectDTO, string sceneName)
        {
            if (gameObjectDTO.WorldZoneId == 0 && !String.IsNullOrEmpty(gameObjectDTO.sceneName))
            {
                WorldZone worldZone = FindZoneBySceneName(gameObjectDTO.sceneName);
                gameObject.WorldZoneId = worldZone != null ? worldZone.Id : 0;
            }
        }

        private WorldZone FindZoneBySceneName(string sceneName)
        {
            var worldZone = _context.WorldZones
                .FirstOrDefault(wz => wz.SceneName == sceneName);
            return worldZone ?? new WorldZone();
        }

        // GET api/GameObject/byZone/{zoneId}
        [HttpGet("byZone/{zoneId}")]
        public async Task<ActionResult<IEnumerable<GameObject>>> GetObjectsInZone(int zoneId)
        {
            if (UserId == null)
            {
                return Unauthorized(new { Message = "User is not authenticated." });
            }
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


        public class GameObjectDTO
        {
            public int Id { get; set; }
            public string Name { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
            public string ObjectType { get; set; } = string.Empty;
            public bool IsActive { get; set; }
            public bool IsDeleted { get; set; }
            public int WorldZoneId { get; set; }
            public string sceneName { get; set; } = string.Empty;
            public float x { get; set; }
            public float y { get; set; }
            public float z { get; set; }
        }

    }
}
