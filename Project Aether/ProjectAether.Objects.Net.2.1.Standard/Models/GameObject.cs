using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Numerics;
using System.Text.Json.Serialization;

namespace ProjectAether.Objects.Net._2._1.Standard.Models
{
    [Serializable]
    public class GameObject
    {

        /// <summary>
        /// Unique identifier for the game object.
        /// </summary>
        public virtual int Id { get; set; }

        /// <summary>
        /// Name of the game object.
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// Detailed description of the game object.
        /// </summary>
        public virtual string Description { get; set; } // Description of the game object   

        /// <summary>
        /// Type of the game object (e.g., "GameCharacter", "PlayerCharacter", "NPC", "Item", "Container", etc.)
        /// </summary>
        public GameObjectType ObjectType { get; set; }

        /// <summary>
        /// Indicates if the game object is currently active in the game world.
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Indicates if the game object has been deleted or removed from the game world.
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// Foreign key to the WorldZone where this object is located.
        /// </summary>
        public int WorldZoneId { get; set; }

        /// <summary>
        /// The name of the prefab.
        /// </summary>
        public string PrefabName { get; set; }

        /// <summary>
        /// The data for configuration of the prefab.
        /// </summary>
        public string PrefabConfigData { get; set; }

        /// <summary>
        /// Navigation property to the WorldZone where this object is located.
        /// </summary>
        [JsonIgnore]
        public WorldZone WorldZone { get; set; }

        [JsonIgnore]
        /// <summary>
        /// X coordinate position of the game object in the world zone.
        /// </summary>
        public float xPosition
        {
            get
            {
                return Position.X;
            }
            set
            {
                Position = new Vector3(value, Position.Y, Position.Z);
            }
        }

        [JsonIgnore]
        /// <summary>
        /// Y coordinate position of the game object in the world zone.
        /// </summary>
        public float yPosition
        {
            get
            {
                return Position.Y;
            }
            set
            {
                Position = new Vector3(Position.X, value, Position.Z);
            }
        }

        [JsonIgnore]
        /// <summary>
        /// Z coordinate position of the game object in the world zone.
        /// </summary>
        public float zPosition
        {
            get
            {
                return Position.Z;
            }
            set
            {
                Position = new Vector3(Position.X, Position.Y, value);
            }
        }

        [NotMapped]
        /// <summary>
        /// Navigation property to the WorldZone where this object is located.
        /// </summary>
        public Vector3 Position { get; set; }

        public GameObject() : base()
        {
            Id = 0;
            Name = string.Empty;
            Description = string.Empty;
            ObjectType = GameObjectType.Item; // Default type is Item
            IsActive = true;
            IsDeleted = false;
            WorldZoneId = 0;
            WorldZone = new WorldZone();
            Position = new Vector3(0, 0, 0);
        }

        //public static implicit operator GameObject(GameObject v)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
