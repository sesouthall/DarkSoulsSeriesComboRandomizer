using DarkSoulsSeriesComboRandomizer.DarkSouls3;

namespace DarkSoulsSeriesComboRandomizer
{
    public class DS3Wrapper : WindowsGameWrapper, IDisposable
    {
        private DS3Hook hook;
        private Thread itemWatchThread;

        private bool shutdown = false;
        private bool disposedValue;

        public override event ItemReactor? OnModItemPickUp;

        public DS3Wrapper(string exePath) : base(exePath)
        {
            hook = new DS3Hook(5000, 5000);
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
                var modItems = inventory.Where(item => item.id >= 0x403D0900);
                foreach (var item in modItems)
                {
                    hook.RemoveItem(item.inventoryIndex);
                    OnModItemPickUp?.Invoke(SoulsGame.DS3, (int)item.id - 0x40000000, (int)item.quantity);
                }
                Thread.Sleep(1000);
            }
        }

        public void GiveItem(SoulsItemType itemType, int itemId, int quantity)
        {
            hook.GiveItem((int)itemType + itemId, quantity);
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