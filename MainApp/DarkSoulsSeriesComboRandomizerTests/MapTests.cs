using DarkSoulsSeriesComboRandomizer;
using DarkSoulsSeriesComboRandomizer.DarkSouls2SotFS;
using DarkSoulsSeriesComboRandomizer.DarkSouls3;
using DarkSoulsSeriesComboRandomizer.DarkSoulsRemastered;
using FluentAssertions;
using SoulsFormats;

namespace DarkSoulsSeriesComboRandomizerTests
{
    public class MapTests
    {
        [Fact]
        public void GetAccessibleItemLots_ShouldNotReturnDuplicateItemLots()
        {
            var paramDef = PARAMDEF.XmlDeserialize(@"ConfigFiles\PARAM\DS1R\Defs\ItemLotParam.xml");
            var map1 = new Map(MapName.DS1StartingCell, "m00_00_00_00", SoulsGame.DSR, new List<string>());
            var map2 = new Map(MapName.NorthernUndeadAsylum, "m00_00_00_00", SoulsGame.DSR, new List<string>());
            map1.connectedMaps.Add(map2);
            map2.connectedMaps.Add(map1);
            map1.ItemLocations.Add(new DSRItemLot(new List<LotSlot>(), new List<SoulsFormats.PARAM.Row> { new PARAM.Row(1, "foo", paramDef) }, LotType.RandomEnemyDrop));
            map2.ItemLocations.Add(new DSRItemLot(new List<LotSlot>(), new List<SoulsFormats.PARAM.Row> { new PARAM.Row(2, "foo", paramDef) }, LotType.RandomEnemyDrop));
            map1.GetAccessibleItemLots(lot => lot.LotType == LotType.RandomEnemyDrop).Count.Should().Be(2);
        }

        [Fact]
        [Trait("Category", "Integration")]
        public void AllMaps_ShouldReturnAReasonableNumberOfRandomEnemyDrops()
        {
            var dsrItems = DSRItemLots.New(@"C:\Program Files (x86)\Steam\steamapps\common\DARK SOULS REMASTERED", "");
            var ds2Items = DS2SotFSItemLots.New(@"C:\Program Files (x86)\Steam\steamapps\common\Dark Souls II Scholar of the First Sin\Game", "");
            var ds3Items = DS3ItemLots.New(@"C:\Program Files (x86)\Steam\steamapps\common\DARK SOULS III\Game", "");

            dsrItems.Load();
            ds2Items.Load();
            ds3Items.Load();

            foreach (var key in dsrItems.Keys.Concat(ds2Items.Keys).Concat(ds3Items.Keys))
            {
                key.Collect();
            }


            var ds1Firelink = dsrItems.Maps[MapName.FirelinkShrine];
            var majula = ds2Items.Maps[MapName.Majula];
            var ds3Firelink = ds3Items.Maps[MapName.CemetaryFirelinkUntendedGraves];
            ds1Firelink.connectedMaps.Add(majula);
            ds1Firelink.connectedMaps.Add(ds3Firelink);
            majula.connectedMaps.Add(ds1Firelink);
            majula.connectedMaps.Add(ds3Firelink);
            ds3Firelink.connectedMaps.Add(ds1Firelink);
            ds3Firelink.connectedMaps.Add(majula);

            var undeadParish = dsrItems.Maps[MapName.UndeadBurgUndeadParish];
            var sensFortress = dsrItems.Maps[MapName.SensFortress];
            undeadParish.connectedMaps.Add(sensFortress);
            sensFortress.connectedMaps.Add(undeadParish);

            var allMaps = dsrItems.Maps.Concat(ds2Items.Maps).Concat(ds3Items.Maps).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            var accessibleRandomDrops = allMaps[MapName.DS1StartingCell].GetAccessibleItemLots(lot => lot.LotType == LotType.RandomEnemyDrop);
            var allRandomDrops = allMaps.Values.SelectMany(map => map.ItemLocations).Where(lot => lot.LotType == LotType.RandomEnemyDrop).ToList();
            var missing = allRandomDrops.Except(accessibleRandomDrops);
            missing.Should().BeEmpty("Getting all random drops directly should be equivalent to getting all random drops from the connected maps.");
        }
    }
}