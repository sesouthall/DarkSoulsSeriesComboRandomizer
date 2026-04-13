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
            npcParam = PARAMUtils.LoadParam(regulationFile, "NpcParam", @"ConfigFiles\PARAM\DS1R\Defs\ItemLotParam.xml");

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
                var enemyParam = npcParam?.Rows.FirstOrDefault(row => row.ID == enemy.NPCParamID);
                if (enemyParam == null) continue; // Enemy references missing NPC Param
                var cell = enemyParam["itemLotId_1"];
                if (cell == null) continue; // NPC Param is malformed
                var itemLotNumber = (int)cell.Value;
                if (itemLotNumber == -1) continue; // Enemy has no drops

                AssignLotToCorrectMap(defaultMap, itemLotNumber, LotType.UnspecifiedEnemy);

                if (entityItemLots.ContainsKey(enemy.EntityID))
                {
                    var lotId = entityItemLots[enemy.EntityID];
                    AssignLotToCorrectMap(defaultMap, lotId, GetEventLotType(itemLotNumber));
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
                    var itemLotNumber = entityItemLots[part.EntityID];
                    var lotType = GetEventLotType(itemLotNumber);
                    AssignLotToCorrectMap(defaultMap, itemLotNumber, lotType);
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
            return 100000 <= itemLotNumber && itemLotNumber < 900000 ? LotType.Boss : LotType.GenericEvent;
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

        private static readonly Dictionary<int, int> entityItemLots = new Dictionary<int, int>()
        {
            { 1010750, 2510 }, // Capra Demon
            { 1000800, 2500 }, // Gaping Dragon
            { 1000800, 52610000 }, // GD tail drop
            { 1100160, 2520 }, // Priscilla
            { 1100160, 27310000 }, // Priscilla tail drop
            { 1200801, 2530 }, // Moonlight Butterfly
            { 1200800, 2540 }, // Sif
            { 1200800, 2541 },
            { 1210402, 2710 }, // Kalameet
            { 1210402, 45110000 }, // Kalameet tail drop
            { 1210820, 2690 }, // Artorias
            { 1210800, 2680 }, // Sanctuary Guardian
            { 1210800, 34720000 }, // Sanctuary Guardian tail drop
            { 1210840, 2700 }, // Manus
            { 1310810, 2560 }, // Nito
            { 1320800, 34510000 }, // Stone Dragon tail drop
            { 1400800, 2570 }, // Queelag
            { 1410400, 22310000 }, // Demon Firesage
            { 1410700, 2670 }, // Centipede Demon
            { 1410802, 2580 }, // Bed of Chaos
            { 1500800, 2590 }, // Iron Golem
            { 1510650, 2600 }, // Gwyndolin
            { 1510801, 2610 }, // Ornstein
            { 1510811, 2620 }, // Smough
            { 1510600, 1090 }, // Gwynevere
            { 6180, 1100 }, // Ingward
            { 1600800, 2630 }, // Four Kings
            { 1700800, 2640 }, // Seath
            { 1700800, 52910000 }, // Seath tail drop
            { 1800800, 2650 }, // Gwyn
            { 1810800, 2660 }, // Asylum Demon
            { 1810800, 2661 },
            { 1810810, 22300000 }, // Stray Demon
            { 1700510, 27100200 }, // Pendant Blue Golem
        };
    }
}
