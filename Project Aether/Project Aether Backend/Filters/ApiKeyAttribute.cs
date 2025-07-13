using Microsoft.AspNetCore.Mvc;

namespace Project_Aether_Backend.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ApiKeyAttribute : ServiceFilterAttribute
    {
        public ApiKeyAttribute() : base(typeof(ApiKeyAuthFilter))
        {
        }
    }
}
