using DarkSoulsSeriesComboRandomizer;
using DarkSoulsSeriesComboRandomizer.DarkSouls2SotFS;
using DarkSoulsSeriesComboRandomizer.DarkSouls3;
using FluentAssertions;
using SoulsFormats;

namespace DarkSoulsSeriesComboRandomizerTests
{
    public class DS3LotSlotSerializerTests
    {
        private static readonly PARAMDEF itemLotParamDef = PARAMDEF.XmlDeserialize(@"ConfigFiles\PARAM\DS3\Defs\ItemLotParam.xml");

        public static List<object[]> LotSlotSets() => [
            [new List<LotSlot> { new(new SoulsItem(SoulsGame.DS3, SoulsItemType.Goods, 1), 1000, 1) }],
            [new List<LotSlot> { new(new SoulsItem(SoulsGame.DS3, SoulsItemType.Goods, 1), 500, 1), new(new SoulsItem(SoulsGame.DS3, SoulsItemType.Goods, 1), 500, 1) }],
        ];

        [Theory]
        [MemberData(nameof(LotSlotSets))]
        public void ParseFromRow_ShouldReturnLotSlotsBasedOnTheGivenRow(List<LotSlot> expectedSlots)
        {
            var row = new PARAM.Row(1, "itemLot", itemLotParamDef);
            foreach (var (slot, index) in expectedSlots.Select((slot, i) => (slot, i + 1)))
            {
                row[$"ItemLotId{index}"].Value = slot.Item.Id;
                row[$"LotItemCategory0{index}"].Value = slot.Item.Type;
                row[$"LotItemBasePoint0{index}"].Value = slot.Weight;
                row[$"LotItemNum{index}"].Value = slot.Amount;
            }

            new DS3LotSlotSerializer(new CrossGameMappings([], [], [])).ParseFromRow(row, LotType.Treasure).Should().BeEquivalentTo(expectedSlots.Select(slot => slot with { Weight = slot.Weight / 10 }));
        }

        [Fact]
        public void ParseFromRow_ShouldNormalizeDropWeightsToATotalOf100()
        {
            var row = new PARAM.Row(1, "itemLot", itemLotParamDef);
            row[$"ItemLotId1"].Value = 1;
            row[$"LotItemCategory01"].Value = SoulsItemType.Goods;
            row[$"LotItemBasePoint01"].Value = 1;
            row[$"LotItemNum1"].Value = 1;

            new DS3LotSlotSerializer(new CrossGameMappings([], [], [])).ParseFromRow(row, LotType.Treasure).Should().ContainSingle().Which.Weight.Should().Be(100);
        }

        [Fact]
        public void ParseFromRow_ShouldIgnoreItemsWithNoChanceToDrop()
        {
            var row = new PARAM.Row(1, "itemLot", itemLotParamDef);
            row[$"ItemLotId1"].Value = 1;
            row[$"LotItemCategory01"].Value = SoulsItemType.Goods;
            row[$"LotItemBasePoint01"].Value = 0;
            row[$"LotItemNum1"].Value = 1;

            new DS3LotSlotSerializer(new CrossGameMappings([], [], [])).ParseFromRow(row, LotType.Treasure).Should().BeEmpty();
        }

        [Fact]
        public void ParseFromRow_ShouldMarkSlotsThatWouldDropZeroOfAnItemAsEmptyDrops()
        {
            var row = new PARAM.Row(1, "itemLot", itemLotParamDef);
            row[$"ItemLotId1"].Value = 1;
            row[$"LotItemCategory01"].Value = SoulsItemType.Goods;
            row[$"LotItemBasePoint01"].Value = 1;
            row[$"LotItemNum1"].Value = 0;

            new DS3LotSlotSerializer(new CrossGameMappings([], [], [])).ParseFromRow(row, LotType.Treasure).Should().ContainSingle().Which.IsEmptyItem.Should().BeTrue();
        }

