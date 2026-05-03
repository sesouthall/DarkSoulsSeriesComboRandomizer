using static SoulsFormats.PARAM;

namespace DarkSoulsSeriesComboRandomizer.DarkSouls2SotFS
{
    public class DS2SotFSLotSlotSerializer(Dictionary<SoulsItem, int> crossGameItemMapping) : ILotSlotSerializer
    {
        private const string ItemIdFieldPattern = "item_lot_{0}";
        private const string ItemWeightFieldPattern = "chance_lot_{0}";
        private const string ItemAmountFieldPattern = "amount_lot_{0}";

        public List<LotSlot> ParseFromRow(Row row, LotType lotTypeGuess)
        {
            // DS2 uses 10 (no item) to represent an empty item in the ItemLotParam2_Other table (treasure from chests, corpses, boss drops, etc.)
            // and 60510000 (rubbish) to represent an empty item in the ItemLotParam2_Chr table (drop tables for killed enemies and NPCs)
            var emptyItemId = lotTypeGuess == LotType.UnspecifiedEnemy ? 60510000 : 10;
            var slots = new List<LotSlot>();
            for (var i = 0; i <= 9; i++)
            {
                var itemId = Convert.ToInt32(row[string.Format(ItemIdFieldPattern, i)].Value);
                var amount = Convert.ToInt32(row[string.Format(ItemAmountFieldPattern, i)].Value);
                var weight = Convert.ToInt32(row[string.Format(ItemWeightFieldPattern, i)].Value);
                var category = SoulsItemType.Goods;

                if (weight > 0)
                {
                    slots.Add(new LotSlot(new SoulsItem(SoulsGame.DS2S, category, itemId), weight, amount, IsEmptyItem: itemId == 0 || itemId == emptyItemId || amount == 0));
                }
            }

            return slots;
        }

        public void WriteToRow(Row row, LotSlot slot, int index)
        {
            var resolvedId = slot.Item.OriginalId;
            if (slot.Item.Game != SoulsGame.DS2S)
            {
                resolvedId = crossGameItemMapping[slot.Item];
            }
            row[string.Format(ItemIdFieldPattern, index)].Value = resolvedId;
            row[string.Format(ItemAmountFieldPattern, index)].Value = slot.Amount;
            // DS2 weights are direct percentages
            // Since we've already normalized the weights for a drop to add to 100,
            // using them directly should be fine. If this is a drop table with lots
            // of items and it picks up lots of common ones, it could go above 100
            // but that shouldn't break anything, it'll just cause the enemy to always
            // drop something, potentially multiple things.
            row[string.Format(ItemWeightFieldPattern, index)].Value = slot.Weight;
        }
    }
}
