using SoulsFormats;
using static SoulsFormats.PARAM;

namespace DarkSoulsSeriesComboRandomizer.DarkSouls2SotFS
{
    public class DS2SotFSItemLot : IItemLot
    {
        // Item lots that should always drop the same items. For example,
        // the Persuer's main fight, and the one-time fight outside the
        // Cardinal Tower bonfire.
        private static readonly List<HashSet<int>> linkedItemLots = new()
        {
            new() {318000, 60008000},
        };

        private const string ItemIdFieldPattern = "item_lot_{0}";
        private const string ItemWeightFieldPattern = "chance_lot_{0}";
        private const string ItemAmountFieldPattern = "amount_lot_{0}";

        public readonly int ID;
        public List<LotSlot> OriginalSlots;
        public List<LotSlot> NewSlots;
        private readonly List<Row> originalRows;
        public LotType type;

        private static Dictionary<int, DS2SotFSItemLot> lotCache = new();

        int IItemLot.ID => ID;

        LotType IItemLot.LotType => type;

        IReadOnlyList<LotSlot> IItemLot.OriginalSlots => OriginalSlots;

        IReadOnlyList<LotSlot> IItemLot.NewSlots => NewSlots;

        SoulsGame IItemLot.Game => SoulsGame.DS2S;

        internal DS2SotFSItemLot(List<LotSlot> slots, List<Row> originalRows, LotType type)
        {
            ID = originalRows.First().ID;
            OriginalSlots = slots;
            NewSlots = new List<LotSlot>(OriginalSlots.Count);
            this.originalRows = originalRows;
            this.type = type;
        }

        public static DS2SotFSItemLot Parse(Row itemLot, LotType lotTypeGuess, PARAM fullItemLotParamTable)
        {
            if (lotCache.TryGetValue(itemLot.ID, out DS2SotFSItemLot? value))
            {
                return value;
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
                    slots.Add(new LotSlot(SoulsGame.DS2S, itemId, category, weight, amount, IsEmptyItem: itemId == 0 || itemId == emptyItemId || amount == 0));
                }
            }

            // DS2 drop chances are already normalized to 100, with an implicit empty drop picking up the rest of the drop chance

            // Sometimes two item lots are linked. e.g. Oscar gives the Asylum F2 East key in dialog, or on death.
            // In those cases, link the equivalent lots, so that they always contain the same item.
            var linkedLotIds = linkedItemLots.SingleOrDefault(set => set.Contains(itemLot.ID)) ?? [itemLot.ID];
            var linkedLots = linkedLotIds.Select(lotId => fullItemLotParamTable.Rows.Single(row => row.ID == lotId)).ToList();

            var actualLotType = lotTypeGuess == LotType.UnspecifiedEnemy ? slots.Count(slot => slot.Weight < 100) > 1 ? LotType.RandomEnemyDrop : LotType.GuaranteedEnemyDrop : lotTypeGuess;
            var parsed = new DS2SotFSItemLot(slots, linkedLots, actualLotType);
            foreach (var linkedLot in linkedLots)
            {
                lotCache[linkedLot.ID] = parsed;
            }
            return parsed;
        }

        public bool CanTake()
        {
            return NewSlots.Count < OriginalSlots.Count;
        }

        public void TakeItems(Queue<LotSlot> unassignedItems, bool partialFill = false)
        {
            // DS2 supports multiple drops from a single item lot, so guaranteed treasures can
            // have more than one item in them. That means a key can get assinged to a lot without
            // fully filling it. To support that, count each item as it's placed and don't start blocking
            // calls until all slots are filled.
            if (NewSlots.Count >= OriginalSlots.Count)
            {
                return;
            }

            for (var i = NewSlots.Count; i < OriginalSlots.Count; i++)
            {
                if (OriginalSlots[i].IsEmptyItem)
                {
                    NewSlots.Add(OriginalSlots[i]);
                    continue;
                }

                if (unassignedItems.TryDequeue(out var slot))
                {
                    if (slot.SourceGame == SoulsGame.DS2S)
                    {
                        NewSlots.Add(slot);
                    }
                    else
                    {
                        var resolvedId = CrossGameMappings.GetMappedItem(new SoulsItem(slot.SourceGame, slot.ItemType, slot.ItemId), SoulsGame.DS2S);
                        NewSlots.Add(new LotSlot(SoulsGame.DS2S, resolvedId, SoulsItemType.Goods, slot.Weight, slot.Amount));
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
            foreach (var originalRow in originalRows)
            {
                int i = 0;
                foreach (var slot in NewSlots)
                {
                    originalRow[string.Format(ItemIdFieldPattern, i)].Value = slot.ItemId;
                    originalRow[string.Format(ItemAmountFieldPattern, i)].Value = slot.Amount;
                    // DS2 weights are direct percentages
                    // Since we've already normalized the weights for a drop to add to 100,
                    // using them directly should be fine. If this is a drop table with lots
                    // of items and it picks up lots of common ones, it could go above 100
                    // but that shouldn't break anything, it'll just cause the enemy to always
                    // drop something, potentially multiple things.
                    originalRow[string.Format(ItemWeightFieldPattern, i)].Value = slot.Weight;
                    i++;
                }
            }
        }
    }
}
