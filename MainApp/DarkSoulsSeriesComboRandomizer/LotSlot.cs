namespace DarkSoulsSeriesComboRandomizer
{
    public record LotSlot(
        SoulsItem Item,
        int Weight,
        int Amount,
        bool IsEmptyItem = false
    );
}
