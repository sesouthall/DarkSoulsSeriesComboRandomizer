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

        const string dsrItemMsgPath = $@"{dsrRoot}\msg\ENGLISH\item.msgbnd.dcx";
		const string dsrMenuMsgPath = $@"{dsrRoot}\msg\ENGLISH\menu.msgbnd.dcx";
        const string ds2MsgPath = $@"{ds2Root}\menu\text\english";  // loose FMG files, not a BND
        const string ds3ItemMsgPath = $@"{ds3Root}\msg\engus\item_dlc2.msgbnd.dcx";
		const string ds3MenuMsgPath = $@"{ds3Root}\msg\engus\menu_dlc2.msgbnd.dcx";

        const string dsrParambndPath = $@"{dsrRoot}\param\GameParam\GameParam.parambnd.dcx";
        const string ds2RegulationPath = $@"{ds2Root}\enc_regulation.bnd.dcx";
        const string ds3Data0BdtPath = $@"{ds3Root}\Data0.bdt";

        const string dsrMenu3Path = $@"{dsrRoot}\menu\menu_3.tpf.dcx";
        const string ds3MenuCommonPath = $@"{ds3Root}\menu\01_common.tpf.dcx";
        const string ds2MenuPath = $@"{ds2Root}\menu\tex\icon\ic_0064330000.tpf";

		const string dsrTalkScriptPath = $@"{dsrRoot}\script\talk";
		const string ds3TalkScriptPath = $@"{ds3Root}\script\talk";

        // Exposed as internal so GameData subclasses can reference it
        internal const string SmithboxRoot = @"C:\Users\sesou\Downloads\Smithbox\Assets\PARAM";

        const string ReportOutputFolder = @".";

		private static Dictionary<string, int> DSRBonfireIds = new Dictionary<string, int>{
			{"Depths", 1002960},
			{"Sunlight Altar", 1012961},
			{"Undead Burg", 1012962},
			{"Undead Parish", 1012964},
			{"Firelink Shrine (DS1)", 1022960},
			{"Painted World of Ariamis", 1102960},
			{"Darkroot Garden", 1202961},
			{"Oolacile Sanctuary", 1212961},
			{"Oolacile Township", 1212962},
			{"Sanctuary Garden", 1212963},
			{"Oolacile Township Dungeon", 1212964},
			{"Chasm of the Abyss", 1212950},
			{"Upper Catacombs", 1302960},
			{"Inner Catacombs", 1302961},
			{"Vamos", 1302962},
			{"Lower Tomb of Giants", 1312960},
			{"Upper Tomb of Giants", 1312961},
			{"Great Hollow", 1320980},
			{"Stone Dragon", 1322960},
			{"Ash Lake", 1322961},
			{"Daughter of Chaos", 1402960},
			{"Lower Blighttown", 1402961},
			{"Upper Blighttown", 1402962},
			{"Lost Izalith", 1412960},
			{"Upper Demon Ruins", 1412961},
			{"Lower Demon Ruins", 1412962},
			{"Demon Ruins Catacombs", 1412963},
			{"Lost Izalith Lava Pits", 1412964},
			{"Sen's Fortress", 1502961},
			{"Chamber of the Princess", 1512950},
			{"Anor Londo (DS1)", 1512960},
			{"Inner Anor Londo", 1512961},
			{"Darkmoon Tomb", 1512962},
			{"Darkroot Basin", 1602961},
			{"Duke's Archives Balcony", 1702960},
			{"Prison Tower (DS1)", 1702961},
			{"Duke's Archives Entrance", 1702962},
			{"Firelink Altar", 1802960},
			{"Undead Asylum Courtyard", 1812960},
			{"Undead Asylum Sewer", 1812961}
		};

        private static Dictionary<string, int> DS2BonfireIds = new Dictionary<string, int>
		{
			{"Fire Keepers' Dwelling", 2650},
			{"The Far Fire", 4650},
			{"Cardinal Tower", 10655},
			{"Soldiers' Rest", 10660},
			{"The Crestfallen's Retreat", 10670},
			{"The Place Unbeknownst", 10675},
			{"Tower of Prayer (Amana)", 11650},
			{"Crumbled Ruins", 11655},
			{"Rhoy's Resting Place", 11660},
			{"Rise of the Dead", 11670},
			{"Lower Brightstone Cove", 14650},
			{"Royal Army Campsite", 14655},
			{"Chapel Threshold", 14660},
			{"Foregarden", 15650},
			{"Ritual Site", 15655},
			{"Straid's Cell", 16650},
			{"Exile Holding Cells", 16655},
			{"The Tower Apart", 16660},
			{"Upper Ramparts", 16665},
			{"McDuff's Workshop", 16670},
			{"Servants' Quarters", 16675},
			{"The Saltfort", 16685},
			{"The Mines", 17650},
			{"Lower Earthen Peak", 17655},
			{"Poison Pool", 17665},
			{"Central Earthen Peak", 17670},
			{"Upper Earthen Peak", 17675},
			{"Unseen Path to Heide", 18650},
			{"Ironhearth Hall", 19650},
			{"Threshold Bridge", 19655},
			{"Eygil's Idol", 19660},
			{"Belfry Sol Approach", 19665},
			{"King's Gate", 21650},
			{"Central Castle Drangleic", 21655},
			{"Forgotten Chamber", 21660},
			{"Under Castle Drangleic", 21665},
			{"Undead Refuge", 23650},
			{"Bridge Approach", 23655},
			{"Undead Lockaway", 23660},
			{"Undead Purgatory", 23665},
			{"Undead Ditch", 24650},
			{"Undead Crypt Entrance", 24655},
			{"Black Gulch Mouth", 25650},
			{"Central Gutter", 25655},
			{"Hidden Chamber", 25660},
			{"Upper Gutter", 25665},
			{"Dragon Aerie", 27650},
			{"Shrine Entrance", 27655},
			{"Old Akelarre", 29650},
			{"Tower of Flame", 31650},
			{"Heide's Ruin", 31655},
			{"The Blue Cathedral", 31660},
			{"Ruined Fork Road", 32655},
			{"Shaded Ruins", 32660},
			{"Gyrm's Respite", 33655},
			{"Ordeal's End", 33660},
			{"Grave Entrance", 34650},
			{"Harval's Resting Place", 34655},
			{"Sanctum Walk", 35650},
			{"Priestess' Chamber", 35655},
			{"Sanctum Nadir", 35665},
			{"Hidden Sanctum Chamber", 35670},
			{"Lair of the Imperfect", 35675},
			{"Sanctum Interior", 35680},
			{"Tower of Prayer (Shulva)", 35685},
			{"Throne Floor", 36650},
			{"Foyer", 36655},
			{"Upper Floor", 36660},
			{"Iron Hallway Entrance", 36665},
			{"Lowermost Floor", 36670},
			{"The Smelter Throne", 36675},
			{"Outer Wall", 37650},
			{"Abandoned Dwelling", 37660},
			{"Lower Garrison", 37665},
			{"Grand Cathedral", 37670},
			{"Expulsion Chamber", 37675},
			{"Inner Wall", 37685}
		};

        private static Dictionary<string, int> DS3BonfireIds = new Dictionary<string, int>
        {
			{"Ashen Grave", 4002959},
			{"Firelink Shrine (DS3)", 4002950},
			{"Cemetery of Ash", 4002951},
			{"Iudex Gundyr", 4002952},
			{"Untended Graves", 4002953},
			{"Champion Gundyr", 4002954},
			{"High Wall of Lothric", 3002950},
			{"Tower on the Wall", 3002955},
			{"Vordt of the Boreal Valley", 3002952},
			{"Dancer of the Boreal Valley", 3002954},
			{"Oceiros, the Consumed King", 3002951},
			{"High Wall of Lothric, Teleport", 3002960},
			{"Foot of the High Wall", 3102954},
			{"Undead Settlement", 3102950},
			{"Cliff Underside", 3102952},
			{"Dilipidated Bridge", 3102953},
			{"Pit of Hollows", 3102951},
			{"Road of Sacrifices", 3302956},
			{"Halfway Fortress", 3302950},
			{"Crucifixion Woods", 3302957},
			{"Crystal Sage", 3302952},
			{"Farron Keep", 3302953},
			{"Keep Ruins", 3302954},
			{"Farron Keep Perimeter", 3302958},
			{"Old Wolf of Farron", 3302955},
			{"Abyss Watchers", 3302951},
			{"Cathedral of the Deep", 3502953},
			{"Cleansing Chapel", 3502950},
			{"Deacons of the Deep", 3502951},
			{"Rosaria's Bed Chamber", 3502952},
			{"Catacombs of Carthus", 3802956},
			{"High Lord Wolnir", 3802950},
			{"Abandoned Tomb", 3802951},
			{"Old King's Antechamber", 3802952},
			{"Demon Ruins", 3802953},
			{"Old Demon King", 3802954},
			{"Irithyll of the Boreal valley", 3702957},
			{"Central Irithyll", 3702954},
			{"Church of Yorshka", 3702950},
			{"Distant Manor", 3702955},
			{"Pontiff Sulyvahn", 3702951},
			{"Water Reserve", 3702956},
			{"Anor Londo (DS3)", 3702953},
			{"Prison Tower (DS3)", 3702958},
			{"Aldrich, Devourer of Gods", 3702952},
			{"Irithyll Dungeon", 3902950},
			{"Profaned Capital", 3902952},
			{"Yhorm The Giant", 3902951},
			{"Lothric Castle", 3012950},
			{"Dragon Barracks", 3012952},
			{"Dragonslayer Armour", 3012951},
			{"Grand Archives", 3412951},
			{"Twin Princes", 3412950},
			{"Archdragon Peak", 3202950},
			{"Dragon-Kin Mausoleum", 3202953},
			{"Great Belfry", 3202952},
			{"Nameless King", 3202951},
			{"Flameless Shrine", 4102950},
			{"Kiln of the First Flame", 4102951},
			{"The First Flame", 4102952},
			{"Snowfield", 4502951},
			{"Rope Bridge Cave", 4502952},
			{"Corvian Settlement", 4502953},
			{"Snowy Mountain Pass", 4502954},
			{"Ariandel Chapel", 4502955},
			{"Sister Friede", 4502950},
			{"Depths of the Painting", 4502957},
			{"Champion's Gravetender", 4502956},
			{"The Dreg Heap", 5002951},
			{"Earthen Peak Ruins", 5002952},
			{"Within the Earthen Peak Ruins", 5002953},
			{"The Demon Prince", 5002950},
			{"The Ringed City", 5102110},
			{"Mausoleum Lookout", 5102952},
			{"Ringed Inner Wall", 5102953},
			{"Ringed City Streets", 5102954},
			{"Shared Grave", 5102955},
			{"Church of Filianore", 5102950},
			{"Filianore's rest", 5112951},
			{"Slave Knight Gael", 5112950},
			{"Darkeater Midir", 5102951}
		};

        static void Main(string[] args)
        {
            Console.WriteLine("=== Dark Souls Item Migrator ===\n");

            var dsrItemMsg = new Bnd3File(dsrItemMsgPath);
			var dsrMenuMsg = new Bnd3File(dsrMenuMsgPath);
            var dsrParambnd = new Bnd3File(dsrParambndPath);
            var dsrIcons = new TpfFile(dsrMenu3Path);

            var ds2Msg = new FmgDirectoryFile(ds2MsgPath);
            var ds2Reg = new DS2RegulationFile(ds2RegulationPath);
            var ds2Icons = new TpfFile(ds2MenuPath);

            var ds3ItemMsg = new Bnd4File(ds3ItemMsgPath);
			var ds3MenuMsg = new Bnd4File(ds3MenuMsgPath);
            var ds3Reg = new DS3RegulationFile(ds3Data0BdtPath);
            var ds3Icons = new TpfFile(ds3MenuCommonPath);

            List<GameFile> files = [dsrItemMsg, dsrMenuMsg, dsrParambnd, dsrIcons, ds2Reg, ds2Icons, ds3ItemMsg, ds3MenuMsg, ds3Reg, ds3Icons];
            List<FmgDirectoryFile> fmgDirs = [ds2Msg];

            Console.WriteLine("Reverting existing changes...");
            RevertAll(files, fmgDirs);

            var dsrData = new DSRGameData(dsrItemMsg, dsrMenuMsg, dsrParambnd, dsrIcons, dsrTalkScriptPath, DSRBaseGoodsParamId);
            var ds2Data = new DS2GameData(ds2Msg, ds2Reg, ds2Icons, DS2BaseGoodsParamId);
            var ds3Data = new DS3GameData(ds3ItemMsg, ds3MenuMsg, ds3Reg, ds3Icons, ds3TalkScriptPath, DS3BaseGoodsParamId);

            MigrateItems(source: dsrData, target: ds3Data, label: "[1/6] Migrating DSR items into DS3...");
            MigrateItems(source: dsrData, target: ds2Data, label: "[2/6] Migrating DSR items into DS2...");
            MigrateItems(source: ds2Data, target: dsrData, label: "[3/6] Migrating DS2 items into DSR...");
            MigrateItems(source: ds2Data, target: ds3Data, label: "[4/6] Migrating DS2 items into DS3...");
            MigrateItems(source: ds3Data, target: dsrData, label: "[5/6] Migrating DS3 items into DSR...");
            MigrateItems(source: ds3Data, target: ds2Data, label: "[6/6] Migrating DS3 items into DS2...");

			Console.WriteLine("Adding bonfire warp messages...");
			dsrData.InjectCrossGameBonfireWarpMessages(DS2BonfireIds.Concat(DS3BonfireIds).ToDictionary(kvp => kvp.Key, kvp => kvp.Value));
            ds2Data.InjectCrossGameBonfireWarpMessages(DSRBonfireIds.Concat(DS3BonfireIds).ToDictionary(kvp => kvp.Key, kvp => kvp.Value));
            ds3Data.InjectCrossGameBonfireWarpMessages(DSRBonfireIds.Concat(DS2BonfireIds).ToDictionary(kvp => kvp.Key, kvp => kvp.Value));

			Console.WriteLine("Updating DS1 and DS3 bonfire menus...");
			dsrData.AddCrossGameWarpsToBonfireMenu();
			ds3Data.AddCrossGameWarpsToBonfireMenu();

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
