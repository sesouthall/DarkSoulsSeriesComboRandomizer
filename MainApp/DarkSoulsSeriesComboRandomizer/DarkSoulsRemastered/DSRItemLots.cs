using SoulsFormats;
using System.IO;

namespace DarkSoulsSeriesComboRandomizer.DarkSoulsRemastered
{
    internal class DSRItemLots
    {
        private readonly string rootDir;
        private readonly string saveDir;
        private readonly BND3 regulationFile;
        private readonly PARAM itemLotParam;
        private readonly PARAM npcParam;

        private string MapFolder => Path.Combine(rootDir, "map", "MapStudio");

        public DSRItemLots(string rootDir, string saveDir, BND3 regulationFile, PARAM itemLotParam, PARAM npcParam)
        {
            this.rootDir = rootDir;
            this.saveDir = saveDir;
            this.regulationFile = regulationFile;
            this.itemLotParam = itemLotParam;
            this.npcParam = npcParam;
        }

        public static DSRItemLots New(string rootDir, string saveDir)
        {
            var regulationFile = BND3.Read(Path.Combine("PreModdedGameFiles", "UnrandomizedRegulationFiles", "GameParam.parambnd.dcx"));

            var itemLotParam = PARAMUtils.LoadParam(regulationFile, "ItemLotParam", @"ConfigFiles\PARAM\DS1R\Defs\ItemLotParam.xml");
            var npcParam = PARAMUtils.LoadParam(regulationFile, "NpcParam", @"ConfigFiles\PARAM\DS1R\Defs\NpcParam.xml");

            return new DSRItemLots(rootDir, saveDir, regulationFile, itemLotParam, npcParam);
        }

        public void Load()
        {
            foreach (var mapFile in Directory.GetFiles(MapFolder))
            {
                var matchingParsedMap = Map.DSRMaps.Values.SingleOrDefault(parsedMap => mapFile.Contains(parsedMap.FileName));
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

            foreach (var map in Map.DSRMaps.Values)
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
                    AssignLotChainToCorrectMap(defaultMap, ParseItemLotChain(itemLotNumber.Value, LotType.UnspecifiedEnemy));
                }

                if (entityItemLots.ContainsKey(enemy.EntityID))
                {
                    var lotIdNumbers = entityItemLots[enemy.EntityID];
                    foreach (var lotId in lotIdNumbers)
                    {
                        AssignLotChainToCorrectMap(defaultMap, ParseItemLotChain(lotId, GetEventLotType(lotId)));
                    }
                }
            }

            foreach (var treasure in mapData.Events.Treasures)
            {
                if (!mapData.Parts.Objects.Any(o => o.Name == treasure.TreasurePartName)) continue; // Treasure isn't obtainable
                var itemLotNumber = treasure.ItemLots[0];
                if (itemLotNumber == -1) continue; // No actual drop at this treasure

                AssignLotChainToCorrectMap(defaultMap, ParseItemLotChain(itemLotNumber, LotType.Treasure));
            }

            foreach (var part in mapData.Parts.Objects)
            {
                if (entityItemLots.ContainsKey(part.EntityID))
                {
                    var lotIdNumbers = entityItemLots[part.EntityID];
                    foreach (var lotId in lotIdNumbers)
                    {
                        AssignLotChainToCorrectMap(defaultMap, ParseItemLotChain(lotId, GetEventLotType(lotId)));
                    }
                }
            }
        }

        private void AssignLotChainToCorrectMap(Map defaultMap, IEnumerable<IItemLot> itemLots)
        {
            if (!itemLots.Any()) return;

            if (Map.DS1NonDefaultMapItemLots.Values.Any(set => set.Contains(itemLots.First().ID)))
            {
                var actualMapName = Map.DS1NonDefaultMapItemLots.Single(kvp => kvp.Value.Contains(itemLots.First().ID)).Key;
                var actualMap = Map.DSRMaps.Values.Single(map => map.Name == actualMapName);
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

        private IEnumerable<DSRItemLot> ParseItemLotChain(int itemLotNumber, LotType lotTypeGuess)
        {
            if (itemLotNumber >= 4000 && itemLotNumber < 5000) yield break; // Firelink fallback chest should be ignored
            PARAM.Row? itemLot;
            while ((itemLot = itemLotParam?.Rows.SingleOrDefault(row => row.ID == itemLotNumber)) != null)
            {
                yield return DSRItemLot.Parse(itemLot, lotTypeGuess, itemLotParam!);
                itemLotNumber++;
            }
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
