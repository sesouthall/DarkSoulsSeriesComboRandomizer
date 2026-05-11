using DarkSoulsSeriesComboRandomizer.DarkSouls2SotFS;
using DarkSoulsSeriesComboRandomizer.DarkSouls3;
using DarkSoulsSeriesComboRandomizer.DarkSoulsRemastered;
using System.IO;

namespace DarkSoulsSeriesComboRandomizer
{
    public record RandomizerOptions(int Seed, string SaveName, bool AllowFirelinkRoofSkip = false);

    internal class ModInstaller(string dsrRoot, string ds2Root, string ds3Root, RandomizerOptions options, List<IModdedFile> filesToModify)
    {
        private bool disposedValue;

        private static readonly string appDataFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "DarkSoulsSeriesComboRandomizer");
        private static readonly string backupFolderPath = Path.Combine(appDataFolderPath, "BackupVanillaFiles");
        public string SaveFolderPath => Path.Combine(appDataFolderPath, options.SaveName);

        private static readonly string DS1SaveFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "NBGI", "DARK SOULS REMASTERED");
        private const string DS1SaveFileName = "DRAKS0005.sl2";

        private static readonly string DS2SaveFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "DarkSoulsII");
        private const string DS2SaveFileName = "DS2SOFS0000.sl2";

        private static readonly string DS3SaveFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "DarkSoulsIII");
        private const string DS3SaveFileName = "DS30000.sl2";

        private static readonly string BonfireMappingsFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "DarkSoulsSeriesComboRandomizer", "BonfireMappings.txt");

        public static ModInstaller New(string dsrRoot, string ds2Root, string ds3Root, RandomizerOptions options)
        {
            var filesToModify = new List<IModdedFile>();
            var saveFolder = Path.Combine(appDataFolderPath, options.SaveName);

            // DSR uses direct file replacement. Just copy things over and save anything we're replacing.
            var ds1ModdedFilePath = Path.Combine("PreModdedGameFiles", "DSR");
            foreach (var moddedFile in Directory.GetFiles(ds1ModdedFilePath, "*", SearchOption.AllDirectories))
            {
                var vanillaFile = moddedFile.Replace(ds1ModdedFilePath, dsrRoot);
                if (moddedFile.Contains("dinput8.dll") || moddedFile.Contains("steam_appid.txt"))
                {
                    filesToModify.Add(new AddedFile(vanillaFile, moddedFile));
                }
                else
                {
                    var backupFile = moddedFile.Replace(ds1ModdedFilePath, backupFolderPath);
                    filesToModify.Add(new BackedUpFile(vanillaFile, backupFile, moddedFile));
                }
            }
            // Since DSR doesn't use a mod loader, we have to manually backup and restore the regulation file
            string vanillaRegulationFile = Path.Combine(dsrRoot, "param", "GameParam", "GameParam.parambnd.dcx");
            string backupRegulationFile = Path.Combine(backupFolderPath, "GameParam.parambnd.dcx");
            string moddedRegulationFile = Path.Combine(saveFolder, "GameParam.parambnd.dcx");
            filesToModify.Add(new BackedUpFile(vanillaRegulationFile, backupRegulationFile, moddedRegulationFile));

            // Back up the DS1 save
            filesToModify.Add(new SaveFile(DS1SaveFolder, backupFolderPath, saveFolder, DS1SaveFileName));

            // DS2 uses ModEngine 1. Copy everything over. ModEngine will handle replacing files.
            var ds2ModdedFilePath = Path.Combine("PreModdedGameFiles", "DS2S");
            foreach (var moddedFile in Directory.GetFiles(ds2ModdedFilePath, "*", SearchOption.AllDirectories))
            {
                var vanillaFile = moddedFile.Replace(ds2ModdedFilePath, ds2Root);
                filesToModify.Add(new AddedFile(vanillaFile, moddedFile));
            }
            // Also add the regulation file to the modengine folder
            vanillaRegulationFile = Path.Combine(ds2Root, "ComboRandomizer", "enc_regulation.bnd.dcx");
            moddedRegulationFile = Path.Combine(saveFolder, "enc_regulation.bnd.dcx");
            filesToModify.Add(new AddedFile(vanillaRegulationFile, moddedRegulationFile));

            // Back up the DS2 save
            filesToModify.Add(new SaveFile(DS2SaveFolder, backupFolderPath, saveFolder, DS2SaveFileName));

            // DS3 uses ModEngine 1 with a downpatched exe.
            // Copy the exe with backup. Everything else is new and will be injected by ModEngine.
            var ds3ModdedFilePath = Path.Combine("PreModdedGameFiles", "DS3");
            foreach (var moddedFile in Directory.GetFiles(ds3ModdedFilePath, "*", SearchOption.AllDirectories))
            {
                var vanillaFile = moddedFile.Replace(ds3ModdedFilePath, ds3Root);
                if (moddedFile.Contains("DarkSoulsIII.exe"))
                {
                    var backupFile = moddedFile.Replace(ds3ModdedFilePath, backupFolderPath);
                    filesToModify.Add(new BackedUpFile(vanillaFile, backupFile, moddedFile));
                }
                else
                {
                    filesToModify.Add(new AddedFile(vanillaFile, moddedFile));
                }
            }
            // Also add the regulation file to the modengine folder
            vanillaRegulationFile = Path.Combine(ds3Root, "ComboRandomizer", "Data0.bdt");
            moddedRegulationFile = Path.Combine(saveFolder, "Data0.bdt");
            filesToModify.Add(new AddedFile(vanillaRegulationFile, moddedRegulationFile));

            // Back up the DS3 save
            filesToModify.Add(new SaveFile(DS3SaveFolder, backupFolderPath, saveFolder, DS3SaveFileName));

            // Treat the BonfireMappingFile as an added file
            filesToModify.Add(new AddedFile(BonfireMappingsFile, BonfireMappingsFile.Replace(appDataFolderPath, saveFolder)));

            return new ModInstaller(dsrRoot, ds2Root, ds3Root, options, filesToModify);
        }

        public void CreateRandomizedRegulationFilesIfNeeded(CrossGameMappings crossGameMappings)
        {
            if (Directory.Exists(SaveFolderPath))
            {
                // This randomization has already been generated. No work needed.
                return;
            }

            Directory.CreateDirectory(SaveFolderPath);

            DSRFiles dsrGameFiles = new(dsrRoot);
            var dsrMaps = MapFactory.DSRMaps(dsrGameFiles, crossGameMappings, [.. MapData.DS1StartingItemLots, MapData.DS1EstusFlaskLot]);
            var dsrKeys = Key.ConstructDS1Keys(dsrMaps);
            DS2SotFSFiles ds2GameFiles = new(ds2Root);
            var ds2Maps = MapFactory.DS2SotFSMaps(ds2GameFiles, crossGameMappings, [MapData.DS2EstusFlaskLot]);
            var ds2Keys = Key.ConstructDS2Keys(ds2Maps);
            DS3Files ds3GameFiles = new(ds3Root);
            var ds3Maps = MapFactory.DS3Maps(ds3GameFiles, crossGameMappings, [MapData.DS3AshenEstusFlaskLot]);
            var ds3Keys = Key.ConstructDS3Keys(ds3Maps);

            CreateBonfireMappings();
            var bonfireMappings = BonfireTriple.ParseBonfireMappings(BonfireMappingsFile);
            Key coiledSword = ds3Keys.Single(key => key.Item == Key.CoiledSword);
            Map.LoadCrossGameWarps(bonfireMappings, dsrMaps, ds2Maps, ds3Maps, coiledSword);

            Map.HandleDS3FirelinkRoofSkip(options.AllowFirelinkRoofSkip, ds3Maps);

            var allMaps = dsrMaps.Concat(ds2Maps).Concat(ds3Maps).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            var allKeys = dsrKeys.Concat(ds2Keys).Concat(ds3Keys).ToList();

            ItemRandomizer.Randomize(allMaps[MapName.DS1StartingCell], allMaps, allKeys, new Random(options.Seed));

            dsrGameFiles.SaveItemLotChanges(SaveFolderPath);
            ds2GameFiles.SaveItemLotChanges(SaveFolderPath);
            ds3GameFiles.SaveItemLotChanges(SaveFolderPath);

            var itemLookupService = new AggregateItemNameLookupService(
                [
                    new DSRItemNameLookupService(dsrGameFiles),
                    new DS2SotFSItemNameLookupService(ds2GameFiles),
                    new DS3ItemNameLookupService(ds3GameFiles)
                ]);

            var hintsLines = allMaps.Values.SelectMany(map => map.GetHintsLines(itemLookupService));
            File.WriteAllLines(Path.Combine(SaveFolderPath, "Hints"), hintsLines);

            File.WriteAllText(Path.Combine(SaveFolderPath, "seed.txt"), options.Seed.ToString());
        }

        private void CreateBonfireMappings()
        {
            var runSpecificBonfireMappingFile = BonfireMappingsFile.Replace(appDataFolderPath, SaveFolderPath);
            if (!File.Exists(runSpecificBonfireMappingFile))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(runSpecificBonfireMappingFile)!);
                File.Copy(Path.Combine("ConfigFiles", "BonfireMappings.txt"), runSpecificBonfireMappingFile);
            }
        }

        public List<string> InstallChanges()
        {
            if (Directory.GetFiles(backupFolderPath, "*", SearchOption.AllDirectories).Length > 0)
            {
                // The last run failed to clean up. Do it now.
                RevertChanges();
            }

            var errors = new List<string>();
            foreach (var modFile in filesToModify)
            {
                if (!modFile.TryUpdate(out var errorMessage))
                {
                    errors.Add(errorMessage);
                }    
            }
            return errors;
        }

        public SoulsGame GetLastGame()
        {
            var lastGame = SoulsGame.DSR;
            if (File.Exists(Path.Combine(SaveFolderPath, "LastGame.txt")))
            {
                var lastGameString = File.ReadAllText(Path.Combine(SaveFolderPath, "LastGame.txt")).Trim();
                lastGame = Enum.Parse<SoulsGame>(lastGameString);
            }
            return lastGame;
        }

        public void SaveLastGame(SoulsGame activeGame)
        {
            File.WriteAllText(Path.Combine(SaveFolderPath, "LastGame.txt"), activeGame.ToString());
        }

        public void RevertChanges()
        {
            foreach (var modFile in filesToModify)
            {
                modFile.RevertUpdate();
            }
        }
    }
}