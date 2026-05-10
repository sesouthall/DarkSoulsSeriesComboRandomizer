using DarkSoulsSeriesComboRandomizer.DarkSouls2SotFS;
using DarkSoulsSeriesComboRandomizer.DarkSouls3;
using DarkSoulsSeriesComboRandomizer.DarkSoulsRemastered;
using System.IO;

namespace DarkSoulsSeriesComboRandomizer
{
    public record RandomizerOptions(int Seed, string SaveName, bool AllowFirelinkRoofSkip = false);

    internal class ModInstaller(string dsrRoot, string ds2Root, string ds3Root, RandomizerOptions options) : IDisposable
    {
        private bool disposedValue;

        private static readonly string backupFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "DarkSoulsSeriesComboRandomizer", "BackupVanillaFiles");
        public string SaveFolderPath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "DarkSoulsSeriesComboRandomizer", options.SaveName);
        private string DSRRegulationFilePath => Path.Combine(dsrRoot, "param", "GameParam", "GameParam.parambnd.dcx");

        private static readonly string DS1SaveFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "NBGI", "DARK SOULS REMASTERED");
        private const string DS1SaveFileName = "DRAKS0005.sl2";

        private static readonly string DS2SaveFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "DarkSoulsII");
        private const string DS2SaveFileName = "DS2SOFS0000.sl2";

        private static readonly string DS3SaveFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "DarkSoulsIII");
        private const string DS3SaveFileName = "DS30000.sl2";

        private static readonly string BonfireMappingsFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "DarkSoulsSeriesComboRandomizer", "BonfireMappings.txt");

        public void InstallStaticChanges()
        {
            if (Directory.GetFiles(backupFolderPath, "*", SearchOption.AllDirectories).Length > 0)
            {
                // Cleanup didn't happen, or failed last time
                RevertChanges();
            }

            CreateBonfireMappings();

            // DSR uses direct file replacement. Just copy things over and save anything we're replacing.
            var ds1ModdedFilePath = Path.Combine("PreModdedGameFiles", "DSR");
            foreach (var file in Directory.GetFiles(ds1ModdedFilePath, "*", SearchOption.AllDirectories))
            {
                var vanillaFile = file.Replace(ds1ModdedFilePath, dsrRoot);
                var backupFile = file.Replace(ds1ModdedFilePath, backupFolderPath);
                if (File.Exists(vanillaFile))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(backupFile)!);
                    File.Move(vanillaFile, backupFile);
                }
                File.Copy(file, vanillaFile);
            }
            // Since DSR doesn't use a mod loader, we have to manually backup and restore the regulation file
            File.Copy(DSRRegulationFilePath, Path.Combine(backupFolderPath, "GameParam.parambnd.dcx"));

            // DS2 uses ModEngine 1. Copy everything over. ModEngine will handle replacing files.
            var ds2ModdedFilePath = Path.Combine("PreModdedGameFiles", "DS2S");
            foreach (var file in Directory.GetFiles(ds2ModdedFilePath, "*", SearchOption.AllDirectories))
            {
                var destinationPath = file.Replace(ds2ModdedFilePath, ds2Root);
                Directory.CreateDirectory(Path.GetDirectoryName(destinationPath)!);
                File.Copy(file, destinationPath);
            }

            // DS3 uses ModEngine 1 with a downpatched exe.
            // Copy the exe with backup. Everything else is new and will be injected by ModEngine.
            var ds3ModdedFilePath = Path.Combine("PreModdedGameFiles", "DS3");
            foreach (var file in Directory.GetFiles(ds3ModdedFilePath, "*", SearchOption.AllDirectories))
            {
                var destinationPath = file.Replace(ds3ModdedFilePath, ds3Root);
                var backupPath = file.Replace(ds3ModdedFilePath, backupFolderPath);
                if (File.Exists(destinationPath)) // Should be only DarkSoulsIII.exe
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(backupPath)!);
                    File.Move(destinationPath, backupPath);
                }
                Directory.CreateDirectory(Path.GetDirectoryName(destinationPath)!);
                File.Copy(file, destinationPath);
            }
        }

        private static void CreateBonfireMappings()
        {
            if (!File.Exists(BonfireMappingsFile))
            {
                File.Copy(Path.Combine("ConfigFiles", "BonfireMappings.txt"), BonfireMappingsFile);
            }
        }

        public void CreateRandomizedRegulationFilesIfNeeded(CrossGameMappings crossGameMappings)
        {
            if (Directory.Exists(SaveFolderPath))
            {
                // This randomization has already been generated.
                // No work needed.
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

        public void InstallRegulationFiles()
        {
            File.Copy(Path.Combine(SaveFolderPath, "GameParam.parambnd.dcx"), DSRRegulationFilePath, true);

            File.Copy(Path.Combine(SaveFolderPath, "enc_regulation.bnd.dcx"), Path.Combine(ds2Root, "ComboRandomizer", "enc_regulation.bnd.dcx"), true);

            File.Copy(Path.Combine(SaveFolderPath, "Data0.bdt"), Path.Combine(ds3Root, "ComboRandomizer", "Data0.bdt"), true);
        }

        public SuccessOrError InstallModSaveFiles()
        {
            var successOrError = new SuccessOrError();

            BackupAndReplaceSingleSaveFile(DS1SaveFolder, DS1SaveFileName, "REMASTERED", successOrError);

            if (!successOrError.Succeeded)
            {
                return successOrError;
            }

            BackupAndReplaceSingleSaveFile(DS2SaveFolder, DS2SaveFileName, "2", successOrError);

            if (!successOrError.Succeeded)
            {
                RestoreSingleSaveFile(DS1SaveFolder, DS1SaveFileName, "REMASTERED", successOrError);
                return successOrError;
            }

            BackupAndReplaceSingleSaveFile(DS3SaveFolder, DS3SaveFileName, "3", successOrError);

            if (!successOrError.Succeeded)
            {
                RestoreSingleSaveFile(DS1SaveFolder, DS1SaveFileName, "REMASTERED", successOrError);
                RestoreSingleSaveFile(DS2SaveFolder, DS2SaveFileName, "2", successOrError);
                return successOrError;
            }

            return successOrError;
        }

        private void BackupAndReplaceSingleSaveFile(string vanillaSaveFolder, string saveFileName, string gameSuffix, SuccessOrError currentStatus)
        {
            var possibleSaveFiles = Directory.GetFiles(vanillaSaveFolder, saveFileName, SearchOption.AllDirectories).ToList();
            if (possibleSaveFiles.Count > 1)
            {
                currentStatus.AddError($@"There is more than one Dark Souls {gameSuffix} save file ({saveFileName}) in {vanillaSaveFolder}.
Please move any backups to another folder so the randomizer can clearly identify the real one.");
                return;
            }
            else if (possibleSaveFiles.Count == 1)
            {
                var saveFile = possibleSaveFiles.Single();
                var backupFile = saveFile.Replace(vanillaSaveFolder, backupFolderPath);
                if (File.Exists(backupFile))
                {
                    currentStatus.AddError($@"There is already a save for Dark Souls {gameSuffix} backed up in {backupFolderPath}.
This probably means the randomizer crashed and didn't restore things correctly.
Please check the save files and manully restore them to the correct locations.
The randomizer's save belongs here: {saveFile.Replace(vanillaSaveFolder, SaveFolderPath)}
The vanilla save belongs here: {saveFile}
The backup folder should not have any files named {saveFileName}");
                    return;
                }
                Directory.CreateDirectory(Path.GetDirectoryName(backupFile)!);
                File.Move(saveFile, backupFile);
            }
            // If there are no save files, there's nothing to back up

            var possibleRandomizedSaves = Directory.GetFiles(SaveFolderPath, saveFileName, SearchOption.AllDirectories).ToList();
            if (possibleRandomizedSaves.Count > 1) // This should only happen if the user edits the app's save folders
            {
                currentStatus.AddError($@"There is more than one Dark Souls {gameSuffix} save file associated with this randomization.
Please check {SaveFolderPath} and remove any extra files named {saveFileName}.");
                return;
            }
            else if (possibleRandomizedSaves.Count == 1)
            {
                var randomizedSaveFile = possibleRandomizedSaves.Single();
                var vanillaLocation = randomizedSaveFile.Replace(SaveFolderPath, vanillaSaveFolder);
                File.Move(randomizedSaveFile, vanillaLocation);
            }
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
            // DSR - Restore backups for any files replaced. Delete the others.
            var ds1ModdedFilePath = Path.Combine("PreModdedGameFiles", "DSR");
            foreach (var file in Directory.GetFiles(ds1ModdedFilePath, "*", SearchOption.AllDirectories))
            {
                var vanillaFile = file.Replace(ds1ModdedFilePath, dsrRoot);
                var backupFile = file.Replace(ds1ModdedFilePath, backupFolderPath);
                File.Delete(vanillaFile);
                if (File.Exists(backupFile))
                {
                    File.Move(backupFile, vanillaFile);
                }
            }
            // Also restore the regulation file, since it isn't handled with the rest of them.
            File.Delete(DSRRegulationFilePath);
            File.Move(Path.Combine(backupFolderPath, "GameParam.parambnd.dcx"), DSRRegulationFilePath);

            // DS2 - No files got replaced, just delete them.
            var ds2ModdedFilePath = Path.Combine("PreModdedGameFiles", "DS2S");
            foreach (var file in Directory.GetFiles(ds2ModdedFilePath, "*", SearchOption.AllDirectories))
            {
                var destinationPath = file.Replace(ds2ModdedFilePath, ds2Root);
                File.Delete(destinationPath);
            }

            // DS3 - Restore DarkSoulsIII.exe, delete everything else
            var ds3ModdedFilePath = Path.Combine("PreModdedGameFiles", "DS3");
            foreach (var file in Directory.GetFiles(ds3ModdedFilePath, "*", SearchOption.AllDirectories))
            {
                var vanillaFile = file.Replace(ds3ModdedFilePath, ds3Root);
                var backupFile = file.Replace(ds3ModdedFilePath, backupFolderPath);
                File.Delete(vanillaFile);
                if (File.Exists(backupFile))
                {
                    File.Move(backupFile, vanillaFile);
                }
            }
        }

        public SuccessOrError RestoreVanillaSaveFiles()
        {
            var successOrError = new SuccessOrError();

            RestoreSingleSaveFile(DS1SaveFolder, DS1SaveFileName, "REMASTERED", successOrError);
            RestoreSingleSaveFile(DS2SaveFolder, DS2SaveFileName, "2", successOrError);
            RestoreSingleSaveFile(DS3SaveFolder, DS3SaveFileName, "3", successOrError);

            return successOrError;
        }

        private void RestoreSingleSaveFile(string vanillaSaveFolder, string saveFileName, string gameSuffix, SuccessOrError currentState)
        {
            var possibleRandomizedSaveFiles = Directory.GetFiles(vanillaSaveFolder, saveFileName, SearchOption.AllDirectories).ToList();
            if (possibleRandomizedSaveFiles.Count > 1) // The user added a save file while the mod was running. Don't touch anything, just bail with a warining.
            {
                currentState.AddError($@"There is more than one Dark Souls {gameSuffix} save file in {vanillaSaveFolder}.
Since I can't tell which one, if any, is associated with the randomizer, I'm leaving them alone.
You can find backups of unrandomized saves in {backupFolderPath}.
If the current save is from a randomized run and you'd like to keep it, the randomizer's save should go in {SaveFolderPath}.");
                return;
            }
            else if (possibleRandomizedSaveFiles.Count == 1)
            {
                var randomizedSaveFileInVanillaSaveFolder = possibleRandomizedSaveFiles.Single();
                var randomizedSaveFileInRandomizerSaveFolder = randomizedSaveFileInVanillaSaveFolder.Replace(vanillaSaveFolder, SaveFolderPath);
                if (File.Exists(randomizedSaveFileInRandomizerSaveFolder))
                {
                    currentState.AddError($@"The randomized save seems to have been correctly copied into Dark Souls {gameSuffix}'s default save location: {vanillaSaveFolder}
However, it also still exists in the randomizer save it should have come from: {SaveFolderPath}
This shouldn't be possible, so I'm leaving everything as-is.
You should be able to find a backup of the unrandomized saves in {backupFolderPath}.
Please put the randomized save here: {randomizedSaveFileInRandomizerSaveFolder}
Please put the unrandomized save here: {randomizedSaveFileInVanillaSaveFolder}
Please make sure there are no files named {saveFileName} in {backupFolderPath}");
                    return;
                }
                Directory.CreateDirectory(Path.GetDirectoryName(randomizedSaveFileInRandomizerSaveFolder)!);
                File.Move(randomizedSaveFileInVanillaSaveFolder, randomizedSaveFileInRandomizerSaveFolder);
            }
            // No randomized save? Either this is a failing install for a new run, or something went very wrong with your game

            var possibleBackedUpSaves = Directory.GetFiles(backupFolderPath, saveFileName, SearchOption.AllDirectories).ToList();
            if (possibleBackedUpSaves.Count > 1) // Multiple backed up saves should never happen, but just in case
            {
                currentState.AddError($@"There is more than one Dark Souls {gameSuffix} save backed up in {backupFolderPath}.
This shouldn't be possible, so I'm leaving everything as-is.
Please identify the correct backup save in {backupFolderPath}
and put it in the same subfolder under {vanillaSaveFolder}.
Please make sure there are no files named {saveFileName} in {backupFolderPath}");
                return;
            }
            else if (possibleBackedUpSaves.Count == 1)
            {
                var backedUpVanillaSave = possibleBackedUpSaves.Single();
                var vanillaSave = backedUpVanillaSave.Replace(backupFolderPath, vanillaSaveFolder);
                File.Move(backedUpVanillaSave, vanillaSave);
            }
            // No backed-up save? It must not have existed when the randomizer launched.
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    RevertChanges();
                }

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