using Microsoft.VisualStudio.Utilities.Internal;
using Serilog;
using System.IO;

namespace DarkSoulsSeriesComboRandomizer
{
    public record RandomizerOptions(int Seed, string SaveName, bool AllowFirelinkRoofSkip = false);

    public class ModInstaller(List<GameInstallSettings> gameConfigs, RandomizerOptions options, List<IModdedFile> filesToModify)
    {
        private static readonly string appDataFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "DarkSoulsSeriesComboRandomizer");
        public string SaveFolderPath => Path.Combine(appDataFolderPath, options.SaveName);

        private static readonly string BonfireMappingsFile = Path.Combine(appDataFolderPath, "BonfireMappings.txt");

        public static ModInstaller New(List<GameInstallSettings> gameConfigs, RandomizerOptions options)
        {
            var filesToModify = new List<IModdedFile>();
            var saveFolder = Path.Combine(appDataFolderPath, options.SaveName);

            foreach (var gameConfig in gameConfigs)
            {
                filesToModify.AddRange(gameConfig.GetFilesToModify(saveFolder));
            }

            // Treat the BonfireMappingFile as an added file
            filesToModify.Add(new AddedFile(BonfireMappingsFile, BonfireMappingsFile.Replace(appDataFolderPath, saveFolder)));

            return new ModInstaller(gameConfigs, options, filesToModify);
        }

        public void CreateRandomizedRegulationFilesIfNeeded(CrossGameMappings crossGameMappings, List<BonfireTriple> bonfireMappings)
        {
            if (Directory.Exists(SaveFolderPath) && File.Exists(Path.Combine(SaveFolderPath, "seed.txt")))
            {
                // This randomization has already been generated. No work needed.
                return;
            }

            Directory.CreateDirectory(SaveFolderPath);

            Dictionary<MapName, Map> allMaps = [];
            List<Key> allKeys = [];

            foreach (var gameConfig in gameConfigs)
            {
                Log.Information($"Parsing maps for {gameConfig.Game}");
                var (maps, keys) = gameConfig.CreateMapsAndKeys(crossGameMappings);
                allMaps.AddRange(maps);
                allKeys.AddRange(keys);
                Log.Information("Done");
            }

            var itemLookupService = new AggregateItemNameLookupService([.. gameConfigs.Select(config => config.CreateLookupService())]);

            Key? coiledSword = allKeys.SingleOrDefault(key => key.Item == Key.CoiledSword);
            Map.LoadCrossGameWarps(bonfireMappings, allMaps, coiledSword);

            Map.HandleDS3FirelinkRoofSkip(options.AllowFirelinkRoofSkip, allMaps);

            Log.Information("Starting item randomization");
            ItemRandomizer.Randomize(allMaps[MapName.DS1StartingCell], allMaps, allKeys, new Random(options.Seed));
            Log.Information("Done");

            foreach (var gameConfig in gameConfigs)
            {
                Log.Information($"Saving item changes for {gameConfig.Game}");
                gameConfig.SaveItemLotChanges(SaveFolderPath);
                Log.Information("Done");
            }

            Log.Information("Writing ComboRandomizer files");
            BonfireTriple.SerializeBonfireMappings(bonfireMappings, Path.Combine(SaveFolderPath, "BonfireMappings.txt"));

            var hintsLines = allMaps.Values.SelectMany(map => map.GetHintsLines(itemLookupService));
            File.WriteAllLines(Path.Combine(SaveFolderPath, "Hints"), hintsLines);

            File.WriteAllText(Path.Combine(SaveFolderPath, "seed.txt"), options.Seed.ToString());
            Log.Information("Done");
        }

        public List<string> InstallChanges()
        {
            if (GameInstallSettings.PreviousBackupsExist())
            {
                // The last run failed to clean up. Do it now.
                var revertFailures = RevertChanges();
                if (revertFailures.Count > 0)
                {
                    return [.. revertFailures.Select(ex => $"{ex.Message}\n{ex.StackTrace}")];
                }
            }

            var errors = new List<string>();
            foreach (var modFile in filesToModify)
            {
                Log.Information($"Updating {modFile}");
                if (!modFile.TryUpdate(out var errorMessage))
                {
                    Log.Error($"Failed to update {modFile}: {errorMessage}");
                    errors.Add(errorMessage);
                }    
            }
            if (errors.Count == 0)
            {
                Log.Information("All files updated");
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

        public List<Exception> RevertChanges()
        {
            var errors = new List<Exception>();
            foreach (var modFile in filesToModify)
            {
                try
                {
                    modFile.RevertUpdate();
                }
                catch (Exception ex)
                {
                    errors.Add(ex);
                }
            }
            return errors;
        }
    }
}