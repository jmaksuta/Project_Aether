namespace ProjectAether.Objects.Models
{
    public class StackableItem : InventoryItem
    {
        public StackableItem() : base()
        {
            IsStackable = true;
        }
    }
}
