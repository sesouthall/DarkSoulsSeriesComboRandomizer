using SoulsFormats;
using System.IO;

namespace DarkSoulsSeriesComboRandomizer.DarkSoulsRemastered
{
    internal class DSRItemLots(string rootDir, string saveDir, BND3 regulationFile, PARAM itemLotParam, PARAM npcParam, ItemLotFactory lotFactory, IReadOnlyDictionary<MapName, Map> maps, IReadOnlyList<Key> keys)
    {
        public IReadOnlyDictionary<MapName, Map> Maps { get; } = maps;
        public IReadOnlyList<Key> Keys { get; } = keys;

        private string MapFolder => Path.Combine(rootDir, "map", "MapStudio");

        public static DSRItemLots New(string rootDir, string saveDir)
        {
            var regulationFile = BND3.Read(Path.Combine("PreModdedGameFiles", "UnrandomizedRegulationFiles", "GameParam.parambnd.dcx"));

            var itemLotParam = PARAMUtils.LoadParam(regulationFile, "ItemLotParam", @"ConfigFiles\PARAM\DS1R\Defs\ItemLotParam.xml");
            var npcParam = PARAMUtils.LoadParam(regulationFile, "NpcParam", @"ConfigFiles\PARAM\DS1R\Defs\NpcParam.xml");

            var crossGameMapping = SoulsItemCsvParser.ParseFile(Path.Combine("ConfigFiles", "DSR_injected_items.csv"));
            var reverseMapping = crossGameMapping.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);
            var lotSerializer = new DSRLotSlotSerializer(reverseMapping);
            var lotFactory = new ItemLotFactory(SoulsGame.DSR, linkedItemLots, lotSerializer);

            var maps = Map.GetDSRMaps();
            var keys = Key.ConstructDS1Keys(maps);

            return new DSRItemLots(rootDir, saveDir, regulationFile, itemLotParam, npcParam, lotFactory, maps, keys);
        }

        public void Load()
        {
            foreach (var mapFile in Directory.GetFiles(MapFolder))
            {
                var matchingParsedMap = Maps.Values.SingleOrDefault(parsedMap => mapFile.Contains(parsedMap.FileName));
                if (matchingParsedMap != null)
                {
                    var mapData = MSB1.Read(mapFile);

                    LoadMapLocationData(mapData, matchingParsedMap);
                }
            }
        }

        private const string WeaponNameFMGFileName = "Weapon_name_";
        private const string ArmorNameFMGFileName = "Armor_name_";
        private const string AccessoryNameFMGFileName = "Accessory_name_";
        private const string GoodsNameFMGFileName = "Item_name_";

        public void Save()
        {
            regulationFile.Files.Single(f => f.Name.Contains("ItemLotParam")).Bytes = itemLotParam.Write();
            regulationFile.Files.Single(f => f.Name.Contains("NpcParam")).Bytes = npcParam.Write();
            regulationFile.Write(Path.Combine(saveDir, "GameParam.parambnd.dcx"));

            var itemTextFile = BND3.Read(Path.Combine(rootDir, "msg", "ENGLISH", "item.msgbnd.dcx"));

            var weaponNameFMG = FMG.Read(itemTextFile.Files.First(f => f.Name.Contains(WeaponNameFMGFileName)).Bytes);
            var armorNameFMG = FMG.Read(itemTextFile.Files.First(f => f.Name.Contains(ArmorNameFMGFileName)).Bytes);
            var accessoryNameFMG = FMG.Read(itemTextFile.Files.First(f => f.Name.Contains(AccessoryNameFMGFileName)).Bytes);
            var goodsNameFMG = FMG.Read(itemTextFile.Files.First(f => f.Name.Contains(GoodsNameFMGFileName)).Bytes);

            foreach (var map in Maps.Values)
            {
                foreach (var itemLocation in map.ItemLocations)
                {
                    foreach (var itemSlot in itemLocation.NewSlots.Where(slot => !slot.IsEmptyItem))
                    {
                        var (resolvedId, resolvedType) = itemSlot.ResolveFor(SoulsGame.DSR);
                        var itemName = resolvedType switch
                        {
                            SoulsItemType.Weapon => weaponNameFMG[resolvedId],
                            SoulsItemType.Armor => armorNameFMG[resolvedId],
                            SoulsItemType.Accessory => accessoryNameFMG[resolvedId],
                            SoulsItemType.Goods => goodsNameFMG[resolvedId],
                            _ => ""
                        };
                        File.AppendAllLines(Path.Combine(saveDir, "Hints.txt"), [$"{itemName}: {map.FriendlyName} ({(itemLocation.LotType == LotType.RandomEnemyDrop ? "Random Drop" : "Fixed Treasure")})"]);
                    }
                }
            }
        }

