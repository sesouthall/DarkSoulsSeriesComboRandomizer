using DarkSoulsSeriesComboRandomizer.DarkSouls2SotFS;
using DarkSoulsSeriesComboRandomizer.DarkSouls3;
using DarkSoulsSeriesComboRandomizer.DarkSoulsRemastered;
using System.IO.Pipes;

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
            var mappingsFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "DarkSoulsSeriesComboRandomizer", "BonfireMappings.txt");
            if (!File.Exists(mappingsFile))
            {
                File.WriteAllText(mappingsFile, "Undead Asylum Courtyard,Fire Keepers' Dwelling,Cemetery of Ash");
            }

            StartPipeServers();

            dsrWrapper = new DSRWrapper(@"C:\Program Files (x86)\Steam\steamapps\common\DARK SOULS REMASTERED\DarkSoulsRemastered.exe");
            ds2Wrapper = new DS2Wrapper(@"C:\Program Files (x86)\Steam\steamapps\common\Dark Souls II Scholar of the First Sin\Game\DarkSoulsII.exe");
            ds3Wrapper = new DS3Wrapper(@"C:\Program Files (x86)\Steam\steamapps\common\DARK SOULS III\Game\DarkSoulsIII.exe");

            dsrWrapper.Start();
            Thread.Sleep(5000);
            dsrWrapper.Pause();
            ds2Wrapper.Start();
            Thread.Sleep(5000);
            ds2Wrapper.Pause();
            ds3Wrapper.Start();
            Thread.Sleep(5000);
            ds3Wrapper.Pause();

            dsrWrapper.Resume();

            while (true)
            {
                Thread.Sleep(10000);
            }

            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.Run(new Form1());
        }

        static void StartPipeServers()
        {
            Task.Factory.StartNew(() =>
            {
                var server = new NamedPipeServerStream("DarkSoulsSeriesComboRandomizerDS1", PipeDirection.InOut, NamedPipeServerStream.MaxAllowedServerInstances, PipeTransmissionMode.Message);
                server.WaitForConnection();
                StreamReader reader = new StreamReader(server);
                while (true)
                {
                    var line = reader.ReadLine()?.Trim()?.Trim('\0', '\v', '\b');
                    if (int.TryParse(line, out var destinationBonfire))
                    {
                        dsrWrapper.Pause();

                        if (destinationBonfire >= 1002960 && destinationBonfire <= 1812961)
                        {
                            dsrWrapper.Resume();
                            dsrWrapper.Warp(destinationBonfire);
                        }
                        else if (destinationBonfire >= 2650 && destinationBonfire <= 37685)
                        {
                            ds2Wrapper.Resume();
                            ds2Wrapper.Warp(destinationBonfire);
                        }
                        else if (destinationBonfire >= 3002950 && destinationBonfire <= 5112951)
                        {
                            ds3Wrapper.Resume();
                            ds3Wrapper.Warp(destinationBonfire);
                        }
                    }
                }
            });

            Task.Factory.StartNew(() =>
            {
                var server = new NamedPipeServerStream("DarkSoulsSeriesComboRandomizerDS2", PipeDirection.InOut, NamedPipeServerStream.MaxAllowedServerInstances, PipeTransmissionMode.Message);
                server.WaitForConnection();
                StreamReader reader = new StreamReader(server);
                while (true)
                {
                    var line = reader.ReadLine()?.Trim()?.Trim('\0', '\v', '\b');
                    if (int.TryParse(line, out var destinationBonfire))
                    {
                        ds2Wrapper.Pause();

                        if (destinationBonfire >= 1002960 && destinationBonfire <= 1812961)
                        {
                            dsrWrapper.Resume();
                            dsrWrapper.Warp(destinationBonfire);
                        }
                        else if (destinationBonfire >= 2650 && destinationBonfire <= 37685)
                        {
                            ds2Wrapper.Resume();
                            ds2Wrapper.Warp(destinationBonfire);
                        }
                        else if (destinationBonfire >= 3002950 && destinationBonfire <= 5112951)
                        {
                            ds3Wrapper.Resume();
                            ds3Wrapper.Warp(destinationBonfire);
                        }
                    }
                }
            });

            Task.Factory.StartNew(() =>
            {
                var server = new NamedPipeServerStream("DarkSoulsSeriesComboRandomizerDS3", PipeDirection.InOut, NamedPipeServerStream.MaxAllowedServerInstances, PipeTransmissionMode.Message);
                server.WaitForConnection();
                StreamReader reader = new StreamReader(server);
                while (true)
                {
                    var line = reader.ReadLine()?.Trim()?.Trim('\0', '\v', '\b');
                    if (int.TryParse(line, out var destinationBonfire))
                    {
                        Thread.Sleep(500);
                        ds3Wrapper.Pause();

                        if (destinationBonfire >= 1002960 && destinationBonfire <= 1812961)
                        {
                            dsrWrapper.Resume();
                            dsrWrapper.Warp(destinationBonfire);
                        }
                        else if (destinationBonfire >= 2650 && destinationBonfire <= 37685)
                        {
                            ds2Wrapper.Resume();
                            ds2Wrapper.Warp(destinationBonfire);
                        }
                        else if (destinationBonfire >= 3002950 && destinationBonfire <= 5112951)
                        {
                            ds3Wrapper.Resume();
                            ds3Wrapper.Warp(destinationBonfire);
                        }
                    }
                }
            });
        }
    }

    public delegate void EventFlagReactor(int eventId);
    public delegate void ItemReactor(int itemId);

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
                var modItems = inventory.Where(item => item.category == 8);
                foreach (var item in modItems)
                {
                    OnModItemPickUp?.Invoke((int)item.id);
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
                var modItems = inventory.Where(item => item.id >= 0x60000000);
                foreach (var item in modItems)
                {
                    OnModItemPickUp?.Invoke((int)item.id);
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
                var modItems = inventory.Where(item => item.id >= 0x80000000);
                foreach (var item in modItems)
                {
                    OnModItemPickUp?.Invoke((int)item.id);
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