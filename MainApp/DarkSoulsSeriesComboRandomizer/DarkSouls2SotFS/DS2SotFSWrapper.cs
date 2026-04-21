namespace DarkSoulsSeriesComboRandomizer.DarkSouls2SotFS
{
    public class DS2SotFSWrapper : WindowsGameWrapper, IDisposable
    {
        private DS2SotFSHook hook;
        private Thread itemWatchThread;

        private bool shutdown = false;
        private bool disposedValue;

        public override event ItemReactor? OnModItemPickUp;

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

        public void Warp(int bonfireId)
        {
            hook.Warp((ushort)bonfireId);
        }

        private void WatchItems()
        {
            while (!shutdown)
            {
                var inventory = hook.GetCurrentInventory();
                var modItems = inventory.Where(item => item.id >= 66000000);
                foreach (var item in modItems)
                {
                    hook.RemoveItem(item.inventoryIndex);
                    OnModItemPickUp?.Invoke(SoulsGame.DS2S, item.id, item.quantityOrDurability < 10 ? item.quantityOrDurability : 1);
                }
                Thread.Sleep(1000);
            }
        }

        public void GiveItem(int itemId, short quantity)
        {
            hook.GiveItem(itemId, quantity, 0, 0);
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