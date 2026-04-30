using DarkSoulsSeriesComboRandomizer;
using DarkSoulsSeriesComboRandomizer.DarkSoulsRemastered;
using FluentAssertions;
using SoulsFormats;

namespace DarkSoulsSeriesComboRandomizerTests
{
    public class DSRItemLotTests
    {
        [Fact]
        public void TakeItems_ShouldOnlyTakeAsManyItemsAsTheItemLotOriginallyHad()
        {
            var paramDef = PARAMDEF.XmlDeserialize(@"ConfigFiles\PARAM\DS1R\Defs\ItemLotParam.xml");
            var itemLot = new DSRItemLot(
                [
                    new LotSlot(new SoulsItem(SoulsGame.DSR, SoulsItemType.Goods, 1), 1, 1, true),
                    new LotSlot(new SoulsItem(SoulsGame.DSR, SoulsItemType.Goods, 1), 1, 1, false)
                ],
                [new PARAM.Row(1, "foo", paramDef)],
                LotType.RandomEnemyDrop);
            var itemsToPlace = new Queue<LotSlot>();
            itemsToPlace.Enqueue(new LotSlot(new SoulsItem(SoulsGame.DSR, SoulsItemType.Goods, 1), 1, 1, false));
            itemsToPlace.Enqueue(new LotSlot(new SoulsItem(SoulsGame.DSR, SoulsItemType.Goods, 1), 1, 1, false));
            itemsToPlace.Enqueue(new LotSlot(new SoulsItem(SoulsGame.DSR, SoulsItemType.Goods, 1), 1, 1, false));

            itemLot.TakeItems(itemsToPlace);
            itemsToPlace.Should().HaveCount(2);
            itemLot.TakeItems(itemsToPlace);
            itemsToPlace.Should().HaveCount(2);
        }
    }
}