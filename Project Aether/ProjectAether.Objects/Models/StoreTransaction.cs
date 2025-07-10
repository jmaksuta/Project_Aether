using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectAether.Objects.Models
{
    public class StoreTransaction
    {
        public int Id { get; set; }
        public DateTime TransactionDate { get; set; } // Date and time of the transaction
        public string UserId { get; set; } // Identifier for the user making the purchase
        public ApplicationUser User { get; set; } // Navigation property to ApplicationUser

    }
}
