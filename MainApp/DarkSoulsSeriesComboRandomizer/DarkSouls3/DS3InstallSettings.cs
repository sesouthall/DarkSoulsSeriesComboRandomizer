using System.IO;

namespace DarkSoulsSeriesComboRandomizer.DarkSouls3
{
    internal sealed class DS3InstallSettings(string gameRoot) : GameInstallSettings("DS3")
    {
        private DS3Files gameFiles = new(gameRoot);

        protected override string GameRoot => gameRoot;
        protected override ISoulsGameFiles GameFiles => gameFiles;
        protected override string VanillaSaveFolder => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "DarkSoulsIII");
        protected override string SaveFileName => "DS30000.sl2";

        public override SoulsGame Game => SoulsGame.DS3;

        protected override bool ShouldBackUpFile(string moddedFile) =>
            moddedFile.Contains("DarkSoulsIII.exe");

        protected override IModdedFile GetRegulationFileModification(string saveFolder)
        {
            string vanilla = Path.Combine(gameRoot, "ComboRandomizer", "Data0.bdt");
            string modded = Path.Combine(saveFolder, "Data0.bdt");
            return new AddedFile(vanilla, modded);
        }

        public override (Dictionary<MapName, Map>, List<Key>) CreateMapsAndKeys(CrossGameMappings crossGameMappings)
        {
            var maps = MapFactory.DS3Maps(gameFiles, crossGameMappings, [MapData.DS3AshenEstusFlaskLot]);
            var keys = Key.ConstructDS3Keys(maps);
            return (maps, keys);
        }
        public override IItemNameLookupService CreateLookupService() =>
            new DS3ItemNameLookupService(gameFiles);
    }
}