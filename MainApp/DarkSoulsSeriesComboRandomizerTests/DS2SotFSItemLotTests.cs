using DarkSoulsSeriesComboRandomizer;
using DarkSoulsSeriesComboRandomizer.DarkSouls2SotFS;
using FluentAssertions;
using SoulsFormats;

namespace DarkSoulsSeriesComboRandomizerTests
{
    public class DS2SotFSItemLotTests
    {
        [Fact]
        public void TakeItems_ShouldOnlyTakeAsManyItemsAsTheItemLotOriginallyHad()
        {
            var paramDef = PARAMDEF.XmlDeserialize(@"ConfigFiles\PARAM\DS2S\Defs\ITEM_LOT_PARAM2.xml");
            var itemLot = new DS2SotFSItemLot(
                [
                    new LotSlot(new SoulsItem(SoulsGame.DS2S, SoulsItemType.Goods, 1), 1, 1, true),
                    new LotSlot(new SoulsItem(SoulsGame.DS2S, SoulsItemType.Goods, 1), 1, 1, false)
                ],
                [new PARAM.Row(1, "foo", paramDef)],
                LotType.RandomEnemyDrop);
            var itemsToPlace = new Queue<LotSlot>();
            itemsToPlace.Enqueue(new LotSlot(new SoulsItem(SoulsGame.DS2S, SoulsItemType.Goods, 1), 1, 1, false));
            itemsToPlace.Enqueue(new LotSlot(new SoulsItem(SoulsGame.DS2S, SoulsItemType.Goods, 1), 1, 1, false));
            itemsToPlace.Enqueue(new LotSlot(new SoulsItem(SoulsGame.DS2S, SoulsItemType.Goods, 1), 1, 1, false));

            itemLot.TakeItems(itemsToPlace);
            itemsToPlace.Should().HaveCount(2);
            itemLot.TakeItems(itemsToPlace);
            itemsToPlace.Should().HaveCount(2);
        }

        [Fact]
        public void TakeItems_ShouldFillTheRestOfTheSlotsWhenPartiallyFilled()
        {
            var paramDef = PARAMDEF.XmlDeserialize(@"ConfigFiles\PARAM\DS2S\Defs\ITEM_LOT_PARAM2.xml");
            var itemLot = new DS2SotFSItemLot(
                [
                    new LotSlot(new SoulsItem(SoulsGame.DS2S, SoulsItemType.Goods, 1), 1, 1, true),
                    new LotSlot(new SoulsItem(SoulsGame.DS2S, SoulsItemType.Goods, 1), 1, 1, false),
                    new LotSlot(new SoulsItem(SoulsGame.DS2S, SoulsItemType.Goods, 1), 1, 1, false)
                ],
                [new PARAM.Row(1, "foo", paramDef)],
                LotType.RandomEnemyDrop);
            var itemsToPlace = new Queue<LotSlot>();
            itemsToPlace.Enqueue(new LotSlot(new SoulsItem(SoulsGame.DS2S, SoulsItemType.Goods, 1), 1, 1, false));
            
            itemLot.TakeItems(itemsToPlace, partialFill: true);
            itemsToPlace.Should().BeEmpty();

            itemsToPlace.Enqueue(new LotSlot(new SoulsItem(SoulsGame.DS2S, SoulsItemType.Goods, 1), 1, 1, false));
            itemsToPlace.Enqueue(new LotSlot(new SoulsItem(SoulsGame.DS2S, SoulsItemType.Goods, 1), 1, 1, false));

            itemLot.TakeItems(itemsToPlace);
            itemsToPlace.Should().HaveCount(1);
            itemLot.TakeItems(itemsToPlace);
            itemsToPlace.Should().HaveCount(1);
        }
    }
}