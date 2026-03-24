using SoulsFormats;

namespace DarkSoulsItemMigrator
{
    class Bnd4File(string path) : GameFile(path)
    {
        public BND4 Bnd { get; private set; } = BND4.Read(path);
        public override void Refresh() => Bnd = BND4.Read(Path);
        protected override void Write() => Bnd.Write(Path);
    }
}
