using DarkSoulsSeriesComboRandomizer;
using SoulsFormats;
using SoulsFormats.Cryptography;

namespace DarkSoulsItemMigrator
{
    // ---------------------------------------------------------------
    // LotSlot — one item slot extracted from a lot row, carrying
    // everything needed to write it back into any game's lot param.
    // ---------------------------------------------------------------
    record LotSlot(
        SoulsGame SourceGame,
        int ItemId,
        SoulsItemType ItemType,
        int Weight,
        int Amount
    );

    // ---------------------------------------------------------------
    // GameLotConfig — describes the field names and category value
    // mappings for one game's ItemLotParam table.
    //
    // NOTE: The category integer values below are best-guess from
    // community documentation. Verify against the actual paramdefs
    // in Smithbox before shipping.
    //   DSR/DS3:  Weapon=0, Armor=1, Accessory(Ring)=2, Good=4
    //   DS2:      Weapon=0, Armor=1, Ring=2,             Item=4
    // ---------------------------------------------------------------
    class GameLotConfig
    {
        public SoulsGame Game { get; init; }

        // Number of item slots per lot row (DSR=8, DS2=5, DS3=8 — verify)
        public int SlotCount { get; init; }

        // Field name patterns. {0} is replaced with the 1-based slot index.
        // e.g. "lotItemId0{0}" → "lotItemId01", "lotItemId02", ...
        public string ItemIdFieldPattern { get; init; } = null!;
        public string? ItemCategoryFieldPattern { get; init; }
        public string ItemWeightFieldPattern { get; init; } = null!;
        public string ItemAmountFieldPattern { get; init; } = null!;

        // Maps SoulsItemType → the integer the game stores in the category field.
        // Cross-game items are always migrated as goods, so only Good is strictly
        // required for cross-game writes, but all types are listed for completeness.
        public IReadOnlyDictionary<SoulsItemType, int> CategoryValues { get; init; } = null!;

        // Reverse map built automatically from CategoryValues
        public IReadOnlyDictionary<int, SoulsItemType> CategoryByValue { get; private set; } = null!;

        // Call after all init properties are set to build the reverse map.
        public GameLotConfig Build()
        {
            CategoryByValue = CategoryValues.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);
            return this;
        }

        // ---------------------------------------------------------------
        // Pre-built configs for each game.
        // ---------------------------------------------------------------

        public static readonly GameLotConfig DSR = new GameLotConfig
        {
            Game = SoulsGame.DSR,
            SlotCount = 8,   // verify against ItemLotParam paramdef
            ItemIdFieldPattern = "lotItemId0{0}",
            ItemCategoryFieldPattern = "lotItemCategory0{0}",
            ItemWeightFieldPattern = "lotItemBasePoint0{0}",
            ItemAmountFieldPattern = "lotItemNum0{0}",
            CategoryValues = new Dictionary<SoulsItemType, int>
            {
                [SoulsItemType.Weapon] = 0x00000000,
                [SoulsItemType.Armor] = 0x10000000,
                [SoulsItemType.Accessory] = 0x20000000,
                [SoulsItemType.Good] = 0x40000000,
            },
        }.Build();

        public static readonly GameLotConfig DS2 = new GameLotConfig
        {
            Game = SoulsGame.DS2,
            SlotCount = 9,
            ItemIdFieldPattern = "item_lot_{0}",
            ItemCategoryFieldPattern = null,  // DS2 has no category field — all items treated as Good
            ItemWeightFieldPattern = "chance_lot_{0}",
            ItemAmountFieldPattern = "amount_lot_{0}",
            CategoryValues = new Dictionary<SoulsItemType, int>
            {
                [SoulsItemType.Good] = 0,  // unused for DS2 but required by the type
            },
        }.Build();

