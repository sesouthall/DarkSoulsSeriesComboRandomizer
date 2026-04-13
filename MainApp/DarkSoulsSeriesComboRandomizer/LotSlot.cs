namespace DarkSoulsSeriesComboRandomizer
{
    // ---------------------------------------------------------------
    // LotSlot — one item slot extracted from a lot row, carrying
    // everything needed to write it back into any game's lot param.
    // ---------------------------------------------------------------
    public record LotSlot(
        SoulsGame SourceGame,
        int ItemId,
        SoulsItemType ItemType,
        int Weight,
        int Amount,
        bool isEmptyItem = false
    );
}
