using DarkSoulsSeriesComboRandomizer;
using FluentAssertions;
using SoulsFormats;

namespace DarkSoulsSeriesComboRandomizerTests
{
    public class ItemLotTests
    {
        private static readonly PARAMDEF itemLotParamDef = PARAMDEF.XmlDeserialize(@"ConfigFiles\PARAM\DS2S\Defs\ITEM_LOT_PARAM2.xml");
        private static SoulsItem Item() => new(SoulsGame.DS2S, SoulsItemType.Goods, 1);
        private static LotSlot EmptySlot() => new(Item(), 1, 1, true);
        private static LotSlot Slot() => new(Item(), 1, 1);

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
                new TestLotSlotSerializer());
            var itemsToPlace = new Queue<LotSlot>();
            itemsToPlace.Enqueue(Slot());
            itemsToPlace.Enqueue(Slot());
            itemsToPlace.Enqueue(Slot());

            itemLot.TakeItems(itemsToPlace);
            itemLot.CanTake().Should().BeFalse();
        }

        [Theory]
        [InlineData(1)]
        [InlineData(7)]
        public void GetHintsLines_ShouldReturnOneHintsLineForEachNewSlotItem(int lotItemCount)
        {
            var slots = new List<LotSlot>();
            for (int i = 0; i < lotItemCount; i++)
            {
                slots.Add(Slot());
            }
            var itemLot = new ItemLot(
                slots,
                [new PARAM.Row(1, "foo", itemLotParamDef)],
                LotType.RandomEnemyDrop,
                new TestLotSlotSerializer());
            var itemsToPlace = new Queue<LotSlot>();
            for (int i = 0; i <= lotItemCount; i++)
            {
                itemsToPlace.Enqueue(Slot());
            }
            itemLot.TakeItems(itemsToPlace);

            itemLot.GetHintsLines(new TestItemNameLookupService(new Dictionary<SoulsItem, string> { { Item(), "item1" } }), "AMap").Should().HaveCount(lotItemCount);
        }

        [Fact]
        public void GetHintsLines_ShouldSkipEmptySlots()
        {
            var itemLot = new ItemLot(
                [
                    EmptySlot(),
                    Slot()
                ],
                [new PARAM.Row(1, "foo", itemLotParamDef)],
                LotType.RandomEnemyDrop,
                new TestLotSlotSerializer());
            var itemsToPlace = new Queue<LotSlot>();
            itemsToPlace.Enqueue(Slot());
            itemsToPlace.Enqueue(Slot());
            itemLot.TakeItems(itemsToPlace);

            itemLot.GetHintsLines(new TestItemNameLookupService(new Dictionary<SoulsItem, string> { { Item(), "item1" } }), "AMap").Should().HaveCount(1);
        }

        [Theory]
        [InlineData(LotType.Boss, "Fixed Treasure")]
        [InlineData(LotType.GenericEvent, "Fixed Treasure")]
        [InlineData(LotType.GuaranteedEnemyDrop, "Fixed Treasure")]
        [InlineData(LotType.RandomEnemyDrop, "Random Drop")]
        [InlineData(LotType.Store, "Fixed Treasure")]
        [InlineData(LotType.Treasure, "Fixed Treasure")]
        public void GetHintsLines_ShouldAnnotateHintsLinesWithTheAppropriateItemLotDescription(LotType lotType, string description)
        {
            var itemLot = new ItemLot(
                [
                    Slot()
                ],
                [new PARAM.Row(1, "foo", itemLotParamDef)],
                lotType,
                new TestLotSlotSerializer());
            var itemsToPlace = new Queue<LotSlot>();
            itemsToPlace.Enqueue(Slot());
            itemLot.TakeItems(itemsToPlace);

            itemLot.GetHintsLines(new TestItemNameLookupService(new Dictionary<SoulsItem, string> { { Item(), "item1" } }), "AMap").Single().Should().Contain(description);
        }

        [Fact]
        public void GetHintsLines_ShouldIncludeTheGivenMapNameInTheHintLine()
        {
            var itemLot = new ItemLot(
                [
                    Slot()
                ],
                [new PARAM.Row(1, "foo", itemLotParamDef)],
                LotType.RandomEnemyDrop,
                new TestLotSlotSerializer());
            var itemsToPlace = new Queue<LotSlot>();
            itemsToPlace.Enqueue(Slot());
            itemLot.TakeItems(itemsToPlace);

            itemLot.GetHintsLines(new TestItemNameLookupService(new Dictionary<SoulsItem, string> { { Item(), "item1" } }), "AMap").Single().Should().Contain("AMap");
        }

        [Fact]
        public void GetHintsLines_ShouldIncludTheItemNameInTheHintLine()
        {
            var itemLot = new ItemLot(
                [
                    Slot()
                ],
                [new PARAM.Row(1, "foo", itemLotParamDef)],
                LotType.RandomEnemyDrop,
                new TestLotSlotSerializer());
            var itemsToPlace = new Queue<LotSlot>();
            itemsToPlace.Enqueue(Slot());
            itemLot.TakeItems(itemsToPlace);

            itemLot.GetHintsLines(new TestItemNameLookupService(new Dictionary<SoulsItem, string> { { Item(), "item1" } }), "AMap").Single().Should().Contain("item1");
        }
    }
}