        private void LoadMapLocationData(MSB1 mapData, Map defaultMap)
        {
            foreach (var enemy in mapData.Parts.Enemies)
            {
                var itemLotNumber = (int?)npcParam?.Rows.FirstOrDefault(row => row.ID == enemy.NPCParamID)?["itemLotId_1"]?.Value;
                if (itemLotNumber.HasValue && itemLotNumber.Value != -1)
                {
                    AssignLotChainToCorrectMap(defaultMap, lotFactory.ParseChain(itemLotNumber.Value, itemLotParam, LotType.UnspecifiedEnemy));
                }

                if (entityItemLots.TryGetValue(enemy.EntityID, out List<int>? lotIdNumbers))
                {
                    foreach (var lotId in lotIdNumbers)
                    {
                        AssignLotChainToCorrectMap(defaultMap, lotFactory.ParseChain(lotId, itemLotParam, GetEventLotType(lotId)));
                    }
                }
            }

            foreach (var treasure in mapData.Events.Treasures)
            {
                if (!mapData.Parts.Objects.Any(o => o.Name == treasure.TreasurePartName)) continue; // Treasure isn't obtainable
                var itemLotNumber = treasure.ItemLots[0];
                if (itemLotNumber == -1) continue; // No actual drop at this treasure
                if (itemLotNumber >= 4000 && itemLotNumber <= 5000) continue; // Ignore Firelink chest treasures

                AssignLotChainToCorrectMap(defaultMap, lotFactory.ParseChain(itemLotNumber, itemLotParam, LotType.Treasure));
            }

            foreach (var part in mapData.Parts.Objects)
            {
                if (entityItemLots.TryGetValue(part.EntityID, out List<int>? lotIdNumbers))
                {
                    foreach (var lotId in lotIdNumbers)
                    {
                        AssignLotChainToCorrectMap(defaultMap, lotFactory.ParseChain(lotId, itemLotParam, GetEventLotType(lotId)));
                    }
                }
            }
        }

        private void AssignLotChainToCorrectMap(Map defaultMap, IEnumerable<ItemLot> itemLots)
        {
            if (!itemLots.Any()) return;

            if (Map.DS1NonDefaultMapItemLots.Values.Any(set => set.Contains(itemLots.First().ID)))
            {
                var actualMapName = Map.DS1NonDefaultMapItemLots.Single(kvp => kvp.Value.Contains(itemLots.First().ID)).Key;
                var actualMap = Maps[actualMapName];
                actualMap.ItemLocations.AddRange(itemLots);
            }
            else
            {
                defaultMap.ItemLocations.AddRange(itemLots);
            }
        }

        private static LotType GetEventLotType(int itemLotNumber)
        {
            return 2000 <= itemLotNumber && itemLotNumber < 3000 ? LotType.Boss : LotType.GenericEvent;
        }

        private static readonly Dictionary<int, List<int>> entityItemLots = new Dictionary<int, List<int>>()
        {
            { 1010750, new List<int>{ 2510 } }, // Capra Demon
            { 1000800, new List<int>{ 2500, 52610000 } }, // Gaping Dragon main and tail drop
            { 1100160, new List<int>{ 2520, 27310000 } }, // Priscilla main and tail
            { 1200801, new List<int>{ 2530 } }, // Moonlight Butterfly
            { 1200800, new List<int>{ 2540, 2541 } }, // Sif soul and convenant ring
            { 1210402, new List<int>{ 2710, 45110000 } }, // Kalameet main and tail
            { 1210820, new List<int>{ 2690 } }, // Artorias
            { 1210800, new List<int>{ 2680, 34720000 } }, // Sanctuary Guardian
            { 1210840, new List<int>{ 2700 } }, // Manus
            { 1310810, new List<int>{ 2560 } }, // Nito
            { 1320800, new List<int>{ 34510000 } }, // Stone Dragon tail drop
            { 1400800, new List<int>{ 2570 } }, // Queelag
            { 1410400, new List<int>{ 22310000 } }, // Demon Firesage
            { 1410700, new List <int>{ 2670 } }, // Centipede Demon
            { 1410802, new List<int>{ 2580 } }, // Bed of Chaos
            { 1500800, new List<int>{ 2590 } }, // Iron Golem
            { 1510650, new List<int>{ 2600 } }, // Gwyndolin
            { 1510801, new List<int>{ 2610 } }, // Ornstein
            { 1510811, new List<int>{ 2620 } }, // Smough
            { 1510600, new List<int>{ 1090 } }, // Gwynevere
            { 6180, new List<int>{ 1100 } }, // Ingward
            { 1600800, new List<int>{ 2630 } }, // Four Kings
            { 1700800, new List<int>{ 2640, 52910000 } }, // Seath main and tail drop
            { 1800800, new List<int>{ 2650 } }, // Gwyn
            { 1810800, new List<int>{ 2660, 2661 } }, // Asylum Demon Big Pilgrim Key and Demon Greathammer
            { 1810810, new List<int>{ 22300000 } }, // Stray Demon
            { 1700510, new List<int>{ 27100200 } }, // Pendant Blue Golem
        };

