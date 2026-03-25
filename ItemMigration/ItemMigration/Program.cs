namespace DarkSoulsItemMigrator
{

    class Program
    {
        const int DSRBaseGoodsParamId = 10000;
        const int DS2BaseGoodsParamId = 66000000;
        const int DS3BaseGoodsParamId = 4000000;

        const string dsrRoot = @"C:\Program Files (x86)\Steam\steamapps\common\DARK SOULS REMASTERED";
        const string ds2Root = @"C:\Program Files (x86)\Steam\steamapps\common\Dark Souls II Scholar of the First Sin\Game";
        const string ds3Root = @"C:\Program Files (x86)\Steam\steamapps\common\DARK SOULS III\Game";

        const string dsrMsgPath = $@"{dsrRoot}\msg\ENGLISH\item.msgbnd.dcx";
        const string ds2MsgPath = $@"{ds2Root}\menu\text\english";  // loose FMG files, not a BND
        const string ds3MsgPath = $@"{ds3Root}\msg\engus\item_dlc2.msgbnd.dcx";

        const string dsrParambndPath = $@"{dsrRoot}\param\GameParam\GameParam.parambnd.dcx";
        const string ds2RegulationPath = $@"{ds2Root}\enc_regulation.bnd.dcx";
        const string ds3Data0BdtPath = $@"{ds3Root}\Data0.bdt";

        const string dsrMenu3Path = $@"{dsrRoot}\menu\menu_3.tpf.dcx";
        const string ds3MenuCommonPath = $@"{ds3Root}\menu\01_common.tpf.dcx";
        const string ds2MenuPath = $@"{ds2Root}\menu\tex\icon\ic_0064330000.tpf";

        // Exposed as internal so DSRGameData and DS3GameData can reference it
        internal const string SmithboxRoot = @"C:\Users\sesou\Downloads\Smithbox\Assets\PARAM";

        const string ReportOutputFolder = @".";

        static void Main(string[] args)
        {
            Console.WriteLine("=== Dark Souls Item Migrator ===\n");

            var dsrMsg = new Bnd3File(dsrMsgPath);
            var dsrParambnd = new Bnd3File(dsrParambndPath);
            var dsrIcons = new TpfFile(dsrMenu3Path);

            var ds2Msg = new FmgDirectoryFile(ds2MsgPath);
            var ds2Reg = new DS2RegulationFile(ds2RegulationPath);
            var ds2Icons = new TpfFile(ds2MenuPath);

            var ds3Msg = new Bnd4File(ds3MsgPath);
            var ds3Reg = new DS3RegulationFile(ds3Data0BdtPath);
            var ds3Icons = new TpfFile(ds3MenuCommonPath);

            List<GameFile> files = [dsrMsg, dsrParambnd, dsrIcons, ds2Reg, ds2Icons, ds3Msg, ds3Reg, ds3Icons];
            List<FmgDirectoryFile> fmgDirs = [ds2Msg];

            Console.WriteLine("Reverting existing changes...");
            RevertAll(files, fmgDirs);

            var dsrData = new DSRGameData(dsrMsg, dsrParambnd, dsrIcons, DSRBaseGoodsParamId);
            var ds2Data = new DS2GameData(ds2Msg, ds2Reg, ds2Icons, DS2BaseGoodsParamId);
            var ds3Data = new DS3GameData(ds3Msg, ds3Reg, ds3Icons, DS3BaseGoodsParamId);

            MigrateItems(source: dsrData, target: ds3Data, label: "[1/6] Migrating DSR items into DS3...");
            MigrateItems(source: dsrData, target: ds2Data, label: "[2/6] Migrating DSR items into DS2...");
            MigrateItems(source: ds2Data, target: dsrData, label: "[3/6] Migrating DS2 items into DSR...");
            MigrateItems(source: ds2Data, target: ds3Data, label: "[4/6] Migrating DS2 items into DS3...");
            MigrateItems(source: ds3Data, target: dsrData, label: "[5/6] Migrating DS3 items into DSR...");
            MigrateItems(source: ds3Data, target: ds2Data, label: "[6/6] Migrating DS3 items into DS2...");

            Console.WriteLine("Writing reports...");
            dsrData.WriteReport(ReportOutputFolder);
            ds3Data.WriteReport(ReportOutputFolder);
            ds2Data.WriteReport(ReportOutputFolder);

            Console.WriteLine("Saving files...");
            SaveAll(files, fmgDirs);

            Console.WriteLine("\nDone! Remember to back up your files before testing.");
        }

        static void RevertAll(List<GameFile> files, List<FmgDirectoryFile> fmgDirs)
        {
            foreach (var file in files) file.Revert();
            foreach (var dir in fmgDirs) dir.Revert();
        }

        static void SaveAll(List<GameFile> files, List<FmgDirectoryFile> fmgDirs)
        {
            foreach (var file in files) file.Save();
            foreach (var dir in fmgDirs) dir.Save();
        }

        // ---------------------------------------------------------------
        // MigrateItems — iterates source.GetAllItems() and hands each one
        // to target.AddNewGood(), then flushes the target's accumulated
        // changes back into its in-memory BNDs.
        // ---------------------------------------------------------------
        static void MigrateItems(GameData source, GameData target, string label)
        {
            Console.WriteLine(label);

            var itemCount = 0;
            foreach (var item in source.GetAllItems())
            {
                target.AddNewGood(item);
                itemCount++;
            }

            target.Flush();

            Console.WriteLine($"  Migrated {itemCount} items");

            Console.WriteLine("  Done.");
        }
    }
}
