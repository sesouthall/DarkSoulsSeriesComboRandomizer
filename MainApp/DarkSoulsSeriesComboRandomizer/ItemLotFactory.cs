using SoulsFormats;
using static SoulsFormats.PARAM;

namespace DarkSoulsSeriesComboRandomizer
{
    public class ItemLotFactory(List<HashSet<int>> linkedItemLots, ILotSlotSerializer lotSerializer)
    {
        private readonly Dictionary<int, ItemLot> lotCache = [];

        public IEnumerable<ItemLot> ParseChain(int firstIdInChain, PARAM itemLotParam, LotType lotTypeGuess)
        {
            Row? itemLot;
            var currentLotNumber = firstIdInChain;
            while ((itemLot = itemLotParam.Rows.SingleOrDefault(row => row.ID == currentLotNumber)) != null)
            {
                var linkedLots = linkedItemLots.SingleOrDefault(set => set.Contains(itemLot.ID))?
                    .Select(id => itemLotParam.Rows.SingleOrDefault(row => row.ID == id))
                    .Where(row => row != null)
                    .ToList()
                    ?? [itemLot];
                yield return Parse(linkedLots!, lotTypeGuess);
                currentLotNumber++;
            }
        }

        public ItemLot Parse(List<Row> linkedItemLots, LotType lotTypeGuess)
        {
            if (lotCache.TryGetValue(linkedItemLots.First().ID, out ItemLot? value))
            {
                return value;
            }

            var slots = lotSerializer.ParseFromRow(linkedItemLots.First(), lotTypeGuess);

            var actualLotType = lotTypeGuess == LotType.UnspecifiedEnemy ?
                (slots.Count(slot => slot.Weight < 100) > 1 ? LotType.RandomEnemyDrop : LotType.GuaranteedEnemyDrop) :
                lotTypeGuess;
            var parsed = new ItemLot(slots, linkedItemLots, actualLotType, lotSerializer);
            foreach (var linkedLot in linkedItemLots)
            {
                lotCache[linkedLot.ID] = parsed;
            }
            return parsed;
        }
    }
}
