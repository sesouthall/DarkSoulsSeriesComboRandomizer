using DarkSoulsSeriesComboRandomizer;
using FluentAssertions;

namespace DarkSoulsSeriesComboRandomizerTests
{
    public class MapTests
    {
        [Fact]
        public void AllConnectedMaps_ShouldReturnTheCurrentMap()
        {
            var map = new Map("TestMap", "m00_00_00_00", SoulsGame.DSR, []);
            map.AllConnectedMaps().Should().Contain(map);
        }

        [Fact]
        public void AllConnectedMaps_ShouldReturnMapsConnectedToTheCurrentMap()
        {
            var map1 = new Map("TestMap1", "m00_00_00_00", SoulsGame.DSR, []);
            var map2 = new Map("TestMap2", "m00_00_00_00", SoulsGame.DSR, []);
            var map3 = new Map("TestMap3", "m00_00_00_00", SoulsGame.DSR, []);
            map1.connectedMaps.Add(map2);
            map2.connectedMaps.Add(map1);
            map2.connectedMaps.Add(map3);
            map3.connectedMaps.Add(map2);

            map1.AllConnectedMaps().Should().Contain(map2).And.Contain(map3);
        }

        [Fact]
        public void AllConnectedMaps_ShouldNotReturnDuplicateMaps()
        {
            var map1 = new Map("TestMap1", "m00_00_00_00", SoulsGame.DSR, []);
            var map2 = new Map("TestMap2", "m00_00_00_00", SoulsGame.DSR, []);
            var map3 = new Map("TestMap3", "m00_00_00_00", SoulsGame.DSR, []);
            map1.connectedMaps.Add(map2);
            map1.connectedMaps.Add(map3);
            map2.connectedMaps.Add(map1);
            map2.connectedMaps.Add(map3);
            map3.connectedMaps.Add(map2);

            map1.AllConnectedMaps().Should().OnlyHaveUniqueItems();
        }
    }
}