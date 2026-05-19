using Microsoft.VisualStudio.Utilities.Internal;
using System.IO;

namespace DarkSoulsSeriesComboRandomizer
{
    public record RandomizerOptions(int Seed, string SaveName, bool AllowFirelinkRoofSkip = false);

    internal class ModInstaller(List<GameInstallSettings> gameConfigs, RandomizerOptions options, List<IModdedFile> filesToModify)
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
            if (Directory.Exists(SaveFolderPath))
            {
                // This randomization has already been generated. No work needed.
                return;
            }

            Directory.CreateDirectory(SaveFolderPath);

            Dictionary<MapName, Map> allMaps = [];
            List<Key> allKeys = [];

            foreach (var gameConfig in gameConfigs)
            {
                var (maps, keys) = gameConfig.CreateMapsAndKeys(crossGameMappings);
                allMaps.AddRange(maps);
                allKeys.AddRange(keys);
            }

            var itemLookupService = new AggregateItemNameLookupService([.. gameConfigs.Select(config => config.CreateLookupService())]);

            Key? coiledSword = allKeys.SingleOrDefault(key => key.Item == Key.CoiledSword);
            Map.LoadCrossGameWarps(bonfireMappings, allMaps, coiledSword);

            Map.HandleDS3FirelinkRoofSkip(options.AllowFirelinkRoofSkip, allMaps);

            ItemRandomizer.Randomize(allMaps[MapName.DS1StartingCell], allMaps, allKeys, new Random(options.Seed));

            foreach (var gameConfig in gameConfigs)
            {
                gameConfig.SaveItemLotChanges(SaveFolderPath);
            }

            BonfireTriple.SerializeBonfireMappings(bonfireMappings, BonfireMappingsFile);

            var hintsLines = allMaps.Values.SelectMany(map => map.GetHintsLines(itemLookupService));
            File.WriteAllLines(Path.Combine(SaveFolderPath, "Hints"), hintsLines);

            File.WriteAllText(Path.Combine(SaveFolderPath, "seed.txt"), options.Seed.ToString());
        }

        public List<string> InstallChanges()
        {
            if (GameInstallSettings.PreviousBackupsExist())
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