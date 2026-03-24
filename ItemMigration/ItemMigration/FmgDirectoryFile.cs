namespace DarkSoulsItemMigrator
{
    // ---------------------------------------------------------------
    // FmgDirectoryFile — GameFile subclass for a directory of loose
    // .fmg files. Revert and Save operate on each .fmg individually
    // rather than on a single archive file.
    // ---------------------------------------------------------------
    class FmgDirectoryFile(string directoryPath) : GameFile(directoryPath)
    {
        public LooseFmgDirectory Bnd { get; private set; } = new LooseFmgDirectory(directoryPath);

        public override void Refresh() => Bnd = new LooseFmgDirectory(Path);

        // Each .fmg is backed up and restored individually.
        // GameFile.Revert / Save operate on Path (the directory) so we
        // shadow both to fan out across the individual files instead.
        public new void Revert()
        {
            foreach (var fmgPath in Directory.EnumerateFiles(Path, "*.fmg"))
            {
                var backup = fmgPath + ".bak_original";
                if (File.Exists(backup))
                    File.Copy(backup, fmgPath, overwrite: true);
            }
            Refresh();
        }

        public new void Save()
        {
            // Back up from disk first (still holds original bytes at this point),
            // then flush in-memory changes out to disk.
            foreach (var fmgPath in Directory.EnumerateFiles(Path, "*.fmg"))
            {
                var backup = fmgPath + ".bak_original";
                if (!File.Exists(backup))
                    File.Copy(fmgPath, backup);
            }
            Bnd.Flush();
        }

        // Required by GameFile but unused — writes are handled by Save() above.
        protected override void Write() { }
    }
}
