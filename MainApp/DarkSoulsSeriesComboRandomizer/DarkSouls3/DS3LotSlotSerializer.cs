using static SoulsFormats.PARAM;

namespace DarkSoulsSeriesComboRandomizer.DarkSouls3
{
    public class DS3LotSlotSerializer(Dictionary<SoulsItem, int> crossGameItemMapping) : ILotSlotSerializer
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

                if (weight > 0)
                {
                    tempSlots.Add(new LotSlot(new SoulsItem(SoulsGame.DS3, parsedCategory, itemId), weight, amount, IsEmptyItem: itemId == 0 || amount == 0 || category == 0xFFFFFFFF));
                }
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

        public void WriteToRow(Row row, LotSlot slot, int index)
        {
            var resolvedId = slot.Item.OriginalId;
            var resolvedType = slot.Item.ItemType;
            var resolvedIndex = index + 1;
            if (slot.Item.Game != SoulsGame.DS3)
            {
                resolvedId = crossGameItemMapping[slot.Item];
                resolvedType = SoulsItemType.Goods;
            }
            row[string.Format(ItemIdFieldPattern, resolvedIndex)].Value = slot.IsEmptyItem ? 0 : resolvedId;
            row[string.Format(ItemAmountFieldPattern, resolvedIndex)].Value = slot.Amount;
            // DS3 usually normalizes total weight to 1000.
            // This may not add to 1000, but it should be close enough.
            // If this is one of the few non-guaranteed drops that sums to 100 instead,
            // congrats, you get lots of drops.
            row[string.Format(ItemWeightFieldPattern, resolvedIndex)].Value = slot.Weight * 10;
            row[string.Format(ItemCategoryFieldPattern, resolvedIndex)].Value = slot.IsEmptyItem ? 0xFFFFFFFF : resolvedType;
        }
    }
}
