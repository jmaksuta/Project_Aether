using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Project_Aether_Backend.Data;
using Project_Aether_Backend.Filters;
using Project_Aether_Backend.Models;

namespace Project_Aether_Backend.Controllers
{
    [ApiController]
    [ServiceFilter(typeof(ApiKeyAuthFilter))] // You can use ServiceFilter directly too
    public class ApiKeyAuthorizedControllerBase : ControllerBase
    {
        protected readonly ApplicationDbContext _context;
        protected readonly UserManager<ApplicationUser> _userManager;

        public ApiKeyAuthorizedControllerBase(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

    }
}
