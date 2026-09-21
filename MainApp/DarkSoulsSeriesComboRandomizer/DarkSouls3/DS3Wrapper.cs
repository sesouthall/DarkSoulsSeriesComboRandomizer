namespace DarkSoulsSeriesComboRandomizer.DarkSouls3
{
    public class DS3Wrapper : SoulsGameWrapper, IDisposable
    {
        private readonly DS3Hook hook;
        private readonly Task itemWatchTask;

        private readonly CancellationTokenSource shutdownTokenSource = new();
        private bool disposedValue;

        public override event ItemReactor? OnModItemPickUp;

        public override SoulsGame Game => SoulsGame.DS3;

        /// <summary>
        /// DS3 is launched through ModEngine3 instead of calling the exe directly.
        /// ModEngine does the lookup for the game file, we just need the path to me3.exe.
        /// </summary>
        /// <param name="me3ExePath">The full path to me3.exe</param>
        /// <param name="me3ProfilePath">The full path to the profile to launch</param>
        public DS3Wrapper(string exePath, bool pauseOnMinimize = true) : base(exePath, "", pauseOnMinimize)
        {
            hook = new DS3Hook(5000, 5000);
            itemWatchTask = new Task(async () => await WatchItems());
        }

        public override async Task Start()
        {
            await base.Start();
            base.Exited += (wrapper) => shutdownTokenSource.Cancel();
            hook.Start();

            await hook.WaitForCharacterLoaded(shutdownTokenSource.Token);

            itemWatchTask.Start();
        }

        public override void Warp(int bonfireId) => hook.Warp(bonfireId);

        public override void GiveItem(SoulsItemType itemType, int itemId, int quantity) => hook.GiveItem((int)itemType + itemId, quantity);

        public override bool ContainsBonfireId(int bonfireId) => bonfireId >= 3002950 && bonfireId <= 5112951;

        private async Task WatchItems()
        {
            while (!shutdownTokenSource.IsCancellationRequested)
            {
                var inventory = hook.GetCurrentInventory();
                var modItems = inventory.Where(item => item.id >= 0x403D0900 && item.id <= 0x403D1552);
                foreach (var item in modItems)
                {
                    hook.RemoveItem(item.inventoryIndex);
                    OnModItemPickUp?.Invoke(SoulsGame.DS3, (int)item.id - 0x40000000, (int)item.quantity);
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