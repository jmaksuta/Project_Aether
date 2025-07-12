using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ProjectAether.Objects.Net._2._1.Standard.Models
{
    [Serializable]
    public class ArchetypeDefinition
    {
        public int Id { get; set; } = 0;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string AvatarImageId { get; set; } = string.Empty;
        public int? StoreItemId { get; set; } // Foreign key to StoreItem
        [JsonIgnore]
        public StoreItem StoreItem { get; set; } // Navigation property to StoreItem
        public int BaseHealth { get; set; } = 0;
        public int BaseMana { get; set; } = 0;
        public int BaseStrength { get; set; } = 0;
        public int BaseAgility { get; set; } = 0;
        public int BaseIntelligence { get; set; } = 0;
        public int BaseIntuition { get; set; } = 0;
        public int BaseCharisma { get; set; } = 0;
        public int BaseLuck { get; set; } = 0;
        public int BaseDefense { get; set; } = 0;
        public int BaseDodge { get; set; } = 0;
        public int BaseSpeed { get; set; } = 0;
        public List<string> StartingAbilities { get; set; } = new List<string>();
        public List<string> StartingEquipment { get; set; } = new List<string>();
    }
}
