namespace DarkSoulsSeriesComboRandomizer
{
    public delegate void ItemReactor(SoulsGame game, int itemId, int quantity);
    public delegate void ExitReactor(SoulsGameWrapper exitedWrapper);
    public abstract class SoulsGameWrapper(string exePath, string args = "") : WindowsGameWrapper(exePath, args)
    {
        public abstract event ItemReactor? OnModItemPickUp;
        public new event ExitReactor? Exited;
        public abstract bool ContainsBonfireId(int bonfireId);
        public abstract void Warp(int destinationBonfire);
        public abstract void GiveItem(SoulsItemType type, int itemId, int quantity);
        public abstract SoulsGame Game { get; }

        public async override Task Start()
        {
            await base.Start();
            base.Exited += OnExit;
        }

        private void OnExit(object? sender, EventArgs e)
        {
            Exited?.Invoke(this);
        }
    }
}