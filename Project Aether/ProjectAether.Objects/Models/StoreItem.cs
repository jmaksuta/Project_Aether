using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectAether.Objects.Models
{
    public class StoreItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public string Currency { get; set; } // e.g., "USD", "EUR", "Gems"
        public string ItemType { get; set; } // e.g., "Cosmetic", "PowerUp", "Subscription"
        public string ImageId { get; set; } // Reference to an image asset
        public int quantityAvailable { get; set; } // For limited stock items
        public bool isActive { get; set; } // Is the item currently available for purchase
        public bool isTaxed { get; set; } // Does the item incur tax
        public double taxRate { get; set; } // Tax rate as a percentage (e.g., 0.07 for 7%)
        public double taxAmount { get; set; }   
    }
}