        // linked locations taken from HotPocketRemix's DarkSoulsItemRandomizer
        private static readonly List<HashSet<int>> linkedItemLots =
        [
            [1000, 6000],
            [1050, 1070, 6280],
            // Oscar dialog or death
            [1080, 6020], // Undead Asylum F2 East Key
            [1081, 2660, 6021], // Big Pilgrim's Key
            [1082, 6022], // Estus Flask

            [1300, 6170],
            [1500, 6740],
            [1510, 41100000],
            [1520, 41400000],
            [2610, 2620],
            [6190, 60001401],
            [6230, 60001100],
            [6231, 60001105],
            [6233, 60001134],
            [12000000, 12010000, 12010100, 12030000],
            [12010200, 12010300],
            [22400000, 22400100],
            [22500000, 22500200],
            [23000000, 23000100, 23000200, 23000300, 23000400, 23000500],
            [23300000, 23300100],
            [23700000, 23700100, 23700200],
            [23800000, 23800100],
            [25000000, 25000300, 25001000, 25002200, 25002500, 25003000, 25003200],
            [25000100, 25001100, 25002300, 25003100],
            [25000200, 25001200, 25002100, 25002400],
            [25400000, 25401000, 25402000],
            [25400100, 25401100, 25402100],
            [25400200, 25401200, 25402200],
            [25500000, 25502000],
            [25500100, 25501000, 25502100, 25503100, 25503200],
            [25500200, 25502200],
            [25600000, 25600300],
            [25600200, 25600400],
            [25601000, 25601300],
            [25601200, 25601400],
            [25701000, 25701200],
            [25702100, 25702200],
            [26900000, 26900200, 26900300],
            [27000000, 27000100],
            [27800000, 27801000, 27801010, 27801020, 27801030, 27802000, 27802010, 27803000, 27803100],
            [27900001, 27905001, 27907001],
            [27900101, 27905100],
            [27901001, 27903001, 27905300],
            [27902001, 27905200],
            [29000000, 29001000, 29002000, 29003000],
            [29000100, 29001100, 29002100, 29003100],
            [29000200, 29001200, 29002200, 29003200],
            [29100000, 29100200, 29101100],
            [29100100, 29101000, 29101300],
            [29300000, 29300100],
            [32500000, 32500100],
            [32700000, 32700100],
            [33000000, 33001000, 33002000, 33003000, 33004000, 33005000, 33006000, 33007000, 33007100, 33007200, 33007300],
            [33400000, 33400100, 33400200],
            [34100000, 34100100],
            [34600000, 34610000],
            [35010000, 35010100],
            [35200200, 35200500],
            [35310000, 35310100],
            [53500000, 53500100],
            [53500002, 53500101],
            [60001133, 60001501],
            [60001402, 60006215, 60006302],
            [60001403, 60006216, 60006303],
            [60001404, 60001106, 60006217, 60006304],
            [60002200, 60006502],
            [60002201, 60006503],
            [60002202, 60006504],
            [60002203, 60006505],
            [60002204, 60006506],
        ];

        public static readonly IReadOnlyList<int> StartingItemLots =
        [
            1810100,
            1810110,
            1810120,
            1810130,
            1810140,
            1810150,
            1810160,
            1810170,
            1810180,
            1810190,
            1810200,
            1810210,
            1810220,
            1810230,
            1810240,
            1810250,
            1810260,
            1810270,
            1810280,
            1810290,
            1810300,
            1810310,
            1810320,
            1810330,
        ];

        public const int EstusFlaskLot = 1082;
    }
}
