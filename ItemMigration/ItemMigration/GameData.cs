using SoulsFormats;
using System.Text.RegularExpressions;

namespace DarkSoulsItemMigrator
{
    // ---------------------------------------------------------------
    // GameData — base class holding everything needed to read items out
    // of a game (GetAllItems) and write new goods into it (AddNewGood).
    //
    // Subclasses wire up the game-specific GameFile objects and supply
    // the IBinder accessors, FMG filenames, and paramdef paths.
    //
    // The target goods param and FMGs are loaded lazily on first access
    // so that AddNewGood can accumulate all additions before Flush()
    // writes the final bytes back into the in-memory BNDs.
    // ---------------------------------------------------------------
    abstract class GameData
    {
        // Icon sheet
        public TpfFile Icons { get; protected init; } = null!;
        public string IconTextureName { get; protected init; } = null!;
        public string IconSheetSourcePath { get; protected init; } = null!;

        // Target goods param and FMG names (all incoming items land here)
        public Regex GoodsParamRegex { get; protected init; } = null!;
        public string GoodsParamdefPath { get; protected init; } = null!;
        public string TargetNameFmgName { get; protected init; } = null!;
        public string TargetDescFmgName { get; protected init; } = null!;
        public string TargetLongDescFmgName { get; protected init; } = null!;

        // ID management
        public int BaseInjectedId { get; protected init; }
        public int DSRIconId { get; protected init; }
        public int DS2SIconId { get; protected init; }
        public int DS3IconId { get; protected init; }
        public string IconIdCellName { get; protected init; } = null!;

        // Source item types to iterate when this game is the migration source
        public IReadOnlyList<ItemType> ItemTypes { get; protected init; } = null!;

        // Template row cloned for every new good added to this game
        public PARAM.Row DefaultParamRow { get; protected init; } = null!;

        // Lazy-loaded target state, accumulated across AddNewGood() calls
        // and flushed back to the BNDs in Flush().
        private PARAM? _targetParam;
        private FMG? _targetNameFmg;
        private FMG? _targetDescFmg;
        private FMG? _targetLongDescFmg;
        private int _nextItemId;

        protected abstract SourceGame GameName { get; }
        protected abstract IBinder ParamBnd { get; }
        protected abstract IBinder MsgBnd { get; }

        private void EnsureTargetLoaded()
        {
            if (_targetParam != null) return;

            _targetParam = PARAM.Read(ParamBnd.Files.Single(f => GoodsParamRegex.IsMatch(f.Name)).Bytes);
            _targetParam.ApplyParamdef(PARAMDEF.XmlDeserialize(GoodsParamdefPath));

            _targetNameFmg = FMG.Read(MsgBnd.Files.First(f => f.Name.Contains($"\\{TargetNameFmgName}")).Bytes);
            _targetDescFmg = FMG.Read(MsgBnd.Files.First(f => f.Name.Contains($"\\{TargetDescFmgName}")).Bytes);
            _targetLongDescFmg = FMG.Read(MsgBnd.Files.First(f => f.Name.Contains($"\\{TargetLongDescFmgName}")).Bytes);

            _nextItemId = BaseInjectedId;
        }

        // ---------------------------------------------------------------
        // GetAllItems — yields every qualifying item from this game's
        // source params and FMGs, skipping rows at or above SkipFromId.
        // ---------------------------------------------------------------
        public IEnumerable<SourceItem> GetAllItems()
        {
            foreach (var itemType in ItemTypes)
            {
                var sourceParam = PARAM.Read(ParamBnd.Files.Single(f => itemType.ParamFileLookupPattern.IsMatch(f.Name)).Bytes);
                sourceParam.ApplyParamdef(PARAMDEF.XmlDeserialize(itemType.ParamdefPath));

                var nameFmg = FMG.Read(MsgBnd.Files.First(f => f.Name.Contains($"\\{itemType.NameFmgName}")).Bytes);
                var longDescFmg = FMG.Read(MsgBnd.Files.First(f => f.Name.Contains($"\\{itemType.LongDescFmgName}")).Bytes);
                FMG? descFmg = itemType.DescFmgName != null
                    ? FMG.Read(MsgBnd.Files.First(f => f.Name.Contains($"\\{itemType.DescFmgName}")).Bytes)
                    : null;

                foreach (var row in sourceParam.Rows)
                {
                    if (row.ID >= itemType.SkipFromId) continue;

                    yield return new SourceItem(
                        Name: nameFmg[row.ID],
                        Desc: descFmg?[row.ID] ?? string.Empty,
                        LongDesc: longDescFmg[row.ID],
                        SourceRow: row,
                        SourceGame: GameName,
                        SourceCategory: itemType.ParamName
                    );
                }
            }
        }

