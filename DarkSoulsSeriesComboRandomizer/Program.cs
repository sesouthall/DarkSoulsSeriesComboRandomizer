using DarkSoulsSeriesComboRandomizer.DarkSouls2SotFS;
using DarkSoulsSeriesComboRandomizer.DarkSouls3;
using DarkSoulsSeriesComboRandomizer.DarkSoulsRemastered;

namespace DarkSoulsSeriesComboRandomizer
{
    internal static class Program
    {
        private static DSRWrapper? dsrWrapper;
        private static DS2Wrapper? ds2Wrapper;
        private static DS3Wrapper? ds3Wrapper;

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            dsrWrapper = new DSRWrapper(@"C:\Program Files (x86)\Steam\steamapps\common\DARK SOULS REMASTERED\DarkSoulsRemastered.exe");
            ds2Wrapper = new DS2Wrapper(@"C:\Program Files (x86)\Steam\steamapps\common\Dark Souls II Scholar of the First Sin\Game\DarkSoulsII.exe");
            ds3Wrapper = new DS3Wrapper(@"C:\Program Files (x86)\Steam\steamapps\common\DARK SOULS III\Game\DarkSoulsIII.exe");

            dsrWrapper.OnModEventSet += RespondToDSREvent;
            ds2Wrapper.OnModEventSet += RespondToDS2Event;
            ds3Wrapper.OnModEventSet += RespondToDS3Event;

            dsrWrapper.Start();
            ds2Wrapper.Start();
            ds3Wrapper.Start();

            Thread.Sleep(1);

            ds2Wrapper.Pause();
            ds3Wrapper.Pause();

            while (true)
            {
                Thread.Sleep(10000);
            }

            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.Run(new Form1());
        }

        static void RespondToDSREvent(int eventId)
        {
            if (eventId == 71810001) // Warp to Cemetary of Ashes
            {
                dsrWrapper.SetEventFlag(71810001, false);
                dsrWrapper.Pause();
                ds3Wrapper.Resume();
                ds3Wrapper.Warp(4002951);
            }
            if (eventId == 71810002)
            {
                dsrWrapper.SetEventFlag(71810002, false);
                dsrWrapper.Pause();
                ds2Wrapper.Resume();
                ds2Wrapper.Warp(2650);
            }
        }

        static void RespondToDS2Event(int eventId)
        {
            if (eventId == 104901) // Warp to Northern Undead Asylum
            {
                ds2Wrapper.SetEventFlag(104901, false);
                ds2Wrapper.Pause();
                dsrWrapper.Resume();
                dsrWrapper.Warp(1812960);
            }
            if (eventId == 104902) // Warp to Cemetary of Ash
            {
                ds2Wrapper.SetEventFlag(104902, false);
                ds2Wrapper.Pause();
                ds3Wrapper.Resume();
                ds3Wrapper.Warp(4002951);
            }
        }

