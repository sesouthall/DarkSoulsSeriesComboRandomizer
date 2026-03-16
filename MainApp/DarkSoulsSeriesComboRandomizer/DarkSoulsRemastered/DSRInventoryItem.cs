namespace DarkSoulsSeriesComboRandomizer.DarkSoulsRemastered
{
    public class DSRInventoryItem
    {
        // In-game properties
        public readonly uint category;
        public readonly uint id;
        public readonly uint quantity;
        public readonly int property4;
        public readonly int property5;
        public readonly uint durability;
        public readonly int property7;

        // convenience properties
        public readonly int inventoryIndex;

        public DSRInventoryItem(uint category, uint id, uint quantity, int property4, int property5, uint durability, int property7, int inventoryIndex)
        {
            this.category = category;
            this.id = id;
            this.quantity = quantity;
            this.property4 = property4;
            this.property5 = property5;
            this.durability = durability;
            this.property7 = property7;
            this.inventoryIndex = inventoryIndex;
        }
    }
}