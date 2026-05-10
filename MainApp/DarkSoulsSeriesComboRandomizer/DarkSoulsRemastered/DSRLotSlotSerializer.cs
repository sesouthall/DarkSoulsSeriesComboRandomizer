using static SoulsFormats.PARAM;

namespace DarkSoulsSeriesComboRandomizer.DarkSoulsRemastered
{
    public class DSRLotSlotSerializer(CrossGameMappings crossGameMappings) : ILotSlotSerializer
    {
        private const string ItemIdFieldPattern = "lotItemId0{0}";
        private const string ItemCategoryFieldPattern = "lotItemCategory0{0}";
        private const string ItemWeightFieldPattern = "lotItemBasePoint0{0}";
        private const string ItemAmountFieldPattern = "lotItemNum0{0}";

        public List<LotSlot> ParseFromRow(Row row, LotType lotTypeGuess)
        {
            var tempSlots = new List<LotSlot>();
            for (var i = 1; i <= 8; i++)
            {
                var itemId = Convert.ToInt32(row[string.Format(ItemIdFieldPattern, i)].Value);
                var amount = Convert.ToInt32(row[string.Format(ItemAmountFieldPattern, i)].Value);
                var weight = Convert.ToInt32(row[string.Format(ItemWeightFieldPattern, i)].Value);
                var category = Convert.ToInt32(row[string.Format(ItemCategoryFieldPattern, i)].Value);
                var parsedCategory = category != -1 ? (SoulsItemType)category : SoulsItemType.Goods;

                if (weight > 0)
                {
                    tempSlots.Add(new LotSlot(new SoulsItem(SoulsGame.DSR, parsedCategory, itemId), weight, amount, IsEmptyItem: itemId == 0 || amount == 0 || category == -1));
                }
            }

            // normalize all weights to sum to 100 to make translation between games easier
            var totalWeight = tempSlots.Sum(slot => slot.Weight);
            var accumulatedWeight = 0;
            var slots = new List<LotSlot>();
            if (totalWeight > 0)
            {
                // The empty drop is conventionally first, so let it pick up any rounding error
                for (var i = tempSlots.Count - 1; i >= 0; i--)
                {
                    var normalizedWeight = i == 0 ? 100 - accumulatedWeight : Math.Max((int)Math.Round((double)tempSlots[i].Weight * 100 / totalWeight), 1);
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
            foreach (var (slot, index) in slots.Select((slot, i) => (slot, i+1)))
            {
                var resolvedItem = crossGameMappings.GetMappedItem(slot.Item, SoulsGame.DSR);

                row[string.Format(ItemIdFieldPattern, index)].Value = slot.IsEmptyItem ? 0 : resolvedItem.Id;
                row[string.Format(ItemAmountFieldPattern, index)].Value = slot.Amount;
                // DSR uses a mix of weights adding to 100-ish and weights adding to 1000-ish
                // 100 seems more common, so use that.
                row[string.Format(ItemWeightFieldPattern, index)].Value = slot.Weight;
                row[string.Format(ItemCategoryFieldPattern, index)].Value = slot.IsEmptyItem ? -1 : resolvedItem.Type;
            }
        }
    }
}