        static void RespondToDS3Event(int eventId)
        {
            if (eventId == 74000001) // Warp to Northern Undead Asylum
            {
                ds3Wrapper.SetEventFlag(74000001, false);
                ds3Wrapper.Pause();
                dsrWrapper.Resume();
                dsrWrapper.Warp(1812960);
            }
            if (eventId == 74000002) // Warp to Northern Undead Asylum
            {
                ds3Wrapper.SetEventFlag(74000002, false);
                ds3Wrapper.Pause();
                ds2Wrapper.Resume();
                ds2Wrapper.Warp(2650);
            }
        }
    }

    public delegate void EventFlagReactor(int eventId);
    public delegate void ItemReactor(int itemId);

    public class DSRWrapper : WindowsGameWrapper, IDisposable
    {
        public readonly IReadOnlyList<int> Events = new List<int>
        {
            71810001, // Warp to Cemetary of Ash
            71810002, // Warp to Fire Keepers' Dwelling
        };

        private DSRHook hook;
        private Thread itemWatchThread;
        private Thread eventWatchThread;

        private bool shutdown = false;
        private bool disposedValue;

        public override event EventFlagReactor? OnModEventSet;
        public override event ItemReactor? OnModItemPickUp;

        public DSRWrapper(string exePath) : base(exePath)
        {
            hook = new DSRHook(5000, 5000);
            itemWatchThread = new Thread(this.WatchItems);
            eventWatchThread = new Thread(this.WatchEvents);
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
            eventWatchThread.Start();
        }

        public void SetEventFlag(int flagId, bool active)
        {
            hook.WriteEventFlag(flagId, active);
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
                var modItems = inventory.Where(item => item.category == 8);
                foreach (var item in modItems)
                {
                    OnModItemPickUp?.Invoke((int)item.id);
                }
                Thread.Sleep(1000);
            }
        }

        private void WatchEvents()
        {
            while (!shutdown)
            {
                foreach (var eventId in Events)
                {
                    if (hook.ReadEventFlag(eventId))
                    {
                        OnModEventSet?.Invoke(eventId);
                    }
                }
                Thread.Sleep(1000);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    shutdown = true;
                    itemWatchThread.Join();
                    eventWatchThread.Join();
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

    public class DS2Wrapper : WindowsGameWrapper, IDisposable
    {
        public readonly IReadOnlyList<int> Events = new List<int>
        {
            104901, // Warp to Northern Undead Asylum
            104902, // Warp to Cemetary of Ash
        };

        private DS2SotFSHook hook;
        private Thread itemWatchThread;
        private Thread eventWatchThread;

        private bool shutdown = false;
        private bool disposedValue;

        public override event EventFlagReactor? OnModEventSet;
        public override event ItemReactor? OnModItemPickUp;

        public DS2Wrapper(string exePath) : base(exePath)
        {
            hook = new DS2SotFSHook(5000, 5000);
            itemWatchThread = new Thread(this.WatchItems);
            eventWatchThread = new Thread(this.WatchEvents);
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
            eventWatchThread.Start();
        }

        public void SetEventFlag(int flagId, bool active)
        {
            hook.WriteEventFlag(flagId, active);
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
                var modItems = inventory.Where(item => item.id >= 0x60000000);
                foreach (var item in modItems)
                {
                    OnModItemPickUp?.Invoke((int)item.id);
                }
                Thread.Sleep(1000);
            }
        }

        private void WatchEvents()
        {
            while (!shutdown)
            {
                foreach (var eventId in Events)
                {
                    if (hook.ReadEventFlag(eventId))
                    {
                        OnModEventSet?.Invoke(eventId);
                    }
                }
                Thread.Sleep(1000);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    shutdown = true;
                    itemWatchThread.Join();
                    eventWatchThread.Join();
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

    public class DS3Wrapper : WindowsGameWrapper, IDisposable
    {
        public readonly IReadOnlyList<int> Events = new List<int>
        {
            74000001, // Warp to Northern Undead Asylum
            74000002, // Warp to Fire Keepers' Dwelling
        };

        private DS3Hook hook;
        private Thread itemWatchThread;
        private Thread eventWatchThread;

        private bool shutdown = false;
        private bool disposedValue;

        public override event EventFlagReactor? OnModEventSet;
        public override event ItemReactor? OnModItemPickUp;

        public DS3Wrapper(string exePath) : base(exePath)
        {
            hook = new DS3Hook(5000, 5000);
            itemWatchThread = new Thread(this.WatchItems);
            eventWatchThread = new Thread(this.WatchEvents);
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
            eventWatchThread.Start();
        }

        public void SetEventFlag(int flagId, bool active)
        {
            hook.WriteEventFlag(flagId, active);
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
                var modItems = inventory.Where(item => item.id >= 0x80000000);
                foreach (var item in modItems)
                {
                    OnModItemPickUp?.Invoke((int)item.id);
                }
                Thread.Sleep(1000);
            }
        }

        private void WatchEvents()
        {
            while (!shutdown)
            {
                foreach (var eventId in Events)
                {
                    if (hook.ReadEventFlag(eventId))
                    {
                        OnModEventSet?.Invoke(eventId);
                    }
                }
                Thread.Sleep(1000);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    shutdown = true;
                    itemWatchThread.Join();
                    eventWatchThread.Join();
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