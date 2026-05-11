using SoulsFormats;
using System.IO;

namespace DarkSoulsSeriesComboRandomizer.DarkSoulsRemastered
{
    public class DSRFiles(string rootDir) : ISoulsGameFiles
    {
        private const string ItemLotParamFileName = "ItemLotParam";
        private const string RegulationFileName = "GameParam.parambnd.dcx";

        private PARAM? itemLotParam = null;
        public PARAM EnemyItemLotParam
        {
            get
            {
                itemLotParam ??= PARAMUtils.LoadParam(RegulationFile, ItemLotParamFileName, @"ConfigFiles\PARAM\DS1R\Defs\ItemLotParam.xml");
                return itemLotParam;
            }
        }

        public PARAM TreasureItemLotParam
        {
            get
            {
                itemLotParam ??= PARAMUtils.LoadParam(RegulationFile, ItemLotParamFileName, @"ConfigFiles\PARAM\DS1R\Defs\ItemLotParam.xml");
                return itemLotParam;
            }
        }

        private BND3? regulationFile = null;
        private BND3 RegulationFile
        {
            get
            {
                regulationFile ??= BND3.Read(Path.Combine("PreModdedGameFiles", "UnrandomizedRegulationFiles", RegulationFileName));
                return regulationFile;
            }
        }

        private PARAM? npcParam = null;
        private PARAM NpcParam
        {
            get
            {
                npcParam ??= PARAMUtils.LoadParam(RegulationFile, "NpcParam", @"ConfigFiles\PARAM\DS1R\Defs\NpcParam.xml");
                return npcParam;
            }
        }

        private BND3? itemTextFile = null;
        public BND3 ItemTextFile
        {
            get
            {
                itemTextFile ??= BND3.Read(Path.Combine("PreModdedGameFiles", "DSR", "msg", "ENGLISH", "item.msgbnd.dcx"));
                return itemTextFile;
            }
        }

        public IEnumerable<int> GetEnemyItemLotIds(MapName mapName)
        {
            var mapFilePath = mapFileNames[mapName];
            var mapFileFullPath = Path.Combine(rootDir, mapFilePath);
            if (!File.Exists(mapFileFullPath))
            {
                yield break;
            }

            var mapData = MSB1.Read(mapFileFullPath);

            foreach (var enemy in mapData.Parts.Enemies)
            {
                var itemLotNumber = (int?)NpcParam?.Rows.FirstOrDefault(row => row.ID == enemy.NPCParamID)?["itemLotId_1"]?.Value;
                if (itemLotNumber.HasValue && itemLotNumber.Value != -1)
                {
                    yield return itemLotNumber.Value;
                }
            }
        }

        public IEnumerable<int> GetTreasureItemLotIds(MapName mapName)
        {
            var mapFilePath = mapFileNames[mapName];
            var mapFileFullPath = Path.Combine(rootDir, mapFilePath);
            if (!File.Exists(mapFileFullPath))
            {
                yield break;
            }

            var mapData = MSB1.Read(mapFileFullPath);

            foreach (var treasure in mapData.Events.Treasures)
            {
                if (!mapData.Parts.Objects.Any(o => o.Name == treasure.TreasurePartName)) continue; // Treasure isn't obtainable
                var itemLotNumber = treasure.ItemLots[0];
                if (itemLotNumber == -1) continue; // No actual drop at this treasure
                if (itemLotNumber >= 4000 && itemLotNumber <= 5000) continue; // Ignore Firelink chest treasures

                yield return itemLotNumber;
            }
        }

        public void SaveItemLotChanges(string destinationFolderPath)
        {
            if (itemLotParam != null)
            {
                RegulationFile.Files.Single(f => f.Name.Contains(ItemLotParamFileName)).Bytes = itemLotParam.Write();
            }
            RegulationFile.Write(Path.Combine(destinationFolderPath, RegulationFileName));
        }

        private readonly Dictionary<MapName, string> mapFileNames = new()
        {
            { MapName.Depths, "map\\MapStudio\\m10_00_00_00.msb" },
            { MapName.UndeadBurgUndeadParish, "map\\MapStudio\\m10_01_00_00.msb" },
            { MapName.FirelinkShrine, "map\\MapStudio\\m10_02_00_00.msb" },
            { MapName.PaintedWorld, "map\\MapStudio\\m11_00_00_00.msb" },
            { MapName.DarkrootGarden, "map\\MapStudio\\m12_00_00_01.msb" },
            { MapName.Oolacile, "map\\MapStudio\\m12_01_00_00.msb" },
            { MapName.Catacombs, "map\\MapStudio\\m13_00_00_00.msb" },
            { MapName.TombOfTheGiants, "map\\MapStudio\\m13_01_00_00.msb" },
            { MapName.GreatHollowAshLake, "map\\MapStudio\\m13_02_00_00.msb" },
            { MapName.Blighttown, "map\\MapStudio\\m14_00_00_00.msb" },
            { MapName.DemonRuinsLostIzalith, "map\\MapStudio\\m14_01_00_00.msb" },
            { MapName.SensFortress, "map\\MapStudio\\m15_00_00_00.msb" },
            { MapName.AnorLondo, "map\\MapStudio\\m15_01_00_00.msb" },
            { MapName.NewLondoRuinsValleyOfDrakes, "map\\MapStudio\\m16_00_00_00.msb" },
            { MapName.DukesArchives, "map\\MapStudio\\m17_00_00_00.msb" },
            { MapName.KilnOfTheFirstFlame, "map\\MapStudio\\m18_00_00_00.msb" },
            { MapName.NorthernUndeadAsylum, "map\\MapStudio\\m18_01_00_00.msb" }
        };
    }
}
