using DarkSoulsSeriesComboRandomizer;
using FluentAssertions;
using SoulsFormats;

namespace DarkSoulsSeriesComboRandomizerTests
{
    public class MapFactoryTests
    {
        [Fact]
        public void ParseMapDefinitions_ShouldConstructAMapWithTheGivenName()
        {
            var factory = new MapFactory(new EmptyGameFiles(), new ItemLotFactory([], new TestLotSlotSerializer([])));
            factory.ParseMapDefinitions([new FileBackedMapDefinition(MapName.AldiaSideRoom, [], [], [], [])], []).Should().ContainKey(MapName.AldiaSideRoom);
        }

        [Fact]
        public void ParseMapDefinitions_ShouldConstructAMapForEachSubMap()
        {
            var factory = new MapFactory(new EmptyGameFiles(), new ItemLotFactory([], new TestLotSlotSerializer([])));
            factory.ParseMapDefinitions([new FileBackedMapDefinition(MapName.AldiaSideRoom, [], [], [],
                [
                    new SubMapDefinition(MapName.AldiasKeep, [], [], [])
                ])], []).Should().ContainKey(MapName.AldiasKeep);
        }

        [Fact]
        public void ParseMapDefinitions_ShouldConstructMapsThatContainTheGivenBonfires()
        {
            var bonfires = new List<string> { "aBonfire", "anotherBonfire", "aThirdBonfire" };
            var factory = new MapFactory(new EmptyGameFiles(), new ItemLotFactory([], new TestLotSlotSerializer([])));
            factory.ParseMapDefinitions([new FileBackedMapDefinition(MapName.AldiaSideRoom, bonfires, [], [], [])], [])[MapName.AldiaSideRoom].Bonfires.Should().BeEquivalentTo(bonfires);
        }

        [Fact]
        public void ParseMapDefinitions_ShouldConnectMapsAccordingToTheirDefinitions()
        {
            var factory = new MapFactory(new EmptyGameFiles(), new ItemLotFactory([], new TestLotSlotSerializer([])));
            var maps = factory.ParseMapDefinitions(
                [
                    new FileBackedMapDefinition(MapName.AldiaSideRoom, [], [MapName.AldiasKeep], [], []),
                    new FileBackedMapDefinition(MapName.AldiasKeep, [], [], [], []),
                ], []);
            maps[MapName.AldiaSideRoom].connectedMaps.Should().Contain(maps[MapName.AldiasKeep]);
        }

        [Fact]
        public void ParseMapDefinitions_CreateOneWayConnections()
        {
            var factory = new MapFactory(new EmptyGameFiles(), new ItemLotFactory([], new TestLotSlotSerializer([])));
            var maps = factory.ParseMapDefinitions(
                [
                    new FileBackedMapDefinition(MapName.AldiaSideRoom, [], [MapName.AldiasKeep], [], []),
                    new FileBackedMapDefinition(MapName.AldiasKeep, [], [], [], []),
                ], []);
            maps[MapName.AldiasKeep].connectedMaps.Should().BeEmpty();
        }

        [Fact]
        public void ParseMapDefinitions_ShouldAssignItemLotsToMaps()
        {
            var factory = new MapFactory(new TestGameFiles([1], [10]), new ItemLotFactory([], new TestLotSlotSerializer([new LotSlot(new SoulsItem(SoulsGame.DSR, SoulsItemType.Accessory, 1), 1, 1)])));
            factory.ParseMapDefinitions([new FileBackedMapDefinition(MapName.AldiaSideRoom, [], [], [], [])], [])[MapName.AldiaSideRoom].ItemLocations.Should().HaveCount(2);
        }

        [Fact]
        public void ParseMapDefinitions_ShouldAssignItemLotsToSubMaps()
        {
            var factory = new MapFactory(new TestGameFiles([1], [10]), new ItemLotFactory([], new TestLotSlotSerializer([new LotSlot(new SoulsItem(SoulsGame.DSR, SoulsItemType.Accessory, 1), 1, 1)])));
            factory.ParseMapDefinitions([new FileBackedMapDefinition(MapName.AldiaSideRoom, [], [], [],
                [
                    new SubMapDefinition(MapName.AldiasKeep, [], [], [1, 10])
                ])], [])[MapName.AldiasKeep].ItemLocations.Should().HaveCount(2);
        }

