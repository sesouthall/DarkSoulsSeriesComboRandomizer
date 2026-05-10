using static SoulsFormats.PARAM;

namespace DarkSoulsSeriesComboRandomizer
{
    public interface ILotSlotSerializer
    {
        List<LotSlot> ParseFromRow(Row row, LotType lotTypeGuess);
        void WriteToRow(Row row, IReadOnlyList<LotSlot> slots);
    }
}
