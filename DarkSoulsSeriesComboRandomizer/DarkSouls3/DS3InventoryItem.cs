namespace DarkSoulsSeriesComboRandomizer.DarkSouls3
{
    public class DS3InventoryItem
    {
        // In-game properties
        public readonly int property1;
        public readonly uint id;
        public readonly uint quantity;
        public readonly int property4;

        // convenience properties
        public readonly int inventoryIndex;

        public DS3InventoryItem(int property1, uint id, uint quantity, int property4, int inventoryIndex)
        {
            this.property1 = property1;
            this.id = id;
            this.quantity = quantity;
            this.property4 = property4;
            this.inventoryIndex = inventoryIndex;
        }
    }
}