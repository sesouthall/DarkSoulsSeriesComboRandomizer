using SoulsFormats;
using SoulsFormats.Cryptography;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using static SoulsFormats.PARAM;

namespace DarkSoulsSeriesComboRandomizer.DarkSouls3
{
    internal class DS3ItemLots
    {
        private readonly string rootDir;
        private readonly string saveDir;
        private BND4 regulationFile;
        private PARAM itemLotParam;
        private PARAM npcParam;

        private DS3ItemLots(string rootDir, string saveDir, BND4 regulationFile, PARAM itemLotParam, PARAM npcParam)
        {
            this.rootDir = rootDir;
            this.saveDir = saveDir;
            this.regulationFile = regulationFile;
            this.itemLotParam = itemLotParam;
            this.npcParam = npcParam;
        }

        public static DS3ItemLots New(string rootDir, string saveDir)
        {
            var regulationFile = RegulationDecryptor.DecryptDS3Regulation(Path.Combine("PreModdedGameFiles", "UnrandomizedRegulationFiles", "Data0.bdt"));

            var itemLotParam = PARAMUtils.LoadParam(regulationFile, "ItemLotParam", @"ConfigFiles\PARAM\DS3\Defs\ItemLotParam.xml");
            var npcParam = PARAMUtils.LoadParam(regulationFile, "NpcParam", @"ConfigFiles\PARAM\DS3\Defs\NpcParam.xml");

            return new DS3ItemLots(rootDir, saveDir, regulationFile, itemLotParam, npcParam);
        }

        public void Load()
        {
            foreach (var mapFile in mapFiles)
            {
                if (TryReadPackedFile(mapFile, (bytes) => MSB3.Read(bytes), out var mapData))
                {
                    var parsedMap = Map.DS3Maps.Values.Single(map => mapFile.Contains(map.FileName));
                    LoadMapLocationData(mapData, parsedMap);
                }
            }

            ClearDataFileCache();
        }

        private static ulong HashFileName(string fileName)
        {
            return fileName.Aggregate(0u, (a, c) => a * 37 + c);
        }

        private const string WeaponNameFMGFileName = "武器名";
        private const string ArmorNameFMGFileName = "防具名";
        private const string AccessoryNameFMGFileName = "アクセサリ名";
        private const string GoodsNameFMGFileName = "アイテム名";

        public void Save()
        {
            regulationFile.Files.Single(f => f.Name.Contains("ItemLotParam")).Bytes = itemLotParam.Write();
            RegulationDecryptor.EncryptDS3Regulation(Path.Combine(saveDir, "Data0.bdt"), regulationFile);

            var itemTextFile = BND4.Read(Path.Combine(rootDir, "ComboRandomizer", "msg", "engus", "item_dlc2.msgbnd.dcx"));

            var weaponNameFMG = itemTextFile.Files.Where(f => f.Name.Contains(WeaponNameFMGFileName)).Select(file => FMG.Read(file.Bytes));
            var armorNameFMG = itemTextFile.Files.Where(f => f.Name.Contains(ArmorNameFMGFileName)).Select(file => FMG.Read(file.Bytes));
            var accessoryNameFMG = itemTextFile.Files.Where(f => f.Name.Contains(AccessoryNameFMGFileName)).Select(file => FMG.Read(file.Bytes));
            var goodsNameFMG = itemTextFile.Files.Where(f => f.Name.Contains(GoodsNameFMGFileName)).Select(file => FMG.Read(file.Bytes));

            foreach (var map in Map.DS3Maps.Values)
            {
                foreach (var itemLocation in map.ItemLocations)
                {
                    foreach (var itemSlot in itemLocation.NewSlots.Where(slot => !slot.IsEmptyItem))
                    {
                        var (resolvedId, resolvedType) = itemSlot.ResolveFor(SoulsGame.DS3);
                        var itemName = resolvedType switch
                        {
                            SoulsItemType.Weapon => weaponNameFMG.FirstOrDefault(fmg => fmg[resolvedId] != null)?[resolvedId] ?? string.Empty,
                            SoulsItemType.Armor => armorNameFMG.FirstOrDefault(fmg => fmg[resolvedId] != null)?[resolvedId] ?? string.Empty,
                            SoulsItemType.Accessory => accessoryNameFMG.FirstOrDefault(fmg => fmg[resolvedId] != null)?[resolvedId] ?? string.Empty,
                            SoulsItemType.Goods => goodsNameFMG.FirstOrDefault(fmg => fmg[resolvedId] != null)?[resolvedId] ?? string.Empty,
                            _ => ""
                        };
                        File.AppendAllLines(Path.Combine(saveDir, "Hints.txt"), [$"{itemName}: {map.FriendlyName} ({(itemLocation.LotType == LotType.RandomEnemyDrop ? "Random Drop" : "Fixed Treasure")})"]);
                    }
                }
            }
        }

