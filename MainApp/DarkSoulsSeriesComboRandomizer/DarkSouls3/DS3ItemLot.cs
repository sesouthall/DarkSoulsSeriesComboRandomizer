using static SoulsFormats.PARAM;

namespace DarkSoulsSeriesComboRandomizer.DarkSouls3
{
    public class DS3ItemLot : IItemLot
    {
        private const string ItemIdFieldPattern = "ItemLotId{0}";
        private const string ItemCategoryFieldPattern = "LotItemCategory0{0}";
        private const string ItemWeightFieldPattern = "LotItemBasePoint0{0}";
        private const string ItemAmountFieldPattern = "LotItemNum{0}";

        public readonly int ID;
        public List<LotSlot> OriginalSlots;
        public List<LotSlot> NewSlots;
        private readonly Row originalRow;
        public LotType type;

        private static Dictionary<int, DS3ItemLot> lotCache = new();

        int IItemLot.ID => ID;

        LotType IItemLot.LotType => type;

        IReadOnlyList<LotSlot> IItemLot.OriginalSlots => OriginalSlots;

        IReadOnlyList<LotSlot> IItemLot.NewSlots => NewSlots;

        SoulsGame IItemLot.Game => SoulsGame.DS3;

        internal DS3ItemLot(List<LotSlot> slots, Row originalRow, LotType type)
        {
            ID = originalRow.ID;
            OriginalSlots = slots;
            NewSlots = new List<LotSlot>(OriginalSlots.Count);
            this.originalRow = originalRow;
            this.type = type;
        }

        public static DS3ItemLot Parse(Row itemLot, LotType lotTypeGuess)
        {
            if (lotCache.TryGetValue(itemLot.ID, out DS3ItemLot? value))
            {
                return value;
            }

            var tempSlots = new List<LotSlot>();
            var slots = new List<LotSlot>();
            for (var i = 1; i <= 8; i++)
            {
                var itemId = Convert.ToInt32(itemLot[string.Format(ItemIdFieldPattern, i)].Value);
                var amount = Convert.ToInt32(itemLot[string.Format(ItemAmountFieldPattern, i)].Value);
                var weight = Convert.ToInt32(itemLot[string.Format(ItemWeightFieldPattern, i)].Value);
                var category = Convert.ToUInt32(itemLot[string.Format(ItemCategoryFieldPattern, i)].Value);
                var parsedCategory = category != 0xFFFFFFFF ? (SoulsItemType)category : SoulsItemType.Goods;

                if (weight > 0)
                {
                    tempSlots.Add(new LotSlot(SoulsGame.DS3, itemId, parsedCategory, weight, amount, IsEmptyItem: itemId == 0 || amount == 0 || category == 0xFFFFFFFF));
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

            var actualLotType = lotTypeGuess == LotType.UnspecifiedEnemy ? slots.Count > 1 ? LotType.RandomEnemyDrop : LotType.GuaranteedEnemyDrop : lotTypeGuess;
            var parsed = new DS3ItemLot(slots, itemLot, actualLotType);
            lotCache[itemLot.ID] = parsed;
            return parsed;
        }

        public bool CanTake()
        {
            return NewSlots.Count < OriginalSlots.Count;
        }

        public void TakeItems(Queue<LotSlot> unassignedItems, bool partialFill = false)
        {
            if (NewSlots.Count >= OriginalSlots.Count)
            {
                return;
            }

            for (var i = 0; i < OriginalSlots.Count; i++)
            {
                if (OriginalSlots[i].IsEmptyItem)
                {
                    NewSlots.Add(OriginalSlots[i]);
                    continue;
                }

                if (unassignedItems.TryDequeue(out var slot))
                {
                    if (slot.SourceGame == SoulsGame.DS3)
                    {
                        NewSlots.Add(slot);
                    }
                    else
                    {
                        var resolvedId = CrossGameMappings.GetMappedItem(new SoulsItem(slot.SourceGame, slot.ItemType, slot.ItemId), SoulsGame.DS3);
                        NewSlots.Add(new LotSlot(SoulsGame.DS3, resolvedId, SoulsItemType.Goods, slot.Weight, slot.Amount));
                    }
                }
                else if (!partialFill)
                {
                    ItemRandomizer.MissingItemCount++;
                    //throw new Exception("Ran out of items!");
                }
            }
        }

        public void Write()
        {
            int i = 1;
            foreach (var slot in NewSlots)
            {
                originalRow[string.Format(ItemIdFieldPattern, i)].Value = slot.ItemId;
                originalRow[string.Format(ItemAmountFieldPattern, i)].Value = slot.Amount;
                // DS3 usually normalizes total weight to 1000.
                // This may not add to 1000, but it should be close enough.
                // If this is one of the few non-guaranteed drops that sums to 100 instead,
                // congrats, you get lots of drops.
                originalRow[string.Format(ItemWeightFieldPattern, i)].Value = slot.Weight * 10;
                originalRow[string.Format(ItemCategoryFieldPattern, i)].Value = slot.ItemType;
                i++;
            }
        }
    }
}
