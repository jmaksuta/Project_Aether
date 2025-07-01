using System.Text.Json.Serialization;

namespace Project_Aether_Backend.Models
{
    [Serializable]
    public class GameObject
    {
        public virtual int Id { get; set; } // Unique identifier for the game object
        public virtual string Name { get; set; } // Name of the game object 
        public virtual string Description { get; set; } // Description of the game object   
        public GameObjectType ObjectType { get; set; } // Type of the game object (e.g., "GameCharacter", "PlayerCharacter")    
        public bool IsActive { get; set; } // Indicates if the game object is active in the game world
        public bool IsDeleted { get; set; } // Indicates if the game object has been deleted or removed from the game world 

        
        public GameObject() : base()
        {
            this.Id = 0;
            this.Name = string.Empty;
            this.Description = string.Empty;
            this.ObjectType = GameObjectType.Item; // Default type is Item
            this.IsActive = true;
            this.IsDeleted = false;
        }
    }
}
