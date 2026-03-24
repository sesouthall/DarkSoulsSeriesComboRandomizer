using SoulsFormats;

namespace DarkSoulsItemMigrator
{
    class Bnd3File(string path) : GameFile(path)
    {
        public BND3 Bnd { get; private set; } = BND3.Read(path);
        public override void Refresh() => Bnd = BND3.Read(Path);
        protected override void Write() => Bnd.Write(Path);
    }
}
