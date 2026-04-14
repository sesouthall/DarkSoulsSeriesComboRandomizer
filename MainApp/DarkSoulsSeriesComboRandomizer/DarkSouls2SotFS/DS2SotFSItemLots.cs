using SoulsFormats;
using static SoulsFormats.PARAM;

namespace DarkSoulsSeriesComboRandomizer.DarkSouls2SotFS
{
    internal class DS2SotFSItemLots
    {
        private readonly string rootDir;
        private BND4? regulationFile;
        private PARAM? itemLotParamChr;
        private PARAM? itemLotParamOther;

        private readonly Dictionary<(SoulsGame, SoulsItemType, int), int> _crossGameItems;

        private string RegulationFilePath => Path.Combine(rootDir, "enc_regulation.bnd.dcx");
        private string RegulationFileBackupPath => $"{RegulationFilePath}.unrandomized";

        public DS2SotFSItemLots(string rootDir, Dictionary<int, SoulsItem> injectedItemsInThisGame)
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
            regulationFile = ReadRegulationFile(RegulationFilePath);

            itemLotParamChr = PARAMUtils.LoadParam(regulationFile, "ItemLotParam2_Chr", @"ConfigFiles\PARAM\DS2S\Defs\ITEM_LOT_PARAM2.xml");
            itemLotParamOther = PARAMUtils.LoadParam(regulationFile, "ItemLotParam2_Other", @"ConfigFiles\PARAM\DS2S\Defs\ITEM_LOT_PARAM2.xml");

            foreach (var dataFileAndKey in LoadDS2Keys())
            {
                using var bhdStream = CryptographyUtil.DecryptRsa($@"{rootDir}\{dataFileAndKey.Item1}.bhd", dataFileAndKey.Item2);
                var bhd = BHD5.Read(bhdStream, BHD5.Game.DarkSouls2);
                using var bdt = File.OpenRead($@"{rootDir}\{dataFileAndKey.Item1}.bdt");

                foreach (var bucket in bhd.Buckets)
                {
                    foreach (var header in bucket)
                    {
                        var matchingMapFile = mapFiles.SingleOrDefault(mapFile => HashFileName(mapFile) == header.FileNameHash);
                        if (matchingMapFile != null)
                        {
                            var bytes = header.ReadFile(bdt);
                            var mapData = MSB2.Read(bytes);

                            var parsedMap = Map.DS2Maps.Single(map => matchingMapFile.Contains(map.FileName));

                            var generatorHeader = bhd.Buckets.SelectMany(bucket => bucket).Single(header => header.FileNameHash == HashFileName($"/param/generatorparam_{parsedMap.FileName}.param"));
                            var generatorBytes = generatorHeader.ReadFile(bdt);
                            var generatorData = PARAM.Read(generatorBytes);
                            generatorData.ApplyParamdef(PARAMDEF.XmlDeserialize(@"ConfigFiles\PARAM\DS2S\Defs\GENERATOR_PARAM.xml"));
                            LoadMapLocationData(mapData, generatorData, parsedMap);
                        }
                    }
                }
            }
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
            if (!File.Exists(RegulationFileBackupPath))
            {
                File.Copy(RegulationFilePath, RegulationFileBackupPath);
            }

            regulationFile.Files.Single(f => f.Name.Contains("ItemLotParam2_Chr")).Bytes = itemLotParamChr.Write();
            regulationFile.Files.Single(f => f.Name.Contains("ItemLotParam2_Other")).Bytes = itemLotParamOther.Write();
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

        private void LoadMapLocationData(MSB2 mapData, PARAM generatorData, Map parsedMap)
        {
            foreach (var enemy in generatorData.Rows)
            {
                // Skip NG+ enemies
                if (enemy.Cells.Any(cell => cell.Def.InternalName == "AppearanceEventId") && (uint)enemy["AppearanceEventId"].Value >= 2 && (uint)enemy["AppearanceEventId"].Value <= 8)
                {
                    continue;
                }

                var itemLotNumber = (uint?)enemy["ItemLotID1"]?.Value;
                if (itemLotNumber.HasValue && itemLotNumber.Value != 0)
                {
                    parsedMap.ItemLocations.AddRange(ParseEnemyItemLotChain(itemLotNumber.Value, LotType.UnspecifiedEnemy));
                }

                if (entityItemLots.ContainsKey((parsedMap.FileName, enemy.ID)))
                {
                    var eventItemLotNumber = entityItemLots[(parsedMap.FileName, enemy.ID)];
                    var lotType = GetEventLotType(eventItemLotNumber);
                    parsedMap.ItemLocations.Add(DS2SotFSItemLot.Parse(itemLotParamOther.Rows.Single(row => row.ID == eventItemLotNumber), lotType));
                }
            }

            foreach (var mapObject in mapData.Parts.Objects)
            {
                var itemLotNumber = mapObject.MapObjectInstanceParamID;
                if (itemLotNumber <= 0) continue; // No actual drop at this treasure
                parsedMap.ItemLocations.AddRange(ParseOtherItemLotChain((uint)itemLotNumber, LotType.Treasure));
            }
        }

        private static LotType GetEventLotType(int itemLotNumber)
        {
            return 100000 <= itemLotNumber && itemLotNumber < 900000 ? LotType.Boss : LotType.GenericEvent;
        }

        private IEnumerable<DS2SotFSItemLot> ParseEnemyItemLotChain(uint itemLotNumber, LotType lotTypeGuess)
        {
            Row? itemLot;
            while ((itemLot = itemLotParamChr?.Rows.SingleOrDefault(row => row.ID == itemLotNumber)) != null)
            {
                yield return DS2SotFSItemLot.Parse(itemLot, lotTypeGuess);
                itemLotNumber++;
            }
        }

        private IEnumerable<DS2SotFSItemLot> ParseOtherItemLotChain(uint itemLotNumber, LotType lotTypeGuess)
        {
            Row? itemLot;
            while ((itemLot = itemLotParamOther?.Rows.SingleOrDefault(row => row.ID == itemLotNumber)) != null)
            {
                yield return DS2SotFSItemLot.Parse(itemLot, lotTypeGuess);
                itemLotNumber++;
            }
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

        private static List<string> mapFiles = new List<string>
        {
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
        };

        // Id ranges from vawser's DS2-Scrambler
        private static readonly Dictionary<(string, int), int> entityItemLots = new Dictionary<(string, int), int>()
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
            { ("m10_32_00_00", 2041), 60009000 },
            // Other?
            { ("m10_10_00_00", 86), 1751000 }, // Cale gives House Key
        };
    }
}
