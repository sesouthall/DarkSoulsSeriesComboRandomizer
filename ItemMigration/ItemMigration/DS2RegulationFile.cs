using SoulsFormats;

namespace DarkSoulsItemMigrator
{
    class DS2RegulationFile(string path) : GameFile(path)
    {
        public BND4 Bnd { get; private set; } = ReadRegulationFile(path);

        public override void Refresh() => Bnd = ReadRegulationFile(Path);

        protected override void Write() => Bnd.Write(Path);

        private static BND4 ReadRegulationFile(string path)
        {
            using var regulationFileStream = File.OpenRead(path);
            using var regulationByteStream = new BinaryReaderEx(false, regulationFileStream);
            using var decompressedRegulationByteStream = SFUtil.GetDecompressedBinaryReader(regulationByteStream, out var compression);
            var headerString = decompressedRegulationByteStream.GetASCII(0);
            if (headerString == "BND4")
            {
                return BND4.Read(path);
            }
            else
            {
                return DecryptDS2Regulation(path);
            }
        }

        /// <summary>
        /// Copied from WitchyBND
        /// </summary>
        private static byte[] ds2RegulationKey =
        { 0x40, 0x17, 0x81, 0x30, 0xDF, 0x0A, 0x94, 0x54, 0x33, 0x09, 0xE1, 0x71, 0xEC, 0xBF, 0x25, 0x4C };

        /// <summary>
        /// Copied from WitchyBND
        /// Decrypts and unpacks DS2's regulation BND4 from the specified path.
        /// </summary>
        private static BND4 DecryptDS2Regulation(string path)
        {
            byte[] bytes = File.ReadAllBytes(path);
            byte[] iv = new byte[16];
            iv[0] = 0x80;
            Array.Copy(bytes, 0, iv, 1, 11);
            iv[15] = 1;
            byte[] input = new byte[bytes.Length - 32];
            Array.Copy(bytes, 32, input, 0, bytes.Length - 32);
            using (var ms = new MemoryStream(input))
            {
                byte[] decrypted = CryptographyUtil.DecryptAesCtr(ms, ds2RegulationKey, iv);
                return BND4.Read(decrypted);
            }
        }
    }
}
