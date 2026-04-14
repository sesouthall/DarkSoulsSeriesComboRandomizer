using static SoulsFormats.PARAM;

namespace DarkSoulsSeriesComboRandomizer.DarkSouls2SotFS
{
    public class DS2SotFSItemLot : IItemLot
    {
        private const string ItemIdFieldPattern = "item_lot_{0}";
        private const string ItemWeightFieldPattern = "chance_lot_{0}";
        private const string ItemAmountFieldPattern = "amount_lot_{0}";

        public readonly int ID;
        public List<LotSlot> Slots;
        private readonly Row originalRow;
        public LotType type;

        private int nextSlotToFill = 0;

        private static Dictionary<int, DS2SotFSItemLot> lotCache = new();
        private static Dictionary<int, SoulsItem> crossGameItems = new();

        int IItemLot.ID => ID;

        LotType IItemLot.LotType => type;

        IReadOnlyList<LotSlot> IItemLot.Slots => Slots;

        SoulsGame IItemLot.Game => SoulsGame.DS2S;

        private DS2SotFSItemLot(List<LotSlot> slots, Row originalRow, LotType type)
        {
            ID = originalRow.ID;
            Slots = slots;
            this.originalRow = originalRow;
            this.type = type;
        }

        public static void Initialize(Dictionary<int, SoulsItem> crossGameItems)
        {
            DS2SotFSItemLot.crossGameItems = crossGameItems;
        }

        public static DS2SotFSItemLot Parse(Row itemLot, LotType lotTypeGuess)
        {
            if (lotCache.ContainsKey(itemLot.ID))
            {
                return lotCache[itemLot.ID];
            }

            // DS2 uses 10 (no item) to represent an empty item in the ItemLotParam2_Other table (treasure from chests, corpses, boss drops, etc.)
            // and 60510000 (rubbish) to represent an empty item in the ItemLotParam2_Chr table (drop tables for killed enemies and NPCs)
            var emptyItemId = lotTypeGuess == LotType.UnspecifiedEnemy ? 60510000 : 10;
            var slots = new List<LotSlot>();
            for (var i = 0; i <= 9; i++)
            {
                var itemId = Convert.ToInt32(itemLot[string.Format(ItemIdFieldPattern, i)].Value);
                var amount = Convert.ToInt32(itemLot[string.Format(ItemAmountFieldPattern, i)].Value);
                var weight = Convert.ToInt32(itemLot[string.Format(ItemWeightFieldPattern, i)].Value);
                var category = SoulsItemType.Goods;

                if (weight > 0)
                {
                    slots.Add(new LotSlot(SoulsGame.DS2S, itemId, category, weight, amount, isEmptyItem: itemId == 0 || itemId == emptyItemId || amount == 0));
                }
            }

            // DS2 drop chances are already normalized to 100, with an implicit empty drop picking up the rest of the drop chance

            var actualLotType = lotTypeGuess == LotType.UnspecifiedEnemy ? slots.Count(slot => slot.Weight < 100) > 1 ? LotType.RandomEnemyDrop : LotType.GuaranteedEnemyDrop : lotTypeGuess;
            var parsed = new DS2SotFSItemLot(slots, itemLot, actualLotType);
            lotCache[itemLot.ID] = parsed;
            return parsed;
        }

        public bool CanTake()
        {
            return nextSlotToFill < Slots.Count;
        }

        public void TakeItems(Queue<LotSlot> unassignedItems)
        {
            // DS2 supports multiple drops from a single item lot, so guaranteed treasures can
            // have more than one item in them. That means a key can get assinged to a lot without
            // fully filling it. To support that, count each item as it's placed and don't start blocking
            // calls until all slots are filled.
            if (nextSlotToFill >= Slots.Count)
            {
                return;
            }

            for (var i = 0; i < Slots.Count; i++)
            {
                if (Slots[i].isEmptyItem)
                {
                    nextSlotToFill++;
                    continue;
                }

                if (unassignedItems.TryDequeue(out var slot))
                {
                    if (slot.SourceGame == SoulsGame.DS2S)
                    {
                        Slots[i] = slot;
                    }
                    else
                    {
                        var resolvedId = crossGameItems.Single(pair => pair.Value == new SoulsItem(slot.SourceGame, slot.ItemType, slot.ItemId)).Key;
                        Slots[i] = new LotSlot(SoulsGame.DS2S, resolvedId, SoulsItemType.Goods, slot.Weight, slot.Amount);
                    }
                    nextSlotToFill++;
                }
            }
        }

        public void Write()
        {
            for (var i = 1; i <= Slots.Count; i++)
            {
                originalRow[string.Format(ItemIdFieldPattern, i)].Value = Slots[i].ItemId;
                originalRow[string.Format(ItemAmountFieldPattern, i)].Value = Slots[i].Amount;
                // DS2 weights are direct percentages
                // Since we've already normalized the weights for a drop to add to 100,
                // using them directly should be fine. If this is a drop table with lots
                // of items and it picks up lots of common ones, it could go above 100
                // but that shouldn't break anything, it'll just cause the enemy to always
                // drop something, potentially multiple things.
                originalRow[string.Format(ItemWeightFieldPattern, i)].Value = Slots[i].Weight;
            }
        }
    }
}
