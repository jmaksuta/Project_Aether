using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Project_Aether_Backend.Data;
using Project_Aether_Backend.Models;
using System.Security.Claims;

namespace Project_Aether_Backend.Controllers
{
    [ApiController]
    [Authorize]
    /// <summary>
    /// Requires authentication for all endpoints in this controller.
    /// </summary>
    public abstract class AuthorizedControllerBase : ControllerBase
    {
        protected readonly ApplicationDbContext _context;
        protected readonly UserManager<ApplicationUser> _userManager;

        protected string UserId
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

        protected string UserName
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

        public AuthorizedControllerBase(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
    }
}
