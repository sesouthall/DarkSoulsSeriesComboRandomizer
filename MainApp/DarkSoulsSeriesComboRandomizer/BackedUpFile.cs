using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace DarkSoulsSeriesComboRandomizer
{
    public class BackedUpFile(string vanillaFile, string backupFile, string moddedFile) : IModdedFile
    {
        public bool TryUpdate([NotNullWhen(false)] out string? errorMessage)
        {
            if (File.Exists(backupFile))
            {
                errorMessage = $@"A file already exists at {backupFile}. This usually means the mod failed to uninstall last time.
Please validate your game and delete this file.";
                return false;
            }

            if (!File.Exists(vanillaFile))
            {
                errorMessage = $@"{vanillaFile} does not exist. This usually means the mod failed to uninstall last time.
Please validate your game and delete this file.";
                return false;
            }

            if(!File.Exists(moddedFile))
            {
                errorMessage = $@"{moddedFile} does not exist. This was supposed to come from the zip folder. Did it get deleted?
Please try re-downloading and unzipping the mod.";
                return false;
            }

            Directory.CreateDirectory(Path.GetDirectoryName(backupFile)!);
            File.Move(vanillaFile, backupFile);
            File.Copy(moddedFile, vanillaFile);
            errorMessage = null;
            return true;
        }

        public void RevertUpdate()
        {
            if (File.Exists(backupFile))
            {
                if (File.Exists(vanillaFile))
                {
                    File.Delete(vanillaFile);
                }
                File.Move(backupFile, vanillaFile);
            }
        }
    }
}