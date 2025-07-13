using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Project_Aether_Backend.Data;
using Project_Aether_Backend.Filters;
using Project_Aether_Backend.Models;

namespace Project_Aether_Backend.Controllers
{
    [ServiceFilter(typeof(ApiKeyAuthFilter))] // You can use ServiceFilter directly too
    public class ApiKeyAuthorizedControllerBase : AuthorizedControllerBase
    {
        public ApiKeyAuthorizedControllerBase(ApplicationDbContext context, UserManager<ApplicationUser> userManager) : base(context, userManager)
        {
        }
    }
}
