using System.Text.RegularExpressions;

namespace DarkSoulsItemMigrator
{
    // ---------------------------------------------------------------
    // ItemType — describes one category of items within a game's data.
    // SkipFromId excludes injected rows when the game is used as source
    // (e.g. rows >= DS3BaseGoodsParamId in DS3 are DSR items, not native).
    // ---------------------------------------------------------------
    record ItemType(
        string ParamName,
        string ParamdefPath,
        string NameFmgName,
        string? DescFmgName,
        string LongDescFmgName,
        int SkipFromId,
        Regex ParamFileLookupPattern
    );
}
