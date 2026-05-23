using System.IO;

namespace DarkSoulsSeriesComboRandomizer
{
    public abstract class GameInstallSettings(string preModdedSubfolder)
    {
        protected readonly string preModdedFilePath = Path.Combine("PreModdedGameFiles", preModdedSubfolder);
        protected static readonly string appDataFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "DarkSoulsSeriesComboRandomizer");
        protected static readonly string backupFolderPath = Path.Combine(appDataFolderPath, "BackupVanillaFiles");

        protected abstract string GameRoot { get; }
        protected abstract string VanillaSaveFolder { get; }
        protected abstract string SaveFileName { get; }
        protected abstract ISoulsGameFiles GameFiles { get; }
        public abstract SoulsGame Game { get; }

        // Override to back up specific files rather than treat them as pure additions
        protected virtual bool ShouldBackUpFile(string moddedFile) => false;

        // Override if the regulation file should be backed up (DSR) vs. just added (DS2, DS3)
        protected abstract IModdedFile GetRegulationFileModification(string saveFolder);

        public static bool PreviousBackupsExist() => Directory.Exists(backupFolderPath) && Directory.GetFiles(backupFolderPath, "*", SearchOption.AllDirectories).Length > 0;

        public IEnumerable<IModdedFile> GetFilesToModify(string saveFolder)
        {
            foreach (var moddedFile in Directory.GetFiles(preModdedFilePath, "*", SearchOption.AllDirectories))
            {
                var vanillaFile = moddedFile.Replace(preModdedFilePath, GameRoot);
                if (ShouldBackUpFile(moddedFile))
                {
                    var backupFile = moddedFile.Replace(preModdedFilePath, backupFolderPath);
                    yield return new BackedUpFile(vanillaFile, backupFile, moddedFile);
                }
                else
                {
                    yield return new AddedFile(vanillaFile, moddedFile);
                }
            }

            yield return GetRegulationFileModification(saveFolder);
            yield return new SaveFile(VanillaSaveFolder, backupFolderPath, saveFolder, SaveFileName);
        }

        public void SaveItemLotChanges(string saveFolder) => GameFiles.SaveItemLotChanges(saveFolder);

        // Override to supply the game-specific files object used in CreateRandomized...
        public abstract (Dictionary<MapName, Map>, List<Key>) CreateMapsAndKeys(CrossGameMappings crossGameMappings);
        public abstract IItemNameLookupService CreateLookupService();
    }
}