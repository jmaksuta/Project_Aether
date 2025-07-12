using System;

namespace ProjectAether.Objects.Net._2._1.Standard.Models
{
    [Serializable]
    public class User
    {
        public string Id { get; set; } // Identifier for the user making the purchase
        public string UserName { get; set; } // Username of the user
        // Add custom fields for your user here.
        public DateTime DateRegistered { get; set; }
        
        public PlayerProfile Player { get; set; }
    }
}
