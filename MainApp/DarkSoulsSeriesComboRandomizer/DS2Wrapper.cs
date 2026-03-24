using DarkSoulsSeriesComboRandomizer.DarkSouls2SotFS;

namespace DarkSoulsSeriesComboRandomizer
{
    public class DS2Wrapper : WindowsGameWrapper, IDisposable
    {
        private DS2SotFSHook hook;
        private Thread itemWatchThread;

        private bool shutdown = false;
        private bool disposedValue;

        public override event ItemReactor? OnModItemPickUp;

        public DS2Wrapper(string exePath) : base(exePath)
        {
            hook = new DS2SotFSHook(5000, 5000);
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
            base.RefreshProcess("DarkSoulsII"); // DS2 has a weird system where the initial process closes almost immediately, but spawns a new one that is the actual game

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
                    OnModItemPickUp?.Invoke(SoulsGame.DS2, (int)item.id, (int)item.quantityOrDurability < 10 ? item.quantityOrDurability : 1);
                }
                Thread.Sleep(1000);
            }
        }

        public void GiveSunlightMedals()
        {
            if (!hook.GetCurrentInventory().Any(item => item.id == 66000157))
            {
                hook.GiveItem(66000157, 1, 0, 0);
            }
            if (!hook.GetCurrentInventory().Any(item => item.id == 66001233))
            {
                hook.GiveItem(66001233, 1, 0, 0);
            }
        }

        public void GiveItem(int itemId, short quantity)
        {
            hook.GiveItem(itemId, quantity, 0, 0);
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