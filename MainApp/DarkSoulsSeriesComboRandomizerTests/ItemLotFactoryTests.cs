using DarkSoulsSeriesComboRandomizer;
using FluentAssertions;
using SoulsFormats;

namespace DarkSoulsSeriesComboRandomizerTests
{
    public class ItemLotFactoryTests
    {
        private static readonly PARAMDEF itemLotParamDef = PARAMDEF.XmlDeserialize(@"ConfigFiles\PARAM\DS2S\Defs\ITEM_LOT_PARAM2.xml");

        [Fact]
        public void ParseChain_ShouldParseAllLotsWithSequentialIdsInTheGivenPARAM()
        {
            var itemLotParam = new PARAM
            {
                Rows =
                [
                    new PARAM.Row(1, "itemLot", itemLotParamDef),
                    new PARAM.Row(2, "itemLot", itemLotParamDef),
                    new PARAM.Row(4, "itemLot", itemLotParamDef),
                ]
            };

            var testSerializer = new TestLotSlotSerializer();
            var lotFactory = new ItemLotFactory(SoulsGame.DSR, [], testSerializer);
            lotFactory.ParseChain(1, itemLotParam, LotType.Treasure).Should().HaveCount(2);
        }

        [Fact]
        public void ParseChain_ShouldLinkItemLotsTogetherBasedOnTheLinkedLotSetsItWasGiven()
        {
            var itemLotParam = new PARAM
            {
                Rows =
                [
                    new PARAM.Row(1, "itemLot", itemLotParamDef),
                    new PARAM.Row(2, "itemLot", itemLotParamDef),
                    new PARAM.Row(4, "itemLot", itemLotParamDef),
                ]
            };

            var testSerializer = new TestLotSlotSerializer();
            var lotFactory = new ItemLotFactory(SoulsGame.DSR, [[1,4]], testSerializer);
            var lot1 = lotFactory.ParseChain(1, itemLotParam, LotType.Treasure).First();
            var lot4 = lotFactory.ParseChain(4, itemLotParam, LotType.Treasure).First();
            lot1.Should().BeSameAs(lot4);
        }

        [Fact]
        public void Parse_ShouldAskTheGivenSerializerToDeserializeTheGivenRow()
        {
            var row = new PARAM.Row(1, "itemLot", itemLotParamDef);

            var testSerializer = new TestLotSlotSerializer();
            var lotFactory = new ItemLotFactory(SoulsGame.DSR, [], testSerializer);
            var _ = lotFactory.Parse([row], LotType.Treasure);
            testSerializer.parsedRows.Should().Contain(row);
        }

        [Fact]
        public void Parse_ShouldOnlyDeserializeOneGivenRowWhenMultipleAreProvided()
        {
            var row = new PARAM.Row(1, "itemLot", itemLotParamDef);
            var linkedRow = new PARAM.Row(2, "itemLot", itemLotParamDef);

            var testSerializer = new TestLotSlotSerializer();
            var lotFactory = new ItemLotFactory(SoulsGame.DSR, [], testSerializer);
            var _ = lotFactory.Parse([row, linkedRow], LotType.Treasure);
            testSerializer.parsedRows.Should().HaveCount(1);
        }

        [Fact]
        public void Parse_ShouldReturnAnItemLotWithOriginalSlotsEqualToTheSerializersReturnedSlots()
        {
            var row = new PARAM.Row(1, "itemLot", itemLotParamDef);

            var slot = new LotSlot(new SoulsItem(SoulsGame.DSR, SoulsItemType.Accessory, 1), 1, 1);
            var testSerializer = new TestLotSlotSerializer([slot]);
            var lotFactory = new ItemLotFactory(SoulsGame.DSR, [], testSerializer);
            var parsedLot = lotFactory.Parse([row], LotType.Treasure);
            parsedLot.OriginalSlots.Should().HaveCount(1).And.Contain(slot);
        }

        [Fact]
        public void Parse_ShouldReturnExactlyTheSameItemLotWhenParsingRowsWithTheSameId()
        {
            var row1 = new PARAM.Row(1, "itemLot", itemLotParamDef);
            var row1Copy = new PARAM.Row(1, "itemLot", itemLotParamDef);
            var lotFactory = new ItemLotFactory(SoulsGame.DSR, [], new TestLotSlotSerializer());
            lotFactory.Parse([row1], LotType.Treasure).Should().BeSameAs(lotFactory.Parse([row1Copy], LotType.Treasure));
        }

        [Fact]
        public void Parse_ShouldReturnExactlyTheSameItemLotWhenParsingRowsWithLinkedLotIds()
        {
            var row = new PARAM.Row(1, "itemLot", itemLotParamDef);
            var linkedRow = new PARAM.Row(2, "itemLot", itemLotParamDef);
            var lotFactory = new ItemLotFactory(SoulsGame.DSR, [[1, 2]], new TestLotSlotSerializer());
            lotFactory.Parse([row, linkedRow], LotType.Treasure).Should().BeSameAs(lotFactory.Parse([linkedRow], LotType.Treasure));
        }
    }
}