using DarkSoulsSeriesComboRandomizer.DarkSoulsRemastered;

namespace DarkSoulsSeriesComboRandomizer
{
    public class DSRWrapper : WindowsGameWrapper, IDisposable
    {
        private DSRHook hook;
        private Thread itemWatchThread;

        private bool shutdown = false;
        private bool disposedValue;

        public override event ItemReactor? OnModItemPickUp;

        public DSRWrapper(string exePath) : base(exePath)
        {
            hook = new DSRHook(5000, 5000);
            itemWatchThread = new Thread(this.WatchItems);
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

        public void Warp(int bonfireId)
        {
            hook.Warp(bonfireId);
        }

        private void WatchItems()
        {
            while (!shutdown)
            {
                var inventory = hook.GetCurrentInventory();
                var modItems = inventory.Where(item => item.category == 0x40000000 && item.id >= 10000);
                foreach (var item in modItems)
                {
                    hook.RemoveItem(item.inventoryIndex);
                    OnModItemPickUp?.Invoke(SoulsGame.DSR, (int)item.id, (int)item.quantity);
                }
                Thread.Sleep(1000);
            }
        }

        public void GiveSunlightMedals()
        {
            if (!hook.GetCurrentInventory().Any(item => item.category == 0x40000000 && item.id == 12297))
            {
                hook.GiveItem(0x40000000, 12297, 1);
            }
            if (!hook.GetCurrentInventory().Any(item => item.category == 0x40000000 && item.id == 13253))
            {
                hook.GiveItem(0x40000000, 13253, 1);
            }
        }

        public void GiveItem(SoulsItemType itemType, int itemId, int quantity)
        {
            hook.GiveItem((int)itemType, itemId, quantity);
        }

        protected virtual void Dispose(bool disposing)
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

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}