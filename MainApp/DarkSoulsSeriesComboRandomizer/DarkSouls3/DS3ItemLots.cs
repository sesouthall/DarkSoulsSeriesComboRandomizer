using SoulsFormats;
using SoulsFormats.Cryptography;
using static SoulsFormats.PARAM;

namespace DarkSoulsSeriesComboRandomizer.DarkSouls3
{
    internal class DS3ItemLots
    {
        private readonly string rootDir;
        private BND4? regulationFile;
        private PARAM? itemLotParam;
        private PARAM? npcParam;

        private readonly Dictionary<(SoulsGame, SoulsItemType, int), int> _crossGameItems;

        private string Data0Path => Path.Combine(rootDir, "Data0.bdt");
        private string Data0BackupPath => $"{Data0Path}.unrandomized";

        public DS3ItemLots(string rootDir, Dictionary<int, SoulsItem> injectedItemsInThisGame)
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
            regulationFile = RegulationDecryptor.DecryptDS3Regulation(Data0Path);

            itemLotParam = PARAMUtils.LoadParam(regulationFile, "ItemLotParam", @"ConfigFiles\PARAM\DS3\Defs\ItemLotParam.xml");
            npcParam = PARAMUtils.LoadParam(regulationFile, "NpcParam", @"ConfigFiles\PARAM\DS3\Defs\NpcParam.xml");

            foreach (var dataFileAndKey in dataFilesAndKeys)
            {
                using var bhdStream = CryptographyUtil.DecryptRsa($@"{rootDir}\{dataFileAndKey.Item1}.bhd", dataFileAndKey.Item2);
                var bhd = BHD5.Read(bhdStream, BHD5.Game.DarkSouls3);
                using var bdt = File.OpenRead($@"{rootDir}\{dataFileAndKey.Item1}.bdt");

                foreach (var bucket in bhd.Buckets)
                {
                    foreach (var header in bucket)
                    {
                        var matchingMapFile = mapFiles.SingleOrDefault(mapFile => HashFileName(mapFile) == header.FileNameHash);
                        if (matchingMapFile != null)
                        {
                            var bytes = header.ReadFile(bdt);
                            var mapData = MSB3.Read(bytes);

                            var parsedMap = Map.DS3Maps.Single(map => matchingMapFile.Contains(map.FileName));
                            LoadMapLocationData(mapData, parsedMap);
                        }
                    }
                }
            }
        }

        private static ulong HashFileName(string fileName)
        {
            return fileName.Aggregate(0u, (a, c) => a * 37 + c);
        }

        public void Save()
        {
            if (!File.Exists(Data0BackupPath))
            {
                File.Copy(Data0Path, Data0BackupPath);
            }

            regulationFile.Files.Single(f => f.Name.Contains("ItemLotParam")).Bytes = itemLotParam.Write();
            regulationFile.Write(Data0Path);
        }

        public void Revert()
        {
            if (File.Exists(Data0BackupPath))
            {
                File.Delete(Data0Path);
                File.Move(Data0BackupPath, Data0Path);
            }
        }

        private void LoadMapLocationData(MSB3 mapData, Map parsedMap)
        {
            foreach (var enemy in mapData.Parts.Enemies)
            {
                var enemyParam = npcParam?.Rows.FirstOrDefault(row => row.ID == enemy.NPCParamID);
                if (enemyParam == null) continue; // Enemy references missing NPC Param
                var cell = enemyParam["ItemLotId1"];
                if (cell == null) continue; // NPC Param is malformed
                var itemLotNumber = (int)cell.Value;
                if (itemLotNumber == -1) continue; // Enemy has no drops

                parsedMap.ItemLocations.AddRange(ParseItemLotChain(itemLotNumber, LotType.UnspecifiedEnemy));

                if (entityItemLots.ContainsKey(enemy.EntityID))
                {
                    var lotId = entityItemLots[enemy.EntityID];
                    parsedMap.ItemLocations.AddRange(ParseItemLotChain(lotId, GetEventLotType(itemLotNumber)));
                }
            }

            foreach (var treasure in mapData.Events.Treasures)
            {
                if (!mapData.Parts.Objects.Any(o => o.Name == treasure.TreasurePartName)) continue; // Treasure isn't obtainable
                var itemLotNumber = treasure.ItemLot1;
                if (itemLotNumber == -1) continue; // No actual drop at this treasure
                parsedMap.ItemLocations.AddRange(ParseItemLotChain(itemLotNumber, LotType.Treasure));
            }

            foreach (var part in mapData.Parts.Objects)
            {
                if (entityItemLots.ContainsKey(part.EntityID))
                {
                    var itemLotNumber = entityItemLots[part.EntityID];
                    parsedMap.ItemLocations.AddRange(ParseItemLotChain(itemLotNumber, GetEventLotType(itemLotNumber)));
                }
            }
        }

