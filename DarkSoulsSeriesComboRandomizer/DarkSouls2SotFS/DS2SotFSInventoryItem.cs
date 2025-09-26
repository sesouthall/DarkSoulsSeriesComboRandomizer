namespace DarkSoulsSeriesComboRandomizer.DarkSouls2SotFS
{
    public class DS2SotFSInventoryItem
    {
        // In-game properties
        public readonly IntPtr previousItemPointer;
        public readonly IntPtr nextItemPointer;
        public readonly int id;
        public readonly int listIndexCategoryAndLocation;
        public readonly int quantityOrDurability;
        public readonly int upgradeLevelAndIdk;

        // Convenience properties
        public readonly int inventoryIndex;

        public DS2SotFSInventoryItem(IntPtr previousItemPointer, IntPtr nextItemPointer, int id, int listIndexCategoryAndLocation, int quantityOrDurability, int upgradeLevelAndIdk, int inventoryIndex)
        {
            this.previousItemPointer = previousItemPointer;
            this.nextItemPointer = nextItemPointer;
            this.id = id;
            this.listIndexCategoryAndLocation = listIndexCategoryAndLocation;
            this.quantityOrDurability = quantityOrDurability;
            this.upgradeLevelAndIdk = upgradeLevelAndIdk;
            this.inventoryIndex = inventoryIndex;
        }
    }
}