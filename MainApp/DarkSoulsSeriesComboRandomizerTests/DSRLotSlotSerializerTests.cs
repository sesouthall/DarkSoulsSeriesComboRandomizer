using DarkSoulsSeriesComboRandomizer;
using DarkSoulsSeriesComboRandomizer.DarkSoulsRemastered;
using FluentAssertions;
using SoulsFormats;

namespace DarkSoulsSeriesComboRandomizerTests
{
    public class DSRLotSlotSerializerTests
    {
        private static readonly PARAMDEF itemLotParamDef = PARAMDEF.XmlDeserialize(@"ConfigFiles\PARAM\DS1R\Defs\ItemLotParam.xml");

        public static List<object[]> LotSlotSets() => [
            [new List<LotSlot> { new(new SoulsItem(SoulsGame.DSR, SoulsItemType.Goods, 1), 100, 1) }],
            [new List<LotSlot> { new(new SoulsItem(SoulsGame.DSR, SoulsItemType.Goods, 1), 50, 1), new(new SoulsItem(SoulsGame.DSR, SoulsItemType.Goods, 1), 50, 1) }],
        ];

        [Theory]
        [MemberData(nameof(LotSlotSets))]
        public void ParseFromRow_ShouldReturnLotSlotsBasedOnTheGivenRow(List<LotSlot> expectedSlots)
        {
            var row = new PARAM.Row(1, "itemLot", itemLotParamDef);
            foreach (var (slot, index) in expectedSlots.Select((slot, i) => (slot, i+1)))
            {
                row[$"lotItemId0{index}"].Value = slot.Item.OriginalId;
                row[$"lotItemCategory0{index}"].Value = slot.Item.ItemType;
                row[$"lotItemBasePoint0{index}"].Value = slot.Weight;
                row[$"lotItemNum0{index}"].Value = slot.Amount;
            }

            new DSRLotSlotSerializer([]).ParseFromRow(row, LotType.Treasure).Should().BeEquivalentTo(expectedSlots);
        }

        [Fact]
        public void ParseFromRow_ShouldNormalizeDropWeightsToATotalOf100()
        {
            var row = new PARAM.Row(1, "itemLot", itemLotParamDef);
            row[$"lotItemId01"].Value = 1;
            row[$"lotItemCategory01"].Value = SoulsItemType.Goods;
            row[$"lotItemBasePoint01"].Value = 1;
            row[$"lotItemNum01"].Value = 1;

            new DSRLotSlotSerializer([]).ParseFromRow(row, LotType.Treasure).Should().ContainSingle().Which.Weight.Should().Be(100);
        }

        [Fact]
        public void ParseFromRow_ShouldIgnoreItemsWithNoChanceToDrop()
        {
            var row = new PARAM.Row(1, "itemLot", itemLotParamDef);
            row[$"lotItemId01"].Value = 1;
            row[$"lotItemCategory01"].Value = SoulsItemType.Goods;
            row[$"lotItemBasePoint01"].Value = 0;
            row[$"lotItemNum01"].Value = 1;

            new DSRLotSlotSerializer([]).ParseFromRow(row, LotType.Treasure).Should().BeEmpty();
        }

        [Fact]
        public void ParseFromRow_ShouldMarkSlotsThatWouldDropZeroOfAnItemAsEmptyDrops()
        {
            var row = new PARAM.Row(1, "itemLot", itemLotParamDef);
            row[$"lotItemId01"].Value = 1;
            row[$"lotItemCategory01"].Value = SoulsItemType.Goods;
            row[$"lotItemBasePoint01"].Value = 1;
            row[$"lotItemNum01"].Value = 0;

            new DSRLotSlotSerializer([]).ParseFromRow(row, LotType.Treasure).Should().ContainSingle().Which.IsEmptyItem.Should().BeTrue();
        }

        [Fact]
        public void ParseFromRow_ShouldMarkSlotsThatDropItemZeroAsEmptyDrops()
        {
            var row = new PARAM.Row(1, "itemLot", itemLotParamDef);
            row[$"lotItemId01"].Value = 0;
            row[$"lotItemCategory01"].Value = SoulsItemType.Goods;
            row[$"lotItemBasePoint01"].Value = 1;
            row[$"lotItemNum01"].Value = 1;

            new DSRLotSlotSerializer([]).ParseFromRow(row, LotType.Treasure).Should().ContainSingle().Which.IsEmptyItem.Should().BeTrue();
        }

        [Fact]
        public void ParseFromRow_ShouldMarkSlotsWithCategoryNegativeOneAsEmptyDrops()
        {
            var row = new PARAM.Row(1, "itemLot", itemLotParamDef);
            row[$"lotItemId01"].Value = 1;
            row[$"lotItemCategory01"].Value = -1;
            row[$"lotItemBasePoint01"].Value = 1;
            row[$"lotItemNum01"].Value = 1;

            new DSRLotSlotSerializer([]).ParseFromRow(row, LotType.Treasure).Should().ContainSingle().Which.IsEmptyItem.Should().BeTrue();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(7)]
        public void WriteToRow_ShouldWriteTheGivenSlotIntoOneMoreThanTheGivenIndexInTheRow(int index)
        {
            var row = new PARAM.Row(1, "itemLot", itemLotParamDef);
            row[$"lotItemId0{index + 1}"].Value = 1;
            row[$"lotItemCategory0{index + 1}"].Value = SoulsItemType.Goods;
            row[$"lotItemBasePoint0{index + 1}"].Value = 1;
            row[$"lotItemNum0{index + 1}"].Value = 1;

            new DSRLotSlotSerializer([]).WriteToRow(row, new LotSlot(new SoulsItem(SoulsGame.DSR, SoulsItemType.Armor, 2), 2, 2), index);
            row[$"lotItemId0{index + 1}"].Value.Should().Be(2);
            row[$"lotItemCategory0{index + 1}"].Value.Should().Be((int)SoulsItemType.Armor);
            row[$"lotItemBasePoint0{index + 1}"].Value.Should().Be(2);
            row[$"lotItemNum0{index + 1}"].Value.Should().Be(2);
        }

        [Fact]
        public void WriteToRow_ShouldTranslateItemsFromOtherGamesToTheEquivalentDS2ItemBeforeWritingToTheRow()
        {
            var row = new PARAM.Row(1, "itemLot", itemLotParamDef);
            row[$"lotItemId01"].Value = 1;
            row[$"lotItemCategory01"].Value = SoulsItemType.Weapon;
            row[$"lotItemBasePoint01"].Value = 1;
            row[$"lotItemNum01"].Value = 1;

            var ds3Item = new SoulsItem(SoulsGame.DS3, SoulsItemType.Armor, 2);
            new DSRLotSlotSerializer(new Dictionary<SoulsItem, int> { { ds3Item, 3 } }).WriteToRow(row, new LotSlot(ds3Item, 2, 2), 0);
            row[$"lotItemId01"].Value.Should().Be(3);
            row[$"lotItemCategory01"].Value.Should().Be((int)SoulsItemType.Goods);
        }
    }
}