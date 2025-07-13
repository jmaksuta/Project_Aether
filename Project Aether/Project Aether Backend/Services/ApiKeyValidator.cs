namespace Project_Aether_Backend.Services
{
    public class ApiKeyValidator : IApiKeyValidator
    {
        private readonly IConfiguration _configuration;
        private const string ApiKeySectionName = "ApiKeys:GameServer"; // Matches the path in appsettings.json

        public ApiKeyValidator(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public bool IsValidApiKey(string userApiKey)
        {
            if (string.IsNullOrWhiteSpace(userApiKey))
            {
                return false;
            }

            // Retrieve the API key from configuration.
            // In production, this would ideally come from environment variables
            // or a secrets manager like Azure Key Vault.
            string? storedApiKey = _configuration[ApiKeySectionName];

            if (string.IsNullOrWhiteSpace(storedApiKey))
            {
                // This indicates a configuration error, you might want to log this
                // or throw an exception in a real application's startup.
                return false;
            }

            // Use a constant-time comparison to prevent timing attacks.
            // You can implement your own or use utilities if available.
            // For simplicity, we'll use string.Equals, but be aware of timing attacks for very sensitive keys.
            return userApiKey.Equals(storedApiKey);
        }
    }
}
