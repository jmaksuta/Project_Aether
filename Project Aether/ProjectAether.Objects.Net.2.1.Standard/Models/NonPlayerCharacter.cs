namespace ProjectAether.Objects.Net._2._1.Standard.Models
{
    public class NonPlayerCharacter : GameCharacter
    {

        public NonPlayerCharacter() : base()
        {
            Name = string.Empty;
            Description = string.Empty;
            ObjectType = GameObjectType.NonPlayerCharacter; // Set the object type to NonPlayerCharacter
            Level = 1;
            Health = 100;
            Mana = 100;
            Experience = 0;
            Inventory = new Inventory();
        }
    }
}
