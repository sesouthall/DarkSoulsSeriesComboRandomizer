using SoulsFormats;
using System.Text.RegularExpressions;

namespace DarkSoulsItemMigrator
{
    // ---------------------------------------------------------------
    // DSRGameData — wires up DSR's BND3 files and FMG names.
    // ---------------------------------------------------------------
    class DSRGameData : GameData
    {
        private readonly Bnd3File _paramBnd;
        private readonly Bnd3File _itemMsgBnd;
        private readonly Bnd3File _menuMsgBnd;
        private readonly string _talkScriptFolder;

        protected override SourceGame GameName => SourceGame.DSR;
        protected override IBinder ParamBnd => _paramBnd.Bnd;
        protected override IBinder ItemMsgBnd => _itemMsgBnd.Bnd;
        protected override IBinder MenuMsgBnd => _menuMsgBnd.Bnd;

        public DSRGameData(Bnd3File itemMsgBnd, Bnd3File menuMsgBnd, Bnd3File paramBnd, TpfFile icons, string talkScriptFolder, int baseInjectedId)
        {
            _itemMsgBnd = itemMsgBnd;
            _menuMsgBnd = menuMsgBnd;
            _paramBnd = paramBnd;
            _talkScriptFolder = talkScriptFolder;

            Icons = icons;
            IconTextureName = "Icon21";
            IconSheetSourcePath = @"Icons\Icon21.dds";
            GoodsParamRegex = new Regex("EquipParamGoods");
            GoodsParamdefPath = $@"{Program.SmithboxRoot}\DS1R\Defs\EquipParamGoods.xml";
            TargetNameFmgName = "Item_name_";
            TargetDescFmgName = "Item_description_";
            TargetLongDescFmgName = "Item_long_desc_";
            BonfireTextFmgName = "Event_text_";
            DS2SIconId = 2159;
            DS3IconId = 2160;
            IconIdCellName = "iconId";
            BaseInjectedId = baseInjectedId;

            var goodsParam = PARAM.Read(paramBnd.Bnd.Files.Single(f => f.Name.Contains("EquipParamGoods")).Bytes);
            goodsParam.ApplyParamdef(PARAMDEF.XmlDeserialize(GoodsParamdefPath));
            DefaultParamRow = goodsParam.Rows.Single(row => row.ID == 117); // Sunlight Medal

            ItemTypes = new List<ItemType>
            {
                new("Weapon",    $@"{Program.SmithboxRoot}\DS1R\Defs\EquipParamWeapon.xml",    "Weapon_name_",    "Weapon_description_",    "Weapon_long_desc_", int.MaxValue, new Regex("EquipParamWeapon")),
                new("Armor", $@"{Program.SmithboxRoot}\DS1R\Defs\EquipParamProtector.xml", "Armor_name_",     "Armor_description_",     "Armor_long_desc_", int.MaxValue, new Regex("EquipParamProtector")),
                new("Accessory", $@"{Program.SmithboxRoot}\DS1R\Defs\EquipParamAccessory.xml", "Accessory_name_", "Accessory_description_", "Accessory_long_desc_", int.MaxValue, new Regex("EquipParamAccessory")),
                new("Goods",     $@"{Program.SmithboxRoot}\DS1R\Defs\EquipParamGoods.xml",     "Item_name_",      "Item_description_",      "Item_long_desc_", baseInjectedId, new Regex("EquipParamGoods")),
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

                var talkScriptFile = BND3.Read(esdFileName);

                var editedFile = false;
                foreach (var scriptFile in talkScriptFile.Files)
                {
                    var parsedScript = ESD.Read(scriptFile.Bytes);
                    var editedStateGroup = false;
                    foreach (var stateGroup in parsedScript.StateGroups.Values)
                    {
                        if (stateGroup.TryGetValue(4, out ESD.State? addDialogToBonfireState) && addDialogToBonfireState.EntryCommands.Any(ESDCommandUtil.IsAddWarpToMenuCommand))
                        {
                            // Skip stateGroups that already have cross game warps
                            if (stateGroup.Any(state => state.Value.EntryCommands.Any(ESDCommandUtil.IsCrossGameWarpCommand)))
                            {
                                Console.WriteLine($"{scriptFile.Name} already has cross game warps.");
                                continue;
                            }

                            var lastState = stateGroup.Keys.Max();
                            var warpToDS2CommandStateId = lastState + 1;
                            var warpToDS3CommandStateId = lastState + 2;

                            var lastMenuIndex = addDialogToBonfireState.EntryCommands
                                .Where(ESDCommandUtil.IsAddTalkListDataCommand)
                                .Max(ESDCommandUtil.GetMenuIndexIgnoreLeave);
                            var warpToDS2MenuIndex = (byte)(lastMenuIndex + 1);
                            var warpToDS3MenuIndex = (byte)(lastMenuIndex + 2);

                            var warpToDS2Command = new ESD.State();
                            warpToDS2Command.Conditions.Add(new ESD.Condition(4, [65, 161]));
                            warpToDS2Command.EntryCommands.Add(new ESD.CommandCall(1, 129));
                            stateGroup.Add(warpToDS2CommandStateId, warpToDS2Command);

                            var warpToDS3Command = new ESD.State();
                            warpToDS3Command.Conditions.Add(new ESD.Condition(4, [65, 161]));
                            warpToDS3Command.EntryCommands.Add(new ESD.CommandCall(1, 130));
                            stateGroup.Add(warpToDS3CommandStateId, warpToDS3Command);

                            var warpToDS2MenuOption = new ESD.CommandCall(5, 19, [130, 128, 0, 0, 0, 132, 161], [130, warpToDS2MenuIndex, 0, 0, 0, 161], [130, 130, 0, 0, 0, 132, 161], [130, 255, 255, 255, 255, 161]);
                            var warpToDS3MenuOption = new ESD.CommandCall(5, 19, [130, 128, 0, 0, 0, 132, 161], [130, warpToDS3MenuIndex, 0, 0, 0, 161], [130, 131, 0, 0, 0, 132, 161], [130, 255, 255, 255, 255, 161]);
                            var warpToDS2Condition = new ESD.Condition(warpToDS2CommandStateId, [87, 132, 130, warpToDS2MenuIndex, 0, 0, 0, 149, 161]);
                            var warpToDS3Condition = new ESD.Condition(warpToDS3CommandStateId, [87, 132, 130, warpToDS3MenuIndex, 0, 0, 0, 149, 161]);
                            addDialogToBonfireState.EntryCommands.Add(warpToDS2MenuOption);
                            addDialogToBonfireState.EntryCommands.Add(warpToDS3MenuOption);
                            addDialogToBonfireState.Conditions.Add(warpToDS2Condition);
                            addDialogToBonfireState.Conditions.Add(warpToDS3Condition);
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
