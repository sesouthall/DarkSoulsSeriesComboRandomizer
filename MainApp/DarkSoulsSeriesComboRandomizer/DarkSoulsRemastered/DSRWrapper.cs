namespace DarkSoulsSeriesComboRandomizer.DarkSoulsRemastered
{
    public class DSRWrapper : SoulsGameWrapper, IDisposable
    {
        private readonly DSRHook hook;
        private readonly Thread itemWatchThread;

        private bool shutdown = false;
        private bool disposedValue;

        public override event ItemReactor? OnModItemPickUp;

        public override SoulsGame Game => SoulsGame.DSR;

        public DSRWrapper(string exePath) : base(exePath)
        {
            hook = new DSRHook(5000, 5000);
            itemWatchThread = new Thread(WatchItems);
        }

        public override void Start()
        {
            base.Start();
            hook.Start();

            while (!hook.CharacterLoaded)
            {
                Thread.Sleep(1000);
            }

            itemWatchThread.Start();
        }

        public override void Warp(int bonfireId) => hook.Warp(bonfireId);

        public override void GiveItem(SoulsItemType itemType, int itemId, int quantity) => hook.GiveItem((int)itemType, itemId, quantity);

        public override bool ContainsBonfireId(int bonfireId) => bonfireId >= 1002960 && bonfireId <= 1812961;

        private void WatchItems()
        {
            while (!shutdown)
            {
                var inventory = hook.GetCurrentInventory();
                var modItems = inventory.Where(item => item.category == 0x40000000 && item.id >= 10000 && item.id <= 14931);
                foreach (var item in modItems)
                {
                    hook.RemoveItem(item.category, item.id);
                    OnModItemPickUp?.Invoke(SoulsGame.DSR, (int)item.id, (int)item.quantity);
                }
                Thread.Sleep(1000);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    shutdown = true;
                    itemWatchThread.Join();
                    hook.Stop();
                }

                base.Dispose(disposing);

                disposedValue = true;
            }
        }

        public new void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}