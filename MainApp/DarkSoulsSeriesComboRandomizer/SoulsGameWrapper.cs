namespace DarkSoulsSeriesComboRandomizer
{
    public abstract class SoulsGameWrapper(string exePath, string args = "") : WindowsGameWrapper(exePath, args)
    {

        public abstract event ItemReactor? OnModItemPickUp;
        public abstract bool ContainsBonfireId(int bonfireId);
        public abstract void Warp(int destinationBonfire);
        public abstract void GiveItem(SoulsItemType type, int itemId, int quantity);
        public abstract SoulsGame Game { get; }
    }
}