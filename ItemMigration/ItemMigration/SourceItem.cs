using SoulsFormats;

namespace DarkSoulsItemMigrator
{
    // ---------------------------------------------------------------
    // SourceItem — one item read from a source game, ready to be handed
    // to a target game's AddNewGood(). Carries the source PARAM.Row so
    // the target can copy any fields it wants beyond just the icon.
    // ---------------------------------------------------------------
    record SourceItem(
        string Name,
        string Desc,
        string LongDesc,
        PARAM.Row SourceRow,
        SourceGame SourceGame,
        string SourceCategory
    );
}
