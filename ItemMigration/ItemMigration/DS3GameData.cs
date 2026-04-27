using SoulsFormats;
using System.Text.RegularExpressions;

namespace DarkSoulsItemMigrator
{
    // ---------------------------------------------------------------
    // DS3GameData — wires up DS3's BND4/regulation files and Japanese
    // FMG names. Rows >= baseInjectedId are DSR items injected by the
    // first migration and are skipped when DS3 is the source.
    // ---------------------------------------------------------------
    class DS3GameData : GameData
    {
        private readonly DS3RegulationFile _paramBnd;
        private readonly Bnd4File _itemMsgBnd;
        private readonly Bnd4File _menuMsgBnd;
        private readonly string _talkScriptFolder;

        protected override SourceGame GameName => SourceGame.DS3;
        protected override IBinder ParamBnd => _paramBnd.Bnd;
        protected override IBinder ItemMsgBnd => _itemMsgBnd.Bnd;
        protected override IBinder MenuMsgBnd => _menuMsgBnd.Bnd;

        public DS3GameData(Bnd4File itemMsgBnd, Bnd4File menuMsgBnd, DS3RegulationFile paramBnd, TpfFile icons, string talkScriptFolder, int baseInjectedId)
        {
            _itemMsgBnd = itemMsgBnd;
            _menuMsgBnd = menuMsgBnd;
            _paramBnd = paramBnd;
            _talkScriptFolder = talkScriptFolder;

            Icons = icons;
            IconTextureName = "MENU_Icon_00000";
            IconSheetSourcePath = @"Icons\MENU_Icon_00000.dds";
            GoodsParamRegex = new Regex("EquipParamGoods");
            GoodsParamdefPath = $@"{Program.SmithboxRoot}\DS3\Defs\EquipParamGoods.xml";
            TargetNameFmgName = "アイテム名";
            TargetDescFmgName = "アイテム説明";
            TargetLongDescFmgName = "アイテムうんちく";
            BonfireTextFmgName = "イベントテキスト.fmg";
            DSRIconId = 20;
            DS2SIconId = 21;
            IconIdCellName = "iconId";
            BaseInjectedId = baseInjectedId;

            var goodsParam = PARAM.Read(paramBnd.Bnd.Files.Single(f => f.Name.Contains("EquipParamGoods")).Bytes);
            goodsParam.ApplyParamdef(PARAMDEF.XmlDeserialize(GoodsParamdefPath));
            DefaultParamRow = goodsParam.Rows.Single(row => row.ID == 375); // Sunlight Medal

            ItemTypes = new List<ItemType>
            {
                new("Weapon",    $@"{Program.SmithboxRoot}\DS3\Defs\EquipParamWeapon.xml",    "武器名",       null,            "武器うんちく",       int.MaxValue, new Regex("EquipParamWeapon")),
                new("Armor", $@"{Program.SmithboxRoot}\DS3\Defs\EquipParamProtector.xml", "防具名",       null,            "防具うんちく",       int.MaxValue, new Regex("EquipParamProtector")),
                new("Accessory", $@"{Program.SmithboxRoot}\DS3\Defs\EquipParamAccessory.xml", "アクセサリ名", "アクセサリ説明", "アクセサリうんちく", int.MaxValue, new Regex("EquipParamAccessory")),
                new("Goods",     $@"{Program.SmithboxRoot}\DS3\Defs\EquipParamGoods.xml",     "アイテム名",   "アイテム説明",   "アイテムうんちく",   baseInjectedId, new Regex("EquipParamGoods")),
            };
        }

