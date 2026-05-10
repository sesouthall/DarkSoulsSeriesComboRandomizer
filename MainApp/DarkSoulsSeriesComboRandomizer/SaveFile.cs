using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace DarkSoulsSeriesComboRandomizer
{
    public class SaveFile(string vanillaFolder, string backupFolder, string moddedFolder, string fileName) : IModdedFile
    {
        public bool TryUpdate([NotNullWhen(false)] out string? errorMessage)
        {
            if (Directory.GetFiles(backupFolder, fileName, SearchOption.AllDirectories).Length != 0)
            {
                errorMessage = $@"Found a backup save file ({fileName}) under {backupFolder}. This usually means the mod failed to uninstall correctly.
If a save file exists under {vanillaFolder} and it contains a randomized character, please put it in the appropriate save folder under {Path.GetDirectoryName(moddedFolder)}.
If the save exists and contains your unmodded characters, please delete the save under {backupFolder}.
If no save exists under {vanillaFolder}, please copy the one in {backupFolder} to the same subdirectory in {vanillaFolder}";
                return false;
            }

            var vanillaSubFolders = DiscoverSaveSubFolders(vanillaFolder, fileName);
            if (vanillaSubFolders.Count > 1)
            {
                errorMessage = $@"Found multiple save files ({fileName}) under {vanillaFolder}. This usually means you've made backups manually.
Please move the backups elsewhere, so this mod can difinitively find your unmodded save file";
                return false;
            }
            else if (vanillaSubFolders.Count == 1)
            {
                var subFolder = vanillaSubFolders[0];
                var vanillaFile = Path.Combine(vanillaFolder, subFolder, fileName);
                var backupFile = Path.Combine(backupFolder, subFolder, fileName);
                var moddedFile = Path.Combine(moddedFolder, subFolder, fileName);
                Directory.CreateDirectory(Path.Combine(backupFolder, subFolder));
                File.Move(vanillaFile, backupFile);
                if (File.Exists(moddedFile))
                {
                    File.Copy(moddedFile, vanillaFile);
                }
            }
            else
            {
                var moddedSubFolders = DiscoverSaveSubFolders(moddedFolder, fileName);
                if (moddedSubFolders.Count > 1)
                {
                    errorMessage = $@"Found multiple save files ({fileName}) in {moddedFolder}. This shouldn't be possible.
Please determine which one you wish to keep and delete the others.";
                    return false;
                }
                else if (moddedSubFolders.Count == 1)
                {
                    var subFolder = moddedSubFolders[0];
                    var vanillaFile = Path.Combine(vanillaFolder, subFolder, fileName);
                    var moddedFile = Path.Combine(moddedFolder, subFolder, fileName);
                    File.Copy(moddedFile, vanillaFile);
                }
            }
            errorMessage = null;
            return true;
        }

        public void RevertUpdate()
        {
            var vanillaSubFolders = DiscoverSaveSubFolders(vanillaFolder, fileName);
            if (vanillaSubFolders.Count > 1)
            {
                throw new Exception($"Too many save files named {fileName} under {vanillaFolder}! Did you mess with them while playing?");
            }
            else if (vanillaSubFolders.Count == 1)
            {
                var subFolder = vanillaSubFolders[0];
                var vanillaFile = Path.Combine(vanillaFolder, subFolder, fileName);
                var moddedFile = Path.Combine(moddedFolder, subFolder, fileName);
                File.Move(vanillaFile, moddedFile, true);
            }

            var backupSubFolders = DiscoverSaveSubFolders(backupFolder, fileName);
            if (backupSubFolders.Count > 1)
            {
                throw new Exception($"Too many save files named {fileName} under {backupFolder}! Did you mess with them while playing?");
            }
            else if (backupSubFolders.Count == 1)
            {
                var subFolder = backupSubFolders[0];
                var vanillaFile = Path.Combine(vanillaFolder, subFolder, fileName);
                var backupFile = Path.Combine(backupFolder, subFolder, fileName);
                File.Move(backupFile, vanillaFile);
            }
        }

        private static List<string> DiscoverSaveSubFolders(string topLevelFolder, string saveFileName)
        {
            var possibleSaveFiles = Directory.GetFiles(topLevelFolder, saveFileName, SearchOption.AllDirectories);
            return [.. possibleSaveFiles.Select(path => Path.GetFileName(Path.GetDirectoryName(path)))];

        }
    }
}