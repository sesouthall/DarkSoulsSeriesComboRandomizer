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
        private readonly Bnd4File _msgBnd;

        protected override SourceGame GameName => SourceGame.DS3;
        protected override IBinder ParamBnd => _paramBnd.Bnd;
        protected override IBinder MsgBnd => _msgBnd.Bnd;

        public DS3GameData(Bnd4File msgBnd, DS3RegulationFile paramBnd, TpfFile icons, int baseInjectedId)
        {
            _msgBnd = msgBnd;
            _paramBnd = paramBnd;

            Icons = icons;
            IconTextureName = "MENU_Icon_00000";
            IconSheetSourcePath = @"C:\Program Files (x86)\Steam\steamapps\common\DARK SOULS III\Game\mod\icons\MENU_Icon_00000\MENU_Icon_00000.dds";
            GoodsParamRegex = new Regex("EquipParamGoods");
            GoodsParamdefPath = $@"{Program.SmithboxRoot}\DS3\Defs\EquipParamGoods.xml";
            TargetNameFmgName = "アイテム名";
            TargetDescFmgName = "アイテム説明";
            TargetLongDescFmgName = "アイテムうんちく";
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
    }
}
