using DarkSoulsSeriesComboRandomizer;
using SoulsFormats;

namespace DarkSoulsSeriesComboRandomizerTests
{
    internal class TestLotSlotSerializer(List<LotSlot>? slotsToReturn = null) : ILotSlotSerializer
    {
        public List<PARAM.Row> parsedRows = [];
        public List<PARAM.Row> writtenRows = [];
        public List<LotSlot> writtenSlots = [];

        public List<LotSlot> ParseFromRow(PARAM.Row row, LotType lotTypeGuess)
        {
            parsedRows.Add(row);
            return slotsToReturn ?? [];
        }

        public void WriteToRow(PARAM.Row row, IReadOnlyList<LotSlot> slots)
        {
            writtenRows.Add(row);
            writtenSlots.AddRange(slots);
        }
    }
}