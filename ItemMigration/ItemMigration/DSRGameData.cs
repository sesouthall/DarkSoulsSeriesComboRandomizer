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
        private readonly Bnd3File _msgBnd;

        protected override SourceGame GameName => SourceGame.DSR;
        protected override IBinder ParamBnd => _paramBnd.Bnd;
        protected override IBinder MsgBnd => _msgBnd.Bnd;

        public DSRGameData(Bnd3File msgBnd, Bnd3File paramBnd, TpfFile icons, int baseInjectedId)
        {
            _msgBnd = msgBnd;
            _paramBnd = paramBnd;

            Icons = icons;
            IconTextureName = "Icon21";
            IconSheetSourcePath = @"C:\Program Files (x86)\Steam\steamapps\common\DARK SOULS III\Game\mod\icons\Icon21\Icon21.dds";
            GoodsParamRegex = new Regex("EquipParamGoods");
            GoodsParamdefPath = $@"{Program.SmithboxRoot}\DS1R\Defs\EquipParamGoods.xml";
            TargetNameFmgName = "Item_name_";
            TargetDescFmgName = "Item_description_";
            TargetLongDescFmgName = "Item_long_desc_";
            DS2SIconId = 2159;
            DS3IconId = 2160;
            IconIdCellName = "iconId";
            BaseInjectedId = baseInjectedId;

            var goodsParam = PARAM.Read(paramBnd.Bnd.Files.Single(f => f.Name.Contains("EquipParamGoods")).Bytes);
            goodsParam.ApplyParamdef(PARAMDEF.XmlDeserialize(GoodsParamdefPath));
            DefaultParamRow = goodsParam.Rows.Single(row => row.ID == 117); // Sunlight Medal

            ItemTypes = new List<ItemType>
            {
                new("Weapon",    $@"{Program.SmithboxRoot}\DS1R\Defs\EquipParamWeapon.xml",    "Weapon_name_",    "Weapon_description_",    "Weapon_long_desc_", baseInjectedId, new Regex("EquipParamWeapon")),
                new("Armor", $@"{Program.SmithboxRoot}\DS1R\Defs\EquipParamProtector.xml", "Armor_name_",     "Armor_description_",     "Armor_long_desc_", baseInjectedId, new Regex("EquipParamProtector")),
                new("Accessory", $@"{Program.SmithboxRoot}\DS1R\Defs\EquipParamAccessory.xml", "Accessory_name_", "Accessory_description_", "Accessory_long_desc_", baseInjectedId, new Regex("EquipParamAccessory")),
                new("Goods",     $@"{Program.SmithboxRoot}\DS1R\Defs\EquipParamGoods.xml",     "Item_name_",      "Item_description_",      "Item_long_desc_", baseInjectedId, new Regex("EquipParamGoods")),
            };
        }
    }
}
