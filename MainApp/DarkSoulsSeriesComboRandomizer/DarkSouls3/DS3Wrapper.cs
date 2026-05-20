namespace DarkSoulsSeriesComboRandomizer.DarkSouls3
{
    public class DS3Wrapper : SoulsGameWrapper, IDisposable
    {
        private readonly DS3Hook hook;
        private readonly Thread itemWatchThread;

        private bool shutdown = false;
        private bool disposedValue;

        public override event ItemReactor? OnModItemPickUp;

        public override SoulsGame Game => SoulsGame.DS3;

        /// <summary>
        /// DS3 is launched through ModEngine3 instead of calling the exe directly.
        /// ModEngine does the lookup for the game file, we just need the path to me3.exe.
        /// </summary>
        /// <param name="me3ExePath">The full path to me3.exe</param>
        /// <param name="me3ProfilePath">The full path to the profile to launch</param>
        public DS3Wrapper(string exePath) : base(exePath)
        {
            hook = new DS3Hook(5000, 5000);
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

        public override void GiveItem(SoulsItemType itemType, int itemId, int quantity) => hook.GiveItem((int)itemType + itemId, quantity);

        public override bool ContainsBonfireId(int bonfireId) => bonfireId >= 3002950 && bonfireId <= 5112951;

        private void WatchItems()
        {
            while (!shutdown)
            {
                var inventory = hook.GetCurrentInventory();
                var modItems = inventory.Where(item => item.id >= 0x403D0900 && item.id <= 0x403D1552);
                foreach (var item in modItems)
                {
                    hook.RemoveItem(item.inventoryIndex);
                    OnModItemPickUp?.Invoke(SoulsGame.DS3, (int)item.id - 0x40000000, (int)item.quantity);
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