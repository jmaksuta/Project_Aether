using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ProjectAether.Objects.Models
{
    public class OnlineConnection
    {
        /// <summary>
        /// Auto-incrementing primary key for the connection record itself
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Adjust length as needed, SignalR connection IDs are typically GUID-like strings
        /// </summary>
        [Required]
        [StringLength(256)] 
        public string ConnectionId { get; set; }

        /// <summary>
        ///  Foreign key to ApplicationUser.Id
        ///  
        /// NOTE:
        /// IdentityUser.Id is NVARCHAR(450) by default
        ///  This will store the ApplicationUser.Id
        /// </summary>
        [StringLength(450)]
        public string UserId { get; set; } //

        /// <summary>
        /// Optional: Denormalize username for easier querying if desired, though you can always join to ApplicationUser
        /// </summary>
        [StringLength(256)]
        public string UserName { get; set; }

        /// <summary>
        /// Use DateTimeOffset for better handling of time zones
        /// </summary>
        [Required]
        public DateTimeOffset ConnectedAt { get; set; }

        /// <summary>
        /// For heartbeat/cleanup
        /// </summary>
        public DateTimeOffset LastActivity { get; set; }

        /// <summary>
        /// Navigation property to the ApplicationUser
        /// </summary>
        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }

        public OnlineConnection() : base()
        {
            Id = 0;
            ConnectionId = string.Empty;
            UserId = string.Empty;
            UserName = string.Empty;
            ConnectedAt = DateTimeOffset.MinValue;
            LastActivity = DateTimeOffset.MinValue;
            User = new ApplicationUser();
    }
    }
}
