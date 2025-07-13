using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Project_Aether_Backend.Services;

namespace Project_Aether_Backend.Filters
{
    public class ApiKeyAuthFilter : IAsyncActionFilter
    {
        private const string ApiKeyHeaderName = "X-API-Key"; // The header your Unity server will send

        private readonly IApiKeyValidator _apiKeyValidator;

        public ApiKeyAuthFilter(IApiKeyValidator apiKeyValidator)
        {
            _apiKeyValidator = apiKeyValidator;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // 1. Check if the header exists
            if (!context.HttpContext.Request.Headers.TryGetValue(ApiKeyHeaderName, out var extractedApiKey))
            {
                context.Result = new UnauthorizedResult(); // 401 Unauthorized
                return;
            }

            // 2. Validate the extracted API key
            if (!_apiKeyValidator.IsValidApiKey(extractedApiKey!)) // '!' to assert non-null
            {
                context.Result = new UnauthorizedResult(); // 401 Unauthorized
                return;
            }

            // If authentication is successful, continue to the action
            await next();
        }
    }
}
