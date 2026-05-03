using static SoulsFormats.PARAM;

namespace DarkSoulsSeriesComboRandomizer
{
    public class ItemLot
    {
        public readonly int ID;
        private readonly List<LotSlot> originalSlots;
        private readonly List<LotSlot> newSlots;
        private readonly List<Row> originalRows;
        public readonly SoulsGame Game;
        private readonly ILotSlotSerializer slotSerializer;

        public LotType LotType { get; }

        public IReadOnlyList<LotSlot> OriginalSlots => originalSlots;

        public IReadOnlyList<LotSlot> NewSlots => newSlots;

        internal ItemLot(List<LotSlot> slots, List<Row> originalRows, LotType type, SoulsGame game, ILotSlotSerializer slotSerializer)
        {
            ID = originalRows.First().ID;
            originalSlots = slots;
            newSlots = new List<LotSlot>(originalSlots.Count);
            this.originalRows = originalRows;
            this.LotType = type;
            Game = game;
            this.slotSerializer = slotSerializer;
        }

        public bool CanTake()
        {
            return newSlots.Count < originalSlots.Count;
        }

        public void TakeItems(Queue<LotSlot> unassignedItems, bool partialFill = false)
        {
            if (newSlots.Count >= originalSlots.Count)
            {
                return;
            }

            for (var i = newSlots.Count; i < originalSlots.Count; i++)
            {
                if (originalSlots[i].IsEmptyItem)
                {
                    newSlots.Add(originalSlots[i]);
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
                foreach (var (slot, index) in newSlots.Select((slot, i) => (slot, i)))
                {
                    slotSerializer.WriteToRow(originalRow, slot, index);
                }
            }
        }
    }
}
