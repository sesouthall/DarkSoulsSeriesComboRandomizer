using System.IO;

namespace DarkSoulsSeriesComboRandomizer.DarkSouls2SotFS
{
    internal sealed class DS2SotFSInstallSettings(string gameRoot) : GameInstallSettings("DS2S")
    {
        private DS2SotFSFiles gameFiles = new(gameRoot);

        protected override string GameRoot => gameRoot;
        protected override ISoulsGameFiles GameFiles => gameFiles;
        protected override string VanillaSaveFolder => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "DarkSoulsII");
        protected override string SaveFileName => "DS2SOFS0000.sl2";

        public override SoulsGame Game => SoulsGame.DS2S;

        protected override IModdedFile GetRegulationFileModification(string saveFolder)
        {
            string vanilla = Path.Combine(gameRoot, "ComboRandomizer", "enc_regulation.bnd.dcx");
            string modded = Path.Combine(saveFolder, "enc_regulation.bnd.dcx");
            return new AddedFile(vanilla, modded);
        }

        public override (Dictionary<MapName, Map>, List<Key>) CreateMapsAndKeys(CrossGameMappings crossGameMappings)
        {
            var maps = MapFactory.DS2SotFSMaps(gameFiles, crossGameMappings, [MapData.DS2EstusFlaskLot]);
            var keys = Key.ConstructDS2Keys(maps);
            return (maps, keys);
        }
        public override IItemNameLookupService CreateLookupService() =>
            new DS2SotFSItemNameLookupService(gameFiles);
    }
}