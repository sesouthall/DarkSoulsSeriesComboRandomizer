namespace DarkSoulsSeriesComboRandomizer.DarkSouls2SotFS
{
    public class DS2SotFSWrapper : SoulsGameWrapper, IDisposable
    {
        private readonly DS2SotFSHook hook;
        private readonly Thread itemWatchThread;

        private bool shutdown = false;
        private bool disposedValue;

        public override event ItemReactor? OnModItemPickUp;

        public override SoulsGame Game => SoulsGame.DS2S;

        public DS2SotFSWrapper(string exePath) : base(exePath)
        {
            hook = new DS2SotFSHook(5000, 5000);
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
            RefreshProcess("DarkSoulsII"); // DS2 has a weird system where the initial process closes almost immediately, but spawns a new one that is the actual game

            itemWatchThread.Start();
        }

        public override void Warp(int bonfireId) => hook.Warp((ushort)bonfireId);

        public override void GiveItem(SoulsItemType itemType, int itemId, int quantity) => hook.GiveItem(itemId, (short)quantity, 0, 0);

        public override bool ContainsBonfireId(int bonfireId) => bonfireId >= 2650 && bonfireId <= 37685;

        private void WatchItems()
        {
            while (!shutdown)
            {
                var inventory = hook.GetCurrentInventory();
                var modItems = inventory.Where(item => item.id >= 66000000 && item.id <= 66005540);
                foreach (var item in modItems)
                {
                    hook.RemoveItem(item.inventoryIndex);
                    OnModItemPickUp?.Invoke(SoulsGame.DS2S, item.id, item.quantityOrDurability < 10 ? item.quantityOrDurability : 1);
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