using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ProjectAether.Objects.Net._2._1.Standard.Models
{
    [Serializable]
    public class Inventory
    {
        public int Id { get; set; } // Unique identifier for the inventory

        [JsonIgnore]
        public ICollection<InventoryItem> Items { get; set; } // Collection of items in the inventory

        public Inventory() : base()
        {
            Id = 0; // Default value for Id
            Items = new List<InventoryItem>();
        }

        public Inventory(int id, ICollection<InventoryItem> items)
        {
            Id = id;
            Items = items ?? new List<InventoryItem>();
        }

        public Inventory(ICollection<InventoryItem> items)
        {
            Id = 0; // Default value for Id
            Items = items ?? new List<InventoryItem>();
        }
    }
}
