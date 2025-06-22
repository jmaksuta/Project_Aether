using System;
using Microsoft.AspNetCore.SignalR.Client;
using System.IO.Pipelines;
using System.Text.Json; // (crucial for JSON serialization/ deserialization)
using System.Threading.Channels;
using Microsoft.Extensions.Logging.Abstractions; // (useful for SignalR's internal logging if you enable it)
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace SignalRUnityDLLHelper
{
    public class UnitySignalRHelper
    {
        private HubConnection _connection;
        public string backendUrl = "https://localhost:7147";
        // Replace with your backend URL
        public string authToken = ""; // This would come from your AuthController login response

        async void Start()
        {
            _connection = new HubConnectionBuilder()
                .WithUrl($"{backendUrl}/chathub", options =>
                {
                    options.AccessTokenProvider = () => Task.FromResult(authToken); // Provide JWT token                                                                    
                })
                .WithAutomaticReconnect()
                .Build();
            _connection.On<string, string>("ReceiveMessage", (user, message) =>
            {
                Console.WriteLine($"Chat Message: [{user}] {message}");
                //Debug.Log($"Chat Message: [{user}] {message}"); // Update UI with the message
            });
            _connection.On<string, string, string>("ReceivePrivateMessage", (sender, recipient, message) =>
            {
                Console.WriteLine($"Private Message from {sender} to {recipient}: {message}");
                //Debug.Log($"Private Message from {sender} to {recipient}: {message}");

                // Update UI with the message
            });
            try
            {
                await _connection.StartAsync();
                Console.WriteLine("SignalR Connected!");
                //Debug.Log("SignalR Connected!");
            }
            catch (System.Exception ex)
            {
                Console.WriteLine($"SignalR Connection Error: {ex.Message}");
                //Debug.LogError($"SignalR Connection Error: {ex.Message}");
            }
        }

        public async void SendChatMessage(string message)
        {
            if (_connection.State == HubConnectionState.Connected)
            {
                await _connection.InvokeAsync("SendGlobalChat", message);
            }
            else
            {
                Console.WriteLine("Not connected to chat hub.");
                //Debug.LogWarning("Not connected to chat hub.");
            }
        }

        void OnApplicationQuit()
        {
            if (_connection != null)
            {
                _connection.StopAsync();
            }
        }
    }
}
