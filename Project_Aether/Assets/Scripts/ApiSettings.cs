using System.ComponentModel;

namespace Assets.Scripts
{
    public class ApiSettings
    {

        private const bool UseSSL = false;

        public enum ApiDomains
        {
            [Description("")]
            None,
            Auth,
            Player,
            PlayerCharacter,
            GameObject,
            Inventory,
            WorldZone
        }

        public static string GetApiUrl()
        {
            return GetApiUrl(ApiDomains.None, "");
        }

        public static string GetApiUrl(ApiDomains apiDomains)
        {
            return GetApiUrl(apiDomains, "");
        }

        public static string GetApiUrl(ApiDomains apiDomains, string endpoint)
        {
            string prefix = (UseSSL) ? "https" : "http";
            string domainEndpoint = ((apiDomains != ApiDomains.None) ? apiDomains.ToString() + "/" : string.Empty) + endpoint;
            return $"{prefix}://{GameConstants.ConnectionIP}:{GameConstants.ConnectionPort}/api/{domainEndpoint}";
        }
    }
}
