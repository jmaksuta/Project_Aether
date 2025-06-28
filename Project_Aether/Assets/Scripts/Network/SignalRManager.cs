using UnityEngine;
using Microsoft.AspNetCore.SignalR.Client;
using System.Threading.Tasks;
using System;
using UnityEngine.UI;
using Microsoft.Extensions.Logging.Abstractions;
using System.Net.Http;
using TMPro; // For UI elements like InputField, Button, Text using System.Collections.Generic; // For generic collections if needed

public class SignalRManager : MonoBehaviour
{

    [Header("SignalR Connection")]
    [Tooltip("The URL of your SignalR Hub (e.g., http://localhost:5000/chathub)")]
    public string hubUrl = "http://localhost:5000/chathub";
    // IMPORTANT: Use http for local dev unless you set up HTTPS on server
    private HubConnection _connection;

    [Header("UI References")]
    public TMP_InputField userInput;
    public TMP_InputField messageInput;
    public Button sendButton;
    public TextMeshProUGUI chatDisplay;
    public ScrollRect chatScrollRect;
    // To auto-scroll chat
    [Header("Connection Status"), Tooltip("To Display connection Status.")]
    public TextMeshProUGUI connectionStatusText;
    // --- Unity Lifecycle Methods ---
    void Awake()
    {
        // Initialize HubConnection in Awake for early setup
        InitializeSignalR();
    }
    void Start()
    {
        // Set up UI listeners
        if (sendButton != null)
        {
            sendButton.onClick.AddListener(SendMessage);
        }
        // Start the connection attempt
        _ = TryConnect();
        // Use '_' to discard the task, as Start() cannot be async directly
    }
    async void OnApplicationQuit()
    {
        // Ensure the connection is gracefully closed when the application quits
        if (_connection != null && _connection.State != HubConnectionState.Disconnected)
        {
            Debug.Log("Stopping SignalR connection...");
            try
            {
                await _connection.StopAsync();
                Debug.Log("SignalR connection stopped.");
            }
            catch (Exception ex)
            {
                Debug.LogError($"<span class=\"math-inline\">\"Error stopping SignalR connection: {ex.Message}");
            }
        }
    }
    // --- SignalR Logic ---
    private void InitializeSignalR()
    {
        _connection = new HubConnectionBuilder()
             .WithUrl(hubUrl, options =>
             {
                 // Optional: Configure transport types (e.g., only WebSockets) 
                 options.Transports = Microsoft.AspNetCore.Http.Connections.HttpTransportType.WebSockets;
                 // For self-signed certificates in dev with HTTPS: 
                 options.HttpMessageHandlerFactory = _ => new HttpClientHandler
                 {
                     ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                 };
             })
                     // Optional: Add MessagePack protocol if you installed Microsoft.AspNetCore.SignalR.Protocols.MessagePack
                     // .AddMessagePackProtocol\(\)
                     // Or .AddJsonProtocol() //if you only use JSON (default)
                     // Optional: Configure logging for SignalR internal messages
                     //.ConfigureLogging(logging =>
                     //{
                     //    logging.SetMinimumLevel(LogLevel.Debug);
                     //    // Requires Microsoft.Extensions.Logging.Abstractions.dll
                     //    logging.AddProvider(new UnityDebugLoggerProvider());
                     //    // Custom logger for Unity Debug.Log 
                     //})
                     .Build();
        // --- Registering Event Handlers ---
        // Handle connection state changes
        _connection.Closed += async (error) =>
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                Debug.LogError($"SignalR Connection closed: {error?.Message}");
                UpdateConnectionStatus($"</span>\"Disconnected: {error?.Message}\"</span>", Color.red);
                AppendMessage("System", "Connection closed. Reconnecting...");
            });
            // Implement a reconnection strategy (e.g., exponential backoff)
            await Task.Delay(UnityEngine.Random.Range(1000, 5000));
            // Wait 1-5 seconds before retrying
            await TryConnect();
        };
        _connection.Reconnecting += (error) =>
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                Debug.LogWarning("SignalR Reconnecting... " + error?.Message);
                UpdateConnectionStatus($"<span class=\"math-inline\">\"Reconnecting...", Color.yellow);
            }
            );
            return Task.CompletedTask;
        };
        _connection.Reconnected += (connectionId) =>
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                Debug.Log("SignalR Reconnected. New connection ID: " + connectionId);
                UpdateConnectionStatus($"</span>\"Connected (ID: {connectionId.Substring(0, 4)}...)", Color.green);
                AppendMessage("System", "Reconnected to Hub!");
            });
            return Task.CompletedTask;
        };
        // Register client-side methods that the Hub can call
        _connection.On<string, string>("ReceiveMessage", (user, message) =>
        {
            // IMPORTANT: UI updates MUST be marshaled to the main Unity thread
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                AppendMessage(user, message);
            });
        });
        // Example of registering a different method with specific data types
        _connection.On<int, float, string>("GameUpdate", (id, score, status) =>
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                Debug.Log($"<span class=\"math-inline\">\"Game Update: ID={id}, Score={score}, Status={status}");
            });
        });
    }

    private async Task TryConnect()
    {
        if (_connection.State != HubConnectionState.Disconnected)
        {
            Debug.LogWarning("Connection is not in 'Disconnected' state. Skipping connection attempt.");
            return;
        }
        UpdateConnectionStatus("Connecting...", Color.grey);
        int retryCount = 0;
        const int maxRetries = 5;
        // Or an infinite loop with delays
        while (_connection.State == HubConnectionState.Disconnected && retryCount < maxRetries)
        {
            try
            {
                Debug.Log($"</span>\"Attempting to connect to SignalR hub ({retryCount + 1}/{maxRetries})...");
                await _connection.StartAsync();
                UnityMainThreadDispatcher.Instance().Enqueue(() =>
                {
                    Debug.Log("SignalR Connection Started. ID: " + _connection.ConnectionId);
                    UpdateConnectionStatus($"<span class=\"math-inline\">\"Connected (ID: {_connection.ConnectionId.Substring(0, 4)}...)", Color.green);
                    AppendMessage("System", "Connected to Chat Hub!");
                }
                );
                return;
                // Connection successful
            }
            catch (Exception ex)
            {
                retryCount++;
                UnityMainThreadDispatcher.Instance().Enqueue(() =>
                {
                    Debug.LogError($"</ span > \"Could not connect to SignalR hub: {ex.Message}. Retrying in {Mathf.Pow(2, retryCount)}s...");
                    UpdateConnectionStatus($"<span class=\"math-inline\">\"Failed to connect (Retry {retryCount})", Color.red);

                }
                );
                // Exponential backoff
                await Task.Delay((int)(Mathf.Pow(2, retryCount) * 1000));
            }
        }
        if (_connection.State != HubConnectionState.Connected)
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                Debug.LogError("Failed to connect to SignalR hub after multiple retries.");
                UpdateConnectionStatus("Connection Failed", Color.red);
                AppendMessage("System", "Failed to connect to Hub after multiple attempts.");
            });
        }
    }
    // Method to send messages to the Hub
    public async void SendMessage()
    {
        if (_connection.State == HubConnectionState.Connected)
        {
            string user = userInput != null ? userInput.text : "UnityClient";
            string message = messageInput != null ? messageInput.text : "";
            if (string.IsNullOrEmpty(message))
            {
                Debug.LogWarning("Message cannot be empty.");
                return;
            }
            try
            {
                // Invoke a method on the server-side Hub.
                // The method name "SendMessage" must match the server-side Hub method.
                await _connection.InvokeAsync("SendMessage", user, message);
                Debug.Log($"</span>\"Sent message: '{message}' as '{user}'");
                if (messageInput != null)
                {
                    messageInput.text = ""; // Clear message input after sending
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error invoking SendMessage on Hub: {ex.Message}");
                AppendMessage("System", $"Error sending message: {ex.Message}");
            }
        }
        else
        {
            Debug.LogWarning("SignalR connection not open. Cannot send message.");
            AppendMessage("System", "Not connected to Hub. Please wait or try reconnecting.");
        }
    }

    // Helper to append messages to the UI display
    private void AppendMessage(string user, string message)
    {
        if (chatDisplay != null)
        {
            chatDisplay.text += $"[{DateTime.Now.ToShortTimeString()}] {user}: {message}\n";
            // Auto-scroll to bottom
            if (chatScrollRect != null)
            {
                Canvas.ForceUpdateCanvases();
                // Ensure layout is updated before scrolling
                chatScrollRect.verticalNormalizedPosition = 0f;
            }
        }
    }

    private void UpdateConnectionStatus(string status, Color color)
    {
        if (connectionStatusText != null)
        {
            connectionStatusText.text = "Status: " + status;
            connectionStatusText.color = color;
        }
    }

    // Optional: Custom Logger for SignalR internal logging to Unity's console
    //private class UnityDebugLoggerProvider : ILoggerProvider
    //{
    //    public Microsoft.Extensions.Logging.ILogger CreateLogger(string categoryName) => new UnityDebugLogger(categoryName);
    //    public void Dispose() { }
    //    private class UnityDebugLogger : ILogger
    //    {
    //        private readonly string _categoryName;
    //        public UnityDebugLogger(string categoryName) => _categoryName = categoryName;
    //        public IDisposable BeginScope<TState>(TState state) => NullScope.Instance;
    //        public bool IsEnabled(LogLevel logLevel) => true;
    //        // Log all levels
    //        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
    //        {
    //            string message = $"[{_categoryName}] {formatter(state, exception)}";
    //            switch (logLevel)
    //            {
    //                case LogLevel.Trace:
    //                case LogLevel.Debug:
    //                case LogLevel.Information:
    //                    Debug.Log(message);
    //                    break;

    //                case LogLevel.Warning:
    //                    Debug.LogWarning(message);
    //                    break;
    //                case LogLevel.Error:
    //                case LogLevel.Critical:
    //                    Debug.LogError(message + (exception != null ? $"\nException: {exception}" : ""));
    //                    break;
    //            }
    //        }
    //    }
    //}

}
