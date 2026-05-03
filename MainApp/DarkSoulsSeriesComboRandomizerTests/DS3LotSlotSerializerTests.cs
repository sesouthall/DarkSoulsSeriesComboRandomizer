using DarkSoulsSeriesComboRandomizer;
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
                row[$"ItemLotId{index}"].Value = slot.Item.OriginalId;
                row[$"LotItemCategory0{index}"].Value = slot.Item.ItemType;
                row[$"LotItemBasePoint0{index}"].Value = slot.Weight;
                row[$"LotItemNum{index}"].Value = slot.Amount;
            }

            new DS3LotSlotSerializer([]).ParseFromRow(row, LotType.Treasure).Should().BeEquivalentTo(expectedSlots.Select(slot => slot with { Weight = slot.Weight / 10 }));
        }

        [Fact]
        public void ParseFromRow_ShouldNormalizeDropWeightsToATotalOf100()
        {
            var row = new PARAM.Row(1, "itemLot", itemLotParamDef);
            row[$"ItemLotId1"].Value = 1;
            row[$"LotItemCategory01"].Value = SoulsItemType.Goods;
            row[$"LotItemBasePoint01"].Value = 1;
            row[$"LotItemNum1"].Value = 1;

            new DS3LotSlotSerializer([]).ParseFromRow(row, LotType.Treasure).Should().ContainSingle().Which.Weight.Should().Be(100);
        }

        [Fact]
        public void ParseFromRow_ShouldIgnoreItemsWithNoChanceToDrop()
        {
            var row = new PARAM.Row(1, "itemLot", itemLotParamDef);
            row[$"ItemLotId1"].Value = 1;
            row[$"LotItemCategory01"].Value = SoulsItemType.Goods;
            row[$"LotItemBasePoint01"].Value = 0;
            row[$"LotItemNum1"].Value = 1;

            new DS3LotSlotSerializer([]).ParseFromRow(row, LotType.Treasure).Should().BeEmpty();
        }

        [Fact]
        public void ParseFromRow_ShouldMarkSlotsThatWouldDropZeroOfAnItemAsEmptyDrops()
        {
            var row = new PARAM.Row(1, "itemLot", itemLotParamDef);
            row[$"ItemLotId1"].Value = 1;
            row[$"LotItemCategory01"].Value = SoulsItemType.Goods;
            row[$"LotItemBasePoint01"].Value = 1;
            row[$"LotItemNum1"].Value = 0;

            new DS3LotSlotSerializer([]).ParseFromRow(row, LotType.Treasure).Should().ContainSingle().Which.IsEmptyItem.Should().BeTrue();
        }

        [Fact]
        public void ParseFromRow_ShouldMarkSlotsThatDropItemZeroAsEmptyDrops()
        {
            var row = new PARAM.Row(1, "itemLot", itemLotParamDef);
            row[$"ItemLotId1"].Value = 0;
            row[$"LotItemCategory01"].Value = SoulsItemType.Goods;
            row[$"LotItemBasePoint01"].Value = 1;
            row[$"LotItemNum1"].Value = 1;

            new DS3LotSlotSerializer([]).ParseFromRow(row, LotType.Treasure).Should().ContainSingle().Which.IsEmptyItem.Should().BeTrue();
        }

        [Fact]
        public void ParseFromRow_ShouldMarkSlotsWithCategory0xFFFFFFFFAsEmptyDrops()
        {
            var row = new PARAM.Row(1, "itemLot", itemLotParamDef);
            row[$"ItemLotId1"].Value = 1;
            row[$"LotItemCategory01"].Value = 0xFFFFFFFF;
            row[$"LotItemBasePoint01"].Value = 1;
            row[$"LotItemNum1"].Value = 1;

            new DS3LotSlotSerializer([]).ParseFromRow(row, LotType.Treasure).Should().ContainSingle().Which.IsEmptyItem.Should().BeTrue();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(7)]
        public void WriteToRow_ShouldWriteTheGivenSlotIntoOneMoreThanTheGivenIndexInTheRow(int index)
        {
            var row = new PARAM.Row(1, "itemLot", itemLotParamDef);
            row[$"ItemLotId{index + 1}"].Value = 1;
            row[$"LotItemCategory0{index + 1}"].Value = SoulsItemType.Goods;
            row[$"LotItemBasePoint0{index + 1}"].Value = 1;
            row[$"LotItemNum{index + 1}"].Value = 1;

            new DS3LotSlotSerializer([]).WriteToRow(row, new LotSlot(new SoulsItem(SoulsGame.DS3, SoulsItemType.Armor, 2), 2, 2), index);
            row[$"ItemLotId{index + 1}"].Value.Should().Be(2);
            row[$"LotItemCategory0{index + 1}"].Value.Should().Be((int)SoulsItemType.Armor);
            row[$"LotItemBasePoint0{index + 1}"].Value.Should().Be(20);
            row[$"LotItemNum{index + 1}"].Value.Should().Be(2);
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
            new DS3LotSlotSerializer(new Dictionary<SoulsItem, int> { { ds2Item, 3 } }).WriteToRow(row, new LotSlot(ds2Item, 2, 2), 0);
            row[$"ItemLotId1"].Value.Should().Be(3);
            row[$"LotItemCategory01"].Value.Should().Be((int)SoulsItemType.Goods);
        }
    }
}