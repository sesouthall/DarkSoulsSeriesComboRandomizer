using SoulsFormats;
using SoulsFormats.Cryptography;

namespace DarkSoulsItemMigrator
{
    class DS3RegulationFile(string path) : GameFile(path)
    {
        public BND4 Bnd { get; private set; } = RegulationDecryptor.DecryptDS3Regulation(path);
        public override void Refresh() => Bnd = RegulationDecryptor.DecryptDS3Regulation(Path);
        protected override void Write() => RegulationDecryptor.EncryptDS3Regulation(Path, Bnd);
    }
}
