using ProjectAether.Objects.Net._2._1.Standard.Models;
using System;
using System.Numerics;
using System.Text.Json.Serialization;

namespace Assets.Scripts.Models
{
    [Serializable]
    public class GameObjectDto
    {
        //[JsonPropertyName("id")]
        public int id;
        //[JsonPropertyName("name")]
        public string name;
        //[JsonPropertyName("description")]
        public string description;
        //[JsonPropertyName("objectType")]
        public int objectType;
        //[JsonPropertyName("isActive")]
        public bool isActive;
        //[JsonPropertyName("isDeleted")]
        public bool isDeleted;
        //[JsonPropertyName("worldZoneId")]
        public int worldZoneId;
        //[JsonPropertyName("prefabName")]
        public string prefabName;
        //[JsonPropertyName("prefabConfigData")]
        public string prefabConfigData;
        //[JsonPropertyName("xPosition")]
        public float xPosition;
        //[JsonPropertyName("yPosition")]
        public float yPosition;
        //[JsonPropertyName("zPosition")]
        public float zPosition;
        //[JsonPropertyName("position")]
        public Vector3 Position;

        public GameObject ToGameObject()
        {
            return new GameObject
            {
                Id = this.id,
                Name = this.name,
                Description = this.description,
                ObjectType = (GameObjectType)this.objectType,
                IsActive = this.isActive,
                IsDeleted = this.isDeleted,
                WorldZoneId = this.worldZoneId,
                PrefabName = this.prefabName,
                PrefabConfigData = this.prefabConfigData,
                xPosition = this.xPosition,
                yPosition = this.yPosition,
                zPosition = this.zPosition,
                //Position = new Vector3(this.xPosition, this.yPosition, this.zPosition),
            };
        }
    }
}
