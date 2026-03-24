namespace DarkSoulsItemMigrator
{
    // ---------------------------------------------------------------
    // GameFile — wraps a single on-disk file so that RevertAll() and
    // SaveAll() can iterate one list without knowing the file type.
    // ---------------------------------------------------------------
    abstract class GameFile(string path)
    {
        public string Path { get; } = path;
        public string BackupPath => Path + ".bak_original";

        public void Revert()
        {
            if (File.Exists(BackupPath))
                File.Copy(BackupPath, Path, overwrite: true);
            Refresh();
        }

        public void Save()
        {
            if (!File.Exists(BackupPath))
                File.Copy(Path, BackupPath);
            Write();
        }

        // Reloads the in-memory object from disk. Called after Revert() so
        // the file's contents are clean before any migration runs.
        public abstract void Refresh();

        protected abstract void Write();
    }
}
