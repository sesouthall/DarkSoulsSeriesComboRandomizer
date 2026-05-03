using SoulsFormats;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace DarkSoulsSeriesComboRandomizer.DarkSouls2SotFS
{
    internal class DS2SotFSItemLots(string rootDir, string saveDir, BND4 regulationFile, PARAM itemLotParamChr, PARAM itemLotParamOther, PARAM enemyParam, ItemLotFactory lotFactory, IReadOnlyDictionary<MapName, Map> maps, IReadOnlyList<Key> keys)
    {
        public IReadOnlyDictionary<MapName, Map> Maps { get; } = maps;
        public IReadOnlyList<Key> Keys { get; } = keys;

        public static DS2SotFSItemLots New(string rootDir, string saveDir)
        {
            var regulationFile = ReadRegulationFile(Path.Combine("PreModdedGameFiles", "UnrandomizedRegulationFiles", "enc_regulation.bnd.dcx"));

            var itemLotParamChr = PARAMUtils.LoadParam(regulationFile, "ItemLotParam2_Chr", @"ConfigFiles\PARAM\DS2S\Defs\ITEM_LOT_PARAM2.xml");
            var itemLotParamOther = PARAMUtils.LoadParam(regulationFile, "ItemLotParam2_Other", @"ConfigFiles\PARAM\DS2S\Defs\ITEM_LOT_PARAM2.xml");
            var enemyParam = PARAMUtils.LoadParam(regulationFile, "EnemyParam", @"ConfigFiles\PARAM\DS2S\Defs\CHR_PARAM.xml");

            var crossGameMapping = SoulsItemCsvParser.ParseFile(Path.Combine("ConfigFiles", "DS2S_injected_items.csv"));
            var reverseMapping = crossGameMapping.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);
            var lotSerializer = new DS2SotFSLotSlotSerializer(reverseMapping);
            var lotFactory = new ItemLotFactory(SoulsGame.DS2S, linkedItemLots, lotSerializer);

            var maps = Map.GetDSRMaps();
            var keys = Key.ConstructDS2Keys(maps);

            return new DS2SotFSItemLots(rootDir, saveDir, regulationFile, itemLotParamChr, itemLotParamOther, enemyParam, lotFactory, maps, keys);
        }

        public void Load()
        {
            foreach (var mapFile in mapFiles)
            {
                if (TryReadPackedFile(mapFile, (bytes) => MSB2.Read(bytes), out var mapData))
                {
                    var parsedMap = Maps.Values.Single(map => mapFile.Contains(map.FileName));

                    if (TryReadPackedFile($"/param/generatorparam_{parsedMap.FileName}.param", (bytes) => PARAM.Read(bytes), out var generatorData) &&
                        TryReadPackedFile($"/param/generatorregistparam_{parsedMap.FileName}.param", (bytes) => PARAM.Read(bytes), out var generatorRegistData))
                    {
                        generatorData.ApplyParamdef(PARAMDEF.XmlDeserialize(@"ConfigFiles\PARAM\DS2S\Defs\GENERATOR_PARAM.xml"));
                        generatorRegistData.ApplyParamdef(PARAMDEF.XmlDeserialize(@"ConfigFiles\PARAM\DS2S\Defs\GENERATOR_REGIST_PARAM.xml"));
                        LoadMapLocationData(mapData, generatorData, generatorRegistData, parsedMap);
                    }
                }
            }

            ClearDataFileCache();
        }

        private static BND4 ReadRegulationFile(string path)
        {
            using var regulationFileStream = File.OpenRead(path);
            using var regulationByteStream = new BinaryReaderEx(false, regulationFileStream);
            using var decompressedRegulationByteStream = SFUtil.GetDecompressedBinaryReader(regulationByteStream, out var compression);
            var headerString = decompressedRegulationByteStream.GetASCII(0);
            if (headerString == "BND4")
            {
                return BND4.Read(path);
            }
            else
            {
                return DecryptDS2Regulation(path);
            }
        }

        /// <summary>
        /// Copied from WitchyBND
        /// </summary>
        private static byte[] ds2RegulationKey =
        { 0x40, 0x17, 0x81, 0x30, 0xDF, 0x0A, 0x94, 0x54, 0x33, 0x09, 0xE1, 0x71, 0xEC, 0xBF, 0x25, 0x4C };

        /// <summary>
        /// Copied from WitchyBND
        /// Decrypts and unpacks DS2's regulation BND4 from the specified path.
        /// </summary>
        private static BND4 DecryptDS2Regulation(string path)
        {
            byte[] bytes = File.ReadAllBytes(path);
            byte[] iv = new byte[16];
            iv[0] = 0x80;
            Array.Copy(bytes, 0, iv, 1, 11);
            iv[15] = 1;
            byte[] input = new byte[bytes.Length - 32];
            Array.Copy(bytes, 32, input, 0, bytes.Length - 32);
            using (var ms = new MemoryStream(input))
            {
                byte[] decrypted = CryptographyUtil.DecryptAesCtr(ms, ds2RegulationKey, iv);
                return BND4.Read(decrypted);
            }
        }

        private static ulong HashFileName(string fileName)
        {
            return fileName.Aggregate(0u, (a, c) => a * 37 + c);
        }

        public void Save()
        {
            regulationFile.Files.Single(f => f.Name.Contains("ItemLotParam2_Chr")).Bytes = itemLotParamChr.Write();
            regulationFile.Files.Single(f => f.Name.Contains("ItemLotParam2_Other")).Bytes = itemLotParamOther.Write();
            regulationFile.Write(Path.Combine(saveDir, "enc_regulation.bnd.dcx"));

            var itemNamesFMG = FMG.Read(Path.Combine(rootDir, "ComboRandomizer", "menu", "text", "english", "itemname.fmg"));

            foreach (var map in Maps.Values)
            {
                foreach (var itemLocation in map.ItemLocations)
                {
                    foreach (var itemSlot in itemLocation.NewSlots.Where(slot => !slot.IsEmptyItem))
                    {
                        var (resolvedId, resolvedType) = itemSlot.ResolveFor(SoulsGame.DS2S);
                        var itemName = itemNamesFMG[resolvedId];
                        File.AppendAllLines(Path.Combine(saveDir, "Hints.txt"), [$"{itemName}: {map.FriendlyName} ({(itemLocation.LotType == LotType.RandomEnemyDrop ? "Random Drop" : "Fixed Treasure")})"]);
                    }
                }
            }

            ClearDataFileCache();
        }

        private readonly Dictionary<string, (BHD5, FileStream)> ds2DataFileCache = [];

        private (BHD5, FileStream) ReadDS2DataFile(string fileName, string key)
        {
            if (ds2DataFileCache.TryGetValue(fileName, out var streams))
            {
                return streams;
            }

            using var bhdStream = CryptographyUtil.DecryptRsa($@"{rootDir}\{fileName}.bhd", key);
            var bhd = BHD5.Read(bhdStream, BHD5.Game.DarkSouls2);
            var bdt = File.OpenRead($@"{rootDir}\{fileName}.bdt");
            ds2DataFileCache[fileName] = (bhd, bdt);
            return (bhd, bdt);
        }

        private void ClearDataFileCache()
        {
            foreach (var (_, bdt) in ds2DataFileCache.Values)
            {
                bdt.Dispose();
            }

            ds2DataFileCache.Clear();
        }

        private bool TryReadPackedFile<T>(string packedFilePath, Func<byte[], T> reader, [NotNullWhen(returnValue: true)] out T? result)
        {
            foreach (var dataFileAndKey in LoadDS2Keys())
            {
                var (bhd, bdt) = ReadDS2DataFile(dataFileAndKey.Item1, dataFileAndKey.Item2);

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

        private void LoadMapLocationData(MSB2 mapData, PARAM generatorData, PARAM generatorRegistData, Map parsedMap)
        {
            foreach (var enemy in generatorData.Rows)
            {
                // Skip NG+ enemies
                if (enemy.Cells.Any(cell => cell.Def.InternalName == "AppearanceEventId") &&
                    (uint)enemy["AppearanceEventId"].Value >= 2 &&
                    (uint)enemy["AppearanceEventId"].Value <= 8)
                {
                    continue;
                }

                var itemLotNumber = (uint?)enemy["ItemLotID1"]?.Value;
                if (itemLotNumber.HasValue && itemLotNumber.Value != 0)
                {
                    AssignLotChainToCorrectMap(parsedMap, lotFactory.ParseChain(Convert.ToInt32(itemLotNumber.Value), itemLotParamChr, LotType.UnspecifiedEnemy));
                }
                var generatorRegistrationNumber = (uint?)enemy["GeneratorRegistParam"]?.Value;
                if (generatorRegistrationNumber.HasValue && generatorRegistrationNumber.Value != 0)
                {
                    var enemyParamId = (int?)generatorRegistData.Rows.SingleOrDefault(row => row.ID == generatorRegistrationNumber.Value)?["EnemyParamID"]?.Value;
                    if (enemyParamId.HasValue && enemyParamId.Value != 0)
                    {
                        var dropLotNumber = (int?)enemyParam.Rows.SingleOrDefault(row => row.ID == enemyParamId.Value)?["death_itemlot_id"]?.Value;
                        if (dropLotNumber.HasValue && dropLotNumber.Value != 0)
                        {
                            AssignLotChainToCorrectMap(parsedMap, lotFactory.ParseChain(dropLotNumber.Value, itemLotParamChr, LotType.UnspecifiedEnemy));
                        }
                    }
                }

                if (entityItemLots.ContainsKey((parsedMap.FileName, enemy.ID)))
                {
                    var eventItemLotNumber = entityItemLots[(parsedMap.FileName, enemy.ID)];
                    var lotType = GetEventLotType(eventItemLotNumber);
                    // This item lot gets parsed directly because DS2 uses sequential event item lots
                    // to represent NG+ drops
                    var parsedLot = lotFactory.Parse([itemLotParamOther.Rows.Single(row => row.ID == eventItemLotNumber)],
                        lotType);
                    AssignLotChainToCorrectMap(parsedMap, [parsedLot]);
                }
            }

            foreach (var mapObject in mapData.Parts.Objects)
            {
                var itemLotNumber = mapObject.MapObjectInstanceParamID;
                if (itemLotNumber <= 0) continue; // No actual drop at this treasure
                AssignLotChainToCorrectMap(parsedMap, lotFactory.ParseChain(itemLotNumber, itemLotParamOther, LotType.Treasure));
            }
        }

        private void AssignLotChainToCorrectMap(Map defaultMap, IEnumerable<ItemLot> itemLots)
        {
            if (!itemLots.Any()) return;

            if (Map.DS2NonDefaultMapItemLots.Values.Any(set => set.Contains(itemLots.First().ID)))
            {
                var actualMapName = Map.DS2NonDefaultMapItemLots.Single(kvp => kvp.Value.Contains(itemLots.First().ID)).Key;
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
            return 100000 <= itemLotNumber && itemLotNumber < 900000 ? LotType.Boss : LotType.GenericEvent;
        }

        // Key discovery logic and archive names taken from UXM
        private List<(string, string)> LoadDS2Keys()
        {
            var archives = new List<string> { "GameDataEbl", "LqChrEbl", "LqMapEbl", "LqObjEbl", "LqPartsEbl" };
            var filesAndKeys = new List<(string, string)>();
            foreach (var archive in archives)
            {
                string pemPath = $@"{rootDir}\{archive.Replace("Ebl", "KeyCode")}.pem";
                filesAndKeys.Add((archive, File.ReadAllText(pemPath)));
            }

            return filesAndKeys;
        }

        private static readonly List<string> mapFiles =
        [
            "/map/m10_02_00_00/m10_02_00_00.msb",
            "/map/m10_04_00_00/m10_04_00_00.msb",
            "/map/m10_10_00_00/m10_10_00_00.msb",
            "/map/m10_14_00_00/m10_14_00_00.msb",
            "/map/m10_15_00_00/m10_15_00_00.msb",
            "/map/m10_16_00_00/m10_16_00_00.msb",
            "/map/m10_17_00_00/m10_17_00_00.msb",
            "/map/m10_18_00_00/m10_18_00_00.msb",
            "/map/m10_19_00_00/m10_19_00_00.msb",
            "/map/m10_23_00_00/m10_23_00_00.msb",
            "/map/m10_25_00_00/m10_25_00_00.msb",
            "/map/m10_27_00_00/m10_27_00_00.msb",
            "/map/m10_29_00_00/m10_29_00_00.msb",
            "/map/m10_30_00_00/m10_30_00_00.msb",
            "/map/m10_31_00_00/m10_31_00_00.msb",
            "/map/m10_32_00_00/m10_32_00_00.msb",
            "/map/m10_33_00_00/m10_33_00_00.msb",
            "/map/m10_34_00_00/m10_34_00_00.msb",
            "/map/m20_10_00_00/m20_10_00_00.msb",
            "/map/m20_11_00_00/m20_11_00_00.msb",
            "/map/m20_21_00_00/m20_21_00_00.msb",
            "/map/m20_24_00_00/m20_24_00_00.msb",
            "/map/m20_26_00_00/m20_26_00_00.msb",
            "/map/m40_03_00_00/m40_03_00_00.msb",
            "/map/m50_35_00_00/m50_35_00_00.msb",
            "/map/m50_36_00_00/m50_36_00_00.msb",
            "/map/m50_37_00_00/m50_37_00_00.msb",
            "/map/m50_38_00_00/m50_38_00_00.msb",
        ];

        // Id ranges from vawser's DS2-Scrambler
        private static readonly Dictionary<(string, int), int> entityItemLots = new()
        {
            // Boss drops (item lots 100000-900000)
            { ("m10_14_00_00", 2500), 106000 },
            { ("m10_23_00_00", 839), 154000 },
            { ("m10_15_00_00", 800), 212000 },
            { ("m10_33_00_00", 852), 223500 },
            { ("m10_34_00_00", 850), 226100 },
            { ("m10_18_00_00", 844), 303300 },
            { ("m10_19_00_00", 806), 305000 },
            { ("m50_36_00_00", 905), 305010 },
            { ("m10_10_00_00", 857), 309600 },
            { ("m10_31_00_00", 842), 309610 },
            { ("m20_10_00_00", 800), 309700 },
            { ("m10_10_00_00", 853), 318000 },
            { ("m10_16_00_00", 8000), 324000 },
            { ("m10_16_00_00", 845), 325000 },
            { ("m10_25_00_00", 851), 326000 },
            { ("m20_21_00_00", 860), 332000 },
            { ("m20_24_00_00", 800), 333000 },
            { ("m10_17_00_00", 800), 500000 },
            { ("m10_17_00_00", 801), 501000 },
            { ("m10_32_00_00", 824), 503000 },
            { ("m20_21_00_00", 825), 504000 },
            { ("m40_03_00_00", 811), 506100 },
            { ("m10_27_00_00", 800), 600000 },
            { ("m20_11_00_00", 802), 602000 },
            { ("m10_14_00_00", 803), 603000 },
            { ("m10_19_00_00", 807), 607000 },
            { ("m20_21_00_00", 863), 611000 },
            { ("m10_23_00_00", 816), 619100 },
            { ("m10_31_00_00", 843), 625000 },
            { ("m10_16_00_00", 848), 626000 },
            { ("m20_21_00_00", 859), 627000 },
            { ("m50_36_00_00", 903), 675000 },
            { ("m50_37_00_00", 907), 679000 },
            { ("m50_37_00_00", 909), 679010 },
            { ("m50_36_00_00", 904), 680000 },
            { ("m50_35_00_00", 901), 681000 },
            { ("m50_35_00_00", 900), 682000 },
            { ("m50_37_00_00", 908), 690000 },
            { ("m50_35_00_00", 721), 862000 },
            // Event drops (item lots 60001000-60100000)
            { ("m10_25_00_00", 6000), 60001000 },
            { ("m10_32_00_00", 2041), 60009000 },
            { ("m10_15_00_00", 5000), 60050000 },
            // Other?
            { ("m10_10_00_00", 86), 1751000 }, // Cale gives House Key
        };

        // Item lots that should always drop the same items. For example,
        // the Persuer's main fight, and the one-time fight outside the
        // Cardinal Tower bonfire.
        private static readonly List<HashSet<int>> linkedItemLots =
        [
            [318000, 60008000],
        ];

        public const int EstusFlaskLot = 1700000;
    }
}
