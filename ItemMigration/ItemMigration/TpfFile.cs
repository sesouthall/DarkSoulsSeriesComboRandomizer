using SoulsFormats;

namespace DarkSoulsItemMigrator
{
    class TpfFile(string path) : GameFile(path)
    {
        public TPF Tpf { get; private set; } = TPF.Read(path);
        public override void Refresh() => Tpf = TPF.Read(Path);
        protected override void Write() => Tpf.Write(Path);
    }
}
