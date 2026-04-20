using SoulsFormats;
using static SoulsFormats.PARAM;

namespace DarkSoulsSeriesComboRandomizer.DarkSoulsRemastered
{
    public class DSRItemLot : IItemLot
    {
        // linked locations taken from HotPocketRemix's DarkSoulsItemRandomizer
        private static readonly List<HashSet<int>> linkedItemLots = new()
        {
            new() {1000, 6000},
            new() {1050, 1070, 6280},
            // Oscar dialog or death
            new() {1080, 6020}, // Undead Asylum F2 East Key
            new() {1081, 2660, 6021}, // Big Pilgrim's Key
            new() {1082, 6022}, // Estus Flask

            new() {1300, 6170},
            new() {1500, 6740},
            new() {1510, 41100000},
            new() {1520, 41400000},
            new() {2610, 2620},
            new() {6190, 60001401},
            new() {6230, 60001100},
            new() {6231, 60001105},
            new() {6233, 60001134},
            new() {12000000, 12010000, 12010100, 12030000},
            new() {12010200, 12010300},
            new() {22400000, 22400100},
            new() {22500000, 22500200},
            new() {23000000, 23000100, 23000200, 23000300, 23000400, 23000500},
            new() {23300000, 23300100},
            new() {23700000, 23700100, 23700200},
            new() {23800000, 23800100},
            new() {25000000, 25000300, 25001000, 25002200, 25002500, 25003000, 25003200},
            new() {25000100, 25001100, 25002300, 25003100},
            new() {25000200, 25001200, 25002100, 25002400},
            new() {25400000, 25401000, 25402000},
            new() {25400100, 25401100, 25402100},
            new() {25400200, 25401200, 25402200},
            new() {25500000, 25502000},
            new() {25500100, 25501000, 25502100, 25503100, 25503200},
            new() {25500200, 25502200},
            new() {25600000, 25600300},
            new() {25600200, 25600400},
            new() {25601000, 25601300},
            new() {25601200, 25601400},
            new() {25701000, 25701200},
            new() {25702100, 25702200},
            new() {26900000, 26900200, 26900300},
            new() {27000000, 27000100},
            new() {27800000, 27801000, 27801010, 27801020, 27801030, 27802000, 27802010, 27803000, 27803100},
            new() {27900001, 27905001, 27907001},
            new() {27900101, 27905100},
            new() {27901001, 27903001, 27905300},
            new() {27902001, 27905200},
            new() {29000000, 29001000, 29002000, 29003000},
            new() {29000100, 29001100, 29002100, 29003100},
            new() {29000200, 29001200, 29002200, 29003200},
            new() {29100000, 29100200, 29101100},
            new() {29100100, 29101000, 29101300},
            new() {29300000, 29300100},
            new() {32500000, 32500100},
            new() {32700000, 32700100},
            new() {33000000, 33001000, 33002000, 33003000, 33004000, 33005000, 33006000, 33007000, 33007100, 33007200, 33007300},
            new() {33400000, 33400100, 33400200},
            new() {34100000, 34100100},
            new() {34600000, 34610000},
            new() {35010000, 35010100},
            new() {35200200, 35200500},
            new() {35310000, 35310100},
            new() {53500000, 53500100},
            new() {53500002, 53500101},
            new() {60001133, 60001501},
            new() {60001402, 60006215, 60006302},
            new() {60001403, 60006216, 60006303},
            new() {60001404, 60001106, 60006217, 60006304},
            new() {60002200, 60006502},
            new() {60002201, 60006503},
            new() {60002202, 60006504},
            new() {60002203, 60006505},
            new() {60002204, 60006506},
        };

        private const string ItemIdFieldPattern = "lotItemId0{0}";
        private const string ItemCategoryFieldPattern = "lotItemCategory0{0}";
        private const string ItemWeightFieldPattern = "lotItemBasePoint0{0}";
        private const string ItemAmountFieldPattern = "lotItemNum0{0}";

        public readonly int ID;
        public List<LotSlot> Slots;
        private readonly List<Row> originalRows;
        public LotType type;

        private bool alreadyTaken = false;

        private static Dictionary<int, DSRItemLot> lotCache = new();

        int IItemLot.ID => ID;

        LotType IItemLot.LotType => type;