        [Fact]
        public void ParseFromRow_ShouldMarkSlotsThatDropItemZeroAsEmptyDrops()
        {
            var row = new PARAM.Row(1, "itemLot", itemLotParamDef);
            row[$"ItemLotId1"].Value = 0;
            row[$"LotItemCategory01"].Value = SoulsItemType.Goods;
            row[$"LotItemBasePoint01"].Value = 1;
            row[$"LotItemNum1"].Value = 1;

            new DS3LotSlotSerializer(new CrossGameMappings([], [], [])).ParseFromRow(row, LotType.Treasure).Should().ContainSingle().Which.IsEmptyItem.Should().BeTrue();
        }

        [Fact]
        public void ParseFromRow_ShouldMarkSlotsWithCategory0xFFFFFFFFAsEmptyDrops()
        {
            var row = new PARAM.Row(1, "itemLot", itemLotParamDef);
            row[$"ItemLotId1"].Value = 1;
            row[$"LotItemCategory01"].Value = 0xFFFFFFFF;
            row[$"LotItemBasePoint01"].Value = 1;
            row[$"LotItemNum1"].Value = 1;

            new DS3LotSlotSerializer(new CrossGameMappings([], [], [])).ParseFromRow(row, LotType.Treasure).Should().ContainSingle().Which.IsEmptyItem.Should().BeTrue();
        }

        [Fact]
        public void WriteToRow_ShouldWriteTheGivenSlotsIntoTheRow()
        {
            var row = new PARAM.Row(1, "itemLot", itemLotParamDef);
            for (int i = 1; i < 9; i++)
            {
                row[$"ItemLotId{i}"].Value = 99;
                row[$"LotItemCategory0{i}"].Value = (int)SoulsItemType.Weapon;
                row[$"LotItemBasePoint0{i}"].Value = 99;
                row[$"LotItemNum{i}"].Value = 99;
            }

            new DS3LotSlotSerializer(new CrossGameMappings([], [], [])).WriteToRow(row,
                [
                    new LotSlot(new SoulsItem(SoulsGame.DS3, SoulsItemType.Goods, 1), 1, 1),
                    new LotSlot(new SoulsItem(SoulsGame.DS3, SoulsItemType.Goods, 2), 2, 2),
                    new LotSlot(new SoulsItem(SoulsGame.DS3, SoulsItemType.Goods, 3), 3, 3),
                    new LotSlot(new SoulsItem(SoulsGame.DS3, SoulsItemType.Goods, 4), 4, 4),
                    new LotSlot(new SoulsItem(SoulsGame.DS3, SoulsItemType.Goods, 5), 5, 5),
                    new LotSlot(new SoulsItem(SoulsGame.DS3, SoulsItemType.Goods, 6), 6, 6),
                    new LotSlot(new SoulsItem(SoulsGame.DS3, SoulsItemType.Goods, 7), 7, 7),
                    new LotSlot(new SoulsItem(SoulsGame.DS3, SoulsItemType.Goods, 8), 8, 8),
                ]);

            for (int i = 1; i < 9; i++)
            {
                row[$"ItemLotId{i}"].Value.Should().Be(i);
                row[$"LotItemCategory0{i}"].Value.Should().Be((int)SoulsItemType.Goods);
                row[$"LotItemBasePoint0{i}"].Value.Should().Be(i*10);
                row[$"LotItemNum{i}"].Value.Should().Be(i);
            }
        }

        [Fact]
        public void WriteToRow_ShouldTranslateItemsFromOtherGamesToTheEquivalentDS2ItemBeforeWritingToTheRow()
        {
            var row = new PARAM.Row(1, "itemLot", itemLotParamDef);
            row[$"ItemLotId1"].Value = 1;
            row[$"LotItemCategory01"].Value = SoulsItemType.Weapon;
            row[$"LotItemBasePoint01"].Value = 1;
            row[$"LotItemNum1"].Value = 1;

            var ds2Item = new SoulsItem(SoulsGame.DS2S, SoulsItemType.Armor, 2);
            new DS3LotSlotSerializer(new CrossGameMappings([], [], new Dictionary<int, SoulsItem> { { 3, ds2Item } })).WriteToRow(row, [new LotSlot(ds2Item, 2, 2)]);
            row[$"ItemLotId1"].Value.Should().Be(3);
            row[$"LotItemCategory01"].Value.Should().Be((int)SoulsItemType.Goods);
        }
    }
}