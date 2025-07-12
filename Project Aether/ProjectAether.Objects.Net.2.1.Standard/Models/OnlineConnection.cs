using System;

namespace ProjectAether.Objects.Net._2._1.Standard.Models
{
    public class OnlineConnection
    {
        /// <summary>
        /// Auto-incrementing primary key for the connection record itself
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Adjust length as needed, SignalR connection IDs are typically GUID-like strings
        /// </summary>
        public string ConnectionId { get; set; }

        /// <summary>
        ///  Foreign key to ApplicationUser.Id
        ///  
        /// NOTE:
        /// IdentityUser.Id is NVARCHAR(450) by default
        ///  This will store the ApplicationUser.Id
        /// </summary>
        public string UserId { get; set; } //

        /// <summary>
        /// Optional: Denormalize username for easier querying if desired, though you can always join to ApplicationUser
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Use DateTimeOffset for better handling of time zones
        /// </summary>
        public DateTimeOffset ConnectedAt { get; set; }

        /// <summary>
        /// For heartbeat/cleanup
        /// </summary>
        public DateTimeOffset LastActivity { get; set; }

        /// <summary>
        /// Navigation property to the ApplicationUser
        /// </summary>
        public virtual User User { get; set; }

        public OnlineConnection() : base()
        {
            Id = 0;
            ConnectionId = string.Empty;
            UserId = string.Empty;
            UserName = string.Empty;
            ConnectedAt = DateTimeOffset.MinValue;
            LastActivity = DateTimeOffset.MinValue;
            User = new User();
    }
    }
}
