using static SoulsFormats.PARAM;

namespace DarkSoulsSeriesComboRandomizer.DarkSouls3
{
    public class DS3LotSlotSerializer(CrossGameMappings crossGameMappings) : ILotSlotSerializer
    {
        private const string ItemIdFieldPattern = "ItemLotId{0}";
        private const string ItemCategoryFieldPattern = "LotItemCategory0{0}";
        private const string ItemWeightFieldPattern = "LotItemBasePoint0{0}";
        private const string ItemAmountFieldPattern = "LotItemNum{0}";

        public List<LotSlot> ParseFromRow(Row row, LotType lotTypeGuess)
        {
            var tempSlots = new List<LotSlot>();
            var slots = new List<LotSlot>();
            for (var i = 1; i <= 8; i++)
            {
                var itemId = Convert.ToInt32(row[string.Format(ItemIdFieldPattern, i)].Value);
                var amount = Convert.ToInt32(row[string.Format(ItemAmountFieldPattern, i)].Value);
                var weight = Convert.ToInt32(row[string.Format(ItemWeightFieldPattern, i)].Value);
                var category = Convert.ToUInt32(row[string.Format(ItemCategoryFieldPattern, i)].Value);
                var parsedCategory = category != 0xFFFFFFFF ? (SoulsItemType)category : SoulsItemType.Goods;

                tempSlots.Add(new LotSlot(new SoulsItem(SoulsGame.DS3, parsedCategory, itemId), weight, amount, IsEmptyItem: itemId == 0 || amount == 0 || weight == 0 || category == 0xFFFFFFFF));
            }

            // normalize all weights to sum to 100 to make translation between games easier
            var totalWeight = tempSlots.Sum(slot => slot.Weight);
            var accumulatedWeight = 0;
            if (totalWeight > 0)
            {
                // The empty drop is conventionally first, so let it pick up any rounding error
                for (var i = tempSlots.Count - 1; i >= 0; i--)
                {
                    var normalizedWeight = i == 0 ? 100 - accumulatedWeight : (int)Math.Round((double)tempSlots[i].Weight * 100 / totalWeight);
                    accumulatedWeight += normalizedWeight;
                    slots.Add(tempSlots[i] with { Weight = normalizedWeight });
                }
                // put the slots back in order
                slots.Reverse();
            }

            return slots;
        }

        public void WriteToRow(Row row, IReadOnlyList<LotSlot> slots)
        {
            foreach (var (slot, index) in slots.Select((slot, i) => (slot, i + 1)))
            {
                var resolvedItem = crossGameMappings.GetMappedItem(slot.Item, SoulsGame.DS3);

                row[string.Format(ItemIdFieldPattern, index)].Value = slot.IsEmptyItem ? 0 : resolvedItem.Id;
                row[string.Format(ItemAmountFieldPattern, index)].Value = slot.Amount;
                // DS3 usually normalizes total weight to 1000.
                // This may not add to 1000, but it should be close enough.
                // If this is one of the few non-guaranteed drops that sums to 100 instead,
                // congrats, you get lots of drops.
                row[string.Format(ItemWeightFieldPattern, index)].Value = slot.Weight * 10;
                row[string.Format(ItemCategoryFieldPattern, index)].Value = slot.IsEmptyItem ? 0xFFFFFFFF : resolvedItem.Type;
            }
        }
    }
}
