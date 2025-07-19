using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace ProjectAether.Objects.Net._2._1.Standard.Models
{
    [Serializable]
    public class WorldZone
    {

        /// <summary>
        /// Unique identifier for the world zone.
        /// </summary>
        [JsonPropertyName("id")]
        [DataMember(Name = "id", IsRequired = true, EmitDefaultValue = true)]
        public int Id { get; set; }

        /// <summary>
        /// Name of the world zone.
        /// </summary>
        [JsonPropertyName("name")]  
        [DataMember(Name = "name", IsRequired = true, EmitDefaultValue = true)]
        public string Name { get; set; }

        /// <summary>
        /// Detailed description of the world zone.
        /// </summary>
        [JsonPropertyName("description")]   
        [DataMember(Name = "description", IsRequired = true, EmitDefaultValue = true)]
        public string Description { get; set; }

        /// <summary>
        /// Identifier for the zone, used for internal tracking.
        /// </summary>
        [JsonPropertyName("zoneId")]    
        [DataMember(Name = "zoneId", IsRequired = true, EmitDefaultValue = true)]
        public int ZoneId { get; set; }

        /// <summary>
        /// Name of the scene associated with this zone.
        /// </summary>
        [JsonPropertyName("sceneName")]
        [DataMember(Name = "sceneName", IsRequired = true, EmitDefaultValue = true)]
        public string SceneName { get; set; }

        /// <summary>
        /// IP address of the server hosting the zone.
        /// </summary>
        [JsonPropertyName("serverIPAddress")]
        [DataMember(Name = "serverIPAddress", IsRequired = true, EmitDefaultValue = true)]
        public string ServerIPAddress { get; set; }

        /// <summary>
        /// Port number of the server hosting the zone.
        /// </summary>
        [JsonPropertyName("serverPort")]
        [DataMember(Name = "serverPort", IsRequired = true, EmitDefaultValue = true)]
        public int ServerPort { get; set; }

        /// <summary>
        /// Collection of game objects located within this zone.
        /// </summary>
        [JsonIgnore, IgnoreDataMember()]
        public ICollection<GameObject> GameObjects { get; set; } = new List<GameObject>(); // Collection of game objects in the zone    

        public WorldZone() : base()
        {
            Id = 0;
            Name = string.Empty;
            Description = string.Empty;
            ZoneId = 0;
            SceneName = string.Empty;
            ServerIPAddress = string.Empty;
            ServerPort = -1;
            GameObjects = new List<GameObject>();
        }

        public void Start()
        {
            // Initialization logic for the world zone can be added here.
            // TODO: should start a new instance of the server shard with the scene name as a starting parameter.
        }

        public void Stop()
        {
            // TODO: should shutdown the shard and move all players to a safe zone.
        }
    }
}