        public static readonly GameLotConfig DS3 = new GameLotConfig
        {
            Game = SoulsGame.DS3,
            SlotCount = 8,   // verify against ItemLotParam paramdef
            ItemIdFieldPattern = "ItemLotId{0}",
            ItemCategoryFieldPattern = "lotItemCategory0{0}",
            ItemWeightFieldPattern = "lotItemBasePoint0{0}",
            ItemAmountFieldPattern = "LotItemNum{0}",
            CategoryValues = new Dictionary<SoulsItemType, int>
            {
                [SoulsItemType.Weapon] = 0x00000000,
                [SoulsItemType.Armor] = 0x10000000,
                [SoulsItemType.Accessory] = 0x20000000,
                [SoulsItemType.Good] = 0x40000000,
            },
        }.Build();
    }

    // ---------------------------------------------------------------
    // GameLotTable — wraps one game's loaded lot PARAM and its config.
    //
    // ExtractSlots() returns a list of per-row slot lists, preserving
    // each row's item count so ApplySlots() can restore the same
    // structure with different items.
    //
    // ApplySlots() takes a matching list of per-row slot lists and
    // writes each back into its corresponding row by index, translating
    // cross-game items to their injected goods ID in this game and
    // updating the category field accordingly.
    // ---------------------------------------------------------------
    class GameLotTable
    {
        private readonly GameLotConfig _config;
        private readonly PARAM _param;

        // Maps (sourceGame, itemType, originalItemId) → injected goods ID in this game,
        // used by ApplySlots to resolve cross-game item IDs. ItemType is included
        // because item IDs can overlap between types within the same game.
        private readonly Dictionary<(SoulsGame, SoulsItemType, int), int> _crossGameItems;

        public GameLotTable(
            PARAM param,
            GameLotConfig config,
            Dictionary<int, SoulsItem> injectedItemsInThisGame)
        {
            _param = param;
            _config = config;

            _crossGameItems = injectedItemsInThisGame
                .ToDictionary(
                    kvp => (kvp.Value.Game, kvp.Value.ItemType, kvp.Value.OriginalId),
                    kvp => kvp.Key
                );
        }

        /// <summary>
        /// Reads every non-empty item slot from every lot row and returns them
        /// as a list of per-row slot lists. Rows with no items are represented
        /// as empty lists. Slots with item ID 0 are skipped.
        /// </summary>
        public List<List<LotSlot>> ExtractSlots()
        {
            var rows = new List<List<LotSlot>>();

            foreach (var row in _param.Rows)
            {
                var rowSlots = new List<LotSlot>();

                for (int i = 1; i <= _config.SlotCount; i++)
                {
                    var itemId = Convert.ToInt32(
                        _param[row.ID][string.Format(_config.ItemIdFieldPattern, i)].Value);

                    var amount = Convert.ToInt32(
                        _param[row.ID][string.Format(_config.ItemAmountFieldPattern, i)].Value);

                    // DS2 marks empty slots with item ID 60510000 and amount 0
                    if (itemId == 0 || (_config.Game == SoulsGame.DS2 && itemId == 60510000 && amount == 0)) continue;

                    var weight = Convert.ToInt32(
                        _param[row.ID][string.Format(_config.ItemWeightFieldPattern, i)].Value);

                    var itemType = SoulsItemType.Good;
                    if (_config.ItemCategoryFieldPattern != null)
                    {
                        var categoryInt = Convert.ToInt32(
                            _param[row.ID][string.Format(_config.ItemCategoryFieldPattern, i)].Value);
                        itemType = _config.CategoryByValue.TryGetValue(categoryInt, out var t)
                            ? t
                            : SoulsItemType.Good;
                    }

                    rowSlots.Add(new LotSlot(_config.Game, itemId, itemType, weight, amount));
                }

                rows.Add(rowSlots);
            }

            return rows;
        }

        /// <summary>
        /// Writes a list of per-row slot lists back into the lot param.
        /// Each inner list corresponds to the lot row at the same index.
        /// Cross-game slots are resolved to their injected goods ID in this
        /// game; native slots use their item ID directly. Any slot positions
        /// beyond the assigned slots in a row are zeroed out.
        /// </summary>
        public void ApplySlots(List<List<LotSlot>> rows)
        {
            for (int rowIndex = 0; rowIndex < _param.Rows.Count; rowIndex++)
            {
                var row = _param.Rows[rowIndex];
                var rowSlots = rows[rowIndex];
                var slotIndex = 1;

                foreach (var slot in rowSlots)
                {
                    int resolvedId;
                    SoulsItemType resolvedType;

                    if (slot.SourceGame == _config.Game)
                    {
                        // Native item — use original ID and type directly.
                        resolvedId = slot.ItemId;
                        resolvedType = slot.ItemType;
                    }
                    else
                    {
                        // Cross-game item — look up its injected goods ID in this game.
                        // All cross-game items are stored as goods regardless of original type.
                        if (!_crossGameItems.TryGetValue((slot.SourceGame, slot.ItemType, slot.ItemId), out resolvedId))
                        {
                            Console.WriteLine(
                                $"  Warning: no injection record for {slot.SourceGame} item " +
                                $"{slot.ItemId} in {_config.Game} — slot zeroed.");
                            resolvedId = 0;
                            resolvedType = SoulsItemType.Good;
                        }
                        else
                        {
                            resolvedType = SoulsItemType.Good;
                        }
                    }

                    _param[row.ID][string.Format(_config.ItemIdFieldPattern, slotIndex)].Value
                        = resolvedId;
                    if (_config.ItemCategoryFieldPattern != null)
                        _param[row.ID][string.Format(_config.ItemCategoryFieldPattern, slotIndex)].Value
                            = _config.CategoryValues[resolvedType];
                    _param[row.ID][string.Format(_config.ItemWeightFieldPattern, slotIndex)].Value
                        = slot.Weight;
                    _param[row.ID][string.Format(_config.ItemAmountFieldPattern, slotIndex)].Value
                        = slot.Amount;

                    slotIndex++;
                }

                // Zero out any remaining slot positions in this row.
                // DS2 uses item ID 60510000 with amount 0 to mark empty slots.
                for (; slotIndex <= _config.SlotCount; slotIndex++)
                {
                    _param[row.ID][string.Format(_config.ItemIdFieldPattern, slotIndex)].Value
                        = _config.Game == SoulsGame.DS2 ? 60510000 : 0;
                    if (_config.ItemCategoryFieldPattern != null)
                        _param[row.ID][string.Format(_config.ItemCategoryFieldPattern, slotIndex)].Value = 0;
                    _param[row.ID][string.Format(_config.ItemWeightFieldPattern, slotIndex)].Value = 0;
                    _param[row.ID][string.Format(_config.ItemAmountFieldPattern, slotIndex)].Value = 0;
                }
            }
        }

        /// <summary>Returns the serialized bytes of the modified param.</summary>
        public byte[] Write() => _param.Write();
    }

    // ---------------------------------------------------------------
    // ItemLotRandomizer — loads lot params for all three games,
    // extracts all item slots into a combined pool while preserving
    // per-row slot counts, shuffles at the slot level, then
    // redistributes back into the original row structures.
    // ---------------------------------------------------------------
    class ItemLotRandomizer
    {
        private readonly string _dsrRoot;
        private readonly string _ds2Root;
        private readonly string _ds3Root;
        private readonly string _reportFolder;
        private readonly Random _rng;

        public ItemLotRandomizer(
            string dsrRoot,
            string ds2Root,
            string ds3Root,
            string reportFolder,
            int? seed = null)
        {
            _dsrRoot = dsrRoot;
            _ds2Root = ds2Root;
            _ds3Root = ds3Root;
            _reportFolder = reportFolder;
            _rng = seed.HasValue ? new Random(seed.Value) : new Random();
        }

        public void Randomize(string paramdefRoot)
        {
            Console.WriteLine("Loading injection maps...");
            var dsrInjected = SoulsItemCsvParser.ParseFile(
                Path.Combine(_reportFolder, "DSR_injected_items.csv"));
            var ds2Injected = SoulsItemCsvParser.ParseFile(
                Path.Combine(_reportFolder, "DS2_injected_items.csv"));
            var ds3Injected = SoulsItemCsvParser.ParseFile(
                Path.Combine(_reportFolder, "DS3_injected_items.csv"));

            Console.WriteLine("Loading lot params...");

            // DSR — BND3 parambnd
            var dsrParambnd = BND3.Read(
                Path.Combine(_dsrRoot, @"param\GameParam\GameParam.parambnd.dcx"));
            var dsrLotParam = LoadParam(
                dsrParambnd, "ItemLotParam",
                Path.Combine(paramdefRoot, @"DS1R\Defs\ItemLotParam.xml"));

            // DS2 — multiple lot tables in plain BND4 regulation
            var ds2Reg = BND4.Read(
                Path.Combine(_ds2Root, "enc_regulation.bnd.dcx"));
            var ds2LotChr = LoadParam(
                ds2Reg, "ItemLotParam_Chr",
                Path.Combine(paramdefRoot, @"DS2S\Defs\ItemLotParam_Chr.xml"));
            var ds2LotOther = LoadParam(
                ds2Reg, "ItemLotParam_Other",
                Path.Combine(paramdefRoot, @"DS2S\Defs\ItemLotParam_Other.xml"));
            var ds2LotSvr = LoadParam(
                ds2Reg, "ItemLotParam_SvrEvent",
                Path.Combine(paramdefRoot, @"DS2S\Defs\ItemLotParam_SvrEvent.xml"));

            // DS3 — encrypted regulation
            var ds3Reg = RegulationDecryptor.DecryptDS3Regulation(
                Path.Combine(_ds3Root, "Data0.bdt"));
            var ds3LotParam = LoadParam(
                ds3Reg, "ItemLotParam",
                Path.Combine(paramdefRoot, @"DS3\Defs\ItemLotParam.xml"));

            var dsrTable = new GameLotTable(dsrLotParam, GameLotConfig.DSR, dsrInjected);
            var ds2ChrTable = new GameLotTable(ds2LotChr, GameLotConfig.DS2, ds2Injected);
            var ds2OtherTable = new GameLotTable(ds2LotOther, GameLotConfig.DS2, ds2Injected);
            var ds2SvrTable = new GameLotTable(ds2LotSvr, GameLotConfig.DS2, ds2Injected);
            var ds3Table = new GameLotTable(ds3LotParam, GameLotConfig.DS3, ds3Injected);

            Console.WriteLine("Shuffling slots...");

            // Extract per-row slot lists from every table.
            // Each entry in allRows is (table, rowSlots) so we can apply back
            // to the right table after shuffling.
            var allRows = new List<(GameLotTable Table, List<LotSlot> RowSlots)>();
            foreach (var table in new[] { dsrTable, ds2ChrTable, ds2OtherTable, ds2SvrTable, ds3Table })
            {
                foreach (var rowSlots in table.ExtractSlots())
                    allRows.Add((table, rowSlots));
            }

            // Flatten all slots across all rows into one pool and shuffle.
            var allSlots = allRows.SelectMany(r => r.RowSlots).ToList();
            Shuffle(allSlots);

            // Refill each row from the shuffled pool, taking exactly as many
            // slots as the row originally held, preserving per-row item counts.
            var slotOffset = 0;
            var rebuiltRows = new Dictionary<GameLotTable, List<List<LotSlot>>>();

            foreach (var (table, rowSlots) in allRows)
            {
                var newRowSlots = allSlots.GetRange(slotOffset, rowSlots.Count);
                slotOffset += rowSlots.Count;

                if (!rebuiltRows.TryGetValue(table, out var tableRows))
                {
                    tableRows = new List<List<LotSlot>>();
                    rebuiltRows[table] = tableRows;
                }
                tableRows.Add(newRowSlots);
            }

            // Apply the rebuilt rows back into each table.
            foreach (var (table, tableRows) in rebuiltRows)
                table.ApplySlots(tableRows);

            Console.WriteLine("Writing lot params...");

            dsrParambnd.Files.Single(f => f.Name.Contains("ItemLotParam")).Bytes = dsrTable.Write();
            dsrParambnd.Write(Path.Combine(_dsrRoot, @"param\GameParam\GameParam.parambnd.dcx"));

            ds2Reg.Files.Single(f => f.Name.Contains("ItemLotParam_Chr")).Bytes = ds2ChrTable.Write();
            ds2Reg.Files.Single(f => f.Name.Contains("ItemLotParam_Other")).Bytes = ds2OtherTable.Write();
            ds2Reg.Files.Single(f => f.Name.Contains("ItemLotParam_SvrEvent")).Bytes = ds2SvrTable.Write();
            ds2Reg.Write(Path.Combine(_ds2Root, "enc_regulation.bnd.dcx"));

            ds3Reg.Files.Single(f => f.Name.Contains("ItemLotParam")).Bytes = ds3Table.Write();
            RegulationDecryptor.EncryptDS3Regulation(
                Path.Combine(_ds3Root, "Data0.bdt"), ds3Reg);

            Console.WriteLine("Done.");
        }

        private static PARAM LoadParam(IBinder bnd, string nameContains, string paramdefPath)
        {
            var param = PARAM.Read(bnd.Files.Single(f => f.Name.Contains(nameContains)).Bytes);
            param.ApplyParamdef(PARAMDEF.XmlDeserialize(paramdefPath));
            return param;
        }

        private void Shuffle<T>(List<T> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = _rng.Next(i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }
    }
}
