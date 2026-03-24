using SoulsFormats;

namespace DarkSoulsItemMigrator
{
    // ---------------------------------------------------------------
    // LooseFmgDirectory — implements IBinder over a directory of loose
    // .fmg files so that GameData's GetAllItems / EnsureTargetLoaded
    // can call MsgBnd.Files.First(...).Bytes without knowing whether
    // the underlying storage is a BND archive or individual files.
    //
    // Bytes are buffered in memory (matching BinderFile's normal
    // behaviour) and only written to disk when Flush() is called.
    // ---------------------------------------------------------------
    class LooseFmgDirectory : IBinder
    {
        // Stores the file path alongside the in-memory bytes so Flush()
        // knows where to write each entry without re-parsing the name.
        private class LooseFmgFile(string filePath) : BinderFile(Binder.FileFlags.None, 0, filePath, File.ReadAllBytes(filePath))
        {
            public string FilePath { get; } = filePath;
            public void Flush() => File.WriteAllBytes(FilePath, Bytes);
        }

        public List<BinderFile> Files { get; }
        public Binder.Format Format { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string Version { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        List<BinderFile> IBinder.Files { get => Files; set => throw new NotImplementedException(); }

        public LooseFmgDirectory(string directoryPath)
        {
            Files = Directory
                .EnumerateFiles(directoryPath, "*.fmg", SearchOption.TopDirectoryOnly)
                .Select(path => (BinderFile)new LooseFmgFile(path))
                .ToList();
        }

        // Writes all buffered byte changes to disk.
        public void Flush()
        {
            foreach (var file in Files.Cast<LooseFmgFile>())
                file.Flush();
        }
    }
}
