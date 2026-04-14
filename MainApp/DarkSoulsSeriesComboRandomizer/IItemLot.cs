namespace DarkSoulsSeriesComboRandomizer
{
    public interface IItemLot
    {
        public int ID { get; }
        public LotType LotType { get; }
        public IReadOnlyList<LotSlot> Slots { get; }
        public SoulsGame Game { get; }

        bool CanTake();
        void TakeItems(Queue<LotSlot> unassignedItems);
        public void Write();
    }
}
