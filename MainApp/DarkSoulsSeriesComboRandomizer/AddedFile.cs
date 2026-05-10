using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace DarkSoulsSeriesComboRandomizer
{
    public class AddedFile(string vanillaFile, string moddedFile) : IModdedFile
    {
        public bool TryUpdate([NotNullWhen(false)] out string? errorMessage)
        {
            if (File.Exists(vanillaFile))
            {
                errorMessage = $@"{vanillaFile} already exists when trying to be installed by the mod. This usually means the mod failed to uninstall last time.
Please delete this file.";
                return false;
            }

            if(!File.Exists(moddedFile))
            {
                errorMessage = $@"{moddedFile} does not exist. This was supposed to come from the zip folder. Did it get deleted?
Please try re-downloading and unzipping the mod.";
                return false;
            }

            Directory.CreateDirectory(Path.GetDirectoryName(vanillaFile)!);
            File.Copy(moddedFile, vanillaFile);
            errorMessage = null;
            return true;
        }

        public void RevertUpdate()
        {
            if (File.Exists(vanillaFile))
            {
                File.Delete(vanillaFile);
            }
        }
    }
}