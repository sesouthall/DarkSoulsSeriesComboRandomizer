namespace DarkSoulsSeriesComboRandomizer
{
    public enum MapName
    {
        // ---- Dark Souls Remastered ------------------------------------------
        DS1StartingCell,
        Depths,
        SewerChamber,
        UndeadBurgUndeadParish,
        UndeadBurgResidence,
        LowerUndeadBurg,
        LowerUndeadBurgResidence,
        FirelinkShrine,
        PaintedWorld,
        PaintedWorldAnnex,
        DarkrootGarden,
        Oolacile,
        OolacileAfterGough,
        Catacombs,
        TombOfTheGiants,
        TombOfTheGiantsPostLordvessel,
        GreatHollowAshLake,
        Blighttown,
        DemonRuinsLostIzalith,
        DemonRuinsLostIzalithPostLordvessel,
        SensFortress,
        SensCage,
        AnorLondo,
        NewLondoRuinsValleyOfDrakes,
        NewLondoRuinsPostSeal,
        ValleyOfDrakes,
        FourKings,
        DukesArchives,
        TowerCell,
        ArchivesTower,
        ArchivesTowerExtra,
        ArchivesTowerGiantCell,
        CrystalCave,
        FirelinkAltar,
        FirstLordSoul,
        SecondLordSoul,
        ThirdLordSoul,
        FourthLordSoul,
        KilnOfTheFirstFlame,
        NorthernUndeadAsylum,
        NorthernUndeadAsylumF2East,
        NorthernUndeadAsylumF2West,

        // ---- Dark Souls II: Scholar of the First Sin ------------------------
        ThingsBetwixt,
        ThingsBetwixtPostStatue,
        Majula,
        MajulaHouse,
        LenigrastsHouse,
        DragonTalonRoom,
        ForestOfFallenGiants,
        ForestOfFallenGiantsPostSoldierKey,
        ForestOfFallenGiantsIronKeyRoom,
        BrightstoneCoveTseldora,
        BrightStoneKeyRoom,
        TseldoraDen,
        AldiasKeep,
        AldiaSideRoom,
        TheLostBastilleBelfryLuna,
        BastilleCells,
        StraidsCell,
        RuinSentinelBuilding,
        HarvestValleyEarthenPeak,
        NomansWharf,
        IronKeepBelfrySol,
        HuntsmansCopseUndeadPurgatory,
        UndeadLockaway,
        TheGutterBlackGulch,
        HiddenChamber,
        HavelArmorRoom,
        DragonAerieDragonShrine,
        MajulaShadeWoods,
        HeidesTowerNomansWharf,
        FloodedPassageSideRoom,
        HeidesTowerOfFlame,
        ShadedWoodsShrineOfWinter,
        VengarlsBodyRoom,
        ChestAfterVengarl,
        LionMageSetChest,
        FangKeyLion,
        DoorsOfPharros,
        GraveOfSaints,
        MemoryOfVammarOrroAndJeigh,
        ShrineOfAmana,
        RiseOfTheDead,
        DrangleicCastleThroneOfWant,
        Nashandra,
        LookingGlassKnightArena,
        UndeadCrypt,
        DragonMemories,
        DarkChasmOfOld,
        ShulvaSanctumCity,
        PriestessChamber,
        DragonSanctum,
        BrumeTower,
        BrumeTowerWithOnlyTowerKey,
        BrumeTowerWithOnlyScorchingIronScepter,
        BrumeTowerWithBothKeys,
        FrozenEleumLoyce,
        FrozenEleumLoyceAfterAava,
        ReinderValley,
        MemoryOfTheKing,

        // ---- Dark Souls III -------------------------------------------------
        HighWallOfLothricGarden,
        OceirosGarden,
        DarkwraithCell,
        LothricCastle,
        UndeadSettlement,
        VelkaShrine,
        ArchdragonPeak,
        RoadOfSacrificesFarronKeep,
        GrandArchives,
        CathedralOfTheDeep,
        IrithyllAnorLondo,
        CatacombsCarthusSmoulderingLake,
        DungeonProfanedCapital,
        LedgeOutsideJailbreakersWindow,
        JailCells,
        OldCell,
        CemetaryFirelinkUntendedGraves,
        UntendedGraves,
        FirelinkTower,
        FirelinkRoof,
        FirstCinders,
        SecondCinders,
        ThirdCinders,
        KilnOfFlameFlamelessShrine,
        PaintedWorldOfAriandel,
        PaintedWorldSecondHalf,
        DregHeap,
        RingedCity,
        FilianloresRest,
    }

    public static class MapNameExtensions
    {
        public static string ToFriendlyName(this MapName name) => name switch
        {
            // ---- Dark Souls Remastered --------------------------------------
            MapName.DS1StartingCell => "DS1 Starting Cell",
            MapName.Depths => "Depths",
            MapName.SewerChamber => "Sewer Chamber",
            MapName.UndeadBurgUndeadParish => "Undead Burg / Undead Parish",
            MapName.UndeadBurgResidence => "Undead Burg Residence",
            MapName.LowerUndeadBurg => "Lower Undead Burg",
            MapName.LowerUndeadBurgResidence => "Lower Undead Burg Residence",
            MapName.FirelinkShrine => "Firelink Shrine",
            MapName.PaintedWorld => "Painted World",
            MapName.PaintedWorldAnnex => "Painted World Annex",
            MapName.DarkrootGarden => "Darkroot Garden",
            MapName.Oolacile => "Oolacile",
            MapName.OolacileAfterGough => "Oolacile After Gough",
            MapName.Catacombs => "Catacombs",
            MapName.TombOfTheGiants => "Tomb of the Giants",
            MapName.TombOfTheGiantsPostLordvessel => "Tomb of the Giants Post Lordvessel",
            MapName.GreatHollowAshLake => "Great Hollow / Ash Lake",
            MapName.Blighttown => "Blighttown",
            MapName.DemonRuinsLostIzalith => "Demon Ruins / Lost Izalith",
            MapName.DemonRuinsLostIzalithPostLordvessel => "Demon Ruins / Lost Izalith Post Lordvessel",
            MapName.SensFortress => "Sen's Fortress",
            MapName.SensCage => "Sen's Cage",
            MapName.AnorLondo => "Anor Londo",
            MapName.NewLondoRuinsValleyOfDrakes => "New Londo Ruins / Valley of Drakes",
            MapName.NewLondoRuinsPostSeal => "New Londo Ruins Post Seal",
            MapName.ValleyOfDrakes => "Valley of Drakes",
            MapName.FourKings => "Four Kings",
            MapName.DukesArchives => "Duke's Archives",
            MapName.TowerCell => "Tower Cell",
            MapName.ArchivesTower => "Archives Tower",
            MapName.ArchivesTowerExtra => "Archives Tower Extra",
            MapName.ArchivesTowerGiantCell => "Archives Tower Giant Cell",
            MapName.CrystalCave => "Crystal Cave",
            MapName.FirelinkAltar => "Firelink Altar",
            MapName.FirstLordSoul => "First Lord Soul",
            MapName.SecondLordSoul => "Second Lord Soul",
            MapName.ThirdLordSoul => "Third Lord Soul",
            MapName.FourthLordSoul => "Fourth Lord Soul",
            MapName.KilnOfTheFirstFlame => "Kiln of the First Flame",
            MapName.NorthernUndeadAsylum => "Northern Undead Asylum",
            MapName.NorthernUndeadAsylumF2East => "Northern Undead Asylum F2 East",
            MapName.NorthernUndeadAsylumF2West => "Northern Undead Asylum F2 West",

            // ---- Dark Souls II: Scholar of the First Sin --------------------
            MapName.ThingsBetwixt => "Things Betwixt",
            MapName.ThingsBetwixtPostStatue => "Things Betwixt Post Statue",
            MapName.Majula => "Majula",
            MapName.MajulaHouse => "Majula House",
            MapName.LenigrastsHouse => "Lenigrast's House",
            MapName.DragonTalonRoom => "Dragon Talon Room",
            MapName.ForestOfFallenGiants => "Forest of Fallen Giants",
            MapName.ForestOfFallenGiantsPostSoldierKey => "Forest of Fallen Giants Post Soldier Key",
            MapName.ForestOfFallenGiantsIronKeyRoom => "Forest of Fallen Giants Iron Key Room",
            MapName.BrightstoneCoveTseldora => "Brightstone Cove Tseldora",
            MapName.BrightStoneKeyRoom => "Brightstone Key Room",
            MapName.TseldoraDen => "Tseldora Den",
            MapName.AldiasKeep => "Aldia's Keep",
            MapName.AldiaSideRoom => "Aldia Side Room",
            MapName.TheLostBastilleBelfryLuna => "The Lost Bastille & Belfry Luna",
            MapName.BastilleCells => "Bastille Cells",
            MapName.StraidsCell => "Straid's Cell",
            MapName.RuinSentinelBuilding => "Ruin Sentinel Building",
            MapName.HarvestValleyEarthenPeak => "Harvest Valley & Earthen Peak",
            MapName.NomansWharf => "No-man's Wharf",
            MapName.IronKeepBelfrySol => "Iron Keep & Belfry Sol",
            MapName.HuntsmansCopseUndeadPurgatory => "Huntsman's Copse & Undead Purgatory",
            MapName.UndeadLockaway => "Undead Lockaway",
            MapName.TheGutterBlackGulch => "The Gutter & Black Gulch",
            MapName.HiddenChamber => "Hidden Chamber",
            MapName.HavelArmorRoom => "Havel Armor Room",
            MapName.DragonAerieDragonShrine => "Dragon Aerie & Dragon Shrine",
            MapName.MajulaShadeWoods => "Majula <-> Shaded Woods",
            MapName.HeidesTowerNomansWharf => "Heide's Tower <-> No-man's Wharf",
            MapName.FloodedPassageSideRoom => "Flooded Passage Side Room",
            MapName.HeidesTowerOfFlame => "Heide's Tower of Flame",
            MapName.ShadedWoodsShrineOfWinter => "Shaded Woods & Shrine of Winter",
            MapName.VengarlsBodyRoom => "Vengarl's Body Room",
            MapName.ChestAfterVengarl => "Chest after Vengarl",
            MapName.LionMageSetChest => "Lion Mage Set Chest",
            MapName.FangKeyLion => "Fang Key Lion",
            MapName.DoorsOfPharros => "Doors of Pharros",
            MapName.GraveOfSaints => "Grave of Saints",
            MapName.MemoryOfVammarOrroAndJeigh => "Memory of Vammar, Orro, and Jeigh",
            MapName.ShrineOfAmana => "Shrine of Amana",
            MapName.RiseOfTheDead => "Rise of the Dead",
            MapName.DrangleicCastleThroneOfWant => "Drangleic Castle & Throne of Want",
            MapName.Nashandra => "Nashandra",
            MapName.LookingGlassKnightArena => "Looking Glass Knight Arena",
            MapName.UndeadCrypt => "Undead Crypt",
            MapName.DragonMemories => "Dragon Memories",
            MapName.DarkChasmOfOld => "Dark Chasm of Old",
            MapName.ShulvaSanctumCity => "Shulva, Sanctum City",
            MapName.PriestessChamber => "Priestess' Chamber",
            MapName.DragonSanctum => "Dragon Sanctum",
            MapName.BrumeTower => "Brume Tower",
            MapName.BrumeTowerWithOnlyTowerKey => "Brume Tower With Only Tower Key",
            MapName.BrumeTowerWithOnlyScorchingIronScepter => "Brume Tower With Only Scorching Iron Scepter",
            MapName.BrumeTowerWithBothKeys => "Brume Tower With Both Keys",
            MapName.FrozenEleumLoyce => "Frozen Eleum Loyce",
            MapName.FrozenEleumLoyceAfterAava => "Frozen Eleum Loyce After Aava",
            MapName.ReinderValley => "Reindeer Valley",
            MapName.MemoryOfTheKing => "Memory of the King",

            // ---- Dark Souls III ---------------------------------------------
            MapName.HighWallOfLothricGarden => "High Wall of Lothric / Garden",
            MapName.OceirosGarden => "Oceiros' Garden",
            MapName.DarkwraithCell => "Darkwraith Cell",
            MapName.LothricCastle => "Lothric Castle",
            MapName.UndeadSettlement => "Undead Settlement",
            MapName.VelkaShrine => "Velka Shrine",
            MapName.ArchdragonPeak => "Archdragon Peak",
            MapName.RoadOfSacrificesFarronKeep => "Road of Sacrifices / Farron Keep",
            MapName.GrandArchives => "Grand Archives",
            MapName.CathedralOfTheDeep => "Cathedral of the Deep",
            MapName.IrithyllAnorLondo => "Irithyll / Anor Londo",
            MapName.CatacombsCarthusSmoulderingLake => "Catacombs Carthus / Smouldering Lake",
            MapName.DungeonProfanedCapital => "Dungeon / Profaned Capital",
            MapName.LedgeOutsideJailbreakersWindow => "Ledge Outside Jailbreaker's Window",
            MapName.JailCells => "Jail Cells",
            MapName.OldCell => "Old Cell",
            MapName.CemetaryFirelinkUntendedGraves => "Cemetery / Firelink / Untended Graves",
            MapName.UntendedGraves => "Untended Graves",
            MapName.FirelinkTower => "Firelink Tower",
            MapName.FirelinkRoof => "Firelink Roof",
            MapName.FirstCinders => "First Cinders",
            MapName.SecondCinders => "Second Cinders",
            MapName.ThirdCinders => "Third Cinders",
            MapName.KilnOfFlameFlamelessShrine => "Kiln of Flame / Flameless Shrine",
            MapName.PaintedWorldOfAriandel => "Painted World of Ariandel",
            MapName.PaintedWorldSecondHalf => "Painted World Second Half",
            MapName.DregHeap => "Dreg Heap",
            MapName.RingedCity => "Ringed City",
            MapName.FilianloresRest => "Filianore's Rest",

            _ => throw new ArgumentOutOfRangeException(nameof(name), name, "Unknown MapName")
        };
    }

    public class Map
    {
        public readonly MapName Name;
        public readonly string FriendlyName;
        public readonly string FileName;
        public readonly SoulsGame SourceGame;
        public readonly IReadOnlyList<string> Bonfires;
        public List<IItemLot> ItemLocations = [];
        public List<Map> connectedMaps = [];

        public static IReadOnlyDictionary<MapName, Map> DSRMaps { get; } = ParseMapDefinitions(
        [
            (MapName.DS1StartingCell,                     "m00_00_00_00", SoulsGame.DSR, new List<string>{ },                                                                                                                  new List<MapName>{ }),
            (MapName.Depths,                              "m10_00_00_00", SoulsGame.DSR, new List<string>{ },                                                                                                                  new List<MapName>{ }),
            (MapName.SewerChamber,                        "m00_00_00_00", SoulsGame.DSR, new List<string>{ "Depths" },                                                                                                         new List<MapName>{ }),
            (MapName.UndeadBurgUndeadParish,              "m10_01_00_00", SoulsGame.DSR, new List<string>{ "Sunlight Altar", "Undead Burg", "Undead Parish" },                                                                 new List<MapName>{ MapName.FirelinkShrine, MapName.DarkrootGarden }),
            (MapName.UndeadBurgResidence,                 "m00_00_00_00", SoulsGame.DSR, new List<string>{ },                                                                                                                  new List<MapName>{ }),
            (MapName.LowerUndeadBurg,                     "m00_00_00_00", SoulsGame.DSR, new List<string>{ },                                                                                                                  new List<MapName>{ MapName.FirelinkShrine }),
            (MapName.LowerUndeadBurgResidence,            "m00_00_00_00", SoulsGame.DSR, new List<string>{ },                                                                                                                  new List<MapName>{ }),
            (MapName.FirelinkShrine,                      "m10_02_00_00", SoulsGame.DSR, new List<string>{ "Firelink Shrine (DS1)" },                                                                                          new List<MapName>{ MapName.NorthernUndeadAsylum, MapName.NewLondoRuinsValleyOfDrakes, MapName.Catacombs, MapName.UndeadBurgUndeadParish }),
            (MapName.PaintedWorld,                        "m11_00_00_00", SoulsGame.DSR, new List<string>{ "Painted World of Ariamis" },                                                                                       new List<MapName>{ MapName.AnorLondo }),
            (MapName.PaintedWorldAnnex,                   "m00_00_00_00", SoulsGame.DSR, new List<string>{ },                                                                                                                  new List<MapName>{ }),
            (MapName.DarkrootGarden,                      "m12_00_00_01", SoulsGame.DSR, new List<string>{ "Darkroot Garden" },                                                                                                new List<MapName>{ MapName.UndeadBurgUndeadParish, MapName.NewLondoRuinsValleyOfDrakes }),
            (MapName.Oolacile,                            "m12_01_00_00", SoulsGame.DSR, new List<string>{ "Oolacile Sanctuary", "Oolacile Township", "Sanctuary Garden", "Oolacile Township Dungeon", "Chasm of the Abyss" }, new List<MapName>{ }),
            (MapName.OolacileAfterGough,                  "m00_00_00_00", SoulsGame.DSR, new List<string>{ },                                                                                                                  new List<MapName>{ MapName.Oolacile }),
            (MapName.Catacombs,                           "m13_00_00_00", SoulsGame.DSR, new List<string>{ "Upper Catacombs", "Inner Catacombs", "Vamos" },                                                                    new List<MapName>{ MapName.FirelinkShrine, MapName.TombOfTheGiants }),
            (MapName.TombOfTheGiants,                     "m13_01_00_00", SoulsGame.DSR, new List<string>{ "Upper Tomb of the Giants", "Lower Tomb of the Giants" },                                                           new List<MapName>{ MapName.Catacombs }),
            (MapName.TombOfTheGiantsPostLordvessel,       "m00_00_00_00", SoulsGame.DSR, new List<string>{ },                                                                                                                  new List<MapName>{ }),
            (MapName.GreatHollowAshLake,                  "m13_02_00_00", SoulsGame.DSR, new List<string>{ "Great Hollow", "Stone Dragon", "Ash Lake" },                                                                       new List<MapName>{ MapName.Blighttown }),
            (MapName.Blighttown,                          "m14_00_00_00", SoulsGame.DSR, new List<string>{ "Daughter of Chaos", "Lower Blighttown", "Upper Blighttown" },                                                      new List<MapName>{ MapName.ValleyOfDrakes, MapName.DemonRuinsLostIzalith, MapName.GreatHollowAshLake }),
            (MapName.DemonRuinsLostIzalith,               "m14_01_00_00", SoulsGame.DSR, new List<string>{ "Upper Demon Ruins", "Lower Demon Ruins" },                                                                         new List<MapName>{ MapName.Blighttown }),
            (MapName.DemonRuinsLostIzalithPostLordvessel, "m00_00_00_00", SoulsGame.DSR, new List<string>{ "Lost Izalith", "Demon Ruins Catacombs", "Lost Izalith Lava Pits" },                                                new List<MapName>{ MapName.Blighttown }),
            (MapName.SensFortress,                        "m15_00_00_00", SoulsGame.DSR, new List<string>{ "Sen's Fortress" },                                                                                                 new List<MapName>{ MapName.AnorLondo }),
            (MapName.SensCage,                            "m00_00_00_00", SoulsGame.DSR, new List<string>{ },                                                                                                                  new List<MapName>{ }),
            (MapName.AnorLondo,                           "m15_01_00_00", SoulsGame.DSR, new List<string>{ "Anor Londo (DS1)", "Inner Anor Londo", "Chamber of the Princess", "Darkmoon Tomb" },                               new List<MapName>{ MapName.SensFortress }),
            (MapName.NewLondoRuinsValleyOfDrakes,         "m16_00_00_00", SoulsGame.DSR, new List<string>{ },                                                                                                                  new List<MapName>{ MapName.FirelinkShrine }),
            (MapName.NewLondoRuinsPostSeal,               "m00_00_00_00", SoulsGame.DSR, new List<string>{ },                                                                                                                  new List<MapName>{ MapName.NewLondoRuinsValleyOfDrakes, MapName.ValleyOfDrakes }),
            (MapName.ValleyOfDrakes,                      "m00_00_00_00", SoulsGame.DSR, new List<string>{ "Darkroot Basin" },                                                                                                 new List<MapName>{ MapName.DarkrootGarden, MapName.Blighttown }),
            (MapName.FourKings,                           "m00_00_00_00", SoulsGame.DSR, new List<string>{ },                                                                                                                  new List<MapName>{ }),
            (MapName.DukesArchives,                       "m17_00_00_00", SoulsGame.DSR, new List<string>{ "Duke's Archives Entrance" },                                                                                       new List<MapName>{ MapName.TowerCell }),
            (MapName.TowerCell,                           "m00_00_00_00", SoulsGame.DSR, new List<string>{ "Prison Tower (DS1)" },                                                                                             new List<MapName>{ }),
            (MapName.ArchivesTower,                       "m00_00_00_00", SoulsGame.DSR, new List<string>{ },                                                                                                                  new List<MapName>{ }),
            (MapName.ArchivesTowerExtra,                  "m00_00_00_00", SoulsGame.DSR, new List<string>{ },                                                                                                                  new List<MapName>{ }),
            (MapName.ArchivesTowerGiantCell,              "m00_00_00_00", SoulsGame.DSR, new List<string>{ },                                                                                                                  new List<MapName>{ }),
            (MapName.CrystalCave,                         "m00_00_00_00", SoulsGame.DSR, new List<string>{ "Duke's Archives Balcony" },                                                                                        new List<MapName>{ MapName.DukesArchives }),
            (MapName.FirelinkAltar,                       "m00_00_00_00", SoulsGame.DSR, new List<string>{ "Firelink Altar" },                                                                                                 new List<MapName>{ }),
            (MapName.FirstLordSoul,                       "m00_00_00_00", SoulsGame.DSR, new List<string>{ },                                                                                                                  new List<MapName>{ }),
            (MapName.SecondLordSoul,                      "m00_00_00_00", SoulsGame.DSR, new List<string>{ },                                                                                                                  new List<MapName>{ }),
            (MapName.ThirdLordSoul,                       "m00_00_00_00", SoulsGame.DSR, new List<string>{ },                                                                                                                  new List<MapName>{ }),
            (MapName.FourthLordSoul,                      "m00_00_00_00", SoulsGame.DSR, new List<string>{ },                                                                                                                  new List<MapName>{ }),
            (MapName.KilnOfTheFirstFlame,                 "m18_00_00_00", SoulsGame.DSR, new List<string>{ },                                                                                                                  new List<MapName>{ }),
            (MapName.NorthernUndeadAsylum,                "m18_01_00_00", SoulsGame.DSR, new List<string>{ "Undead Asylum Courtyard", "Undead Asylum Sewer" },                                                                 new List<MapName>{ }),
            (MapName.NorthernUndeadAsylumF2East,          "m00_00_00_00", SoulsGame.DSR, new List<string>{ },                                                                                                                  new List<MapName>{ MapName.NorthernUndeadAsylum }),
            (MapName.NorthernUndeadAsylumF2West,          "m00_00_00_00", SoulsGame.DSR, new List<string>{ },                                                                                                                  new List<MapName>{ MapName.NorthernUndeadAsylum }),
        ]);

        public static Dictionary<MapName, HashSet<int>> DS1NonDefaultMapItemLots = new()
        {
            { MapName.DS1StartingCell,                     new HashSet<int>{ 1810000 } },
            { MapName.LowerUndeadBurg,                     new HashSet<int>{ 2510, 1010020, 1010370, 1010380, 1010381, 1010382, 1010383, 1010384, 1010430, 1010490, 60001200, 60001201, 60001202, 60001203, 60001204, 60001205, 60001206, 60001207, 60001208, 60001209, 60001210, 60001211, 60001212, 60001213, 60001214, 60001215, 60001216, 60001217, 60001218, 60001219, 60001220 } },
            { MapName.LowerUndeadBurgResidence,            new HashSet<int>{ 1010510, 1010511, 1010512, 1010513, 1010514 } },
            { MapName.UndeadBurgResidence,                 new HashSet<int>{ 1010460 } },
            { MapName.PaintedWorldAnnex,                   new HashSet<int>{ 1100060, 1100061, 1100062, 1100063, 1100090, 1100100, 1100320, 1100340, 1100370 } },
            { MapName.OolacileAfterGough,                  new HashSet<int>{ 1510, 2710, 1210250, 41100001, 45100000, 45110000, 60006600, 60006601, 60006602, 60006603, 60006604, 60006608, 60006609, 60006610, 60006611 } },
            { MapName.TombOfTheGiantsPostLordvessel,       new HashSet<int>{ 2560, 1310200, 1310220, 1310230, 1310240, 1310241, 1310242, 1310243, 1310290, 52200000 } },
            { MapName.DemonRuinsLostIzalithPostLordvessel, new HashSet<int>{ 2670, 1410160, 1410230, 1410250, 1410270, 22310000, 2580, 6620, 1410000, 1410010, 1410020, 1410030, 1410310, 1410320, 1410330, 1410340, 1410360, 1410380, 1410390, 1410400, 1410410, 1410500, 1410520, 34800100, 52300000, 52400000, 54000000, 54010000 } },
            { MapName.SensCage,                            new HashSet<int>{ 1500420 } },
            { MapName.NewLondoRuinsPostSeal,               new HashSet<int>{ 1600250, 1600260, 1600270, 1600280, 1600290, 1600310, 1600330, 1600360, 1600361, 1600362, 1600363, 1600364, 1600370, 1600500, 1600510 } },
            { MapName.ValleyOfDrakes,                      new HashSet<int>{ 1600170, 1600180, 1600190, 1600200, 1600210, 1600220, 1600221, 1600222, 1600223, 1600224, 1600380, 34200200 } },
            { MapName.FourKings,                           new HashSet<int>{ 2630 } },
            { MapName.TowerCell,                           new HashSet<int>{ 26900100 } },
            { MapName.ArchivesTower,                       new HashSet<int>{ 2020, 1700070, 1700071, 1700072, 1700073, 1700074, 1700210, 1700630, 33300100, 33300200 } },
            { MapName.ArchivesTowerExtra,                  new HashSet<int>{ 1700060, 1700080 } },
            { MapName.ArchivesTowerGiantCell,              new HashSet<int>{ 1700200 } },
            { MapName.CrystalCave,                         new HashSet<int>{ 2640, 1700150, 1700160, 1700170, 1700180, 52900100, 52910000 } },
            { MapName.NorthernUndeadAsylumF2East,          new HashSet<int>{ 1810220, 1810221, 1810250, 1810280, 1810310 } },
            { MapName.NorthernUndeadAsylumF2West,          new HashSet<int>{ 1810060 } },
            { MapName.FirelinkShrine,                      new HashSet<int>{ 1810070 } }, // Technically not in Firelink, this is the item on the left as you leave the Asylum. I didn't feel like making another zone for it.
        };

        public static IReadOnlyDictionary<MapName, Map> DS2Maps { get; } = ParseMapDefinitions(
        [
            (MapName.ThingsBetwixt,                        "m10_02_00_00", SoulsGame.DS2S, new List<string>{ "Fire Keepers' Dwelling" },                                                                       new List<MapName>{ MapName.Majula }),
            (MapName.ThingsBetwixtPostStatue,              "m00_00_00_00", SoulsGame.DS2S, new List<string>{ },                                                                                                new List<MapName>{ MapName.ThingsBetwixt }),
            (MapName.Majula,                               "m10_04_00_00", SoulsGame.DS2S, new List<string>{ "The Far Fire" },                                                                                 new List<MapName>{ MapName.ThingsBetwixt, MapName.ForestOfFallenGiants, MapName.MajulaShadeWoods, MapName.HeidesTowerOfFlame, MapName.HuntsmansCopseUndeadPurgatory, MapName.GraveOfSaints }),
            (MapName.MajulaHouse,                          "m00_00_00_00", SoulsGame.DS2S, new List<string>{ },                                                                                                new List<MapName>{ MapName.Majula }),
            (MapName.LenigrastsHouse,                      "m00_00_00_00", SoulsGame.DS2S, new List<string>{ },                                                                                                new List<MapName>{ }),
            (MapName.DragonTalonRoom,                      "m00_00_00_00", SoulsGame.DS2S, new List<string>{ },                                                                                                new List<MapName>{ }),
            (MapName.ForestOfFallenGiants,                 "m10_10_00_00", SoulsGame.DS2S, new List<string>{ "The Crestfallen's Retreat", "Cardinal Tower" },                                                  new List<MapName>{ MapName.Majula }),
            (MapName.ForestOfFallenGiantsPostSoldierKey,   "m00_00_00_00", SoulsGame.DS2S, new List<string>{ "Soldier's Rest", "The Place Unbeknownst" },                                                      new List<MapName>{ MapName.TheLostBastilleBelfryLuna }),
            (MapName.ForestOfFallenGiantsIronKeyRoom,      "m00_00_00_00", SoulsGame.DS2S, new List<string>{ },                                                                                                new List<MapName>{ }),
            (MapName.BrightstoneCoveTseldora,              "m10_14_00_00", SoulsGame.DS2S, new List<string>{ "Royal Army Campfire", "Chapel Threshold", "Lower Brightstone Cove" },                            new List<MapName>{ MapName.DoorsOfPharros }),
            (MapName.BrightStoneKeyRoom,                   "m00_00_00_00", SoulsGame.DS2S, new List<string>{ },                                                                                                new List<MapName>{ }),
            (MapName.TseldoraDen,                          "m00_00_00_00", SoulsGame.DS2S, new List<string>{ },                                                                                                new List<MapName>{ }),
            (MapName.AldiasKeep,                           "m10_15_00_00", SoulsGame.DS2S, new List<string>{ "Foregarden", "Ritual Site" },                                                                    new List<MapName>{ MapName.DragonAerieDragonShrine }),
            (MapName.AldiaSideRoom,                        "m00_00_00_00", SoulsGame.DS2S, new List<string>{ },                                                                                                new List<MapName>{ }),
            (MapName.TheLostBastilleBelfryLuna,            "m10_16_00_00", SoulsGame.DS2S, new List<string>{ "Exile Holding Cells", "McDuff's Workshop", "The Tower Apart", "The Saltfort" },                  new List<MapName>{ }),
            (MapName.BastilleCells,                        "m00_00_00_00", SoulsGame.DS2S, new List<string>{ },                                                                                                new List<MapName>{ }),
            (MapName.StraidsCell,                          "m00_00_00_00", SoulsGame.DS2S, new List<string>{ "Straid's Cell" },                                                                                new List<MapName>{ }),
            (MapName.RuinSentinelBuilding,                 "m00_00_00_00", SoulsGame.DS2S, new List<string>{ "Servants' Quarters", "Upper Ramparts" },                                                         new List<MapName>{ MapName.TheLostBastilleBelfryLuna }),
            //(MapName.BelfryLuna,                           "m00_00_00_00", SoulsGame.DS2S, new List<string>{ "Upper Ramparts" },                                                                               new List<MapName>{ MapName.RuinSentinelBuilding }),
            (MapName.HarvestValleyEarthenPeak,             "m10_17_00_00", SoulsGame.DS2S, new List<string>{ "Poison Pool", "The Mines", "Lower Earthen Peak", "Central Earthen Peak", "Upper Earthen Peak" }, new List<MapName>{ MapName.HuntsmansCopseUndeadPurgatory, MapName.IronKeepBelfrySol }),
            (MapName.NomansWharf,                          "m10_18_00_00", SoulsGame.DS2S, new List<string>{ "Unseen Path to Heide" },                                                                         new List<MapName>{ MapName.HeidesTowerNomansWharf, MapName.TheLostBastilleBelfryLuna }),
            (MapName.IronKeepBelfrySol,                    "m10_19_00_00", SoulsGame.DS2S, new List<string>{ "Threshold Bridge", "Ironhearth Hall", "Eygil's Idol", "Belfry Sol Approach" },                   new List<MapName>{ MapName.HarvestValleyEarthenPeak }),
            (MapName.HuntsmansCopseUndeadPurgatory,        "m10_23_00_00", SoulsGame.DS2S, new List<string>{ "Undead Refuge", "Bridge Approach", "Undead Purgatory" },                                         new List<MapName>{ MapName.Majula, MapName.HarvestValleyEarthenPeak }),
            (MapName.UndeadLockaway,                       "m00_00_00_00", SoulsGame.DS2S, new List<string>{ "Undead Lockaway" },                                                                              new List<MapName>{ }),
            (MapName.TheGutterBlackGulch,                  "m10_25_00_00", SoulsGame.DS2S, new List<string>{ "Upper Gutter", "Central Gutter", "Black Gulch Mouth" },                                          new List<MapName>{ }),
            (MapName.HiddenChamber,                        "m00_00_00_00", SoulsGame.DS2S, new List<string>{ "Hidden Chamber" },                                                                               new List<MapName>{ MapName.TheGutterBlackGulch }),
            (MapName.HavelArmorRoom,                       "m00_00_00_00", SoulsGame.DS2S, new List<string>{ },                                                                                                new List<MapName>{ }),
            (MapName.DragonAerieDragonShrine,              "m10_27_00_00", SoulsGame.DS2S, new List<string>{ "Dragon Aerie", "Shrine Entrance" },                                                              new List<MapName>{ MapName.AldiasKeep }),
            (MapName.MajulaShadeWoods,                     "m10_29_00_00", SoulsGame.DS2S, new List<string>{ "Old Akelarre" },                                                                                 new List<MapName>{ MapName.ShadedWoodsShrineOfWinter }),
            (MapName.HeidesTowerNomansWharf,               "m10_30_00_00", SoulsGame.DS2S, new List<string>{ },                                                                                                new List<MapName>{ MapName.HeidesTowerOfFlame, MapName.NomansWharf }),
            (MapName.FloodedPassageSideRoom,               "m00_00_00_00", SoulsGame.DS2S, new List<string>{ },                                                                                                new List<MapName>{ }),
            (MapName.HeidesTowerOfFlame,                   "m10_31_00_00", SoulsGame.DS2S, new List<string>{ "Heide's Ruin", "Tower of Flame", "The Blue Cathedral" },                                         new List<MapName>{ MapName.Majula, MapName.HeidesTowerNomansWharf }),
            (MapName.ShadedWoodsShrineOfWinter,            "m10_32_00_00", SoulsGame.DS2S, new List<string>{ "Ruined Fork Road", "Shaded Ruins" },                                                             new List<MapName>{ MapName.DoorsOfPharros, MapName.DrangleicCastleThroneOfWant, MapName.DarkChasmOfOld }),
            (MapName.VengarlsBodyRoom,                     "m00_00_00_00", SoulsGame.DS2S, new List<string>{ },                                                                                                new List<MapName>{ }),
            (MapName.ChestAfterVengarl,                    "m00_00_00_00", SoulsGame.DS2S, new List<string>{ },                                                                                                new List<MapName>{ }),
            (MapName.LionMageSetChest,                     "m00_00_00_00", SoulsGame.DS2S, new List<string>{ },                                                                                                new List<MapName>{ }),
            (MapName.FangKeyLion,                          "m00_00_00_00", SoulsGame.DS2S, new List<string>{ },                                                                                                new List<MapName>{ MapName.ShadedWoodsShrineOfWinter }),
            (MapName.DoorsOfPharros,                       "m10_33_00_00", SoulsGame.DS2S, new List<string>{ "Gyrm's Respite" },                                                                               new List<MapName>{ MapName.ShadedWoodsShrineOfWinter, MapName.BrightstoneCoveTseldora }),
            (MapName.GraveOfSaints,                        "m10_34_00_00", SoulsGame.DS2S, new List<string>{ "Harval's Resting Place", "Grave Entrance" },                                                     new List<MapName>{ MapName.TheGutterBlackGulch }),
            (MapName.MemoryOfVammarOrroAndJeigh,           "m20_10_00_00", SoulsGame.DS2S, new List<string>{ },                                                                                                new List<MapName>{ }),
            (MapName.ShrineOfAmana,                        "m20_11_00_00", SoulsGame.DS2S, new List<string>{ "Tower of Prayer (Amana)", "Crumbled Ruins", "Rhoy's Resting Place" },                            new List<MapName>{ MapName.LookingGlassKnightArena, MapName.UndeadCrypt }),
            (MapName.RiseOfTheDead,                        "m00_00_00_00", SoulsGame.DS2S, new List<string>{ "Rise of the Dead" },                                                                             new List<MapName>{ }),
            (MapName.DrangleicCastleThroneOfWant,          "m20_21_00_00", SoulsGame.DS2S, new List<string>{ "King's Gate", "Forgotten Chamber", "Under Castle Drangleic", "Central Castle Drangleic" },       new List<MapName>{ MapName.ShadedWoodsShrineOfWinter, MapName.DarkChasmOfOld }),
            (MapName.Nashandra,                            "m00_00_00_00", SoulsGame.DS2S, new List<string>{ },                                                                                                new List<MapName>{ }),
            (MapName.LookingGlassKnightArena,              "m00_00_00_00", SoulsGame.DS2S, new List<string>{ },                                                                                                new List<MapName>{ MapName.DrangleicCastleThroneOfWant, MapName.ShrineOfAmana }),
            (MapName.UndeadCrypt,                          "m20_24_00_00", SoulsGame.DS2S, new List<string>{ "Undead Crypt Entrance", "Undead Ditch" },                                                        new List<MapName>{ MapName.ShrineOfAmana, MapName.MemoryOfTheKing }),
            (MapName.DragonMemories,                       "m20_26_00_00", SoulsGame.DS2S, new List<string>{ },                                                                                                new List<MapName>{ MapName.BrightstoneCoveTseldora }),
            (MapName.DarkChasmOfOld,                       "m40_03_00_00", SoulsGame.DS2S, new List<string>{ },                                                                                                new List<MapName>{ }),
            (MapName.ShulvaSanctumCity,                    "m50_35_00_00", SoulsGame.DS2S, new List<string>{ "Sanctum Walk", "Tower of Prayer (Shulva)", "Hidden Sanctum Chamber", "Lair of the Imperfect" },  new List<MapName>{ }),
            (MapName.PriestessChamber,                     "m00_00_00_00", SoulsGame.DS2S, new List<string>{ "Priestess' Chamber" },                                                                           new List<MapName>{ MapName.ShulvaSanctumCity }),
            (MapName.DragonSanctum,                        "m00_00_00_00", SoulsGame.DS2S, new List<string>{ "Sanctum Interior", "Sanctum Nadir" },                                                            new List<MapName>{ }),
            (MapName.BrumeTower,                           "m50_36_00_00", SoulsGame.DS2S, new List<string>{ "Throne Floor", "Upper Floor", "Foyer", "Lowermost Floor" },                                      new List<MapName>{ }),
            (MapName.BrumeTowerWithOnlyTowerKey,           "m00_00_00_00", SoulsGame.DS2S, new List<string>{ },                                                                                                new List<MapName>{ }),
            (MapName.BrumeTowerWithOnlyScorchingIronScepter, "m00_00_00_00", SoulsGame.DS2S, new List<string>{ "Lowermost Floor" },                                                                            new List<MapName>{ }),
            (MapName.BrumeTowerWithBothKeys,               "m00_00_00_00", SoulsGame.DS2S, new List<string>{ "Smelter Throne", "Iron Hallway Entrance" },                                                      new List<MapName>{ }), // Technically connected to both single-key zones, but for simplicity I picked this one
            (MapName.FrozenEleumLoyce,                     "m50_37_00_00", SoulsGame.DS2S, new List<string>{ "Outer Wall", "Abandoned Dwelling", "Inner Wall", "Lower Garrison" },                             new List<MapName>{ MapName.ShadedWoodsShrineOfWinter }),
            (MapName.FrozenEleumLoyceAfterAava,            "m00_00_00_00", SoulsGame.DS2S, new List<string>{ "Grand Cathedral" },                                                                              new List<MapName>{ }),
            (MapName.ReinderValley,                        "m00_00_00_00", SoulsGame.DS2S, new List<string>{ "Expulsion Chamber" },                                                                            new List<MapName>{ MapName.FrozenEleumLoyce }),
            (MapName.MemoryOfTheKing,                      "m50_38_00_00", SoulsGame.DS2S, new List<string>{ },                                                                                                new List<MapName>{ }),
        ]);

        public static Dictionary<MapName, HashSet<int>> DS2NonDefaultMapItemLots = new()
        {
            { MapName.ThingsBetwixtPostStatue,              new HashSet<int>{ 10026100 } },
            { MapName.ForestOfFallenGiantsPostSoldierKey,   new HashSet<int>{ 10106070, 10106080, 10106120, 10106610, 10106010, 10106630, 10106620, 10106430, 10105120, 10106370, 10106370, 318000 } },
            { MapName.ForestOfFallenGiantsIronKeyRoom,      new HashSet<int>{ 10106460, 10106350, 10106360, 10106480, 10106470, 10105110 } },
            { MapName.LookingGlassKnightArena,              new HashSet<int>{ 20216080, 20216060, 20216061, 20216070, 20216120, 504000, 20215150 } },
            { MapName.BastilleCells,                        new HashSet<int>{ 10166440, 10166441, 10166330, 10166350 } },
            { MapName.RuinSentinelBuilding,                 new HashSet<int>{ 10166270, 10166320, 325000, 10165210, 10166000, 10165080, 10166100, 10166150, 10166180, 10166290, 10166370, 10166020, 10166380, 10165130, 10165220, 10166160, 10165200, 10166170, 324000, 10165230, 10166250, 10166390 } },
            //{ MapName.BelfryLuna,                         new HashSet<int>{ 10165220, 10166160, 10165200, 10166170, 324000, 10165230, 10166250, 10166390 } }, // I've decided to ignore Pharros Lockstones for now. If I do include them, this needs to come back.
            { MapName.MajulaHouse,                          new HashSet<int>{ 10046100, 10045010, 10045510 } },
            { MapName.LenigrastsHouse,                      new HashSet<int>{ 10045040 } },
            { MapName.DragonTalonRoom,                      new HashSet<int>{ 10046140, 10045020, 10045050, 10045030 } },
            { MapName.HiddenChamber,                        new HashSet<int>{ 10256360 } },
            { MapName.HavelArmorRoom,                       new HashSet<int>{ 10256000 } },
            { MapName.BrightStoneKeyRoom,                   new HashSet<int>{ 10145130 } },
            { MapName.TseldoraDen,                          new HashSet<int>{ 10145070 } },
            { MapName.AldiaSideRoom,                        new HashSet<int>{ 10156040, 10156140, 1153300, 1153400, 1153500 } },
            { MapName.FloodedPassageSideRoom,               new HashSet<int>{ 10306030, 10305010 } },
            { MapName.VengarlsBodyRoom,                     new HashSet<int>{ 10326230 } },
            { MapName.ChestAfterVengarl,                    new HashSet<int>{ 10325120 } },
            { MapName.LionMageSetChest,                     new HashSet<int>{ 10325040 } },
            { MapName.FangKeyLion,                          new HashSet<int>{ 60009000 } },
            { MapName.RiseOfTheDead,                        new HashSet<int>{ 20116110, 20115110 } },
            { MapName.Nashandra,                            new HashSet<int>{ 627000 } },
            { MapName.PriestessChamber,                     new HashSet<int>{ 50355190, 50355200, 50355210, 50355220, 50355230, 50355240, 50355150, 50356610, 50356620, 50356670, 50355180, 50356390, 50355140, 862000 } },
            { MapName.DragonSanctum,                        new HashSet<int>{ 681000, 682000, 50356450, 50356520, 50356490, 50356460, 50356470, 50356480 } },
            { MapName.BrumeTowerWithOnlyTowerKey,           new HashSet<int>{ 50365030, 50366520, 50365570 } },
            { MapName.BrumeTowerWithOnlyScorchingIronScepter, new HashSet<int>{ 50365680, 50366760, 50365080, 50368010, 50368080, 675000, 50368070, 50366720, 50366850, 50366830, 50366210, 50366870, 50366860, 50366710, 50366680, 50366700, 50365650, 50365550, 50366240, 50366890, 50366880, 50367130, 50366250, 50365020, 50366530, 50366070, 50365580 } },
            { MapName.BrumeTowerWithBothKeys,               new HashSet<int>{ 50366740, 50367010, 50367040, 50367050, 50366990, 50366980, 50367000, 305010, 50367060, 50366920, 50366930, 50366910, 50366940, 50366970, 50366960, 680000 } },
            { MapName.FrozenEleumLoyceAfterAava,            new HashSet<int>{ 50375710, 690000, 50376760, 50376750, 50376010, 50376060, 50376310, 50376320, 50376180, 50376190, 50376660, 50375560, 50376200, 50376630, 50376690, 50376680, 50376670, 50376610, 50376620, 50376640, 50376650, 50376520, 50376420, 50376430, 50376440, 50375540, 50375520, 50375510, 50376570, 50376300, 50376580, 50376770, 50376510, 50375740, 50376400, 50375680, 50375580, 50375590, 50375600, 50375610, 50375550, 50376150, 50375690, 50375700, 50375660, 50376380, 50375670 } },
            { MapName.ReinderValley,                        new HashSet<int>{ 50376730, 50376210, 50376740, 50376220, 50376230, 50376460, 50376710, 50376470 } },
        };

        public static IReadOnlyDictionary<MapName, Map> DS3Maps { get; } = ParseMapDefinitions(
        [
            (MapName.HighWallOfLothricGarden,              "m30_00_00_00", SoulsGame.DS3, new List<string>{ "High Wall of Lothric", "Vordt of the Boreal Valley", "Tower on the Wall" },                                                                                    new List<MapName>{ }),
            (MapName.OceirosGarden,                        "m00_00_00_00", SoulsGame.DS3, new List<string>{ "Oceiros, the Consumed King", "Dancer of the Boreal Valley" },                                                                                                  new List<MapName>{ MapName.HighWallOfLothricGarden, MapName.UntendedGraves }),
            (MapName.DarkwraithCell,                       "m00_00_00_00", SoulsGame.DS3, new List<string>{ },                                                                                                                                                              new List<MapName>{ }),
            (MapName.LothricCastle,                        "m30_01_00_00", SoulsGame.DS3, new List<string>{ "Dragonslayer Armour", "Lothric Castle", "Dragon Barracks" },                                                                                                   new List<MapName>{ MapName.HighWallOfLothricGarden }),
            (MapName.UndeadSettlement,                     "m31_00_00_00", SoulsGame.DS3, new List<string>{ "Pit of Hollows", "Undead Settlement", "Cliff Underside", "Dilapidated Bridge", "Foot of the High Wall" },                                                      new List<MapName>{ MapName.RoadOfSacrificesFarronKeep }),
            (MapName.VelkaShrine,                          "m00_00_00_00", SoulsGame.DS3, new List<string>{ },                                                                                                                                                              new List<MapName>{ MapName.UndeadSettlement }),
            (MapName.ArchdragonPeak,                       "m32_00_00_00", SoulsGame.DS3, new List<string>{ "Archdragon Peak", "Great Belfry", "Dragon-Kin Mausoleum", "Nameless King" },                                                                                   new List<MapName>{ }),
            (MapName.RoadOfSacrificesFarronKeep,           "m33_00_00_00", SoulsGame.DS3, new List<string>{ "Road of Sacrifices", "Halfway Fortress", "Crucifixion Woods", "Crystal Sage", "Farron Keep", "Keep Ruins", "Farron Keep Perimeter", "Old Wolf of Farron", "Abyss Watchers" }, new List<MapName>{ MapName.UndeadSettlement, MapName.CathedralOfTheDeep, MapName.CatacombsCarthusSmoulderingLake }),
            (MapName.GrandArchives,                        "m34_01_00_00", SoulsGame.DS3, new List<string>{ "Grand Archives", "Twin Princes" },                                                                                                                             new List<MapName>{ MapName.LothricCastle }),
            (MapName.CathedralOfTheDeep,                   "m35_00_00_00", SoulsGame.DS3, new List<string>{ "Cathedral of the Deep", "Cleansing Chapel", "Rosaria's Bed Chamber", "Deacons of the Deep" },                                                                  new List<MapName>{ MapName.RoadOfSacrificesFarronKeep, MapName.PaintedWorldOfAriandel }),
            (MapName.IrithyllAnorLondo,                    "m37_00_00_00", SoulsGame.DS3, new List<string>{ "Irithyll of the Boreal Valley", "Central Irithyll", "Church of Yorshka", "Distant Manor", "Pontiff Sulyvahn", "Water Reserve", "Anor Londo (DS3)", "Prison Tower (DS3)", "Aldrich, Devourer of Gods" }, new List<MapName>{ MapName.DungeonProfanedCapital }),
            (MapName.CatacombsCarthusSmoulderingLake,      "m38_00_00_00", SoulsGame.DS3, new List<string>{ "Catacombs of Carthus", "High Lord Wolnir", "Abandoned Tomb", "Old King's Antechamber", "Demon Ruins", "Old Demon King" },                                      new List<MapName>{ MapName.RoadOfSacrificesFarronKeep }),
            (MapName.DungeonProfanedCapital,               "m39_00_00_00", SoulsGame.DS3, new List<string>{ "Irithyll Dungeon", "Profaned Capital", "Yhorm the Giant" },                                                                                                    new List<MapName>{ MapName.ArchdragonPeak, MapName.IrithyllAnorLondo }),
            (MapName.LedgeOutsideJailbreakersWindow,       "m00_00_00_00", SoulsGame.DS3, new List<string>{ },                                                                                                                                                              new List<MapName>{ MapName.DungeonProfanedCapital }),
            (MapName.JailCells,                            "m00_00_00_00", SoulsGame.DS3, new List<string>{ },                                                                                                                                                              new List<MapName>{ }),
            (MapName.OldCell,                              "m00_00_00_00", SoulsGame.DS3, new List<string>{ },                                                                                                                                                              new List<MapName>{ }),
            (MapName.CemetaryFirelinkUntendedGraves,       "m40_00_00_00", SoulsGame.DS3, new List<string>{ "Firelink Shrine (DS3)", "Cemetery of Ash", "Iudex Gundyr", "Untended Graves", "Champion Gundyr" },                                                             new List<MapName>{ }),
            (MapName.UntendedGraves,                       "m00_00_00_00", SoulsGame.DS3, new List<string>{ "Untended Graves" },                                                                                                                                            new List<MapName>{ }),
            (MapName.FirelinkTower,                        "m00_00_00_00", SoulsGame.DS3, new List<string>{ },                                                                                                                                                              new List<MapName>{ MapName.CemetaryFirelinkUntendedGraves }),
            (MapName.FirelinkRoof,                         "m00_00_00_00", SoulsGame.DS3, new List<string>{ },                                                                                                                                                              new List<MapName>{ MapName.CemetaryFirelinkUntendedGraves }),
            (MapName.FirstCinders,                         "m00_00_00_00", SoulsGame.DS3, new List<string>{ },                                                                                                                                                              new List<MapName>{ }),
            (MapName.SecondCinders,                        "m00_00_00_00", SoulsGame.DS3, new List<string>{ },                                                                                                                                                              new List<MapName>{ }),
            (MapName.ThirdCinders,                         "m00_00_00_00", SoulsGame.DS3, new List<string>{ },                                                                                                                                                              new List<MapName>{ }),
            (MapName.KilnOfFlameFlamelessShrine,           "m41_00_00_00", SoulsGame.DS3, new List<string>{ "Flameless Shrine", "Kiln of the First Flame", "Soul of Cinder" },                                                                                              new List<MapName>{ MapName.DregHeap }),
            (MapName.PaintedWorldOfAriandel,               "m45_00_00_00", SoulsGame.DS3, new List<string>{ "Snowfield", "Rope Bridge Cave", "Corvian Settlement", "Ariandel Chapel", "Sister Friede", "Depths of the Painting", "Champion's Gravetender" },                new List<MapName>{ }),
            (MapName.PaintedWorldSecondHalf,               "m00_00_00_00", SoulsGame.DS3, new List<string>{ "Snowy Mountain Pass" },                                                                                                                                        new List<MapName>{ MapName.PaintedWorldOfAriandel, MapName.DregHeap }),
            (MapName.DregHeap,                             "m50_00_00_00", SoulsGame.DS3, new List<string>{ "The Dreg Heap", "Earthen Peak Ruins", "Within the Earthen Peak Ruins", "The Deamon Prince" },                                                                  new List<MapName>{ }),
            (MapName.RingedCity,                           "m51_00_00_00", SoulsGame.DS3, new List<string>{ "Mausoleum Lookout", "Ringed Inner Wall", "Ringed City Streets", "Shared Grave", "Church of Filianore", "Darkeater Midir" },                                    new List<MapName>{ MapName.FilianloresRest }),
            (MapName.FilianloresRest,                      "m51_01_00_00", SoulsGame.DS3, new List<string>{ "Filianore's Rest", "Slave Knight Gael" },                                                                                                                      new List<MapName>{ }),
        ]);

        public static Dictionary<MapName, HashSet<int>> DS3NonDefaultMapItemLots = new()
        {
            { MapName.DarkwraithCell,                      new HashSet<int>{ 60940 } },
            { MapName.OceirosGarden,                       new HashSet<int>{ 3000540, 3000000, 3000530, 3000480, 3000430, 3000431, 3000432, 3000433, 3000470, 3000630, 3000620, 3000510, 3000570, 3000520, 3000500, 2020, 3000840, 3000800 } },
            { MapName.LedgeOutsideJailbreakersWindow,      new HashSet<int>{ 3900100 } },
            { MapName.JailCells,                           new HashSet<int>{ 3900500, 3900820 } },
            { MapName.OldCell,                             new HashSet<int>{ 3900400 } },
            { MapName.VelkaShrine,                         new HashSet<int>{ 3100220, 3100260, 3100070, 3100340, 3100330, 3100740, 3100300 } },
            { MapName.UntendedGraves,                      new HashSet<int>{ 4000250, 4000220, 4000240, 4000270, 4000260, 4000310, 4000280 } },
            { MapName.FirelinkTower,                       new HashSet<int>{ 4000190, 4000350, 4000351, 4000352, 4000170 } },
            { MapName.FirelinkRoof,                        new HashSet<int>{ 4000160, 4000180, 4000700 } }, // If not assuming tree skip, include in Firelink Tower, otherwise include in Firelink
            { MapName.PaintedWorldSecondHalf,              new HashSet<int>{ 4500310, 4500320, 4500330, 4500340, 4500350, 4500360, 4500370, 4500380, 4500390, 4500400, 4500410, 4500420, 4500430, 4500460, 4500470, 4500471, 4500472, 4500473, 4500480, 4500570, 4500571, 2300 } },
        };

        public static IReadOnlyDictionary<MapName, Map> AllMaps = DSRMaps.Concat(DS2Maps).Concat(DS3Maps).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        private Map(MapName name, string fileName, SoulsGame sourceGame, IReadOnlyList<string> bonfires)
        {
            Name = name;
            FriendlyName = name.ToFriendlyName();
            FileName = fileName;
            SourceGame = sourceGame;
            Bonfires = bonfires;
        }

        private static Dictionary<MapName, Map> ParseMapDefinitions(List<(MapName Name, string FileName, SoulsGame SourceGame, List<string> Bonfires, List<MapName> ConnectedMapNames)> mapDefinitions)
        {
            var mapsByName = new Dictionary<MapName, Map>();

            foreach (var mapDefinition in mapDefinitions)
            {
                var map = new Map(mapDefinition.Name, mapDefinition.FileName, mapDefinition.SourceGame, mapDefinition.Bonfires);
                mapsByName.Add(mapDefinition.Name, map);
            }

            foreach (var mapDefinition in mapDefinitions)
            {
                var currentMap = mapsByName[mapDefinition.Name];
                foreach (var connectedMapName in mapDefinition.ConnectedMapNames)
                {
                    currentMap.connectedMaps.Add(mapsByName[connectedMapName]);
                }
            }

            return mapsByName;
        }

        public static void LoadCrossGameWarps(List<BonfireTriple> bonfireMapping)
        {
            foreach (var bonfireTriple in bonfireMapping)
            {
                var dsrMap = DSRMaps.Values.Single(map => map.Bonfires.Contains(bonfireTriple.DS1Bonfire));
                var ds2Map = DS2Maps.Values.Single(map => map.Bonfires.Contains(bonfireTriple.DS2Bonfire));
                var ds3Map = DS3Maps.Values.Single(map => map.Bonfires.Contains(bonfireTriple.DS3Bonfire));

                if (bonfireTriple.DS3Bonfire == "Firelink Shrine (DS3)") // DS3's Firelink bonfire doesn't exist until you get the coiled sword, so it needs special handling.
                {
                    var coiledSword = Key.DS3Keys.Single(key => key.itemId == 2137);
                    coiledSword.AddUnlockedConnection((ds3Map, dsrMap));
                    coiledSword.AddUnlockedConnection((ds3Map, ds2Map));
                }
                else
                {
                    dsrMap.connectedMaps.Add(ds2Map);
                    dsrMap.connectedMaps.Add(ds3Map);
                    ds2Map.connectedMaps.Add(dsrMap);
                    ds2Map.connectedMaps.Add(ds3Map);
                    ds3Map.connectedMaps.Add(dsrMap);
                    ds3Map.connectedMaps.Add(ds2Map);
                }
            }
        }

        public static void HandleDS3FirelinkRoofSkip(bool allow)
        {
            if (allow)
            {
                var firelink = Map.DS3Maps[MapName.CemetaryFirelinkUntendedGraves];
                var firelinkRoof = Map.DS3Maps[MapName.FirelinkRoof];
                firelink.connectedMaps.Add(firelinkRoof);
            }
            else
            {
                var firelinkTower = Map.DS3Maps[MapName.FirelinkTower];
                var firelinkRoof = Map.DS3Maps[MapName.FirelinkRoof];
                firelinkTower.connectedMaps.Add(firelinkRoof);
            }
        }

        public bool CanReach(MapName mapName)
        {
            var result = false;
            VisitAllConnectedMaps(map => result = result || map.Name == mapName);
            return result;
        }

        public bool CanUse(Key key)
        {
            var result = false;
            VisitAllConnectedMaps(map => result = result || key.ConnectionsUnlocked.Any(connection => connection.Item1 == map || connection.Item2 == map));
            return result;
        }

        public List<IItemLot> GetAccessibleItemLots(Func<IItemLot, bool> lotSelector)
        {
            var result = new List<IItemLot>();
            VisitAllConnectedMaps(map => result.AddRange(map.ItemLocations.Where(lotSelector)));
            return result;
        }

        private void VisitAllConnectedMaps(Action<Map> visitor)
        {
            Queue<Map> mapsToSearch = new Queue<Map>();
            HashSet<Map> visitedMaps = new HashSet<Map>();
            mapsToSearch.Enqueue(this);
            while (mapsToSearch.Count > 0)
            {
                var nextMap = mapsToSearch.Dequeue();
                visitor(nextMap);
                visitedMaps.Add(nextMap);
                foreach (var newMap in nextMap.connectedMaps.Where(connectedMap => !visitedMaps.Contains(connectedMap)))
                {
                    mapsToSearch.Enqueue(newMap);
                }
            }
        }
    }
}
