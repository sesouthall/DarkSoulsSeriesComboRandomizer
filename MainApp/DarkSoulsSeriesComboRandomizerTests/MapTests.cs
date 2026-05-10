using DarkSoulsSeriesComboRandomizer;
using FluentAssertions;
using SoulsFormats;

namespace DarkSoulsSeriesComboRandomizerTests
{
    public class MapTests
    {
        [Fact]
        public void AllConnectedMaps_ShouldReturnTheCurrentMap()
        {
            var map = new Map("TestMap", [], []);
            map.AllConnectedMaps().Should().Contain(map);
        }

        [Fact]
        public void AllConnectedMaps_ShouldReturnMapsConnectedToTheCurrentMap()
        {
            var map1 = new Map("TestMap1", [], []);
            var map2 = new Map("TestMap2", [], []);
            var map3 = new Map("TestMap3", [], []);
            map1.connectedMaps.Add(map2);
            map2.connectedMaps.Add(map1);
            map2.connectedMaps.Add(map3);
            map3.connectedMaps.Add(map2);

            map1.AllConnectedMaps().Should().Contain(map2).And.Contain(map3);
        }

        [Fact]
        public void AllConnectedMaps_ShouldNotReturnDuplicateMaps()
        {
            var map1 = new Map("TestMap1", [], []);
            var map2 = new Map("TestMap2", [], []);
            var map3 = new Map("TestMap3", [], []);
            map1.connectedMaps.Add(map2);
            map1.connectedMaps.Add(map3);
            map2.connectedMaps.Add(map1);
            map2.connectedMaps.Add(map3);
            map3.connectedMaps.Add(map2);

            map1.AllConnectedMaps().Should().OnlyHaveUniqueItems();
        }

        [Fact]
        public void CanUse_ShouldReturnTrueIfTheGivenKeyConnectsThisMapToAnother()
        {
            var map1 = new Map("TestMap1", [], []);
            var map2 = new Map("TestMap2", [], []);
            var key = new Key(new SoulsItem(SoulsGame.DSR, SoulsItemType.Weapon, 1), 1, [(map1, map2)]);

            map1.CanUse(key).Should().BeTrue();
        }

        [Fact]
        public void CanUse_ShouldReturnFalseIfTheGivenKeyDoesNotAddConnectionsToThisMap()
        {
            var map1 = new Map("TestMap1", [], []);
            var map2 = new Map("TestMap2", [], []);
            var map3 = new Map("TestMap3", [], []);
            var key = new Key(new SoulsItem(SoulsGame.DSR, SoulsItemType.Weapon, 1), 1, [(map1, map2)]);

            map3.CanUse(key).Should().BeFalse();
        }

        [Fact]
        public void ConnectTo_ShouldConnectThisMapToTheGivenMapAndViceVersa()
        {
            var map1 = new Map("TestMap1", [], []);
            var map2 = new Map("TestMap2", [], []);

            map1.ConnectTo(map2);

            map1.AllConnectedMaps().Should().Contain(map2);
            map2.AllConnectedMaps().Should().Contain(map1);
        }

        private static readonly PARAMDEF itemLotParamDef = PARAMDEF.XmlDeserialize(@"ConfigFiles\PARAM\DS2S\Defs\ITEM_LOT_PARAM2.xml");

        private static SoulsItem Item(int id) => new(SoulsGame.DSR, SoulsItemType.Goods, id);

        private static LotSlot Slot(int id) => new(Item(id), 1, 1);

        [Fact]
        public void GetHinsLines_ShouldGetHintsLinesFromAllItemLots()
        {
            var itemLot1 = new ItemLot(
                        [Slot(1)],
                        [new PARAM.Row(1, "test", itemLotParamDef)],
                        LotType.Treasure,
                        new TestLotSlotSerializer([])
                    );
            var itemLot2 = new ItemLot(
                        [Slot(1)],
                        [new PARAM.Row(1, "test", itemLotParamDef)],
                        LotType.RandomEnemyDrop,
                        new TestLotSlotSerializer([])
                    );
            var itemsToTake = new Queue<LotSlot>();
            itemsToTake.Enqueue(Slot(1));
            itemsToTake.Enqueue(Slot(2));
            itemLot1.TakeItems(itemsToTake);
            itemLot2.TakeItems(itemsToTake);
            var map = new Map("TestMap", [], 
                [
                    itemLot1,
                    itemLot2,
                ]
            );

            TestItemNameLookupService itemNameLookupService = new(new Dictionary<SoulsItem, string> { { Item(1), "item1" }, { Item(2), "item2" } });
            map.GetHintsLines(itemNameLookupService).Should().NotBeEmpty().And.Contain([..itemLot1.GetHintsLines(itemNameLookupService, "TestMap"), ..itemLot2.GetHintsLines(itemNameLookupService, "TestMap")]);
        }
    }
}