namespace DarkSoulsSeriesComboRandomizer
{
    // ---------------------------------------------------------------
    // LotSlot — one item slot extracted from a lot row, carrying
    // everything needed to write it back into any game's lot param.
    // ---------------------------------------------------------------
    public record LotSlot(
        SoulsItem Item,
        int Weight,
        int Amount,
        bool IsEmptyItem = false
    )
    {
        public (int, SoulsItemType) ResolveFor(SoulsGame game)
        {
            if (game == Item.Game)
            {
                return (Item.OriginalId, Item.ItemType);
            }

            return (CrossGameMappings.GetMappedItem(Item, game), SoulsItemType.Goods);
        }
    }
}
