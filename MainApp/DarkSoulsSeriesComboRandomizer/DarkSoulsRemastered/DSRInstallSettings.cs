using System.IO;

namespace DarkSoulsSeriesComboRandomizer.DarkSoulsRemastered
{
    internal sealed class DSRInstallSettings(string gameRoot) : GameInstallSettings("DSR")
    {
        private DSRFiles gameFiles = new(gameRoot);

        protected override string GameRoot => gameRoot;
        protected override ISoulsGameFiles GameFiles => gameFiles;
        protected override string VanillaSaveFolder => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "NBGI", "DARK SOULS REMASTERED");
        protected override string SaveFileName => "DRAKS0005.sl2";

        public override SoulsGame Game => SoulsGame.DSR;

        protected override bool ShouldBackUpFile(string moddedFile) =>
            !moddedFile.Contains("dinput8.dll") && !moddedFile.Contains("steam_appid.txt");

        protected override IModdedFile GetRegulationFileModification(string saveFolder)
        {
            var vanilla = Path.Combine(gameRoot, "param", "GameParam", "GameParam.parambnd.dcx");
            var backup = Path.Combine(backupFolderPath, "GameParam.parambnd.dcx");
            var modded = Path.Combine(saveFolder, "GameParam.parambnd.dcx");
            return new BackedUpFile(vanilla, backup, modded);
        }

        public override (Dictionary<MapName, Map>, List<Key>) CreateMapsAndKeys(CrossGameMappings crossGameMappings)
        {
            var maps = MapFactory.DSRMaps(gameFiles, crossGameMappings, [.. MapData.DS1StartingItemLots, MapData.DS1EstusFlaskLot]);
            var keys = Key.ConstructDS1Keys(maps);
            return (maps, keys);
        }
        public override IItemNameLookupService CreateLookupService() =>
            new DSRItemNameLookupService(gameFiles);
    }
}