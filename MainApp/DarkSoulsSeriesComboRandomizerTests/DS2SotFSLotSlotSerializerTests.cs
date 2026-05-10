using DarkSoulsSeriesComboRandomizer;
using DarkSoulsSeriesComboRandomizer.DarkSouls2SotFS;
using FluentAssertions;
using SoulsFormats;

namespace DarkSoulsSeriesComboRandomizerTests
{
    public class DS2SotFSLotSlotSerializerTests
    {
        private static readonly PARAMDEF itemLotParamDef = PARAMDEF.XmlDeserialize(@"ConfigFiles\PARAM\DS2S\Defs\ITEM_LOT_PARAM2.xml");

        public static List<object[]> LotSlotSets() => [
            [new List<LotSlot> { new(new SoulsItem(SoulsGame.DS2S, SoulsItemType.Goods, 1), 1, 1) }],
            [new List<LotSlot> { new(new SoulsItem(SoulsGame.DS2S, SoulsItemType.Goods, 1), 1, 1), new(new SoulsItem(SoulsGame.DS2S, SoulsItemType.Goods, 1), 1, 1) }],
        ];

        [Theory]
        [MemberData(nameof(LotSlotSets))]
        public void ParseFromRow_ShouldReturnLotSlotsBasedOnTheGivenRow(List<LotSlot> expectedSlots)
        {
            var row = new PARAM.Row(1, "itemLot", itemLotParamDef);
            foreach (var (slot, index) in expectedSlots.Select((slot, i) => (slot, i)))
            {
                row[$"item_lot_{index}"].Value = slot.Item.Id;
                row[$"chance_lot_{index}"].Value = slot.Weight;
                row[$"amount_lot_{index}"].Value = slot.Amount;
            }

            new DS2SotFSLotSlotSerializer(new CrossGameMappings([], [], [])).ParseFromRow(row, LotType.Treasure).Should().BeEquivalentTo(expectedSlots);
        }

        [Fact]
        public void ParseFromRow_ShouldIgnoreItemsWithNoChanceToDrop()
        {
            var row = new PARAM.Row(1, "itemLot", itemLotParamDef);
            row[$"item_lot_0"].Value = 1;
            row[$"chance_lot_0"].Value = 0;
            row[$"amount_lot_0"].Value = 1;

            new DS2SotFSLotSlotSerializer(new CrossGameMappings([], [], [])).ParseFromRow(row, LotType.Treasure).Should().BeEmpty();
        }

        [Fact]
        public void ParseFromRow_ShouldMarkSlotsThatWouldDropZeroOfAnItemAsEmptyDrops()
        {
            var row = new PARAM.Row(1, "itemLot", itemLotParamDef);
            row[$"item_lot_0"].Value = 1;
            row[$"chance_lot_0"].Value = 1;
            row[$"amount_lot_0"].Value = 0;

            new DS2SotFSLotSlotSerializer(new CrossGameMappings([], [], [])).ParseFromRow(row, LotType.Treasure).Should().ContainSingle().Which.IsEmptyItem.Should().BeTrue();
        }

        [Fact]
        public void ParseFromRow_ShouldMarkSlotsThatDropItemZeroAsEmptyDrops()
        {
            var row = new PARAM.Row(1, "itemLot", itemLotParamDef);
            row[$"item_lot_0"].Value = 0;
            row[$"chance_lot_0"].Value = 1;
            row[$"amount_lot_0"].Value = 1;

            new DS2SotFSLotSlotSerializer(new CrossGameMappings([], [], [])).ParseFromRow(row, LotType.Treasure).Should().ContainSingle().Which.IsEmptyItem.Should().BeTrue();
        }

        [Theory]
        [InlineData(LotType.Treasure, 10)]
        [InlineData(LotType.UnspecifiedEnemy, 60510000)]
        public void ParseFromRow_ShouldMarkSlotsThatDropTheEmptyItemSentinelForTheirDropTypeAsEmptyDrops(LotType lotType, int emptyItemSentinelId)
        {
            var row = new PARAM.Row(1, "itemLot", itemLotParamDef);
            row[$"item_lot_0"].Value = emptyItemSentinelId;
            row[$"chance_lot_0"].Value = 1;
            row[$"amount_lot_0"].Value = 1;

            new DS2SotFSLotSlotSerializer(new CrossGameMappings([], [], [])).ParseFromRow(row, lotType).Should().ContainSingle().Which.IsEmptyItem.Should().BeTrue();
        }

        [Fact]
        public void WriteToRow_ShouldWriteTheGivenSlotsIntoTheRow()
        {
            var row = new PARAM.Row(1, "itemLot", itemLotParamDef);
            for (int i = 0; i < 10; i++)
            {
                row[$"item_lot_{i}"].Value = 99;
                row[$"chance_lot_{i}"].Value = 99;
                row[$"amount_lot_{i}"].Value = 99;
            }

            new DS2SotFSLotSlotSerializer(new CrossGameMappings([], [], [])).WriteToRow(row, 
                [
                    new LotSlot(new SoulsItem(SoulsGame.DS2S, SoulsItemType.Goods, 0), 0, 0),
                    new LotSlot(new SoulsItem(SoulsGame.DS2S, SoulsItemType.Goods, 1), 1, 1),
                    new LotSlot(new SoulsItem(SoulsGame.DS2S, SoulsItemType.Goods, 2), 2, 2),
                    new LotSlot(new SoulsItem(SoulsGame.DS2S, SoulsItemType.Goods, 3), 3, 3),
                    new LotSlot(new SoulsItem(SoulsGame.DS2S, SoulsItemType.Goods, 4), 4, 4),
                    new LotSlot(new SoulsItem(SoulsGame.DS2S, SoulsItemType.Goods, 5), 5, 5),
                    new LotSlot(new SoulsItem(SoulsGame.DS2S, SoulsItemType.Goods, 6), 6, 6),
                    new LotSlot(new SoulsItem(SoulsGame.DS2S, SoulsItemType.Goods, 7), 7, 7),
                    new LotSlot(new SoulsItem(SoulsGame.DS2S, SoulsItemType.Goods, 8), 8, 8),
                    new LotSlot(new SoulsItem(SoulsGame.DS2S, SoulsItemType.Goods, 9), 9, 9),
                ]);

            for (int i = 0; i < 10; i++)
            {
                row[$"item_lot_{i}"].Value.Should().Be(i);
                row[$"chance_lot_{i}"].Value.Should().Be(i);
                row[$"amount_lot_{i}"].Value.Should().Be(i);
            }
        }

        [Fact]
        public void WriteToRow_ShouldTranslateItemsFromOtherGamesToTheEquivalentDS2ItemBeforeWritingToTheRow()
        {
            var row = new PARAM.Row(1, "itemLot", itemLotParamDef);
            row[$"item_lot_0"].Value = 1;
            row[$"chance_lot_0"].Value = 1;
            row[$"amount_lot_0"].Value = 1;

            var ds1Item = new SoulsItem(SoulsGame.DSR, SoulsItemType.Armor, 2);
            new DS2SotFSLotSlotSerializer(new CrossGameMappings([], new Dictionary<int, SoulsItem> { { 3, ds1Item } }, [])).WriteToRow(row, [new LotSlot(ds1Item, 2, 2)]);
            row[$"item_lot_0"].Value.Should().Be(3);
        }
    }
}