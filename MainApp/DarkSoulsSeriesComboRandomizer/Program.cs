using System.IO.Pipes;

namespace DarkSoulsSeriesComboRandomizer
{
    internal static class Program
    {
        private static DSRWrapper? dsrWrapper;
        private static DS2Wrapper? ds2Wrapper;
        private static DS3Wrapper? ds3Wrapper;

        private static Dictionary<int, SoulsItem> dsrMapping;
        private static Dictionary<int, SoulsItem> ds2Mapping;
        private static Dictionary<int, SoulsItem> ds3Mapping;

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

            dsrMapping = SoulsItemCsvParser.ParseFile(@"ConfigFiles\DSR_injected_items.csv");
            ds2Mapping = SoulsItemCsvParser.ParseFile(@"ConfigFiles\DS2S_injected_items.csv");
            ds3Mapping = SoulsItemCsvParser.ParseFile(@"ConfigFiles\DS3_injected_items.csv");

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

            dsrWrapper.OnModItemPickUp += SendItemToCorrectGame;
            ds2Wrapper.OnModItemPickUp += SendItemToCorrectGame;
            ds3Wrapper.OnModItemPickUp += SendItemToCorrectGame;

            ds3Wrapper.Resume();

            while (true)
            {
                //dsrWrapper.GiveSunlightMedals();
                //ds2Wrapper.GiveSunlightMedals();
                ds3Wrapper.GiveSunlightMedals();
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

        static void SendItemToCorrectGame(SoulsGame sourceGame, int itemId, int quantity)
        {
            var originalItem = sourceGame switch
            {
                SoulsGame.DSR => dsrMapping[itemId],
                SoulsGame.DS2 => ds2Mapping[itemId],
                SoulsGame.DS3 => ds3Mapping[itemId],
                _ => throw new NotImplementedException()
            };

            switch (originalItem.Game)
            {
                case SoulsGame.DSR:
                    dsrWrapper.GiveItem(originalItem.ItemType, originalItem.OriginalId, quantity);
                    break;
                case SoulsGame.DS2:
                    ds2Wrapper.GiveItem(originalItem.OriginalId, (short)quantity);
                    break;
                case SoulsGame.DS3:
                    ds3Wrapper.GiveItem(originalItem.ItemType, originalItem.OriginalId, quantity);
                    break;
                default:
                    throw new NotImplementedException();
            };
        }
    }

    public delegate void ItemReactor(SoulsGame game, int itemId, int quantity);
}