        public void AddCrossGameWarpsToBonfireMenu()
        {
            foreach (var esdFileName in Directory.GetFiles(_talkScriptFolder).Where(file => !file.Contains("bak_original")))
            {
                // Revert changes if there were any to this ezstate script
                if (File.Exists($"{esdFileName}.bak_original"))
                {
                    File.Copy($"{esdFileName}.bak_original", esdFileName, overwrite: true);
                }
                var talkScriptFile = BND4.Read(esdFileName);

                var editedFile = false;
                foreach (var scriptFile in talkScriptFile.Files)
                {
                    var parsedScript = ESD.Read(scriptFile.Bytes);
                    var editedStateGroup = false;
                    foreach (var stateGroup in parsedScript.StateGroups.Values)
                    {
                        if (stateGroup.TryGetValue(2, out ESD.State? addDialogToBonfireState) && addDialogToBonfireState.EntryCommands.Any(ESDCommandUtil.IsAddWarpToMenuCommand))
                        {
                            // Edit stateGroups that already have cross game warps
                            if (stateGroup.Any(state => state.Value.EntryCommands.Any(ESDCommandUtil.IsCrossGameWarpCommand)))
                            {
                                Console.WriteLine($"{scriptFile.Name} already has cross game warps.");
                                continue;
                            }

                            var lastState = stateGroup.Keys.Max();
                            var warpToDS1CommandStateId = lastState + 1;
                            var warpToDS2CommandStateId = lastState + 2;

                            var lastMenuIndex = addDialogToBonfireState.EntryCommands
                                .Where(ESDCommandUtil.IsAddTalkListDataCommand)
                                .Max(ESDCommandUtil.GetMenuIndexIgnoreLeave);
                            var warpToDS1MenuIndex = (byte)(lastMenuIndex + 1);
                            var warpToDS2MenuIndex = (byte)(lastMenuIndex + 2);

                            var warpToDS1Command = new ESD.State();
                            warpToDS1Command.Conditions.Add(new ESD.Condition(1, [65, 161]));
                            warpToDS1Command.EntryCommands.Add(new ESD.CommandCall(1, 128));
                            stateGroup.Add(warpToDS1CommandStateId, warpToDS1Command);

                            var warpToDS2Command = new ESD.State();
                            warpToDS2Command.Conditions.Add(new ESD.Condition(1, [65, 161]));
                            warpToDS2Command.EntryCommands.Add(new ESD.CommandCall(1, 129));
                            stateGroup.Add(warpToDS2CommandStateId, warpToDS2Command);

                            var warpToDS1MenuOption = new ESD.CommandCall(5, 19, [130, 128, 0, 0, 0, 132, 161], [130, warpToDS1MenuIndex, 0, 0, 0, 161], [130, 129, 0, 0, 0, 132, 161], [130, 255, 255, 255, 255, 161]);
                            var warpToDS2MenuOption = new ESD.CommandCall(5, 19, [130, 128, 0, 0, 0, 132, 161], [130, warpToDS2MenuIndex, 0, 0, 0, 161], [130, 130, 0, 0, 0, 132, 161], [130, 255, 255, 255, 255, 161]);
                            var warpToDS1Condition = new ESD.Condition(warpToDS1CommandStateId, [87, 132, 130, warpToDS1MenuIndex, 0, 0, 0, 149, 161]);
                            var warpToDS2Condition = new ESD.Condition(warpToDS2CommandStateId, [87, 132, 130, warpToDS2MenuIndex, 0, 0, 0, 149, 161]);
                            addDialogToBonfireState.EntryCommands.Add(warpToDS1MenuOption);
                            addDialogToBonfireState.EntryCommands.Add(warpToDS2MenuOption);
                            // The Firelink Shrine bonfire needs these added to state group 5, not 4.
                            var conditionsStateGroup = scriptFile.Name.Contains("t400000") ? 5 : 4;
                            stateGroup[conditionsStateGroup].Conditions.Add(warpToDS1Condition);
                            stateGroup[conditionsStateGroup].Conditions.Add(warpToDS2Condition);
                            editedStateGroup = true;
                        }
                    }

                    if (editedStateGroup)
                    {
                        scriptFile.Bytes = parsedScript.Write();
                        editedFile = true;
                    }
                }

                if (editedFile)
                {
                    // Back up original ezstate file before editing
                    if (!File.Exists($"{esdFileName}.bak_original"))
                    {
                        File.Copy(esdFileName, $"{esdFileName}.bak_original");
                    }
                    talkScriptFile.Write(esdFileName);
                }
            }
        }
    }
}