        [Fact]
        public void ParseMapDefinitions_ShouldNotAssignLotsClaimedBySubMapsToTheirParents()
        {
            var factory = new MapFactory(new TestGameFiles([1], [10]), new ItemLotFactory([], new TestLotSlotSerializer([new LotSlot(new SoulsItem(SoulsGame.DSR, SoulsItemType.Accessory, 1), 1, 1)])));
            factory.ParseMapDefinitions([new FileBackedMapDefinition(MapName.AldiaSideRoom, [], [], [],
                [
                    new SubMapDefinition(MapName.AldiasKeep, [], [], [1, 10])
                ])], [])[MapName.AldiaSideRoom].ItemLocations.Should().BeEmpty();
        }

        [Fact]
        public void ParseMapDefinitions_ShouldAssignLotsChainsBasedOnTheirFirstElement()
        {
            var factory = new MapFactory(new TestGameFiles([1], [10, 11]), new ItemLotFactory([], new TestLotSlotSerializer([new LotSlot(new SoulsItem(SoulsGame.DSR, SoulsItemType.Accessory, 1), 1, 1)])));
            factory.ParseMapDefinitions([new FileBackedMapDefinition(MapName.AldiaSideRoom, [], [], [],
                [
                    new SubMapDefinition(MapName.AldiasKeep, [], [], [10])
                ])], [])[MapName.AldiasKeep].ItemLocations.Should().HaveCount(2);
        }

        [Fact]
        public void ParseMapDefinitions_ShouldNotAssignItemLotsMarkedAsIgnored()
        {
            var factory = new MapFactory(new TestGameFiles([1], [10]), new ItemLotFactory([], new TestLotSlotSerializer([new LotSlot(new SoulsItem(SoulsGame.DSR, SoulsItemType.Accessory, 1), 1, 1)])));
            factory.ParseMapDefinitions([new FileBackedMapDefinition(MapName.AldiaSideRoom, [], [], [], [])], [1])[MapName.AldiaSideRoom].ItemLocations.Should().HaveCount(1);
        }
    }

    public class EmptyGameFiles : ISoulsGameFiles
    {
        public PARAM EnemyItemLotParam => throw new NotImplementedException();

        public PARAM TreasureItemLotParam => throw new NotImplementedException();

        public IEnumerable<int> GetEnemyItemLotIds(MapName mapName)
        {
            return [];
        }

        public IEnumerable<int> GetTreasureItemLotIds(MapName mapName)
        {
            return [];
        }

        public void SaveItemLotChanges(string destinationFilePath)
        {
            throw new NotImplementedException();
        }
    }

    public class TestGameFiles(List<int> enemyItemLotIds, List<int> treasureItemLotIds) : ISoulsGameFiles
    {
        private static readonly PARAMDEF itemLotParamDef = PARAMDEF.XmlDeserialize(@"ConfigFiles\PARAM\DS2S\Defs\ITEM_LOT_PARAM2.xml");

        public PARAM EnemyItemLotParam => new() { Rows = [.. enemyItemLotIds.Select(id => new PARAM.Row(id, "enemyDrop", itemLotParamDef))] };

        public PARAM TreasureItemLotParam => new() { Rows = [.. treasureItemLotIds.Select(id => new PARAM.Row(id, "treasureDrop", itemLotParamDef))] };

        public IEnumerable<int> GetEnemyItemLotIds(MapName mapName)
        {
            return enemyItemLotIds;
        }

        public IEnumerable<int> GetTreasureItemLotIds(MapName mapName)
        {
            return treasureItemLotIds;
        }

        public void SaveItemLotChanges(string destinationFilePath)
        {
            throw new NotImplementedException();
        }
    }
}