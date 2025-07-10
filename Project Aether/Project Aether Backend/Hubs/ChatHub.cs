using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Project_Aether_Backend.Data;
using ProjectAether.Objects.Models;
using System.Security.Claims;

namespace Project_Aether_Backend.Hubs
{
    [Authorize] // Only authenticated users can connect to this hub
    public class ChatHub : Hub
    {
        private readonly ApplicationDbContext _dbContext;

        private string ConnectionId => Context.ConnectionId;

        private string UserId
        {
            get
            {
                string result = string.Empty;
                if (Context.User != null)
                {
                    Claim? claim = Context.User.FindFirst(ClaimTypes.NameIdentifier);
                    if (claim != null)
                    {
                        result = claim.Value;
                    }
                }
                return result;
            }
        }

        private string UserName
        {
            get
            {
                string result = string.Empty;
                if (Context != null && Context.User != null && Context.User.Identity != null && Context.User.Identity.Name != null)
                {
                    result = Context.User.Identity.Name;
                }
                return result;
            }
        }

        public ChatHub(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task SendMessage(string user, string message)
        {
            // Send to all connected clients
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        public async Task SendGlobalChat(string message)
        {
            // Get the current user's name from the authenticated context
            var username = this.UserName;
            await Clients.All.SendAsync("ReceiveGlobalChat", username, message);
        }

        // Example: Sending a message to a specific user
        public async Task SendPrivateMessage(string recipientUsername, string message)
        {
            var senderUsername = this.UserName;

            // Find the connection ID of the recipient
            // This is a simplification; in a real app, you'd likely map User ID to Connection ID.
            // For a proof of concept, we assume recipientUsername is also the connection ID or you have a lookup.
            // For true private messaging, you would manage user-to-connection mappings.
            // A better way is using groups or a user-ID based mapping.
            // Clients.User(recipientUsername).SendAsync("ReceivePrivateMessage",senderUsername, message);
            await Clients.All.SendAsync("ReceivePrivateMessage", senderUsername, recipientUsername, message); // For simplicity, broadcast for now.
        }

        public override async Task OnConnectedAsync()
        {
            try
            {
                // Ensure the user is authenticated and we have their UserId
                EnsureUserAuthenticated();

                //--- Enforce One-User-One-Connection Logic ---
                UpdateOrInsertConnection();

                Console.WriteLine($"User '{this.UserName}' (ID: {this.UserId}) connected with new ConnectionId: {this.ConnectionId}. Added to DB.");

                // --- Optional: Broadcast new online user to all clients ---
                await BroadcaseOnlineUsersToAllClients();
                // -----------------------------------------------------------

            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                await base.OnConnectedAsync();
            }
        }

        private void EnsureUserAuthenticated()
        {
            if (string.IsNullOrEmpty(this.UserId))
            {
                Console.WriteLine($"Anonymous user connected with ID: {this.ConnectionId}. Not tracking in OnlineConnections.");
                // For a game, you might disconnect anonymous users immediately.
                Context.Abort(); // Disconnects the anonymous user
                throw new HubException("User must be authenticated to connect to this hub.");
            }
            Console.WriteLine($"User with name: {this.UserName} is authenticated. User ID: {this.UserId}, Connection ID: {this.ConnectionId}");
        }

        private async void UpdateOrInsertConnection()
        {
            var existingConnection = await _dbContext.OnlineConnections
                                                     .FirstOrDefaultAsync(oc => oc.UserId == this.UserId);

            if (existingConnection != null)
            {
                await DisconnectExisting(existingConnection);
                await UpdateExistingRecord(existingConnection);
            }
            else
            {
                await InsertNewRecord();
            }
        }

        private async Task DisconnectAndDeleteExisting(OnlineConnection existing)
        {
            // User is already connected. Disconnect the *previous* connection.
            Console.WriteLine($"User '{this.UserName}' (ID: {this.UserId}) already has an active connection (ID: {existing.ConnectionId}). Disconnecting old connection.");

            // 1. Notify the old client that they are being disconnected
            await DisconnectExisting(existing);

            // 2. Remove the old connection from the database
            await DeleteExisting(existing);

            // It's also good practice to try and forcibly disconnect the old SignalR connection on the server side
            // This is a bit trickier with just ConnectionId. If using Azure SignalR Service, it provides direct APIs.
            // For self-hosted, you'd rely on the client receiving "ForceDisconnect" and closing, or the connection eventually timing out.
            // Alternatively, you could try:
            // await Clients.Client(existingConnection.ConnectionId).SendCoreAsync("SignalR:CloseConnection", Array.Empty<object>());
            // but this isn't a public API and might not be reliable. Rely on client-side logic to disconnect.
        }

        private async Task DisconnectExisting(OnlineConnection existing)
        {
            // User is already connected. Disconnect the *previous* connection.
            Console.WriteLine($"User '{this.UserName}' (ID: {this.UserId}) already has an active connection (ID: {existing.ConnectionId}). Disconnecting old connection.");

            // 1. Notify the old client that they are being disconnected
            await Clients.Client(existing.ConnectionId).SendAsync("ForceDisconnect", "You have logged in from another location.");

            // 2. Remove the old connection from the database
            _dbContext.OnlineConnections.Remove(existing);
            await _dbContext.SaveChangesAsync();

            // It's also good practice to try and forcibly disconnect the old SignalR connection on the server side
            // This is a bit trickier with just ConnectionId. If using Azure SignalR Service, it provides direct APIs.
            // For self-hosted, you'd rely on the client receiving "ForceDisconnect" and closing, or the connection eventually timing out.
            // Alternatively, you could try:
            // await Clients.Client(existingConnection.ConnectionId).SendCoreAsync("SignalR:CloseConnection", Array.Empty<object>());
            // but this isn't a public API and might not be reliable. Rely on client-side logic to disconnect.
        }

        private async Task DeleteExisting(OnlineConnection existing)
        {
            // User is already connected. Disconnect the *previous* connection.
            Console.WriteLine($"Deleting Online Connection Id={existing.Id}, for User Id='{existing.UserId}' and Username='{existing.UserName}'.");

            // 2. Remove the old connection from the database
            _dbContext.OnlineConnections.Remove(existing);
            await _dbContext.SaveChangesAsync();
        }

        private async Task UpdateExistingRecord(OnlineConnection existing)
        {
            Console.WriteLine($"User '{this.UserName}' (ID: {this.UserId}) already has an active connection (ID: {existing.ConnectionId}). Disconnecting old connection.");

            var newOnlineConnection = new OnlineConnection
            {
                ConnectionId = this.ConnectionId,
                UserId = this.UserId,
                UserName = this.UserName,
                ConnectedAt = DateTimeOffset.UtcNow,
                LastActivity = DateTimeOffset.UtcNow
            };

            _dbContext.OnlineConnections.Update(newOnlineConnection);
            await _dbContext.SaveChangesAsync();
        }

        private async Task InsertNewRecord()
        {
            var newOnlineConnection = new OnlineConnection
            {
                ConnectionId = this.ConnectionId,
                UserId = this.UserId,
                UserName = this.UserName,
                ConnectedAt = DateTimeOffset.UtcNow,
                LastActivity = DateTimeOffset.UtcNow
            };

            _dbContext.OnlineConnections.Add(newOnlineConnection);
            await _dbContext.SaveChangesAsync();
        }

        private async Task BroadcaseOnlineUsersToAllClients()
        {
            var currentlyOnlineUserNames = await _dbContext.OnlineConnections
                                                           .Where(oc => oc.UserName != null)
                                                           .Select(oc => oc.UserName)
                                                           .Distinct()
                                                           .ToListAsync();
            await Clients.All.SendAsync("UserStatusUpdate", currentlyOnlineUserNames);
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            //Console.WriteLine($"User {Context.User.Identity.Name} disconnected with ID:{Context.ConnectionId}.");
            //// Optionally, remove user from "online" list via Redis
            //await base.OnDisconnectedAsync(exception);
            //-----------------------------------------------------------------------
            string connectionId = Context.ConnectionId;

            // Find and remove the connection record from the database
            FindAndRemoveConnection(connectionId);

            await base.OnDisconnectedAsync(exception);
        }

        private async void FindAndRemoveConnection(string connectionId)
        {
            var onlineConnection = await _dbContext.OnlineConnections
                                                   .FirstOrDefaultAsync(oc => oc.ConnectionId == connectionId);

            if (onlineConnection != null)
            {
                _dbContext.OnlineConnections.Remove(onlineConnection);
                await _dbContext.SaveChangesAsync();
                Console.WriteLine($"User '{onlineConnection.UserName}' (ID: {onlineConnection.UserId}) disconnected with ConnectionId: {connectionId}. Removed from DB.");

                // --- Optional: Broadcast updated online user list to all clients ---
                var currentlyOnlineUserNames = await _dbContext.OnlineConnections
                                                               .Where(oc => oc.UserName != null)
                                                               .Select(oc => oc.UserName)
                                                               .Distinct()
                                                               .ToListAsync();
                await Clients.All.SendAsync("UserStatusUpdate", currentlyOnlineUserNames);
                // -----------------------------------------------------------
            }
            else
            {
                Console.WriteLine($"Disconnected ConnectionId {connectionId} not found in DB (might have been removed already or never properly added).");
            }
        }

        public async Task GetCurrentOnlineUsers()
        {
            var onlineUserNames = await _dbContext.OnlineConnections
                .Where(oc => oc.User != null)
                .Select(oc => oc.UserName)
                .Distinct()
                .ToListAsync();

            await Clients.Caller.SendAsync("ReceiveCurrentOnlineUsers", onlineUserNames);
        }

        // Example: Sending a message only to a specific online user
        public async Task SendPrivateMessage2(string targetUserName, string message)
        {
            // Find all connection IDs for the target user from the database
            var targetUserConnectionIds = await _dbContext.OnlineConnections
                .Where(oc => oc.UserName == targetUserName)
                .Select(oc => oc.ConnectionId).ToListAsync();
            if (targetUserConnectionIds.Any())
            {
                await Clients.Clients(targetUserConnectionIds).SendAsync("ReceivePrivateMessage", Context.User?.Identity?.Name, message);
            }
            else
            {
                // Handle case where target user is not found or not online
                await Clients.Caller.SendAsync("ErrorMessage", $"User '{targetUserName}' is not currently online.");
            }
        }

    }
}
