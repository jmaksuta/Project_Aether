using Assets.Scripts;
using Assets.Scripts.Api;
using ProjectAether.Objects.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using static ProjectAetherBackendApi;

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

}
