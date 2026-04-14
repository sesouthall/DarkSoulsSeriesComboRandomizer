using SoulsFormats;
using static SoulsFormats.PARAM;

namespace DarkSoulsSeriesComboRandomizer.DarkSoulsRemastered
{
    internal class DSRItemLots
    {
        private readonly string rootDir;
        private BND3? regulationFile;
        private PARAM? itemLotParam;
        private PARAM? npcParam;

        private readonly Dictionary<(SoulsGame, SoulsItemType, int), int> _crossGameItems;

        private string RegulationFilePath => Path.Combine(rootDir, "param", "GameParam", "GameParam.parambnd.dcx");
        private string RegulationFileBackupPath => $"{RegulationFilePath}.unrandomized";
        private string MapFolder => Path.Combine(rootDir, "map", "MapStudio");

        public DSRItemLots(string rootDir, Dictionary<int, SoulsItem> injectedItemsInThisGame)
        {
            this.rootDir = rootDir;

            _crossGameItems = injectedItemsInThisGame
                .ToDictionary(
                    kvp => (kvp.Value.Game, kvp.Value.ItemType, kvp.Value.OriginalId),
                    kvp => kvp.Key
                );
        }

        public void Load()
        {
            regulationFile = BND3.Read(RegulationFilePath);

            itemLotParam = PARAMUtils.LoadParam(regulationFile, "ItemLotParam", @"ConfigFiles\PARAM\DS1R\Defs\ItemLotParam.xml");
            npcParam = PARAMUtils.LoadParam(regulationFile, "NpcParam", @"ConfigFiles\PARAM\DS1R\Defs\NpcParam.xml");

            foreach (var mapFile in Directory.GetFiles(MapFolder))
            {
                var matchingParsedMap = Map.DSRMaps.SingleOrDefault(parsedMap => mapFile.Contains(parsedMap.FileName));
                if (matchingParsedMap != null)
                {
                    var mapData = MSB1.Read(mapFile);

                    LoadMapLocationData(mapData, matchingParsedMap);
                }
            }
        }

        public void Save()
        {
            if (!File.Exists(RegulationFileBackupPath))
            {
                File.Copy(RegulationFilePath, RegulationFileBackupPath);
            }

            regulationFile.Files.Single(f => f.Name.Contains("ItemLotParam")).Bytes = itemLotParam.Write();
            regulationFile.Files.Single(f => f.Name.Contains("NpcParam")).Bytes = npcParam.Write();
            regulationFile.Write(RegulationFilePath);
        }

        public void Revert()
        {
            if (File.Exists(RegulationFileBackupPath))
            {
                File.Delete(RegulationFilePath);
                File.Move(RegulationFileBackupPath, RegulationFilePath);
            }
        }

        private void LoadMapLocationData(MSB1 mapData, Map defaultMap)
        {
            foreach (var enemy in mapData.Parts.Enemies)
            {
                var itemLotNumber = (int?)npcParam?.Rows.FirstOrDefault(row => row.ID == enemy.NPCParamID)?["itemLotId_1"]?.Value;
                if (itemLotNumber.HasValue && itemLotNumber.Value != -1)
                {
                    AssignLotToCorrectMap(defaultMap, itemLotNumber.Value, LotType.UnspecifiedEnemy);
                }

                if (entityItemLots.ContainsKey(enemy.EntityID))
                {
                    var lotIdNumbers = entityItemLots[enemy.EntityID];
                    foreach (var lotId in lotIdNumbers)
                    {
                        AssignLotToCorrectMap(defaultMap, lotId, GetEventLotType(lotId));
                    }
                }
            }

            foreach (var treasure in mapData.Events.Treasures)
            {
                if (!mapData.Parts.Objects.Any(o => o.Name == treasure.TreasurePartName)) continue; // Treasure isn't obtainable
                var itemLotNumber = treasure.ItemLots[0];
                if (itemLotNumber == -1) continue; // No actual drop at this treasure

                AssignLotToCorrectMap(defaultMap, itemLotNumber, LotType.Treasure);
            }

            foreach (var part in mapData.Parts.Objects)
            {
                if (entityItemLots.ContainsKey(part.EntityID))
                {
                    var lotIdNumbers = entityItemLots[part.EntityID];
                    foreach (var lotId in lotIdNumbers)
                    {
                        AssignLotToCorrectMap(defaultMap, lotId, GetEventLotType(lotId));
                    }
                }
            }
        }

        private void AssignLotToCorrectMap(Map defaultMap, int itemLotNumber, LotType lotType)
        {
            if (Map.DS1NonDefaultMapItemLots.Values.Any(set => set.Contains(itemLotNumber)))
            {
                var actualMapName = Map.DS1NonDefaultMapItemLots.Single(kvp => kvp.Value.Contains(itemLotNumber)).Key;
                var actualMap = Map.DSRMaps.Single(map => map.FriendlyName == actualMapName);
                actualMap.ItemLocations.AddRange(ParseItemLotChain(itemLotNumber, lotType));
            }
            else
            {
                defaultMap.ItemLocations.AddRange(ParseItemLotChain(itemLotNumber, lotType));
            }
        }


        private static LotType GetEventLotType(int itemLotNumber)
        {
            return 2000 <= itemLotNumber && itemLotNumber < 3000 ? LotType.Boss : LotType.GenericEvent;
        }

        private IEnumerable<DSRItemLot> ParseItemLotChain(int itemLotNumber, LotType lotTypeGuess)
        {
            Row? itemLot;
            while ((itemLot = itemLotParam?.Rows.SingleOrDefault(row => row.ID == itemLotNumber)) != null)
            {
                yield return DSRItemLot.Parse(itemLot, lotTypeGuess, itemLotParam);
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
    }
}
