using static SoulsFormats.PARAM;

namespace DarkSoulsSeriesComboRandomizer
{
    public class ItemLot
    {
        public readonly int ID;
        private readonly List<LotSlot> newSlots;
        private readonly IReadOnlyList<Row> originalRows;
        private readonly ILotSlotSerializer slotSerializer;

        public LotType LotType { get; }

        public IReadOnlyList<LotSlot> OriginalSlots { get; }

        public IReadOnlyList<LotSlot> NewSlots => newSlots;

        internal ItemLot(List<LotSlot> slots, List<Row> originalRows, LotType type, ILotSlotSerializer slotSerializer)
        {
            ID = originalRows.First().ID;
            OriginalSlots = slots;
            newSlots = new List<LotSlot>(OriginalSlots.Count);
            this.originalRows = originalRows;
            this.LotType = type;
            this.slotSerializer = slotSerializer;
        }

        public bool CanTake()
        {
            return newSlots.Count < OriginalSlots.Count;
        }

        public void TakeItems(Queue<LotSlot> unassignedItems, bool partialFill = false)
        {
            if (newSlots.Count >= OriginalSlots.Count)
            {
                return;
            }

            for (var i = newSlots.Count; i < OriginalSlots.Count; i++)
            {
                if (OriginalSlots[i].IsEmptyItem)
                {
                    newSlots.Add(OriginalSlots[i]);
                    continue;
                }

                if (unassignedItems.TryDequeue(out var slot))
                {
                    newSlots.Add(slot);
                }
            }
        }

        public void Write()
        {
            foreach (var originalRow in originalRows)
            {
                slotSerializer.WriteToRow(originalRow, NewSlots);
            }
        }

        internal List<string> GetHintsLines(IItemNameLookupService itemNameLookup, string mapName)
        {
            var hintsLines = new List<string>();
            var lotDescription = LotType == LotType.RandomEnemyDrop ? "Random Drop" : "Fixed Treasure";
            foreach (var slot in NewSlots.Where(slot => !slot.IsEmptyItem))
            {
                if (itemNameLookup.TryGetItemName(slot.Item, out var itemName))
                {
                    hintsLines.Add($"{itemName}: {mapName} ({lotDescription})");
                }
            }
            return hintsLines;
        }
    }
}
