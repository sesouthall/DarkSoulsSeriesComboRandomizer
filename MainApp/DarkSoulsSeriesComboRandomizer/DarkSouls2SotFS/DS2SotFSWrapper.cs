namespace DarkSoulsSeriesComboRandomizer.DarkSouls2SotFS
{
    public class DS2SotFSWrapper : SoulsGameWrapper, IDisposable
    {
        private readonly DS2SotFSHook hook;
        private readonly Task itemWatchTask;

        private readonly CancellationTokenSource shutdownTokenSource = new();
        private bool disposedValue;

        public override event ItemReactor? OnModItemPickUp;

        public override SoulsGame Game => SoulsGame.DS2S;

        public DS2SotFSWrapper(string exePath) : base(exePath)
        {
            hook = new DS2SotFSHook(5000, 5000);
            itemWatchTask = new Task(async () => await WatchItems());
        }

        public override async Task Start()
        {
            await base.Start();
            base.Exited += (wrapper) => shutdownTokenSource.Cancel();
            hook.Start();

            await hook.WaitForCharacterLoaded(shutdownTokenSource.Token);
            RefreshProcess("DarkSoulsII"); // DS2 has a weird system where the initial process closes almost immediately, but spawns a new one that is the actual game

            itemWatchTask.Start();
        }

        public override void Warp(int bonfireId) => hook.Warp((ushort)bonfireId);

        public override void GiveItem(SoulsItemType itemType, int itemId, int quantity) => hook.GiveItem(itemId, (short)quantity, 0, 0);

        public override bool ContainsBonfireId(int bonfireId) => bonfireId >= 2650 && bonfireId <= 37685;

        private async Task WatchItems()
        {
            while (!shutdownTokenSource.IsCancellationRequested)
            {
                var inventory = hook.GetCurrentInventory();
                var modItems = inventory.Where(item => item.id >= 66000000 && item.id <= 66005540);
                foreach (var item in modItems)
                {
                    hook.RemoveItem(item.inventoryIndex);
                    OnModItemPickUp?.Invoke(SoulsGame.DS2S, item.id, item.quantityOrDurability < 10 ? item.quantityOrDurability : 1);
                }
                await Task.Delay(1000, shutdownTokenSource.Token).ContinueWith(task => { /*suppress cancellation exception*/ });
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    shutdownTokenSource.Cancel();
                    if (itemWatchTask.Status == TaskStatus.Running)
                    {
                        itemWatchTask.Wait();
                    }
                    hook.Stop();
                    shutdownTokenSource.Dispose();
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