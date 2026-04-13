namespace DarkSoulsSeriesComboRandomizer
{
    public interface IItemLot
    {
        public LotType LotType { get; }
        public IReadOnlyList<LotSlot> Slots { get; }

        void TakeItems(Queue<LotSlot> unassignedItems, Dictionary<(SoulsGame, SoulsItemType, int), int> crossGameItems);
        public void Write();
    }
}