        private Dictionary<string, (BHD5, FileStream)> ds3DataFileCache = new Dictionary<string, (BHD5, FileStream)>();

        private (BHD5, FileStream) ReadDS3DataFile(string fileName, string key)
        {
            if (ds3DataFileCache.TryGetValue(fileName, out var streams))
            {
                return streams;
            }

            using var bhdStream = CryptographyUtil.DecryptRsa($@"{rootDir}\{fileName}.bhd", key);
            var bhd = BHD5.Read(bhdStream, BHD5.Game.DarkSouls3);
            var bdt = File.OpenRead($@"{rootDir}\{fileName}.bdt");
            ds3DataFileCache[fileName] = (bhd, bdt);
            return (bhd, bdt);
        }

        private void ClearDataFileCache()
        {
            foreach (var (_, bdt) in ds3DataFileCache.Values)
            {
                bdt.Dispose();
            }

            ds3DataFileCache.Clear();
        }

        private bool TryReadPackedFile<T>(string packedFilePath, Func<byte[], T> reader, [NotNullWhen(returnValue: true)] out T? result)
        {
            foreach (var dataFileAndKey in dataFilesAndKeys)
            {
                var (bhd, bdt) = ReadDS3DataFile(dataFileAndKey.Item1, dataFileAndKey.Item2);

                foreach (var bucket in bhd.Buckets)
                {
                    foreach (var header in bucket)
                    {
                        if (header.FileNameHash == HashFileName(packedFilePath))
                        {
                            result = reader(header.ReadFile(bdt))!;
                            return true;
                        }
                    }
                }
            }

            result = default;
            return false;
        }

        private void LoadMapLocationData(MSB3 mapData, Map parsedMap)
        {
            foreach (var enemy in mapData.Parts.Enemies)
            {
                var itemLotNumber = (int?) npcParam?.Rows.FirstOrDefault(row => row.ID == enemy.NPCParamID)?["ItemLotId1"]?.Value;
                if (itemLotNumber.HasValue && itemLotNumber.Value != -1)
                {
                    AssignLotChainToCorrectMap(parsedMap, ParseItemLotChain(itemLotNumber.Value, LotType.UnspecifiedEnemy));
                }
                

                if (entityItemLots.ContainsKey(enemy.EntityID))
                {
                    var lotId = entityItemLots[enemy.EntityID];
                    AssignLotChainToCorrectMap(parsedMap, ParseItemLotChain(lotId, GetEventLotType(lotId)));
                }
            }

            foreach (var treasure in mapData.Events.Treasures)
            {
                if (!mapData.Parts.Objects.Any(o => o.Name == treasure.TreasurePartName)) continue; // Treasure isn't obtainable
                var itemLotNumber = treasure.ItemLot1;
                if (itemLotNumber == -1) continue; // No actual drop at this treasure
                AssignLotChainToCorrectMap(parsedMap, ParseItemLotChain(itemLotNumber, LotType.Treasure));
            }

            foreach (var part in mapData.Parts.Objects)
            {
                if (entityItemLots.ContainsKey(part.EntityID))
                {
                    var itemLotNumber = entityItemLots[part.EntityID];
                    AssignLotChainToCorrectMap(parsedMap, ParseItemLotChain(itemLotNumber, GetEventLotType(itemLotNumber)));
                }
            }

            foreach (var talkLot in talkLots.Where(lot => lot.Item2 == parsedMap.Name))
            {
                AssignLotChainToCorrectMap(parsedMap, ParseItemLotChain(talkLot.Item1, LotType.GenericEvent));
            }
        }

        private void AssignLotChainToCorrectMap(Map defaultMap, IEnumerable<IItemLot> itemLots)
        {
            if (!itemLots.Any()) return;

            if (Map.DS3NonDefaultMapItemLots.Values.Any(set => set.Contains(itemLots.First().ID)))
            {
                var actualMapName = Map.DS3NonDefaultMapItemLots.Single(kvp => kvp.Value.Contains(itemLots.First().ID)).Key;
                var actualMap = Map.DS3Maps.Values.Single(map => map.Name == actualMapName);
                actualMap.ItemLocations.AddRange(itemLots);
            }
            else
            {
                defaultMap.ItemLocations.AddRange(itemLots);
            }
        }

