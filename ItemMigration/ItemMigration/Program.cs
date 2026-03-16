using SoulsFormats;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SoulsFormats.Cryptography;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace DarkSoulsItemMigrator
{
    class Program
    {
        const int DS3BaseGoodsParamId = 4000000;
        const int DS3BaseKnowledgeId = 7000;
        const int DS3BaseIconId = 288;

        const string dsrMsgPath = @"C:\Program Files (x86)\Steam\steamapps\common\DARK SOULS REMASTERED\msg\ENGLISH\item.msgbnd.dcx";
        const string ds3MsgPath = @"C:\Program Files (x86)\Steam\steamapps\common\DARK SOULS III\Game\msg\engus\item_dlc2.msgbnd.dcx";

        const string dsrParambndPath = @"C:\Program Files (x86)\Steam\steamapps\common\DARK SOULS REMASTERED\param\GameParam\GameParam.parambnd.dcx";
        const string ds3Data0BdtPath = @"C:\Program Files (x86)\Steam\steamapps\common\DARK SOULS III\Game\Data0.bdt";

        const string dsrMenu0Path = @"C:\Program Files (x86)\Steam\steamapps\common\DARK SOULS REMASTERED\menu\menu_0.tpf.dcx";
        const string dsrMenu3Path = @"C:\Program Files (x86)\Steam\steamapps\common\DARK SOULS REMASTERED\menu\menu_3.tpf.dcx";

        const string ds3MenuCommonPath = @"C:\Program Files (x86)\Steam\steamapps\common\DARK SOULS III\Game\menu\01_common.tpf.dcx";

        const string ds3KnowledgePath = @"C:\Program Files (x86)\Steam\steamapps\common\DARK SOULS III\Game\menu\knowledge";
        const string CustomDS1IconSheetsFolder = @"C:\Program Files (x86)\Steam\steamapps\common\DARK SOULS III\Game\mod\icons\EditedDSRIcons\";
        const string iconSheetWithCustomIcons = @"C:\Program Files (x86)\Steam\steamapps\common\DARK SOULS III\Game\mod\icons\MENU_Icon_00000\MENU_Icon_00000.dds";

        static void Main(string[] args)
        {
            Console.WriteLine("=== Dark Souls Item Migrator ===\n");

            //var iconSheetMapping = MigrateTextureSheets();
            MigrateItemNames();

            Console.WriteLine("\nDone! Remember to back up your files before testing.");
        }

        // ---------------------------------------------------------------
        // ITEM NAMES
        // DSR stores names in FMG (fmessage) files inside a msgbnd archive.
        // DS3 uses the same format, so we can directly map row IDs.
        // ---------------------------------------------------------------
        static void MigrateItemNames()
        {
            Console.WriteLine("[1/2] Migrating item names...");

            // Load DSR text and params
            var dsrMsgBnd = BND3.Read(dsrMsgPath);
            var dsrParamBnd = BND3.Read(dsrParambndPath);

            // Load DS3 text and params
            var ds3MsgBnd = BND4.Read(ds3MsgPath);
            var ds3Bnd = RegulationDecryptor.DecryptDS3Regulation(ds3Data0BdtPath);

            var ds3Param = PARAM.Read(ds3Bnd.Files.Single(file => file.Name.Contains("EquipParamGoods")).Bytes);
            ds3Param.ApplyParamdef(PARAMDEF.XmlDeserialize(@"C:\Users\sesou\Downloads\Smithbox-2-0-5-10-06-2025\Assets\PARAM\DS3\Defs\EquipParamGoods.xml"));
            var defaultDs3Item = ds3Param.Rows.Single(row => row.ID == 117); // Sunlight Medal

            var ds3ItemNamesFmg = FMG.Read(ds3MsgBnd.Files.Single(file => file.Name.Contains("\\アイテム名.fmg")).Bytes);
            var ds3ItemDescriptionsFmg = FMG.Read(ds3MsgBnd.Files.Single(file => file.Name.Contains("\\アイテム説明.fmg")).Bytes);
            var ds3ItemLongDescriptionsFmg = FMG.Read(ds3MsgBnd.Files.Single(file => file.Name.Contains("\\アイテムうんちく.fmg")).Bytes);

            // Load DS3 icon sheets
            var ds3SmallIcons = TPF.Read(ds3MenuCommonPath);

            // FMG file names we care about
            var itemTypes = new[]
            {
                "Weapon",
                "Armor",
                "Accessory",
                "Item",
            };

            var currentItemId = DS3BaseGoodsParamId;
            var copiedIconIds = new Dictionary<int, int>();
            foreach (var itemType in itemTypes)
            {
                var paramName = itemType == "Armor" ? "Protector" : itemType == "Item" ? "Goods" : itemType;
                var ds1rParam = PARAM.Read(dsrParamBnd.Files.Single(file => file.Name.Contains($"EquipParam{paramName}")).Bytes);
                ds1rParam.ApplyParamdef(PARAMDEF.XmlDeserialize($@"C:\Users\sesou\Downloads\Smithbox-2-0-5-10-06-2025\Assets\PARAM\DS1R\Defs\EquipParam{paramName}.xml"));
                var nameFmg = FMG.Read(dsrMsgBnd.Files.First(file => file.Name.Contains($"\\{itemType}_name_.fmg")).Bytes);
                var descriptionFmg = FMG.Read(dsrMsgBnd.Files.First(file => file.Name.Contains($"\\{itemType}_description_.fmg")).Bytes);
                var longDescriptionFmg = FMG.Read(dsrMsgBnd.Files.First(file => file.Name.Contains($"\\{itemType}_long_desc_.fmg")).Bytes);

                foreach (var itemParam in ds1rParam.Rows)
                {
                    ds3ItemNamesFmg.Entries.Add(new FMG.Entry(currentItemId, nameFmg[itemParam.ID]));
                    ds3ItemDescriptionsFmg.Entries.Add(new FMG.Entry(currentItemId, descriptionFmg[itemParam.ID]));
                    ds3ItemLongDescriptionsFmg.Entries.Add(new FMG.Entry(currentItemId, longDescriptionFmg[itemParam.ID]));

                    var newItem = new PARAM.Row(defaultDs3Item);
                    newItem.ID = currentItemId;
                    //var iconIdFieldName = itemType == "Armor" ? "iconIdM" : "iconId";
                    //var ds1IconId = (UInt16)itemParam[iconIdFieldName].Value;
                    //if (itemType == "Item" && ds1IconId >= 7000)
                    //{
                    //    ds1IconId -= 3904; // Offset from gestures' virtual texture sheet, to the actual location.
                    //}
                    newItem["iconId"].Value = 20; //CalculateNewIconId(ds1IconId, iconSheetMapping);

                    ds3Param.Rows.Add(newItem);
                    currentItemId++;
                }
            }

            // Replace Menu_Icon_00000 with my version containing DS1 and DS2 icons

            var customDS1Icons = File.ReadAllBytes(iconSheetWithCustomIcons);
            var oldIconSheet = ds3SmallIcons.Textures.Single(texture => texture.Name.Contains("MENU_Icon_00000"));
            var oldIconIndex = ds3SmallIcons.Textures.IndexOf(oldIconSheet);
            ds3SmallIcons.Textures.Remove(oldIconSheet);
            ds3SmallIcons.Textures.Insert(oldIconIndex, new TPF.Texture($"MENU_Icon_00000", oldIconSheet.Format, oldIconSheet.Flags1, customDS1Icons, oldIconSheet.Platform));

            // Save DS3 msgbnd

            ds3MsgBnd.Files.Single(file => file.Name.Contains("\\アイテム名.fmg")).Bytes = ds3ItemNamesFmg.Write();
            ds3MsgBnd.Files.Single(file => file.Name.Contains("\\アイテム説明.fmg")).Bytes = ds3ItemDescriptionsFmg.Write();
            ds3MsgBnd.Files.Single(file => file.Name.Contains("\\アイテムうんちく.fmg")).Bytes = ds3ItemLongDescriptionsFmg.Write();

            string msgBackupPath = ds3MsgPath + ".bak_original";
            if (!File.Exists(msgBackupPath)) File.Copy(ds3MsgPath, msgBackupPath); // backup
            ds3MsgBnd.Write(ds3MsgPath);

            // Save DS3 EquipParam

            ds3Bnd.Files.Single(file => file.Name.Contains("EquipParamGoods")).Bytes = ds3Param.Write();

            string bndBackupPath = ds3Data0BdtPath + ".bak_original";
            if (!File.Exists(bndBackupPath)) File.Copy(ds3Data0BdtPath, bndBackupPath); // backup
            RegulationDecryptor.EncryptDS3Regulation(ds3Data0BdtPath, ds3Bnd);

            // Save DS3 icons

            string menuCommonBackup = ds3MenuCommonPath + ".bak_original";
            if (!File.Exists(menuCommonBackup)) File.Copy(ds3MenuCommonPath, menuCommonBackup); // backup
            ds3SmallIcons.Write(ds3MenuCommonPath);

            Console.WriteLine("  Saved item names.");
        }

        //private static int CalculateNewIconId(UInt16 oldIconId, Dictionary<int, int> iconSheetMapping)
        //{
        //    var oldIconSheetGroup = oldIconId / 1000;
        //    var oldIconIndexInGroup = oldIconId % 1000;
        //    var iconIndexOnSheet = oldIconIndexInGroup % 131;
        //    if (oldIconId == 204) //Gough's Great Arrows are weird
        //    {
        //        iconIndexOnSheet = 120;
        //    }
        //    var oldIconSheetId = (oldIconSheetGroup * 10) + (oldIconIndexInGroup / 131);
        //    var newIconSheetId = iconSheetMapping[oldIconSheetId];
        //    var newIconSheetGroup = newIconSheetId / 1000;
        //    var newIconSheetNumberInGroup = newIconSheetId % 1000;
        //    return newIconSheetGroup * 1000 + newIconSheetNumberInGroup * 144 + iconIndexOnSheet;
        //}

        //private static Dictionary<int, int> MigrateTextureSheets()
        //{
        //    var iconSheetMapping = new Dictionary<int, int>();
        //    var ds3SmallIcons = TPF.Read(ds3MenuCommonPath);
        //    var ds1IconSheetRegex = new Regex("Icon(?<sheetNumber>\\d\\d)");
        //    foreach (var ds1IconSheet in Directory.GetFiles(CustomDS1IconSheetsFolder))
        //    {
        //        var customDS1Icons = File.ReadAllBytes(ds1IconSheet);
        //        var match = ds1IconSheetRegex.Match(ds1IconSheet);
        //        var oldSheetNumber = int.Parse(match.Groups["sheetNumber"].Value);
        //        var newSheetNumber = GetNextEmptySheetNumber(ds3SmallIcons);
        //        iconSheetMapping[oldSheetNumber] = newSheetNumber;
        //        ds3SmallIcons.Textures.Add(new TPF.Texture($"MENU_Icon_{newSheetNumber:D5}", ds3SmallIcons.Textures[0].Format, ds3SmallIcons.Textures[0].Flags1, customDS1Icons, ds3SmallIcons.Textures[0].Platform));
        //    }
        //    string menuCommonBackup = ds3MenuCommonPath + ".bak_original";
        //    if (!File.Exists(menuCommonBackup)) File.Copy(ds3MenuCommonPath, menuCommonBackup); // backup
        //    ds3SmallIcons.Write(ds3MenuCommonPath);
        //    return iconSheetMapping;
        //}

        //private static int GetNextEmptySheetNumber(TPF textures)
        //{
        //    int nextSheetNumber = 0;
        //    while (textures.Textures.Any(texture => texture.Name.Contains($"MENU_Icon_{nextSheetNumber:D5}")))
        //    {
        //        nextSheetNumber++;
        //        if (nextSheetNumber % 1000 > 6)
        //        {
        //            nextSheetNumber += 993;
        //        }
        //        if (nextSheetNumber >= 10000)
        //        {
        //            throw new Exception("No available sheet number");
        //        }
        //    }
        //    return nextSheetNumber;
        //}
    }

    //if (!copiedIconIds.ContainsKey(iconId))
    //{
    //    var textureSheet = dsrMenu0.Textures.Find(texture => texture.Name == $"Icon{textureSheetId:D2}") ?? dsrMenu3.Textures.Find(texture => texture.Name == $"Icon{textureSheetId:D2}") ?? throw new Exception();
    //    var iconRow = iconIndex / 12;
    //    var iconCol = iconIndex % 12;
    //    if (nameEntry == "Gough's Great Arrow")
    //    {
    //        iconRow = 11;
    //        iconCol = 0;
    //    }
    //    using (var parsedTextureSheet = Pfim.Dds.Create(textureSheet.Bytes, new Pfim.PfimConfig()))
    //    {
    //        using var icon = Image.LoadPixelData<Bgra32>(parsedTextureSheet.Data, parsedTextureSheet.Width, parsedTextureSheet.Height);
    //        icon.Configuration.PreferContiguousImageBuffers = true;
    //        icon.Mutate(x => x.Crop(new Rectangle(164 * iconCol, 184 * iconRow, 164, 184)).Resize(512, 512, KnownResamplers.Spline));
    //        using var bmpMemoryStream = new MemoryStream();
    //        icon.SaveAsBmp(bmpMemoryStream);
    //        bmpMemoryStream.Position = 0;
    //        var iconData = SharpDX.Toolkit.Graphics.Image.Load(bmpMemoryStream);
    //        using var ddsMemoryStream = new MemoryStream();
    //        iconData.Save(ddsMemoryStream, SharpDX.Toolkit.Graphics.ImageFileType.Dds);
    //        var newIcon = new TPF();
    //        newIcon.Textures.Add(new TPF.Texture($"MENU_Knowledge_{currentKnowledgeId:D5}", defaultDs3Icon.Textures[0].Format, defaultDs3Icon.Textures[0].Flags1, ddsMemoryStream.GetBuffer(), TPF.TPFPlatform.PC));
    //        newIcon.Write($"{ds3KnowledgePath}\\menu_knowledge_{currentKnowledgeId:D5}.tpf.dcx");
    //    }
    //    copiedIconIds[iconId] = currentKnowledgeId;
    //    currentKnowledgeId++;
    //}
}