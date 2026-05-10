using SoulsFormats;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace DarkSoulsSeriesComboRandomizer.DarkSouls2SotFS
{
    public class DS2SotFSFiles(string rootDir) : ISoulsGameFiles
    {
        private const string ItemLotParamChrFileName = "ItemLotParam2_Chr";
        private const string ItemLotParamOtherFileName = "ItemLotParam2_Other";
        private const string EnemyParamFileName = "EnemyParam";
        private const string RegulationFileName = "enc_regulation.bnd.dcx";

        private PARAM? itemLotParamChr = null;
        public PARAM EnemyItemLotParam
        {
            get
            {
                itemLotParamChr ??= PARAMUtils.LoadParam(RegulationFile, ItemLotParamChrFileName, @"ConfigFiles\PARAM\DS2S\Defs\ITEM_LOT_PARAM2.xml");
                return itemLotParamChr;
            }
        }

        private PARAM? itemLotParamOther = null;
        public PARAM TreasureItemLotParam
        {
            get
            {
                itemLotParamOther ??= PARAMUtils.LoadParam(RegulationFile, ItemLotParamOtherFileName, @"ConfigFiles\PARAM\DS2S\Defs\ITEM_LOT_PARAM2.xml");
                return itemLotParamOther;
            }
        }

        public FMG ItemNamesFMG => FMG.Read(Path.Combine("PreModdedGameFiles", "DS2S", "ComboRandomizer", "menu", "text", "english", "itemname.fmg"));

        private BND4? regulationFile = null;
        private BND4 RegulationFile
        {
            get
            {
                regulationFile ??= ReadRegulationFile(Path.Combine("PreModdedGameFiles", "UnrandomizedRegulationFiles", RegulationFileName));
                return RegulationFile;
            }
        }

        private PARAM EnemyParam => PARAMUtils.LoadParam(RegulationFile, EnemyParamFileName, @"ConfigFiles\PARAM\DS2S\Defs\CHR_PARAM.xml");

        public IEnumerable<int> GetEnemyItemLotIds(MapName mapName)
        {
            var mapFilePath = mapFileNames[mapName];
            if (!TryGetGeneratorDataForMap(mapFilePath, out var generatorData) ||
                !TryGetGeneratorRegistDataForMap(mapFilePath, out var generatorRegistData))
            {
                yield break;
            }

            foreach (var enemy in generatorData.Rows)
            {
                // Skip NG+ enemies
                if ((uint)enemy["AppearanceEventId"].Value >= 2 &&
                    (uint)enemy["AppearanceEventId"].Value <= 8)
                {
                    continue;
                }

                var itemLotNumber = (uint)enemy["ItemLotID1"].Value;
                if (itemLotNumber != 0)
                {
                    yield return Convert.ToInt32(itemLotNumber);
                }

                var generatorRegistrationNumber = (uint)enemy["GeneratorRegistParam"].Value;
                if (generatorRegistrationNumber != 0)
                {
                    var enemyParamId = (int?)generatorRegistData.Rows.SingleOrDefault(row => row.ID == generatorRegistrationNumber)?["EnemyParamID"].Value;
                    if (enemyParamId.HasValue && enemyParamId.Value != 0)
                    {
                        var dropLotNumber = (int?)EnemyParam.Rows.SingleOrDefault(row => row.ID == enemyParamId.Value)?["death_itemlot_id"].Value;
                        if (dropLotNumber.HasValue && dropLotNumber.Value != 0)
                        {
                            yield return dropLotNumber.Value;
                        }
                    }
                }
            }
        }

        public IEnumerable<int> GetTreasureItemLotIds(MapName mapName)
        {
            var mapFilePath = mapFileNames[mapName];
            if (!TryGetMapDataForMap(mapFilePath, out var mapData))
            {
                yield break;
            }

            foreach (var mapObject in mapData.Parts.Objects)
            {
                var itemLotNumber = mapObject.MapObjectInstanceParamID;
                if (itemLotNumber <= 0) continue; // No actual drop at this treasure
                yield return itemLotNumber;
            }
        }

        public void SaveItemLotChanges(string destinationFolderPath)
        {
            if (itemLotParamChr != null)
            {
                RegulationFile.Files.Single(f => f.Name.Contains(ItemLotParamChrFileName)).Bytes = itemLotParamChr.Write();
            }
            if (itemLotParamOther != null)
            {
                RegulationFile.Files.Single(f => f.Name.Contains(ItemLotParamOtherFileName)).Bytes = itemLotParamOther.Write();
            }
            RegulationFile.Write(Path.Combine(destinationFolderPath, RegulationFileName));
        }

        private bool TryGetGeneratorDataForMap(string mapFilePath, [NotNullWhen(returnValue: true)] out PARAM? generatorData)
        {
            var mapFileName = Path.GetFileNameWithoutExtension(mapFilePath);
            if (TryReadPackedFile($"/param/generatorparam_{mapFileName}.param", (bytes) => PARAM.Read(bytes), out generatorData))
            {
                generatorData.ApplyParamdef(PARAMDEF.XmlDeserialize(@"ConfigFiles\PARAM\DS2S\Defs\GENERATOR_PARAM.xml"));
                return true;
            }
            generatorData = null;
            return false;
        }

        private bool TryGetGeneratorRegistDataForMap(string mapFilePath, [NotNullWhen(returnValue: true)] out PARAM? generatorRegistData)
        {
            var mapFileName = Path.GetFileNameWithoutExtension(mapFilePath);
            if (TryReadPackedFile($"/param/generatorregistparam_{mapFileName}.param", (bytes) => PARAM.Read(bytes), out generatorRegistData))
            {
                generatorRegistData.ApplyParamdef(PARAMDEF.XmlDeserialize(@"ConfigFiles\PARAM\DS2S\Defs\GENERATOR_REGIST_PARAM.xml"));
                return true;
            }
            generatorRegistData = null;
            return false;
        }

        private bool TryGetMapDataForMap(string mapFilePath, [NotNullWhen(returnValue: true)] out MSB2? mapData)
        {
            var mapFileName = Path.GetFileNameWithoutExtension(mapFilePath);
            if (TryReadPackedFile(mapFilePath, (bytes) => MSB2.Read(bytes), out mapData))
            {
                return true;
            }
            mapData = null;
            return false;
        }

        private static BND4 ReadRegulationFile(string path)
        {
            using var regulationFileStream = File.OpenRead(path);
            using var regulationByteStream = new BinaryReaderEx(false, regulationFileStream);
            using var decompressedRegulationByteStream = SFUtil.GetDecompressedBinaryReader(regulationByteStream, out var compression);
            var headerString = decompressedRegulationByteStream.GetASCII(0, 4);
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
        private static readonly byte[] ds2RegulationKey =
            [0x40, 0x17, 0x81, 0x30, 0xDF, 0x0A, 0x94, 0x54, 0x33, 0x09, 0xE1, 0x71, 0xEC, 0xBF, 0x25, 0x4C];

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

        private static ulong HashFileName(string fileName)
        {
            return fileName.Aggregate(0u, (a, c) => a * 37 + c);
        }

        private readonly Dictionary<MapName, string> mapFileNames = new()
        {
            { MapName.ThingsBetwixt, "/map/m10_02_00_00/m10_02_00_00.msb" },
            { MapName.Majula, "/map/m10_04_00_00/m10_04_00_00.msb" },
            { MapName.ForestOfFallenGiants, "/map/m10_10_00_00/m10_10_00_00.msb" },
            { MapName.BrightstoneCoveTseldora, "/map/m10_14_00_00/m10_14_00_00.msb" },
            { MapName.AldiasKeep, "/map/m10_15_00_00/m10_15_00_00.msb" },
            {MapName.TheLostBastilleBelfryLuna, "/map/m10_16_00_00/m10_16_00_00.msb" },
            {MapName.HarvestValleyEarthenPeak, "/map/m10_17_00_00/m10_17_00_00.msb" },
            {MapName.NomansWharf, "/map/m10_18_00_00/m10_18_00_00.msb" },
            {MapName.IronKeepBelfrySol, "/map/m10_19_00_00/m10_19_00_00.msb" },
            {MapName.TheGutterBlackGulch, "/map/m10_25_00_00/m10_25_00_00.msb" },
            {MapName.DragonAerieDragonShrine, "/map/m10_27_00_00/m10_27_00_00.msb" },
            {MapName.MajulaShadedWoods, "/map/m10_29_00_00/m10_29_00_00.msb" },
            {MapName.HeidesTowerNomansWharf, "/map/m10_30_00_00/m10_30_00_00.msb" },
            {MapName.HeidesTowerOfFlame, "/map/m10_31_00_00/m10_31_00_00.msb" },
            {MapName.ShadedWoodsShrineOfWinter, "/map/m10_32_00_00/m10_32_00_00.msb" },
            {MapName.DoorsOfPharros, "/map/m10_33_00_00/m10_33_00_00.msb" },
            {MapName.GraveOfSaints, "/map/m10_34_00_00/m10_34_00_00.msb" },
            {MapName.MemoryOfVammarOrroAndJeigh, "/map/m20_10_00_00/m20_10_00_00.msb" },
            {MapName.ShrineOfAmana, "/map/m20_11_00_00/m20_11_00_00.msb" },
            {MapName.DrangleicCastleThroneOfWant, "/map/m20_21_00_00/m20_21_00_00.msb" },
            {MapName.UndeadCrypt, "/map/m20_24_00_00/m20_24_00_00.msb" },
            {MapName.DragonMemories, "/map/m20_26_00_00/m20_26_00_00.msb" },
            {MapName.DarkChasmOfOld, "/map/m40_03_00_00/m40_03_00_00.msb" },
            {MapName.ShulvaSanctumCity, "/map/m50_35_00_00/m50_35_00_00.msb" },
            {MapName.BrumeTower, "/map/m50_36_00_00/m50_36_00_00.msb" },
            {MapName.FrozenEleumLoyce, "/map/m50_37_00_00/m50_37_00_00.msb" },
            {MapName.MemoryOfTheKing, "/map/m50_38_00_00/m50_38_00_00.msb" }
        };
    }
}
