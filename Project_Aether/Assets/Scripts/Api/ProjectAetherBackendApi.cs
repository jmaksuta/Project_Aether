using Assets.Scripts;
using Assets.Scripts.Api;
using ProjectAether.Objects.Net._2._1.Standard.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class ProjectAetherBackendApi
{

    public static async Task<LoginResponse> Login(string username, string password)
    {
        LoginRequest loginRequest = new LoginRequest(username, password); // Replace with actual user credentials
        return await Login(loginRequest);
    }

    public static async Task<LoginResponse> Login(LoginRequest loginRequest)
    {
        string authApiUrl = ApiSettings.GetApiUrl(ApiSettings.ApiDomains.Auth);

        string requestBody = JsonUtility.ToJson(loginRequest);

        var request = UnityWebRequest.Post(authApiUrl + "login", requestBody, "application/json");

        request.SetRequestHeader("accept", "*/*");
        request.SetRequestHeader("User-Agent", "UnityGameClient/1.0");
        request.SetRequestHeader("Accept-Encoding", "gzip, deflate, br");
        request.SetRequestHeader("Connection", "keep-alive");
        request.SetRequestHeader("Content-Type", "application/json");
        //request.SetRequestHeader("Authorization", "Bearer " + _playerAuthToken); // If you have a token to pass
        await request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            if (request.responseCode == (long)HttpStatusCode.Unauthorized)
            {
                var errorResponse = JsonUtility.FromJson<UnauthorizedResponse>(request.downloadHandler.text);
                string errorMessage = string.Join("\r\n", errorResponse.Message);
                Debug.LogError($"Authentication failed: {request.error} - {errorMessage}");
                throw new UnauthorizedException(errorMessage);
            }
            else
            {
                string errorMessage = $"Authentication failed: {request.error} - {request.downloadHandler.text}";
                Debug.LogError(errorMessage);
                throw new HttpException((HttpStatusCode)request.responseCode, errorMessage);
            }
        }

        // Assuming your backend returns a JSON with an auth token and player ID
        var authResponse = JsonUtility.FromJson<LoginResponse>(request.downloadHandler.text);

        return authResponse;
    }



    [Serializable]
    public class LoginRequest
    {
        public string Username;
        public string Password;

        public LoginRequest(string username, string password)
        {
            Username = username;
            Password = password;
        }
    }

    [Serializable]
    public class LoginResponse
    {
        public string token;
        //public string PlayerId;
        public string userId;
        public string username;
        // Add other fields your auth response might have (e.g., player name)
    }

    public static async Task<RegisterResponse> Register(string Username, string Email, string Password)
    {
        RegisterRequest registerRequest = new RegisterRequest(Username, Email, Password);
        return await Register(registerRequest);
    }


    public static async Task<RegisterResponse> Register(RegisterRequest registerRequest)
    {
        string authApiUrl = ApiSettings.GetApiUrl(ApiSettings.ApiDomains.Auth);

        string requestBody = JsonUtility.ToJson(registerRequest);

        var request = UnityWebRequest.Post(authApiUrl + "register", requestBody, "application/json");

        request.SetRequestHeader("accept", "*/*");
        request.SetRequestHeader("User-Agent", "UnityGameClient/1.0");
        request.SetRequestHeader("Accept-Encoding", "gzip, deflate, br");
        request.SetRequestHeader("Connection", "keep-alive");
        request.SetRequestHeader("Content-Type", "application/json");
        //request.SetRequestHeader("Authorization", "Bearer " + _playerAuthToken); // If you have a token to pass
        await request.SendWebRequest();


        if (request.result != UnityWebRequest.Result.Success)
        {
            if (request.responseCode == (long)HttpStatusCode.BadRequest)
            {
                var errorResponse = JsonUtility.FromJson<ErrorResponse>(request.downloadHandler.text);
                string errorMessage = string.Join("\r\n", errorResponse.Errors);
                Debug.LogError($"Authentication failed: {request.error} - {request.downloadHandler.text}");
                throw new BadRequestException(errorMessage);
            }
            else
            {
                string errorMessage = $"Authentication failed: {request.error} - {request.downloadHandler.text}";
                Debug.LogError(errorMessage);
                throw new HttpException((HttpStatusCode)request.responseCode, errorMessage);
            }
        }

        // Assuming your backend returns a JSON with an auth token and player ID
        var authResponse = JsonUtility.FromJson<RegisterResponse>(request.downloadHandler.text);

        return authResponse;
    }

    [Serializable]
    public class RegisterRequest
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public RegisterRequest()
        {
            this.Username = string.Empty;
            this.Email = string.Empty;
            this.Password = string.Empty;
        }

        public RegisterRequest(string Username, string Email, string Password) : this()
        {
            this.Username = Username;
            this.Email = Email;
            this.Password = Password;
        }
    }

    [Serializable]
    public class RegisterResponse
    {
        public string message { get; set; }
    }

    /// <summary>
    /// GET api/PlayerCharacter/
    /// </summary>
    /// <returns></returns>
    /// <exception cref="BadRequestException"></exception>
    /// <exception cref="HttpException"></exception>
    public static async Task<GetPlayerCharactersResponse> GetPlayerCharacters()
    {
        string authApiUrl = ApiSettings.GetApiUrl(ApiSettings.ApiDomains.PlayerCharacter);

        //string requestBody = JsonUtility.ToJson(registerRequest);

        var request = UnityWebRequest.Get(authApiUrl + "");

        request.SetRequestHeader("accept", "*/*");
        request.SetRequestHeader("User-Agent", "UnityGameClient/1.0");
        request.SetRequestHeader("Accept-Encoding", "gzip, deflate, br");
        request.SetRequestHeader("Connection", "keep-alive");
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + AuthManager.Instance.AuthToken); // If you have a token to pass
        await request.SendWebRequest();


        if (request.result != UnityWebRequest.Result.Success)
        {
            if (request.responseCode == (long)HttpStatusCode.BadRequest)
            {
                var errorResponse = JsonUtility.FromJson<ErrorResponse>(request.downloadHandler.text);
                string errorMessage = string.Join("\r\n", errorResponse.Errors);
                Debug.LogError($"Authentication failed: {request.error} - {request.downloadHandler.text}");
                throw new BadRequestException(errorMessage);
            }
            else
            {
                string errorMessage = $"Authentication failed: {request.error} - {request.downloadHandler.text}";
                Debug.LogError(errorMessage);
                throw new HttpException((HttpStatusCode)request.responseCode, errorMessage);
            }
        }

        // Assuming your backend returns a JSON with an auth token and player ID
        var authResponse = JsonUtility.FromJson<GetPlayerCharactersResponse>(request.downloadHandler.text);

        return authResponse;
    }

    [Serializable]
    public class GetPlayerCharactersResponse
    {
        public List<PlayerCharacter> characters { get; set; }
    }

    internal static Task<List<ArchetypeDefinition>> GetAvailableClasses()
    {
        throw new NotImplementedException();
    }

    public static async Task<WorldZone> GetWorldZoneBySceneName(string apiKey, string sceneName)
    {
        string worldZoneUrl = ApiSettings.GetApiUrl(ApiSettings.ApiDomains.WorldZone);

        // i.e.: /api/WorldZone/ByName?sceneName=05_ForestZone
        var request = UnityWebRequest.Get($"{worldZoneUrl}ByName?sceneName={sceneName}");
        // You might need to add an API key or internal server token for authorization
        request.SetRequestHeader("X-API-KEY", apiKey);
        await request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"Failed to load persistent data for {sceneName}: {request.error} - {request.downloadHandler.text}");
            return null;
        }

        // Assuming your backend returns a JSON list of InteractableObjectData
        // You'll need to define InteractableObjectData and a wrapper class if your API returns an array directly
        // TODO: add this for real game.
        // Example: var loadedObjects = JsonUtility.FromJson<InteractableObjectDataListWrapper>(jsonResponse).objects;
        string jsonResponse = request.downloadHandler.text;
        Debug.Log($"Loaded data for {sceneName}: {jsonResponse}"); // For debugging

        WorldZoneDTO worldZoneDto = JsonUtility.FromJson<WorldZoneDTO>(jsonResponse);
        WorldZone worldZone = worldZoneDto.ToWorldZone();

        return worldZone;
    }

    public class WorldZoneDTO
    {
        public int id;
        public string name;
        public string description;
        public int zoneId;
        public string sceneName;
        public string serverIPAddress;
        public int serverPort;

        public WorldZone ToWorldZone()
        {
            return new WorldZone
            {
                Id = this.id,
                Name = this.name,
                Description = this.description,
                ZoneId = this.zoneId,
                SceneName = this.sceneName,
                ServerIPAddress = this.serverIPAddress,
                ServerPort = this.serverPort
            };
        }
    }

    public static async Task<List<ProjectAether.Objects.Net._2._1.Standard.Models.GameObject>> GetGameObjectsInWorldZone(string apiKey, WorldZone worldZone)
    {
        if (worldZone == null)
        {
            Debug.LogError("WorldZone is null. Cannot load game objects.");
            return null;
        }
        // GET api/WorldZone/Objects/ByZone/{Id}
        string zoneObjectsUrl = ApiSettings.GetApiUrl(ApiSettings.ApiDomains.WorldZone) + $"Objects/ByZone/{worldZone.Id}";

        var request = UnityWebRequest.Get(zoneObjectsUrl);
        // You might need to add an API key or internal server token for authorization
        request.SetRequestHeader("X-API-KEY", apiKey);
        await request.SendWebRequest();
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"Failed to load objects for zone {worldZone.Id}: {request.error} - {request.downloadHandler.text}");
            return null;
        }
        string jsonResponse = request.downloadHandler.text;
        Debug.Log($"Loaded game objects for worldZone Id: {worldZone.Id}: {jsonResponse}");

        return DeserializeGameObjectList(jsonResponse);
    }

    private static List<ProjectAether.Objects.Net._2._1.Standard.Models.GameObject> DeserializeGameObjectList(string jsonResponse)
    {
        List<ProjectAether.Objects.Net._2._1.Standard.Models.GameObject> result = new List<ProjectAether.Objects.Net._2._1.Standard.Models.GameObject>();   
        try
        {
            result = JsonSerializer.Deserialize<List<ProjectAether.Objects.Net._2._1.Standard.Models.GameObject>>(jsonResponse);
            
        }
        catch (System.Exception e)
        {
            Debug.LogError($"JSON Deserialization Error: {e.Message}");
        }
        return result;
    }

}
