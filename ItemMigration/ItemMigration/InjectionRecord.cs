namespace DarkSoulsItemMigrator
{
    // ---------------------------------------------------------------
    // InjectionRecord — tracks one injected good so GameData can write
    // a per-game CSV report mapping injected IDs back to their origins.
    // ---------------------------------------------------------------
    record InjectionRecord(
        int InjectedId,
        SourceGame SourceGame,
        string SourceCategory,
        int SourceRowId
    );
}
