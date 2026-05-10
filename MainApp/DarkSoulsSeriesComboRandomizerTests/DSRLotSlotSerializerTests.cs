using DarkSoulsSeriesComboRandomizer;
using DarkSoulsSeriesComboRandomizer.DarkSouls3;
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
                row[$"lotItemId0{index}"].Value = slot.Item.Id;
                row[$"lotItemCategory0{index}"].Value = slot.Item.Type;
                row[$"lotItemBasePoint0{index}"].Value = slot.Weight;
                row[$"lotItemNum0{index}"].Value = slot.Amount;
            }

            new DSRLotSlotSerializer(new CrossGameMappings([], [], [])).ParseFromRow(row, LotType.Treasure).Should().BeEquivalentTo(expectedSlots);
        }

        [Fact]
        public void ParseFromRow_ShouldNormalizeDropWeightsToATotalOf100()
        {
            var row = new PARAM.Row(1, "itemLot", itemLotParamDef);
            row[$"lotItemId01"].Value = 1;
            row[$"lotItemCategory01"].Value = SoulsItemType.Goods;
            row[$"lotItemBasePoint01"].Value = 1;
            row[$"lotItemNum01"].Value = 1;

            new DSRLotSlotSerializer(new CrossGameMappings([], [], [])).ParseFromRow(row, LotType.Treasure).Should().ContainSingle().Which.Weight.Should().Be(100);
        }

        [Fact]
        public void ParseFromRow_ShouldIgnoreItemsWithNoChanceToDrop()
        {
            var row = new PARAM.Row(1, "itemLot", itemLotParamDef);
            row[$"lotItemId01"].Value = 1;
            row[$"lotItemCategory01"].Value = SoulsItemType.Goods;
            row[$"lotItemBasePoint01"].Value = 0;
            row[$"lotItemNum01"].Value = 1;

            new DSRLotSlotSerializer(new CrossGameMappings([], [], [])).ParseFromRow(row, LotType.Treasure).Should().BeEmpty();
        }

        [Fact]
        public void ParseFromRow_ShouldMarkSlotsThatWouldDropZeroOfAnItemAsEmptyDrops()
        {
            var row = new PARAM.Row(1, "itemLot", itemLotParamDef);
            row[$"lotItemId01"].Value = 1;
            row[$"lotItemCategory01"].Value = SoulsItemType.Goods;
            row[$"lotItemBasePoint01"].Value = 1;
            row[$"lotItemNum01"].Value = 0;

            new DSRLotSlotSerializer(new CrossGameMappings([], [], [])).ParseFromRow(row, LotType.Treasure).Should().ContainSingle().Which.IsEmptyItem.Should().BeTrue();
        }

        [Fact]
        public void ParseFromRow_ShouldMarkSlotsThatDropItemZeroAsEmptyDrops()
        {
            var row = new PARAM.Row(1, "itemLot", itemLotParamDef);
            row[$"lotItemId01"].Value = 0;
            row[$"lotItemCategory01"].Value = SoulsItemType.Goods;
            row[$"lotItemBasePoint01"].Value = 1;
            row[$"lotItemNum01"].Value = 1;

            new DSRLotSlotSerializer(new CrossGameMappings([], [], [])).ParseFromRow(row, LotType.Treasure).Should().ContainSingle().Which.IsEmptyItem.Should().BeTrue();
        }

        [Fact]
        public void ParseFromRow_ShouldMarkSlotsWithCategoryNegativeOneAsEmptyDrops()
        {
            var row = new PARAM.Row(1, "itemLot", itemLotParamDef);
            row[$"lotItemId01"].Value = 1;
            row[$"lotItemCategory01"].Value = -1;
            row[$"lotItemBasePoint01"].Value = 1;
            row[$"lotItemNum01"].Value = 1;

            new DSRLotSlotSerializer(new CrossGameMappings([], [], [])).ParseFromRow(row, LotType.Treasure).Should().ContainSingle().Which.IsEmptyItem.Should().BeTrue();
        }

        [Fact]
        public void WriteToRow_ShouldWriteTheGivenSlotsIntoTheRow()
        {
            var row = new PARAM.Row(1, "itemLot", itemLotParamDef);
            for (int i = 1; i < 9; i++)
            {
                row[$"lotItemId0{i}"].Value = 99;
                row[$"lotItemCategory0{i}"].Value = (int)SoulsItemType.Weapon;
                row[$"lotItemBasePoint0{i}"].Value = 99;
                row[$"lotItemNum0{i}"].Value = 99;
            }

            new DSRLotSlotSerializer(new CrossGameMappings([], [], [])).WriteToRow(row,
                [
                    new LotSlot(new SoulsItem(SoulsGame.DSR, SoulsItemType.Goods, 1), 1, 1),
                    new LotSlot(new SoulsItem(SoulsGame.DSR, SoulsItemType.Goods, 2), 2, 2),
                    new LotSlot(new SoulsItem(SoulsGame.DSR, SoulsItemType.Goods, 3), 3, 3),
                    new LotSlot(new SoulsItem(SoulsGame.DSR, SoulsItemType.Goods, 4), 4, 4),
                    new LotSlot(new SoulsItem(SoulsGame.DSR, SoulsItemType.Goods, 5), 5, 5),
                    new LotSlot(new SoulsItem(SoulsGame.DSR, SoulsItemType.Goods, 6), 6, 6),
                    new LotSlot(new SoulsItem(SoulsGame.DSR, SoulsItemType.Goods, 7), 7, 7),
                    new LotSlot(new SoulsItem(SoulsGame.DSR, SoulsItemType.Goods, 8), 8, 8),
                ]);

            for (int i = 1; i < 9; i++)
            {
                row[$"lotItemId0{i}"].Value.Should().Be(i);
                row[$"lotItemCategory0{i}"].Value.Should().Be((int)SoulsItemType.Goods);
                row[$"lotItemBasePoint0{i}"].Value.Should().Be(i);
                row[$"lotItemNum0{i}"].Value.Should().Be(i);
            }
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
            new DSRLotSlotSerializer(new CrossGameMappings(new Dictionary<int, SoulsItem> { { 3, ds3Item } }, [], [])).WriteToRow(row, [new LotSlot(ds3Item, 2, 2)]);
            row[$"lotItemId01"].Value.Should().Be(3);
            row[$"lotItemCategory01"].Value.Should().Be((int)SoulsItemType.Goods);
        }
    }
}