        private static LotType GetEventLotType(int itemLotNumber)
        {
            return 2000 <= itemLotNumber && itemLotNumber < 2500 ? LotType.Boss : LotType.GenericEvent;
        }

        private IEnumerable<DS3ItemLot> ParseItemLotChain(int itemLotNumber, LotType lotTypeGuess)
        {
            Row? itemLot;
            while ((itemLot = itemLotParam.Rows.SingleOrDefault(row => row.ID == itemLotNumber)) != null)
            {
                yield return DS3ItemLot.Parse(itemLot, lotTypeGuess, itemLotParam);
                itemLotNumber++;
            }
        }

        // Stolen shamelessly from TheFifthMatt's SoulsRandomizers
        private static readonly Dictionary<int, int> entityItemLots = new Dictionary<int, int>()
        {
            // Bosses. Event 970
            { 3000800, 2000 },
            { 3000899, 2010 },
            { 3000830, 2020 },
            { 3010800, 2030 },
            { 3410830, 2040 },
            { 3100800, 2060 },
            { 3200800, 2070 },
            { 3200850, 2080 },
            { 3300850, 2090 },
            { 3300801, 2100 },
            { 3500800, 2110 },
            { 3700850, 2120 },
            { 3700800, 2130 },
            { 3800800, 2140 },
            { 3800830, 2150 },
            { 3900800, 2170 },
            { 4000800, 2180 },
            { 4000830, 2190 },
            { 4100800, 2200 },
            { 4500800, 2300 },
            { 4500860, 2310 },
            { 5000800, 2330 },
            { 5100800, 2340 },
            { 5100850, 2350 },
            { 5110800, 2360 },
            // Entity does not respawn. Event 20005341
            { 3000630, 21500000 },
            { 3010610, 13103000 },
            { 3010310, 21504000 },
            { 3010311, 21504010 },
            { 3100831, 30600000 },
            { 3100860, 13102000 },
            { 3100610, 21501000 },
            { 3100611, 21501010 },
            { 3200300, 31412000 },
            { 3200259, 21509000 },
            { 3300384, 31000000 },
            { 3300385, 21502030 },
            { 3300386, 21502040 },
            { 3300387, 21502050 },
            { 3300388, 21502060 },
            { 3300389, 21502000 },
            { 3300390, 21502010 },
            { 3300391, 21502020 },
            { 3300560, 52000000 },
            { 3300180, 57800 },
            { 3300182, 57800 },
            { 3300184, 57900 },
            { 3410200, 13210000 },
            { 3410210, 13101000 },
            { 3410370, 21505000 },
            { 3410371, 21505010 },
            { 3410372, 21505020 },
            { 3410373, 21505030 },
            { 3410374, 21505040 },
            { 3410375, 21505050 },
            { 3410376, 21505060 },
            { 3410377, 21505070 },
            { 3500370, 31001000 },
            { 3500372, 21503000 },
            { 3500373, 21503010 },
            { 3500669, 21800010 },
            { 3500194, 58700 },
            { 3700193, 58500 },
            { 3700194, 57900 },
            { 3700240, 22500000 },
            { 3700300, 21507000 },
            { 3700301, 21507010 },
            { 3700302, 21507020 },
            { 3700303, 21507030 },
            { 3700304, 21507040 },
            { 3800500, 30601000 },
            { 3800499, 22000000 },
            { 3800552, 21506020 },
            { 3800553, 21506030 },
            { 3800554, 21506040 },
            { 3800555, 21506000 },
            { 3800556, 21506010 },
            { 3800196, 58400 },
            { 3900192, 58600 },
            { 3900340, 21508000 },
            { 3900341, 21508010 },
            { 3900342, 21508020 },
            { 3900343, 21508030 },
            { 3900373, 20400020 },
            { 4000380, 31002000 },
            { 4000381, 31004000 },
            { 4000382, 31004000 },
            { 4000390, 21509500 },
            { 4500680, 21509600 },
            { 4500682, 21509620 },
            { 4500684, 21509640 },
            { 4500685, 21509650 },
            { 4500687, 21509670 },
            { 4500688, 21509680 },
            { 4500689, 21509690 },
            { 5100290, 21509800 },
            { 5100291, 21509810 },
            // Unused crystal lizards?
            // { 5100292, 21509820 },
            // { 5100293, 21509830 },
            // { 5100295, 21509850 },
            { 5100294, 21509840 },
            { 5100296, 21509860 },
            { 5100170, 59600 },
            { 5100172, 59700 },
            { 5100174, 59800 },
            // Entity respawns, but only drops once. Event 20005350
            { 3000238, 12800420 },
            { 3000352, 11901120 },
            { 3100570, 22800000 },
            { 3200291, 57400 },
            { 3200297, 57500 },
            { 3300200, 22700020 },
            { 3300494, 31200110 },
            { 3300495, 31200210 },
            { 3300510, 22702010 },
            { 3500586, 52230310 },
            { 3700350, 12303010 },
            { 3900259, 20601010 },
            // Entity respawns, but only drops once. Event 20005351
            { 5100300, 62800110 },
            { 5100310, 62800010 },
            { 5100320, 62800210 },
            { 5100240, 62600230 },
            { 5110240, 62600240 },
            // Treasure spawns after pot holding corpse breaks. Event 20005521
            { 3001251, 3000170 },
            // Treasure with no visible attached entity. Event 20005525
            { 3001260, 3000650 },
            { 3101290, 4200 },
            { 3101291, 3100630 },
            // { 3101292, 3100660 },  // Note: This one also has a treasure in the map itself... fixed in the area event script
            { 3201480, 3200300 },
            { 3201481, 3200310 },
            { 3301320, 3300950 },
            { 3301321, 3300960 },
            { 3301322, 3300970 },
            { 3301323, 3300980 },
            { 3501540, 3500850 },
            { 3501541, 3500860 },
            { 3501542, 3500870 },
            { 3501543, 3500880 },
            { 3501544, 3500890 },
            { 3701590, 3700840 },
            { 4001728, 4000300 },
            { 4001222, 4000340 },
            // Armor which spawns after killing another entity in a different area. Event 20005526
            { 3001900, 3000950 },
            { 3101700, 60830 },
            { 4001221, 4000330 },
            // Treasure with no visible attached entity. Event 20005527
            { 5101680, 5100670 },
            { 5101684, 5100900 },
            { 5101685, 5100910 },
            { 5101686, 5100920 },
            // Treasure available at different points in NPC quests. Often shows up in the NPC's final location. Event 20006030
            { 3101715, 62510 },
            { 3501715, 60920 },
            { 3701701, 50600 },
            { 3701722, 53000 },
            { 3901706, 62140 },
            { 4001727, 61610 },
            { 4001750, 60410 },
            { 4001760, 60730 },
            { 4001780, 60810 },
            { 4501711, 55500 },
            { 4501716, 55400 },
            { 5001700, 66200 },
            { 5101705, 66230 },
            // Entity does not respawn. Event 13000380 in High Wall
            { 3000660, 60940 },
            // Item awarded when using Path of the Dragon. Event 13205910 in Archdragon
            // There is no attached visible entity, so grab the closest enemy
            { 3200262, 3200900 },
            { 3200235, 3200910 },
            // Entity does not respawn. Event 13500276 in Cathedral
            { 3500668, 21800110 },
            { 4000700, 60200 },
            // Patches' conditional drop of Catarina Set. Event 20006032
            { 3500721, 52020 },
            { 3500720, 52020 },
            { 4000790, 52020 },
            // Custom unique drops from event scripts in specific maps
            { 3000850, 31410000 },
            { 3000705, 62320 },
            { 3010835, 31411000 },
            { 3010836, 31411100 },
            { 3100741, 4210 },
            { 3300720, 60710 },
            { 3410500, 12902200 },
            { 3700725, 60930 },
            { 3700241, 22501010 },
            { 3700706, 61930 },
            { 3901250, 3900900 },
            { 4500176, 59200 },
            { 4500802, 4700 },
            { 4500701, 55200 },
        };

