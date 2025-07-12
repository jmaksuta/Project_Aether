using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectAether.Objects.Net._2._1.Standard.Models
{
    [Serializable]
    public class StoreTransaction
    {
        public int Id { get; set; }
        public DateTime TransactionDate { get; set; } // Date and time of the transaction
        public string UserId { get; set; } // Identifier for the user making the purchase
        public User User { get; set; } // Navigation property to ApplicationUser

    }
}