        IReadOnlyList<LotSlot> IItemLot.Slots => Slots;

        SoulsGame IItemLot.Game => SoulsGame.DSR;

        private DSRItemLot(List<LotSlot> slots, List<Row> originalRows, LotType type)
        {
            ID = originalRows.First().ID;
            Slots = slots;
            this.originalRows = originalRows;
            this.type = type;
        }

        public static DSRItemLot Parse(Row itemLot, LotType lotTypeGuess, PARAM? fullItemLotParamTable)
        {
            if (lotCache.ContainsKey(itemLot.ID))
            {
                return lotCache[itemLot.ID];
            }

            var slots = ParseLotSlots(itemLot);

            // Sometimes two item lots are linked. e.g. Oscar gives the Asylum F2 East key in dialog, or on death.
            // In those cases, link the equivalent lots, so that they always contain the same item.
            var linkedLotIds = linkedItemLots.SingleOrDefault(set => set.Contains(itemLot.ID)) ?? new HashSet<int> { itemLot.ID };
            var linkedLots = linkedLotIds.Select(lotId => fullItemLotParamTable.Rows.Single(row => row.ID == lotId)).ToList();

            var actualLotType = lotTypeGuess == LotType.UnspecifiedEnemy ? slots.Count > 1 ? LotType.RandomEnemyDrop : LotType.GuaranteedEnemyDrop : lotTypeGuess;
            var parsed = new DSRItemLot(slots, linkedLots, actualLotType);
            foreach (var linkedLot in linkedLots)
            {
                lotCache[linkedLot.ID] = parsed;
            }
            return parsed;
        }

        private static List<LotSlot> ParseLotSlots(Row itemLot)
        {
            var tempSlots = new List<LotSlot>();
            for (var i = 1; i <= 8; i++)
            {
                var itemId = Convert.ToInt32(itemLot[string.Format(ItemIdFieldPattern, i)].Value);
                var amount = Convert.ToInt32(itemLot[string.Format(ItemAmountFieldPattern, i)].Value);
                var weight = Convert.ToInt32(itemLot[string.Format(ItemWeightFieldPattern, i)].Value);
                var category = Convert.ToInt32(itemLot[string.Format(ItemCategoryFieldPattern, i)].Value);
                var parsedCategory = category != -1 ? (SoulsItemType)category : SoulsItemType.Goods;

                if (weight > 0)
                {
                    tempSlots.Add(new LotSlot(SoulsGame.DSR, itemId, parsedCategory, weight, amount, isEmptyItem: itemId == 0 || amount == 0 || category == 0xFFFFFFFF));
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

        public bool CanTake()
        {
            return !alreadyTaken;
        }

        public void TakeItems(Queue<LotSlot> unassignedItems)
        {
            if (alreadyTaken)
            {
                return;
            }

            for (var i = 0; i < Slots.Count; i++)
            {
                if (Slots[i].isEmptyItem) continue;

                if (unassignedItems.TryDequeue(out var slot))
                {
                    if (slot.SourceGame == SoulsGame.DSR)
                    {
                        Slots[i] = slot;
                    }
                    else
                    {
                        var resolvedId = CrossGameMappings.GetMappedItem(new SoulsItem(slot.SourceGame, slot.ItemType, slot.ItemId), SoulsGame.DSR);
                        Slots[i] = new LotSlot(SoulsGame.DSR, resolvedId, SoulsItemType.Goods, slot.Weight, slot.Amount);
                    }
                }
            }

            alreadyTaken = true;
        }

        public void Write()
        {
            foreach (var originalRow in originalRows)
            {
                int i = 1;
                foreach (var slot in Slots)
                {
                    originalRow[string.Format(ItemIdFieldPattern, i)].Value = slot.ItemId;
                    originalRow[string.Format(ItemAmountFieldPattern, i)].Value = slot.Amount;
                    // DSR uses a mix of weights adding to 100-ish and weights adding to 1000-ish
                    // 100 seems more common, so use that.
                    originalRow[string.Format(ItemWeightFieldPattern, i)].Value = slot.Weight;
                    originalRow[string.Format(ItemCategoryFieldPattern, i)].Value = slot.ItemType;
                    i++;
                }
            }
        }
    }
}