        // ---------------------------------------------------------------
        // AddNewGood — appends one item into this game's goods param and
        // goods FMGs at the next sequential injected ID.
        // ---------------------------------------------------------------
        public void AddNewGood(SourceItem item)
        {
            EnsureTargetLoaded();

            _targetNameFmg!.Entries.Add(new FMG.Entry(_nextItemId, item.Name));
            _targetDescFmg!.Entries.Add(new FMG.Entry(_nextItemId, item.Desc));
            _targetLongDescFmg!.Entries.Add(new FMG.Entry(_nextItemId, item.LongDesc));

            var newRow = new PARAM.Row(DefaultParamRow);
            newRow.ID = _nextItemId;
            newRow[IconIdCellName].Value = item.SourceGame switch
            {
                SourceGame.DSR => DSRIconId,
                SourceGame.DS2S => DS2SIconId,
                SourceGame.DS3 => DS3IconId,
                _ => throw new NotImplementedException()
            };
            _targetParam!.Rows.Add(newRow);

            _injectionLog.Add(new InjectionRecord(_nextItemId, item.SourceGame, item.SourceCategory, item.SourceRow.ID));

            _nextItemId++;
        }

        // ---------------------------------------------------------------
        // Flush — writes the accumulated param and FMG changes back into
        // the in-memory BNDs so SaveAll() can write them to disk.
        // Also replaces the icon sheet texture with the custom one.
        // ---------------------------------------------------------------
        public void Flush()
        {
            if (_targetParam == null) return; // nothing was added

            MsgBnd.Files.First(f => f.Name.Contains($"\\{TargetNameFmgName}")).Bytes = _targetNameFmg!.Write();
            MsgBnd.Files.First(f => f.Name.Contains($"\\{TargetDescFmgName}")).Bytes = _targetDescFmg!.Write();
            MsgBnd.Files.First(f => f.Name.Contains($"\\{TargetLongDescFmgName}")).Bytes = _targetLongDescFmg!.Write();
            ParamBnd.Files.Single(f => GoodsParamRegex.IsMatch(f.Name)).Bytes = _targetParam.Write();

            ReplaceIconSheet();
        }

        protected virtual void ReplaceIconSheet()
        {
            var customIcons = File.ReadAllBytes(IconSheetSourcePath);
            var oldTexture = Icons.Tpf.Textures.Single(t => t.Name.Contains(IconTextureName));
            var oldIndex = Icons.Tpf.Textures.IndexOf(oldTexture);
            Icons.Tpf.Textures.Remove(oldTexture);
            Icons.Tpf.Textures.Insert(oldIndex, new TPF.Texture(IconTextureName, oldTexture.Format, oldTexture.Flags1, customIcons, oldTexture.Platform));
        }

        // ---------------------------------------------------------------
        // WriteReport — writes a CSV mapping each injected goods ID back
        // to the source game, source item category, and source row ID.
        // One file is written per target game into outputFolder.
        // ---------------------------------------------------------------
        private readonly List<InjectionRecord> _injectionLog = [];

        public void WriteReport(string outputFolder)
        {
            if (_injectionLog.Count == 0) return;

            Directory.CreateDirectory(outputFolder);
            var csvPath = System.IO.Path.Combine(outputFolder, $"{GameName}_injected_items.csv");

            using var writer = new StreamWriter(csvPath, append: false, encoding: System.Text.Encoding.UTF8);
            writer.WriteLine("InjectedId,SourceGame,SourceCategory,SourceRowId");
            foreach (var record in _injectionLog)
                writer.WriteLine($"{record.InjectedId},{record.SourceGame},{record.SourceCategory},{record.SourceRowId}");

            Console.WriteLine($"  Report written to {csvPath}");
        }
    }
}
