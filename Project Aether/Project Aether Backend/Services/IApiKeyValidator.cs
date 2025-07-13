namespace Project_Aether_Backend.Services
{
    public interface IApiKeyValidator
    {
        bool IsValidApiKey(string userApiKey);
    }
}
