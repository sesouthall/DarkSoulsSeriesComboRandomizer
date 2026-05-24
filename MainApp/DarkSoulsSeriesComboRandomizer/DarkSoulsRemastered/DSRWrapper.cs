namespace DarkSoulsSeriesComboRandomizer.DarkSoulsRemastered
{
    public class DSRWrapper : SoulsGameWrapper, IDisposable
    {
        private readonly DSRHook hook;
        private readonly Task itemWatchTask;

        private readonly CancellationTokenSource shutdownTokenSource = new();
        private bool disposedValue;

        public override event ItemReactor? OnModItemPickUp;

        public override SoulsGame Game => SoulsGame.DSR;

        public DSRWrapper(string exePath) : base(exePath)
        {
            hook = new DSRHook(5000, 5000);
            itemWatchTask = new Task(async () => await WatchItems());
        }

        public override async Task Start()
        {
            await base.Start();
            hook.Start();

            await hook.WaitForCharacterLoaded(shutdownTokenSource.Token);

            itemWatchTask.Start();
        }

        public override void Warp(int bonfireId) => hook.Warp(bonfireId);

        public override void GiveItem(SoulsItemType itemType, int itemId, int quantity) => hook.GiveItem((int)itemType, itemId, quantity);

        public override bool ContainsBonfireId(int bonfireId) => bonfireId >= 1002960 && bonfireId <= 1812961;

        private async Task WatchItems()
        {
            while (!shutdownTokenSource.IsCancellationRequested)
            {
                var inventory = hook.GetCurrentInventory();
                var modItems = inventory.Where(item => item.category == 0x40000000 && item.id >= 10000 && item.id <= 14931);
                foreach (var item in modItems)
                {
                    hook.RemoveItem(item.category, item.id);
                    OnModItemPickUp?.Invoke(SoulsGame.DSR, (int)item.id, (int)item.quantity);
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