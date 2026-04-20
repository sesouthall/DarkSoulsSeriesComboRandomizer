namespace DarkSoulsSeriesComboRandomizer
{
    internal static partial class Program
    {
        private const string dsrRoot = @"C:\Program Files (x86)\Steam\steamapps\common\DARK SOULS REMASTERED";
        private const string ds2Root = @"C:\Program Files (x86)\Steam\steamapps\common\Dark Souls II Scholar of the First Sin\Game";
        private const string ds3Root = @"C:\Program Files (x86)\Steam\steamapps\common\DARK SOULS III\Game";

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            var saveName = "testSave";

            CreateBonfireMappings();

            CrossGameMappings.Initialize();

            using var installer = new ModInstaller(dsrRoot, ds2Root, ds3Root, new RandomizerOptions(0, saveName));

            installer.InstallStaticChanges();
            installer.CreateRandomizedRegulationFilesIfNeeded();
            installer.InstallRegulationFiles();

            using var gameCoordinationServer = new GameCoordinationServer(Path.Combine(dsrRoot, "DarkSoulsRemastered.exe"), Path.Combine(ds2Root, "DarkSoulsII.exe"), Path.Combine(ds3Root, "DarkSoulsIII.exe"));

            var tokenSource = new CancellationTokenSource();

            var gameCoordinationTask = Task.Run(() => gameCoordinationServer.Start(tokenSource.Token));
            gameCoordinationTask.Wait();

            return;

            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.Run(new Form1());
        }

        private static void CreateBonfireMappings()
        {
            var bonfireMappingsFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "DarkSoulsSeriesComboRandomizer", "BonfireMappings.txt");
            if (!File.Exists(bonfireMappingsFile))
            {
                File.WriteAllText(bonfireMappingsFile, "Undead Asylum Courtyard,Fire Keepers' Dwelling,Cemetery of Ash");
            }

            var bonfireMappings = BonfireTriple.ParseBonfireMappings(bonfireMappingsFile);
            Map.LoadCrossGameWarps(bonfireMappings);
        }
    }

    public record RandomizerOptions(int seed, string saveName, bool allowFirelinkRoofSkip = false);
}