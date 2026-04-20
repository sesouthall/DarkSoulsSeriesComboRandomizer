using DarkSoulsSeriesComboRandomizer.DarkSouls2SotFS;
using DarkSoulsSeriesComboRandomizer.DarkSouls3;
using DarkSoulsSeriesComboRandomizer.DarkSoulsRemastered;

namespace DarkSoulsSeriesComboRandomizer
{
    internal class ModInstaller : IDisposable
    {
        private readonly string dsrRoot;
        private readonly string ds2Root;
        private readonly string ds3Root;
        private readonly RandomizerOptions options;
        private bool disposedValue;

        private static readonly string backupFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "DarkSoulsSeriesComboRandomizer", "BackupVanillaFiles");
        private string SaveFolderPath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "DarkSoulsSeriesComboRandomizer", options.saveName);
        private string DSRRegulationFilePath => Path.Combine(dsrRoot, "param", "GameParam", "GameParam.parambnd.dcx");

        public ModInstaller(string dsrRoot, string ds2Root, string ds3Root, RandomizerOptions options)
        {
            this.dsrRoot = dsrRoot;
            this.ds2Root = ds2Root;
            this.ds3Root = ds3Root;
            this.options = options;
        }

        public void InstallStaticChanges()
        {
            // DSR uses direct file replacement. Just copy things over and save anything we're replacing.
            var ds1ModdedFilePath = Path.Combine("PreModdedGameFiles", "DSR");
            foreach (var file in Directory.GetFiles(ds1ModdedFilePath, "*", SearchOption.AllDirectories))
            {
                var vanillaFile = file.Replace(ds1ModdedFilePath, dsrRoot);
                var backupFile = file.Replace(ds1ModdedFilePath, backupFolderPath);
                if (File.Exists(vanillaFile))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(backupFile));
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
                Directory.CreateDirectory(Path.GetDirectoryName(destinationPath));
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
                    Directory.CreateDirectory(Path.GetDirectoryName(backupPath));
                    File.Move(destinationPath, backupPath);
                }
                Directory.CreateDirectory(Path.GetDirectoryName(destinationPath));
                File.Copy(file, destinationPath);
            }
        }

        public void CreateRandomizedRegulationFilesIfNeeded()
        {
            if (Directory.Exists(SaveFolderPath))
            {
                // This randomization has already been generated.
                // No work needed.
                return;
            }

            Directory.CreateDirectory(SaveFolderPath);

            var dsrItems = DSRItemLots.New(dsrRoot, SaveFolderPath);
            var ds2Items = DS2SotFSItemLots.New(ds2Root, SaveFolderPath);
            var ds3Items = DS3ItemLots.New(ds3Root, SaveFolderPath);

            dsrItems.Load();
            ds2Items.Load();
            ds3Items.Load();

            Map.HandleDS3FirelinkRoofSkip(options.allowFirelinkRoofSkip);

            ItemRandomizer.Randomize(Map.AllMaps[MapName.DS1StartingCell], Map.AllMaps, Key.AllKeys, new List<(int, SoulsGame)>(), new Random(options.seed));

            dsrItems.Save();
            ds2Items.Save();
            ds3Items.Save();
        }

        public void InstallRegulationFiles()
        {
            File.Copy(Path.Combine(SaveFolderPath, "GameParam.parambnd.dcx"), DSRRegulationFilePath, true);

            File.Copy(Path.Combine(SaveFolderPath, "enc_regulation.bnd.dcx"), Path.Combine(ds2Root, "ComboRandomizer", "enc_regulation.bnd.dcx"), true);

            File.Copy(Path.Combine(SaveFolderPath, "Data0.bdt"), Path.Combine(ds3Root, "ComboRandomizer", "Data0.bdt"), true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // DSR - Restore backups for any files replaced. Delete the others.
                    var ds1ModdedFilePath = Path.Combine("PreModdedGameFiles", "DSR");
                    foreach (var file in Directory.GetFiles(ds1ModdedFilePath))
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
                    foreach (var file in Directory.GetFiles(ds2ModdedFilePath))
                    {
                        var destinationPath = file.Replace(ds2ModdedFilePath, ds2Root);
                        File.Delete(destinationPath);
                    }

                    // DS3 - Restore DarkSoulsIII.exe, delete everything else
                    var ds3ModdedFilePath = Path.Combine("PreModdedGameFiles", "DS3");
                    foreach (var file in Directory.GetFiles(ds3ModdedFilePath))
                    {
                        var vanillaFile = file.Replace(ds3ModdedFilePath, dsrRoot);
                        var backupFile = file.Replace(ds3ModdedFilePath, backupFolderPath);
                        File.Delete(vanillaFile);
                        if (File.Exists(backupFile))
                        {
                            File.Move(backupFile, vanillaFile);
                        }
                    }
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