        // Copied from TheFifthMatt's SoulsRandomizers
        // Associated maps are my own. Sometimes they are the zone where the item is obtained,
        // sometimes they are the zone that must be reached to trigger the dialog to get the item
        // Technically, it's possible to get to the triggering zone without having access to the
        // NPC/statue/other dialog location, but these won't get key items, so it should be fine.
        private static readonly List<(int, MapName)> talkLots = new List<(int, MapName)>
        {
            ( 4207, MapName.LothricCastle ), // Great Lightning Spear
            ( 4217, MapName.UndeadSettlement ), // Warmth
            ( 4220, MapName.RoadOfSacrificesFarronKeep ), // Watchdogs of Farron covenant item
            ( 4226, MapName.RoadOfSacrificesFarronKeep ), // Artorias Greatshield
            ( 4230, MapName.IrithyllAnorLondo ), // Aldrich Faithful covenant item
            ( 4237, MapName.IrithyllAnorLondo ), // Archdeacon's Great Staff
            ( 4240, MapName.RoadOfSacrificesFarronKeep ), // Blue Sentinels covenant item
            ( 4250, MapName.IrithyllAnorLondo ), // Blade of the Darkmoon covenant item
            ( 4260, MapName.CathedralOfTheDeep ), // Rosaria's Fingers covenant item
            ( 4267, MapName.CathedralOfTheDeep ), // Man-grub's Staff
            ( 4270, MapName.HighWallOfLothricGarden ), // Way of Blue covenant item
            ( 60300, MapName.IrithyllAnorLondo ), // Anri's Straight Sword
            //( 60310, MapName.GrandArchives ), // Twin Princes Greatsword - no event id, and at most one per randomizer playthrough anyway
            ( 60400, MapName.CatacombsCarthusSmoulderingLake ), // Morion Blade
            ( 60600, MapName.IrithyllAnorLondo ), // Blade of the Darkmoon covenant item again?
            ( 60700, MapName.RoadOfSacrificesFarronKeep ), // Heavy Gem from Hawkwood
            ( 60703, MapName.RoadOfSacrificesFarronKeep ), // Another Heavy Gem from Hawkwood? Maybe on-death drop?
            ( 60610, MapName.IrithyllAnorLondo ), // Darkmoon Ring
            ( 60630, MapName.IrithyllAnorLondo ), // Darkmoon Ring again?
            ( 60720, MapName.RoadOfSacrificesFarronKeep ), // Farron Ring
            ( 60805, MapName.IrithyllAnorLondo ), // Blessed Mail Breaker from Sirris
            ( 60900, MapName.UndeadSettlement ), // Cracked Red Eye Orb from Leonhard
            ( 60910, MapName.UndeadSettlement ), // Lift Key from Leonhard
            ( 61000, MapName.ArchdragonPeak ), // Hawkwood's Swordgrass
            ( 61200, MapName.UndeadSettlement ), // Blue Tearstone Ring
            ( 61300, MapName.RoadOfSacrificesFarronKeep ), // Young Dragon Ring
            ( 61310, MapName.CatacombsCarthusSmoulderingLake ), // Slumbering Dragoncrest Ring
            ( 61400, MapName.UndeadSettlement ), // Pyromancy Flame
            ( 61900, MapName.CathedralOfTheDeep ), // Ring of the Evil Eye
            ( 62000, MapName.CathedralOfTheDeep), // Rusted Coin from Patches
            ( 62010, MapName.FirelinkTower ), // Rusted Gold Coin from Patches
            ( 62100, MapName.UndeadSettlement ), // First Siegbrau
            ( 62103, MapName.IrithyllAnorLondo ), // Second Siegbrau
            ( 62105, MapName.DungeonProfanedCapital ), // Third Siegbrau
            ( 62120, MapName.DungeonProfanedCapital ), // Emit Force
            ( 62130, MapName.OldCell ), // Titanite Slab from Siegward
            ( 62300, MapName.HighWallOfLothricGarden ), // Small Lothric Banner
            ( 62310, MapName.HighWallOfLothricGarden ), // Way of Blue covenant item again?
            ( 62500, MapName.HighWallOfLothricGarden ), // Young White Branch from Giant
            ( 62600, MapName.RoadOfSacrificesFarronKeep ), // Blue Sentinels covenant item again?
            ( 63100, MapName.UndeadSettlement ), // Mound-makers covenant item
            ( 63110, MapName.PaintedWorldOfAriandel ), // Homeward bone from Drowsy Forlorn in the Painted World
            ( 65400, MapName.PaintedWorldSecondHalf ), // Titanite Slab from Corvian
            ( 65500, MapName.PaintedWorldOfAriandel ), // Chillbite Ring
            ( 66210, MapName.DregHeap ), // Titanite Slab from Lapp
            ( 66220, MapName.RingedCity ), // Seigbrau from Lapp
            ( 66300, MapName.RingedCity ), // Sacred Chime of Filianore
            ( 66310, MapName.RingedCity ), // Titanite Slab from Shira
        };