        private static LotType GetEventLotType(int itemLotNumber)
        {
            return 2000 <= itemLotNumber && itemLotNumber < 2500 ? LotType.Boss : LotType.GenericEvent;
        }

        private IEnumerable<DS3ItemLot> ParseItemLotChain(int itemLotNumber, LotType lotTypeGuess)
        {
            Row? itemLot;
            while ((itemLot = itemLotParam?.Rows.SingleOrDefault(row => row.ID == itemLotNumber)) != null)
            {
                yield return DS3ItemLot.Parse(itemLot, lotTypeGuess);
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

        // From UXM's full DS3 file list
        private static List<string> mapFiles = new List<string>
        {
            "/map/mapstudio/m30_00_00_00.msb.dcx",
            "/map/mapstudio/m30_01_00_00.msb.dcx",
            "/map/mapstudio/m30_02_00_00.msb.dcx",
            "/map/mapstudio/m31_00_00_00.msb.dcx",
            "/map/mapstudio/m31_01_00_00.msb.dcx",
            "/map/mapstudio/m31_02_00_00.msb.dcx",
            "/map/mapstudio/m31_03_00_00.msb.dcx",
            "/map/mapstudio/m31_04_00_00.msb.dcx",
            "/map/mapstudio/m31_05_00_00.msb.dcx",
            "/map/mapstudio/m31_06_00_00.msb.dcx",
            "/map/mapstudio/m31_07_00_00.msb.dcx",
            "/map/mapstudio/m31_08_00_00.msb.dcx",
            "/map/mapstudio/m31_09_00_00.msb.dcx",
            "/map/mapstudio/m31_90_00_00.msb.dcx",
            "/map/mapstudio/m32_00_00_00.msb.dcx",
            "/map/mapstudio/m32_90_00_00.msb.dcx",
            "/map/mapstudio/m33_00_00_00.msb.dcx",
            "/map/mapstudio/m33_01_00_00.msb.dcx",
            "/map/mapstudio/m34_00_00_00.msb.dcx",
            "/map/mapstudio/m34_01_00_00.msb.dcx",
            "/map/mapstudio/m35_00_00_00.msb.dcx",
            "/map/mapstudio/m36_00_00_00.msb.dcx",
            "/map/mapstudio/m36_90_00_00.msb.dcx",
            "/map/mapstudio/m37_00_00_00.msb.dcx",
            "/map/mapstudio/m38_00_00_00.msb.dcx",
            "/map/mapstudio/m39_00_00_00.msb.dcx",
            "/map/mapstudio/m40_00_00_00.msb.dcx",
            "/map/mapstudio/m41_00_00_00.msb.dcx",
            "/map/mapstudio/m45_00_00_00.msb.dcx",
            "/map/mapstudio/m46_00_00_00.msb.dcx",
            "/map/mapstudio/m47_00_00_00.msb.dcx",
            "/map/mapstudio/m50_00_00_00.msb.dcx",
            "/map/mapstudio/m51_00_00_00.msb.dcx",
            "/map/mapstudio/m51_01_00_00.msb.dcx",
            "/map/mapstudio/m53_00_00_00.msb.dcx",
            "/map/mapstudio/m54_00_00_00.msb.dcx",
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
    }
}
