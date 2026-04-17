// ---------------------------------------------------------------
// DS2GameData — wires up DS2's BND4 regulation file and flat English
// FMG files. All four param types share the same three FMG files
// since DS2 does not split text by item category.
//
// DS2 uses bare param file names (WeaponParam, ArmorParam, etc.)
// rather than the EquipParam* prefix used by DSR and DS3, so each
// ItemType supplies an explicit ParamFilePattern.
// ---------------------------------------------------------------
using DarkSoulsItemMigrator;
using SoulsFormats;
using System.Text.RegularExpressions;

class DS2GameData : GameData
{
    private readonly DS2RegulationFile _paramBnd;
    private readonly FmgDirectoryFile _msgBnd;

    protected override SourceGame GameName => SourceGame.DS2S;
    protected override IBinder ParamBnd => _paramBnd.Bnd;
    protected override IBinder ItemMsgBnd => _msgBnd.Bnd;
    protected override IBinder MenuMsgBnd => _msgBnd.Bnd;

    public DS2GameData(FmgDirectoryFile msgBnd, DS2RegulationFile paramBnd, TpfFile icons, int baseInjectedId)
    {
        _msgBnd = msgBnd;
        _paramBnd = paramBnd;

        Icons = icons;
        IconSheetSourcePath = @"C:\Program Files (x86)\Steam\steamapps\common\DARK SOULS III\Game\mod\icons\NewDS2Icons\ic_0064410000.dds";
        GoodsParamRegex = new Regex("^ItemParam");
        GoodsParamdefPath = $@"{Program.SmithboxRoot}\DS2S\Defs\ITEM_PARAM.xml";
        TargetNameFmgName = "itemname.fmg";
        TargetDescFmgName = "simpleexplanation.fmg";
        TargetLongDescFmgName = "detailedexplanation.fmg";
        BonfireTextFmgName = "bofire.fmg";
        DSRIconId = 64410000;
        DS3IconId = 64420000;
        IconIdCellName = "icon_id";
        BaseInjectedId = baseInjectedId;

        var goodsParam = PARAM.Read(paramBnd.Bnd.Files.Single(f => f.Name.StartsWith("ItemParam")).Bytes);
        goodsParam.ApplyParamdef(PARAMDEF.XmlDeserialize(GoodsParamdefPath));
        DefaultParamRow = goodsParam.Rows.Single(row => row.ID == 62120000); // Sunlight Medal

        // ItemParam contains all weapons, armor, etc.
        ItemTypes = new List<ItemType>
            {
                new("Goods",      $@"{Program.SmithboxRoot}\DS2S\Defs\ITEM_PARAM.xml",    "itemname.fmg", "simpleexplanation.fmg", "detailedexplanation.fmg", baseInjectedId, new Regex("^ItemParam")),
            };
    }

    protected override void ReplaceIconSheet()
    {
        var oldTexture = Icons.Tpf.Textures[0];

        var customDSRIcon = File.ReadAllBytes(@"Icons\ic_0064410000.dds");
        var newDSRTexture = new TPF
        {
            Flag2 = Icons.Tpf.Flag2,
            Encoding = Icons.Tpf.Encoding,
            Platform = Icons.Tpf.Platform
        };
        newDSRTexture.Textures.Add(new TPF.Texture("IT_IC_0064410000", oldTexture.Format, oldTexture.Flags1, customDSRIcon, oldTexture.Platform));
        newDSRTexture.Write(@"C:\Program Files (x86)\Steam\steamapps\common\Dark Souls II Scholar of the First Sin\Game\menu\tex\icon\ic_0064410000.tpf");

        var customDS3Icon = File.ReadAllBytes(@"Icons\ic_0064420000.dds");
        var newDS3Texture = new TPF
        {
            Flag2 = Icons.Tpf.Flag2,
            Encoding = Icons.Tpf.Encoding,
            Platform = Icons.Tpf.Platform
        };
        newDS3Texture.Textures.Add(new TPF.Texture("IT_IC_0064420000", oldTexture.Format, oldTexture.Flags1, customDS3Icon, oldTexture.Platform));
        newDS3Texture.Write(@"C:\Program Files (x86)\Steam\steamapps\common\Dark Souls II Scholar of the First Sin\Game\menu\tex\icon\ic_0064420000.tpf");
    }
}