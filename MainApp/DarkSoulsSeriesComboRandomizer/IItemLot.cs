namespace DarkSoulsSeriesComboRandomizer
{
    public interface IItemLot
    {
        public int ID { get; }
        public LotType LotType { get; }
        public IReadOnlyList<LotSlot> OriginalSlots { get; }
        public IReadOnlyList<LotSlot> NewSlots { get; }
        public SoulsGame Game { get; }

        bool CanTake();
        void TakeItems(Queue<LotSlot> unassignedItems, bool partialFill = false);
        public void Write();
    }
}
