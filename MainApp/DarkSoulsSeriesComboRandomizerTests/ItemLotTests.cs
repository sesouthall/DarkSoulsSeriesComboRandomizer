using DarkSoulsSeriesComboRandomizer;
using FluentAssertions;
using SoulsFormats;

namespace DarkSoulsSeriesComboRandomizerTests
{
    public class ItemLotTests
    {
        private static readonly PARAMDEF itemLotParamDef = PARAMDEF.XmlDeserialize(@"ConfigFiles\PARAM\DS2S\Defs\ITEM_LOT_PARAM2.xml");
        private static LotSlot EmptySlot() => new(new SoulsItem(SoulsGame.DS2S, SoulsItemType.Goods, 1), 1, 1, true);
        private static LotSlot Slot() => new(new SoulsItem(SoulsGame.DS2S, SoulsItemType.Goods, 1), 1, 1);

        [Fact]
        public void TakeItems_ShouldOnlyTakeAsManyItemsAsTheItemLotOriginallyHad()
        {
            var itemLot = new ItemLot(
                [
                    EmptySlot(),
                    Slot()
                ],
                [new PARAM.Row(1, "foo", itemLotParamDef)],
                LotType.RandomEnemyDrop,
                SoulsGame.DS2S,
                new TestLotSlotSerializer());
            var itemsToPlace = new Queue<LotSlot>();
            itemsToPlace.Enqueue(Slot());
            itemsToPlace.Enqueue(Slot());
            itemsToPlace.Enqueue(Slot());

            itemLot.TakeItems(itemsToPlace);
            itemsToPlace.Should().HaveCount(2);
            itemLot.TakeItems(itemsToPlace);
            itemsToPlace.Should().HaveCount(2);
        }

        [Fact]
        public void TakeItems_ShouldFillTheRestOfTheSlotsWhenPartiallyFilled()
        {
            var itemLot = new ItemLot(
                [
                    EmptySlot(),
                    Slot(),
                    Slot()
                ],
                [new PARAM.Row(1, "foo", itemLotParamDef)],
                LotType.RandomEnemyDrop,
                SoulsGame.DS2S,
                new TestLotSlotSerializer());
            var itemsToPlace = new Queue<LotSlot>();
            itemsToPlace.Enqueue(Slot());

            itemLot.TakeItems(itemsToPlace, partialFill: true);
            itemsToPlace.Should().BeEmpty();

            itemsToPlace.Enqueue(Slot());
            itemsToPlace.Enqueue(Slot());

            itemLot.TakeItems(itemsToPlace);
            itemsToPlace.Should().HaveCount(1);
            itemLot.TakeItems(itemsToPlace);
            itemsToPlace.Should().HaveCount(1);
        }

        [Fact]
        public void CanTake_ShouldReturnTrueWhenNoItemsHaveBeenTaken()
        {
            var itemLot = new ItemLot(
                [
                    EmptySlot(),
                    Slot(),
                    Slot()
                ],
                [new PARAM.Row(1, "foo", itemLotParamDef)],
                LotType.RandomEnemyDrop,
                SoulsGame.DS2S,
                new TestLotSlotSerializer());
            var itemsToPlace = new Queue<LotSlot>();
            itemsToPlace.Enqueue(Slot());

            itemLot.CanTake().Should().BeTrue();
        }

        [Fact]
        public void CanTake_ShouldReturnTrueWhenTheSlotsArePartiallyFilled()
        {
            var itemLot = new ItemLot(
                [
                    EmptySlot(),
                    Slot(),
                    Slot()
                ],
                [new PARAM.Row(1, "foo", itemLotParamDef)],
                LotType.RandomEnemyDrop,
                SoulsGame.DS2S,
                new TestLotSlotSerializer());
            var itemsToPlace = new Queue<LotSlot>();
            itemsToPlace.Enqueue(Slot());

            itemLot.TakeItems(itemsToPlace, partialFill: true);
            itemLot.CanTake().Should().BeTrue();
        }

        [Fact]
        public void CanTake_ShouldReturnFalseWhenTheSlotsAreFilled()
        {
            var itemLot = new ItemLot(
                [
                    EmptySlot(),
                    Slot()
                ],
                [new PARAM.Row(1, "foo", itemLotParamDef)],
                LotType.RandomEnemyDrop,
                SoulsGame.DS2S,
                new TestLotSlotSerializer());
            var itemsToPlace = new Queue<LotSlot>();
            itemsToPlace.Enqueue(Slot());
            itemsToPlace.Enqueue(Slot());
            itemsToPlace.Enqueue(Slot());

            itemLot.TakeItems(itemsToPlace);
            itemLot.CanTake().Should().BeFalse();
        }
    }
}