        // From UXM's full DS3 file list
        private static List<string> mapFiles = new List<string>
        {
            "/map/mapstudio/m30_00_00_00.msb.dcx",
            "/map/mapstudio/m30_01_00_00.msb.dcx",
            "/map/mapstudio/m31_00_00_00.msb.dcx",
            "/map/mapstudio/m32_00_00_00.msb.dcx",
            "/map/mapstudio/m33_00_00_00.msb.dcx",
            "/map/mapstudio/m34_01_00_00.msb.dcx",
            "/map/mapstudio/m35_00_00_00.msb.dcx",
            "/map/mapstudio/m37_00_00_00.msb.dcx",
            "/map/mapstudio/m38_00_00_00.msb.dcx",
            "/map/mapstudio/m39_00_00_00.msb.dcx",
            "/map/mapstudio/m40_00_00_00.msb.dcx",
            "/map/mapstudio/m41_00_00_00.msb.dcx",
            "/map/mapstudio/m45_00_00_00.msb.dcx",
            "/map/mapstudio/m50_00_00_00.msb.dcx",
            "/map/mapstudio/m51_00_00_00.msb.dcx",
            "/map/mapstudio/m51_01_00_00.msb.dcx",
        };

        // From UXM's ArchiveKeys
        private static List<(string, string)> dataFilesAndKeys = new List<(string, string)>
        {
            ("Data1", @"-----BEGIN RSA PUBLIC KEY-----
MIIBCwKCAQEA05hqyboW/qZaJ3GBIABFVt1X1aa0/sKINklvpkTRC+5Ytbxvp18L
M1gN6gjTgSJiPUgdlaMbptVa66MzvilEk60aHyVVEhtFWy+HzUZ3xRQm6r/2qsK3
8wXndgEU5JIT2jrBXZcZfYDCkUkjsGVkYqjBNKfp+c5jlnNwbieUihWTSEO+DA8n
aaCCzZD3e7rKhDQyLCkpdsGmuqBvl02Ou7QeehbPPno78mOYs2XkP6NGqbFFGQwa
swyyyXlQ23N15ZaFGRRR0xYjrX4LSe6OJ8Mx/Zkec0o7L28CgwCTmcD2wO8TEATE
AUbbV+1Su9uq2+wQxgnsAp+xzhn9og9hmwIEC35bSQ==
-----END RSA PUBLIC KEY-----"),

            ("Data2", @"-----BEGIN RSA PUBLIC KEY-----
MIIBCwKCAQEAvCZAK9UfPdk5JaTlG7n1r0LSVzIan3h0BSLaMXQHOwO7tTGpvtdX
m2ZLY9y8SVmOxWTQqRq14aVGLTKDyH87hPuKd47Y0E5K5erTqBbXW6AD4El1eir2
VJz/pwHt73FVziOlAnao1A5MsAylZ9B5QJyzHJQG+LxzMzmWScyeXlQLOKudfiIG
0qFw/xhRMLNAI+iypkzO5NKblYIySUV5Dx7649XdsZ5UIwJUhxONsKuGS+MbeTFB
mTMehtNj5EwPxGdT4CBPAWdeyPhpoHJHCbgrtnN9akwQmpwdBBxT/sTD16Adn9B+
TxuGDQQALed4S4KvM+fadx27pQz8pP9VLwIEL67iCQ==
-----END RSA PUBLIC KEY-----"),

            ("Data3", @"-----BEGIN RSA PUBLIC KEY-----
MIIBCwKCAQEAqLytWD20TSXPeAA1RGDwPW18nJwe2rBX+0HPtdzFmQc/KmQlWrP+
94k6KClK5f7m0xUHwT8+yFGLxPdRvUPyOhBEnRA6tkObVDSxij5y0Jh4h4ilAO73
I8VMcmscS71UKkck4444+eR4vVd+SPlzIu8VgqLefvEn/sX/pAevDp7w+gD0NgvO
e9U6iWEXKwTOPB97X+Y2uB03gSSognmV8h2dtUFJ4Ryn5jrpWmsuUbdvGp0CWBKH
CFruNXnfsG0hlf9LqbVmEzbFl/MhjBmbVjjtelorZsoLPK+OiPTHW5EcwwnPh1vH
FFGM7qRMc0yvHqJnniEWDsSz8Bvg+GxpgQIEC8XNVw==
-----END RSA PUBLIC KEY-----"),

            ("Data4", @"-----BEGIN RSA PUBLIC KEY-----
MIIBCwKCAQEArfUaZWjYAUaZ0q+5znpX55GeyepawCZ5NnsMjIW9CA3vrOgUGRkh
6aAU9frlafQ81LQMRgAznOnQGE7K3ChfySDpq6b47SKm4bWPqd7Ulh2DTxIgi6QP
qm4UUJL2dkLaCnuoya/pGMOOvhT1LD/0CKo/iKwfBcYf/OAnwSnxMRC3SNRugyvF
ylCet9DEdL5L8uBEa4sV4U288ZxZSZLg2tB10xy5SHAsm1VNP4Eqw5iJbqHEDKZW
n2LJP5t5wpEJvV2ACiA4U5fyjQLDzRwtCKzeK7yFkKiZI95JJhU/3DnVvssjIxku
gYZkS9D3k9m+tkNe0VVrd4mBEmqVxg+V9wIEL6Y6tw==
-----END RSA PUBLIC KEY-----"),

            ("Data5", @"-----BEGIN RSA PUBLIC KEY-----
MIIBCwKCAQEAvKTlU3nka4nQesRnYg1NWovCCTLhEBAnjmXwI69lFYfc4lvZsTrQ
E0Y25PtoP0ZddA3nzflJNz1rBwAkqfBRGTeeTCAyoNp/iel3EAkid/pKOt3JEkHx
rojRuWYSQ0EQawcBbzCfdLEjizmREepRKHIUSDWgu0HTmwSFHHeCFbpBA1h99L2X
izH5XFTOu0UIcUmBLsK6DYsIj5QGrWaxwwXcTJN/X+/syJ/TbQK9W/TCGaGiirGM
1u2wvZXSZ7uVM3CHwgNhAMiqLvqORygcDeNqxgq+dXDTxka43j7iPJWdHs8b25fy
aH3kbUxKlDGaEENNNyZQcQrgz8Q76jIE0QIEFUsz9w==
-----END RSA PUBLIC KEY-----"),

            ("DLC1", @"-----BEGIN RSA PUBLIC KEY-----
MIIBCwKCAQEAsCGM9dFwzaIOUIin3DXy7xrmI2otKGLZJQyKi5X3znKhSTywpcFc
KoW6hgjeh4fJW24jhzwBosG6eAzDINm+K02pHCG8qZ/D/hIbu+ui0ENDKqrVyFhn
QtX5/QJkVQtj8M4a0FIfdtE3wkxaKtP6IXWIy4DesSdGWONVWLfi2eq62A5ts5MF
qMoSV3XjTYuCgXqZQ6eOE+NIBQRqpZxLNFSzbJwWXpAg2kBMkpy5+ywOByjmWzUw
jnIFl1T17R8DpTU/93ojx+/q1p+b1o5is5KcoP7QwjOqzjHJH8bTytzRbgmRcDMW
3ahxgI070d45TMXK2YwRzI6/JbM1P29anQIEFezyYw==
-----END RSA PUBLIC KEY-----"),

            ("DLC2", @"-----BEGIN RSA PUBLIC KEY-----
MIIBCwKCAQEAtCXU9a/GBMVoqtpQox9p0/5sWPaIvDp8avLFnIBhN7vkgTwulZHi
u64vZAiUAdVeFX4F+Qtk+5ivK488Mu2CzAMJcz5RvyMQJtOQXuDDqzIv21Tr5zuu
sswoErHxxP8TZNxkHm7Ram7Oqtn7LQnMTYxsBgZZ34yJkRtAmZnGoCu5YaUR5euk
8lF75idi97ssczUNV212tLzIMa1YOV7sxOb7+gc0VTIqs3pa+OXLPI/bMfwUc/KN
jur5aLDDntQHGx5zuNtc78gMGwlmPqDhgTusKPO4VyKvoL0kITYvukoXJATaa1HI
WVUjhLm+/uj8r8PNgolerDeS+8FM5Bpe9QIEHwCZLw==
-----END RSA PUBLIC KEY-----")
        };

        public const int AshenEstusFlaskLot = 4000505;
    }
}
