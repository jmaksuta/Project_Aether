namespace Project_Aether_Backend.Models
{
    public class NonPlayerCharacter : GameCharacter
    {

        public NonPlayerCharacter() : base()
        {
            this.Name = string.Empty;
            this.Description = string.Empty;
            this.ObjectType = GameObjectType.NonPlayerCharacter; // Set the object type to NonPlayerCharacter
            this.Level = 1;
            this.Health = 100;
            this.Mana = 100;
            this.Experience = 0;
            this.Inventory = new Inventory();
        }
    }
}
