using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectAether.Objects.Models
{
    public class StoreTransactionItem
    {
        public int Id { get; set; }
        public int StoreTransactionId { get; set; } // Foreign key to StoreTransaction
        public StoreTransaction StoreTransaction { get; set; } // Navigation property to StoreTransaction
        public int StoreItemId { get; set; } // Foreign key to StoreItem
        public StoreItem StoreItem { get; set; } // Navigation property to StoreItem
        public int Quantity { get; set; } // Quantity of the item purchased
        public double UnitPrice { get; set; } // Price per unit at the time of purchase
        public double TotalPrice { get; set; } // Total price for this item (UnitPrice * Quantity)
        public double TaxAmount { get; set; } // Tax amount for this item
        public double TaxRate { get; set; } // Tax rate applied to this item
    }
}
