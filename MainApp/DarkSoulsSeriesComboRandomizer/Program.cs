using DarkSoulsSeriesComboRandomizer.DarkSouls2SotFS;
using DarkSoulsSeriesComboRandomizer.DarkSouls3;
using DarkSoulsSeriesComboRandomizer.DarkSoulsRemastered;
using System.IO.Pipes;

namespace DarkSoulsSeriesComboRandomizer
{
    internal static partial class Program
    {
        private static DSRWrapper? dsrWrapper;
        private static DS2Wrapper? ds2Wrapper;
        private static DS3Wrapper? ds3Wrapper;

        private static Dictionary<int, SoulsItem>? dsrMapping;
        private static Dictionary<int, SoulsItem>? ds2Mapping;
        private static Dictionary<int, SoulsItem>? ds3Mapping;

        private static string dsrRoot = @"C:\Program Files (x86)\Steam\steamapps\common\DARK SOULS REMASTERED";
        private static string ds2Root = @"C:\Program Files (x86)\Steam\steamapps\common\Dark Souls II Scholar of the First Sin\Game";
        private static string ds3Root = @"C:\Program Files (x86)\Steam\steamapps\common\DARK SOULS III\Game";

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            var bonfireMappingsFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "DarkSoulsSeriesComboRandomizer", "BonfireMappings.txt");
            if (!File.Exists(bonfireMappingsFile))
            {
                File.WriteAllText(bonfireMappingsFile, "Undead Asylum Courtyard,Fire Keepers' Dwelling,Cemetery of Ash");
            }

            var bonfireMappings = ParseBonfireMappings(bonfireMappingsFile);
            
            dsrMapping = SoulsItemCsvParser.ParseFile(@"ConfigFiles\DSR_injected_items.csv");
            ds2Mapping = SoulsItemCsvParser.ParseFile(@"ConfigFiles\DS2S_injected_items.csv");
            ds3Mapping = SoulsItemCsvParser.ParseFile(@"ConfigFiles\DS3_injected_items.csv");

            DSRItemLot.Initialize(dsrMapping);
            DS2SotFSItemLot.Initialize(ds2Mapping);
            DS3ItemLot.Initialize(ds3Mapping);

            var dsrItems = new DSRItemLots(dsrRoot, dsrMapping);
            var ds2Items = new DS2SotFSItemLots(ds2Root, ds2Mapping);
            var ds3Items = new DS3ItemLots(ds3Root, ds3Mapping);

            dsrItems.Load();
            ds2Items.Load();
            ds3Items.Load();

            foreach (var mapping in bonfireMappings)
            {
                var dsrMap = Map.DSRMaps.Single(map => map.Bonfires.Contains(mapping.DS1Bonfire));
                var ds2Map = Map.DS2Maps.Single(map => map.Bonfires.Contains(mapping.DS2Bonfire));
                var ds3Map = Map.DS3Maps.Single(map => map.Bonfires.Contains(mapping.DS3Bonfire));

                dsrMap.connectedMaps.Add(ds2Map);
                dsrMap.connectedMaps.Add(ds3Map);
                ds2Map.connectedMaps.Add(dsrMap);
                ds2Map.connectedMaps.Add(ds3Map);
                ds3Map.connectedMaps.Add(dsrMap);
                ds3Map.connectedMaps.Add(ds2Map);
            }

            var firelinkTower = Map.DS3Maps.Single(map => map.FriendlyName == "Firelink Tower");
            var firelinkRoof = Map.DS3Maps.Single(map => map.FriendlyName == "Firelink Roof");
            firelinkTower.connectedMaps.Add(firelinkRoof);

            ItemRandomizer.Randomize(Map.DSRMaps.Single(map => map.FriendlyName == "DS1 Starting Cell"), Map.AllMaps, Key.AllKeys, new List<(int, SoulsGame)>(), new Random(0));

            dsrItems.Save();
            ds2Items.Save();
            ds3Items.Save();

            StartPipeServers();

            dsrWrapper = new DSRWrapper($@"{dsrRoot}\DarkSoulsRemastered.exe");
            ds2Wrapper = new DS2Wrapper($@"{ds2Root}\DarkSoulsII.exe");
            ds3Wrapper = new DS3Wrapper($@"{ds3Root}\DarkSoulsIII.exe");

            dsrWrapper.Start();
            Thread.Sleep(15000);
            dsrWrapper.Pause();
            ds2Wrapper.Start();
            Thread.Sleep(15000);
            ds2Wrapper.Pause();
            ds3Wrapper.Start();
            Thread.Sleep(15000);
            ds3Wrapper.Pause();

            dsrWrapper.OnModItemPickUp += SendItemToCorrectGame;
            ds2Wrapper.OnModItemPickUp += SendItemToCorrectGame;
            ds3Wrapper.OnModItemPickUp += SendItemToCorrectGame;

            dsrWrapper.Resume();

            var exit = false;
            while (!exit)
            {
                Thread.Sleep(10000);
            }

            dsrWrapper.OnModItemPickUp -= SendItemToCorrectGame;
            ds2Wrapper.OnModItemPickUp -= SendItemToCorrectGame;
            ds3Wrapper.OnModItemPickUp -= SendItemToCorrectGame;

            dsrWrapper.Resume();
            ds2Wrapper.Resume();
            ds3Wrapper.Resume();

            dsrWrapper.Dispose();
            ds2Wrapper.Dispose();
            ds3Wrapper.Dispose();

            dsrItems.Revert();
            ds2Items.Revert();
            ds3Items.Revert();
            
            return;

            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.Run(new Form1());
        }

        private static List<BonfireTriple> ParseBonfireMappings(string mappingFile)
        {
            var lines = File.ReadAllLines(mappingFile);
            var result = new List<BonfireTriple>();
            foreach (string line in lines)
            {
                var bonfireNames = line.Split(',');
                if (bonfireNames.Length != 3)
                {
                    throw new InvalidDataException($"Line '{line}' is expected to contain three bonfire names, but does not");
                }
                result.Add(new BonfireTriple(bonfireNames[0], bonfireNames[1], bonfireNames[2]));
            }
            return result;
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
                SoulsGame.DS2S => ds2Mapping[itemId],
                SoulsGame.DS3 => ds3Mapping[itemId],
                _ => throw new NotImplementedException()
            };

            switch (originalItem.Game)
            {
                case SoulsGame.DSR:
                    dsrWrapper.GiveItem(originalItem.ItemType, originalItem.OriginalId, quantity);
                    break;
                case SoulsGame.DS2S:
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