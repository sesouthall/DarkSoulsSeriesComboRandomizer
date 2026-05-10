namespace DarkSoulsSeriesComboRandomizer
{
    public abstract record MapDefinition(MapName Name, List<string> Bonfires, List<MapName> MapsConnectedWithoutKeys);

    public record SubMapDefinition(
        MapName Name,
        List<string> Bonfires,
        List<MapName> MapsConnectedWithoutKeys,
        HashSet<int> ItemLotSeeds) : MapDefinition(Name, Bonfires, MapsConnectedWithoutKeys);

    public record FileBackedMapDefinition(
        MapName Name,
        List<string> Bonfires,
        List<MapName> MapsConnectedWithoutKeys,
        Dictionary<int, LotType> ExtraItemLots,
        List<SubMapDefinition> SubMaps) : MapDefinition(Name, Bonfires, MapsConnectedWithoutKeys);

    public static class MapData
    {
        public static readonly List<FileBackedMapDefinition> DS1MapDefinitions =
        [
            new(MapName.Depths, Bonfires: [], MapsConnectedWithoutKeys: [], ExtraItemLots: new Dictionary<int, LotType>{ { 2500, LotType.Boss }, { 52610000, LotType.GenericEvent } }, 
                [
                    new(MapName.SewerChamber, Bonfires: ["Depths"], MapsConnectedWithoutKeys: [], ItemLotSeeds: [])
                ]),
            new(MapName.UndeadBurgUndeadParish, Bonfires: ["Sunlight Altar", "Undead Burg", "Undead Parish"], MapsConnectedWithoutKeys: [MapName.FirelinkShrine, MapName.DarkrootGarden], new Dictionary<int, LotType>{ { 2510, LotType.Boss } },
                [
                    new(MapName.LowerUndeadBurg, Bonfires: [], MapsConnectedWithoutKeys: [MapName.FirelinkShrine], ItemLotSeeds: [2510, 1010020]),
                    new(MapName.LowerUndeadBurgResidence, Bonfires: [], MapsConnectedWithoutKeys: [], ItemLotSeeds: [1010510]),
                    new(MapName.UndeadBurgResidence, Bonfires: [], MapsConnectedWithoutKeys:[], ItemLotSeeds: [1010460]),
                ]),
            new(MapName.FirelinkShrine, Bonfires: ["Firelink Shrine (DS1)"], MapsConnectedWithoutKeys: [MapName.NorthernUndeadAsylumExit, MapName.NewLondoRuinsValleyOfDrakes, MapName.Catacombs, MapName.UndeadBurgUndeadParish], ExtraItemLots: [],
                [
                    new(MapName.FirelinkAltar, Bonfires: ["Firelink Altar"], MapsConnectedWithoutKeys: [], ItemLotSeeds: []),
                    new(MapName.FirstLordSoul, Bonfires: [], MapsConnectedWithoutKeys:[], ItemLotSeeds: []),
                    new(MapName.SecondLordSoul, Bonfires: [], MapsConnectedWithoutKeys:[], ItemLotSeeds: []),
                    new(MapName.ThirdLordSoul, Bonfires: [], MapsConnectedWithoutKeys:[], ItemLotSeeds: []),
                ]),
            new(MapName.PaintedWorld, Bonfires: ["Painted World of Ariamis"], MapsConnectedWithoutKeys: [MapName.AnorLondo], ExtraItemLots: new Dictionary<int, LotType>{ { 2520, LotType.Boss }, { 27310000, LotType.GenericEvent } },
                [
                    new(MapName.PaintedWorldAnnex, Bonfires: [], MapsConnectedWithoutKeys: [], ItemLotSeeds: [1100060])
                ]),
            new(MapName.DarkrootGarden, Bonfires: ["Darkroot Garden"], MapsConnectedWithoutKeys: [MapName.UndeadBurgUndeadParish, MapName.NewLondoRuinsValleyOfDrakes], ExtraItemLots: new Dictionary<int, LotType>{ { 2530, LotType.Boss }, { 2540, LotType.Boss }, { 2541, LotType.Boss } },
                []),
            new(MapName.Oolacile, Bonfires: ["Oolacile Sanctuary", "Oolacile Township", "Sanctuary Garden", "Oolacile Township Dungeon", "Chasm of the Abyss"], MapsConnectedWithoutKeys: [], ExtraItemLots: new Dictionary<int, LotType>{ { 2680, LotType.Boss }, { 34720000, LotType.GenericEvent }, { 2690, LotType.Boss }, { 2700, LotType.Boss }, { 2710, LotType.Boss }, { 45110000, LotType.GenericEvent } },
                [
                    new(MapName.OolacileAfterGough, Bonfires: [], MapsConnectedWithoutKeys: [MapName.Oolacile], ItemLotSeeds: [1510, 2710])
                ]),
            new(MapName.Catacombs, Bonfires: ["Upper Catacombs", "Inner Catacombs", "Vamos"], MapsConnectedWithoutKeys: [MapName.FirelinkShrine, MapName.TombOfTheGiants], ExtraItemLots: [], 
                []),
            new(MapName.TombOfTheGiants, Bonfires: ["Upper Tomb of the Giants", "Lower Tomb of the Giants"], MapsConnectedWithoutKeys: [MapName.Catacombs], ExtraItemLots: new Dictionary<int, LotType>{ { 2560, LotType.Boss } },
                [
                    new(MapName.TombOfTheGiantsPostLordvessel, Bonfires: [], MapsConnectedWithoutKeys: [], ItemLotSeeds: [2560, 1310200])
                ]),
            new(MapName.GreatHollowAshLake, Bonfires: ["Great Hollow", "Stone Dragon", "Ash Lake"], MapsConnectedWithoutKeys: [MapName.Blighttown], ExtraItemLots: new Dictionary<int, LotType>{ { 34510000, LotType.GenericEvent } },
                []),
            new(MapName.Blighttown, Bonfires: ["Daughter of Chaos", "Lower Blighttown", "Upper Blighttown"], MapsConnectedWithoutKeys: [MapName.ValleyOfDrakes, MapName.DemonRuinsLostIzalith, MapName.GreatHollowAshLake], ExtraItemLots: new Dictionary < int, LotType > { { 2570, LotType.Boss } }, 
                []),
            new(MapName.DemonRuinsLostIzalith, Bonfires: ["Upper Demon Ruins", "Lower Demon Ruins"], MapsConnectedWithoutKeys: [MapName.Blighttown], ExtraItemLots: new Dictionary<int, LotType>{ { 2580, LotType.Boss }, { 2670, LotType.Boss }, { 22310000, LotType.Boss } },
                [
                    new(MapName.DemonRuinsLostIzalithPostLordvessel, Bonfires: ["Lost Izalith", "Demon Ruins Catacombs", "Lost Izalith Lava Pits"], MapsConnectedWithoutKeys: [MapName.Blighttown], ItemLotSeeds: [2670, 1410160])
                ]),
            new(MapName.SensFortress, Bonfires: ["Sen's Fortress"], MapsConnectedWithoutKeys: [MapName.AnorLondo], ExtraItemLots: new Dictionary<int, LotType>{ { 2590, LotType.Boss } },
                [
                    new(MapName.SensCage, Bonfires: [], MapsConnectedWithoutKeys: [], ItemLotSeeds: [1500420])
                ]),
            new(MapName.AnorLondo, Bonfires: ["Anor Londo (DS1)", "Inner Anor Londo", "Chamber of the Princess", "Darkmoon Tomb"], MapsConnectedWithoutKeys: [MapName.SensFortress], ExtraItemLots: new Dictionary<int, LotType>{ { 1090, LotType.GenericEvent }, { 2600, LotType.Boss }, { 2610, LotType.Boss }, { 2620 ,LotType.Boss } },
                []),
            new(MapName.NewLondoRuinsValleyOfDrakes, Bonfires: [], MapsConnectedWithoutKeys: [MapName.FirelinkShrine], ExtraItemLots: new Dictionary<int, LotType>{ { 1100, LotType.GenericEvent }, { 2630, LotType.Boss } },
                [
                    new(MapName.NewLondoRuinsPostSeal, Bonfires: [], MapsConnectedWithoutKeys: [MapName.NewLondoRuinsValleyOfDrakes, MapName.ValleyOfDrakes], ItemLotSeeds: [1600250]),
                    new(MapName.ValleyOfDrakes, Bonfires: ["Darkroot Basin"], MapsConnectedWithoutKeys: [MapName.DarkrootGarden, MapName.Blighttown], ItemLotSeeds: [1600170]),
                    new(MapName.FourKings, Bonfires: [], MapsConnectedWithoutKeys: [], ItemLotSeeds: [2630]),
                ]),
            new(MapName.DukesArchives, Bonfires: ["Duke's Archives Entrance"], MapsConnectedWithoutKeys: [MapName.TowerCell], ExtraItemLots: new Dictionary<int, LotType>{ { 2640, LotType.Boss }, { 52910000, LotType.GenericEvent }, { 27100200, LotType.GenericEvent } },
                [
                    new(MapName.TowerCell, Bonfires: ["Prison Tower (DS1)"], MapsConnectedWithoutKeys: [], ItemLotSeeds: [26900100]),
                    new(MapName.ArchivesTower, Bonfires: [], MapsConnectedWithoutKeys: [], ItemLotSeeds: [2020, 1700070]),
                    new(MapName.ArchivesTowerExtra, Bonfires: [], MapsConnectedWithoutKeys: [], ItemLotSeeds: [1700060]),
                    new(MapName.ArchivesTowerGiantCell, Bonfires: [], MapsConnectedWithoutKeys: [], ItemLotSeeds: [1700200]),
                    new(MapName.CrystalCave, Bonfires: ["Duke's Archives Balcony"], MapsConnectedWithoutKeys: [MapName.DukesArchives], ItemLotSeeds: [2640, 1700150]),
                ]),
            new(MapName.KilnOfTheFirstFlame, [], [], new Dictionary<int, LotType>{ { 2650, LotType.GenericEvent } /* Gwyn's soul should be a Boss drop, but since it ends the game, I've made it a GenericEvent so it isn't a valid location to place a key.*/ }, 
                []),
            new(MapName.NorthernUndeadAsylum, ["Undead Asylum Courtyard", "Undead Asylum Sewer"], [], new Dictionary<int, LotType>{ { 2660, LotType.Boss }, { 2661, LotType.Boss }, { 22300000, LotType.Boss } },
                [
                    new(MapName.DS1StartingCell, Bonfires: [], MapsConnectedWithoutKeys: [], ItemLotSeeds: [1810000]),
                    new(MapName.NorthernUndeadAsylumF2East, Bonfires: [], MapsConnectedWithoutKeys: [MapName.NorthernUndeadAsylum], ItemLotSeeds: [1810220]),
                    new(MapName.NorthernUndeadAsylumF2West, Bonfires: [], MapsConnectedWithoutKeys: [MapName.NorthernUndeadAsylum], ItemLotSeeds: [1810060]),
                    new(MapName.NorthernUndeadAsylumExit, Bonfires: [], MapsConnectedWithoutKeys: [MapName.FirelinkShrine], ItemLotSeeds: [1810070]),
                ]),
        ];

        // linked locations taken from HotPocketRemix's DarkSoulsItemRandomizer
        public static readonly List<HashSet<int>> DS1LinkedItemLots =
        [
            [1000, 6000],
            [1050, 1070, 6280],
            // Oscar dialog or death
            [1080, 6020], // Undead Asylum F2 East Key
            [1081, 2660, 6021], // Big Pilgrim's Key
            [1082, 6022], // Estus Flask

            [1300, 6170],
            [1500, 6740],
            [1510, 41100000],
            [1520, 41400000],
            [2610, 2620],
            [6190, 60001401],
            [6230, 60001100],
            [6231, 60001105],
            [6233, 60001134],
            [12000000, 12010000, 12010100, 12030000],
            [12010200, 12010300],
            [22400000, 22400100],
            [22500000, 22500200],
            [23000000, 23000100, 23000200, 23000300, 23000400, 23000500],
            [23300000, 23300100],
            [23700000, 23700100, 23700200],
            [23800000, 23800100],
            [25000000, 25000300, 25001000, 25002200, 25002500, 25003000, 25003200],
            [25000100, 25001100, 25002300, 25003100],
            [25000200, 25001200, 25002100, 25002400],
            [25400000, 25401000, 25402000],
            [25400100, 25401100, 25402100],
            [25400200, 25401200, 25402200],
            [25500000, 25502000],
            [25500100, 25501000, 25502100, 25503100, 25503200],
            [25500200, 25502200],
            [25600000, 25600300],
            [25600200, 25600400],
            [25601000, 25601300],
            [25601200, 25601400],
            [25701000, 25701200],
            [25702100, 25702200],
            [26900000, 26900200, 26900300],
            [27000000, 27000100],
            [27800000, 27801000, 27801010, 27801020, 27801030, 27802000, 27802010, 27803000, 27803100],
            [27900001, 27905001, 27907001],
            [27900101, 27905100],
            [27901001, 27903001, 27905300],
            [27902001, 27905200],
            [29000000, 29001000, 29002000, 29003000],
            [29000100, 29001100, 29002100, 29003100],
            [29000200, 29001200, 29002200, 29003200],
            [29100000, 29100200, 29101100],
            [29100100, 29101000, 29101300],
            [29300000, 29300100],
            [32500000, 32500100],
            [32700000, 32700100],
            [33000000, 33001000, 33002000, 33003000, 33004000, 33005000, 33006000, 33007000, 33007100, 33007200, 33007300],
            [33400000, 33400100, 33400200],
            [34100000, 34100100],
            [34600000, 34610000],
            [35010000, 35010100],
            [35200200, 35200500],
            [35310000, 35310100],
            [53500000, 53500100],
            [53500002, 53500101],
            [60001133, 60001501],
            [60001402, 60006215, 60006302],
            [60001403, 60006216, 60006303],
            [60001404, 60001106, 60006217, 60006304],
            [60002200, 60006502],
            [60002201, 60006503],
            [60002202, 60006504],
            [60002203, 60006505],
            [60002204, 60006506],
        ];

        public static readonly IReadOnlyList<int> DS1StartingItemLots =
        [
            1810100,
            1810110,
            1810120,
            1810130,
            1810140,
            1810150,
            1810160,
            1810170,
            1810180,
            1810190,
            1810200,
            1810210,
            1810220,
            1810230,
            1810240,
            1810250,
            1810260,
            1810270,
            1810280,
            1810290,
            1810300,
            1810310,
            1810320,
            1810330,
        ];

        public const int DS1EstusFlaskLot = 1082;

        public static readonly List<FileBackedMapDefinition> DS2MapDefinitions =
        [
            new(MapName.ThingsBetwixt, Bonfires: ["Fire Keepers' Dwelling"], MapsConnectedWithoutKeys: [MapName.Majula], ExtraItemLots: [],
                [
                    new(MapName.ThingsBetwixtPostStatue, Bonfires: [], MapsConnectedWithoutKeys: [MapName.ThingsBetwixt], ItemLotSeeds: [10026100])
                ]),
            new(MapName.Majula, Bonfires: ["The Far Fire"], MapsConnectedWithoutKeys: [MapName.ThingsBetwixt, MapName.ForestOfFallenGiants, MapName.MajulaShadedWoods, MapName.HeidesTowerOfFlame, MapName.HuntsmansCopseUndeadPurgatory, MapName.GraveOfSaints], ExtraItemLots: [],
                [
                    new(MapName.MajulaHouse, Bonfires: [], MapsConnectedWithoutKeys: [MapName.Majula], ItemLotSeeds: [10046100]),
                    new(MapName.LenigrastsHouse, Bonfires: [], MapsConnectedWithoutKeys: [], ItemLotSeeds: [10045040]),
                    new(MapName.DragonTalonRoom, Bonfires: [], MapsConnectedWithoutKeys: [], ItemLotSeeds: [10046140]),
                ]),
            new(MapName.ForestOfFallenGiants, Bonfires: ["The Crestfallen's Retreat", "Cardinal Tower"], MapsConnectedWithoutKeys: [MapName.Majula], ExtraItemLots: new Dictionary<int, LotType>{ { 309600, LotType.Boss }, { 318000, LotType.Boss }, { 1751000, LotType.GenericEvent } },
                [
                    new(MapName.ForestOfFallenGiantsPostSoldierKey, Bonfires: ["Soldier's Rest", "The Place Unbeknownst"], MapsConnectedWithoutKeys: [MapName.TheLostBastilleBelfryLuna], ItemLotSeeds: [10106070, 10106080, 10106120, 10106610, 10106010, 10106630, 10106620, 10106430, 10105120, 10106370, 318000]),
                    new(MapName.ForestOfFallenGiantsIronKeyRoom, Bonfires: [], MapsConnectedWithoutKeys: [], ItemLotSeeds: [10106460, 10106350, 10106360, 10106480, 10106470, 10105110]),
                ]),
            new(MapName.BrightstoneCoveTseldora, Bonfires: ["Royal Army Campfire", "Chapel Threshold", "Lower Brightstone Cove"], MapsConnectedWithoutKeys: [MapName.DoorsOfPharros], ExtraItemLots: new Dictionary<int, LotType>{ { 106000, LotType.Boss }, { 603000, LotType.Boss } },
                [
                    new(MapName.BrightStoneKeyRoom, Bonfires: [], MapsConnectedWithoutKeys: [], ItemLotSeeds: [10145130]),
                    new(MapName.TseldoraDen, Bonfires: [], MapsConnectedWithoutKeys: [], ItemLotSeeds: [10145070]),
                ]),
            new(MapName.AldiasKeep, Bonfires: ["Foregarden", "Ritual Site"], MapsConnectedWithoutKeys: [MapName.DragonAerieDragonShrine], ExtraItemLots: new Dictionary<int, LotType>{ { 212000, LotType.Boss }, { 60050000, LotType.GenericEvent } },
                [
                    new(MapName.AldiaSideRoom, Bonfires: [], MapsConnectedWithoutKeys: [], ItemLotSeeds: [10156040, 10156140, 1153300, 1153400, 1153500])
                ]),
            new(MapName.TheLostBastilleBelfryLuna, Bonfires: ["Exile Holding Cells", "McDuff's Workshop", "The Tower Apart", "The Saltfort"], MapsConnectedWithoutKeys: [], ExtraItemLots: new Dictionary<int, LotType>{ { 324000, LotType.Boss }, { 325000, LotType.Boss }, { 626000, LotType.Boss } },
                [
                    new(MapName.BastilleCells, Bonfires: [], MapsConnectedWithoutKeys: [], ItemLotSeeds: [10166440, 10166441, 10166330, 10166350]),
                    new(MapName.RuinSentinelBuilding, Bonfires: ["Servants' Quarters", "Upper Ramparts"], MapsConnectedWithoutKeys: [MapName.TheLostBastilleBelfryLuna], ItemLotSeeds: [10166270, 10166320, 325000, 10165210, 10166000, 10165080, 10166100, 10166150, 10166180, 10166290, 10166370, 10166020, 10166380, 10165130, 10165220, 10166160, 10165200, 10166170, 324000, 10165230, 10166250, 10166390]),
                    //new(MapName.BelfryLuna, ["Upper Ramparts"], [MapName.RuinSentinelBuilding], [10165220, 10166160, 10165200, 10166170, 324000, 10165230, 10166250, 10166390]), // I've decided to ignore Pharros Lockstones for now. If I do include them, this needs to come back.
                ]),
            new(MapName.HarvestValleyEarthenPeak, Bonfires: ["Poison Pool", "The Mines", "Lower Earthen Peak", "Central Earthen Peak", "Upper Earthen Peak"], MapsConnectedWithoutKeys: [MapName.HuntsmansCopseUndeadPurgatory, MapName.IronKeepBelfrySol], ExtraItemLots: new Dictionary<int, LotType>{ { 500000, LotType.Boss }, { 501000, LotType.Boss } },
                []),
            new(MapName.NomansWharf, Bonfires: ["Unseen Path to Heide"], MapsConnectedWithoutKeys: [MapName.HeidesTowerNomansWharf, MapName.TheLostBastilleBelfryLuna], ExtraItemLots: new Dictionary<int, LotType>{ {303300, LotType.Boss } },
                []),
            new(MapName.IronKeepBelfrySol, Bonfires: ["Threshold Bridge", "Ironhearth Hall", "Eygil's Idol", "Belfry Sol Approach"], MapsConnectedWithoutKeys: [MapName.HarvestValleyEarthenPeak], ExtraItemLots: new Dictionary<int, LotType>{ { 305000, LotType.Boss }, { 607000, LotType.Boss } },
                []),
            new(MapName.HuntsmansCopseUndeadPurgatory, Bonfires: ["Undead Refuge", "Bridge Approach", "Undead Purgatory"], MapsConnectedWithoutKeys: [MapName.Majula, MapName.HarvestValleyEarthenPeak], ExtraItemLots: new Dictionary<int, LotType>{ { 154000, LotType.Boss }, { 619100, LotType.Boss } },
                [
                    new(MapName.UndeadLockaway, Bonfires: ["Undead Lockaway"], MapsConnectedWithoutKeys: [], ItemLotSeeds: [])
                ]),
            new(MapName.TheGutterBlackGulch, Bonfires: ["Upper Gutter", "Central Gutter", "Black Gulch Mouth"], MapsConnectedWithoutKeys: [], ExtraItemLots: new Dictionary<int, LotType>{ { 326000, LotType.Boss }, { 60001000, LotType.GenericEvent } },
                [
                    new(MapName.HiddenChamber, Bonfires: ["Hidden Chamber"], MapsConnectedWithoutKeys: [MapName.TheGutterBlackGulch], ItemLotSeeds: [10256360]),
                    new(MapName.HavelArmorRoom, Bonfires: [], MapsConnectedWithoutKeys: [], ItemLotSeeds: [10256000]),
                ]),
            new(MapName.DragonAerieDragonShrine, Bonfires: ["Dragon Aerie", "Shrine Entrance"], MapsConnectedWithoutKeys: [MapName.AldiasKeep], ExtraItemLots: new Dictionary<int, LotType>{ { 600000, LotType.Boss } }, 
                []),
            new(MapName.MajulaShadedWoods, Bonfires: ["Old Akelarre"], MapsConnectedWithoutKeys: [MapName.ShadedWoodsShrineOfWinter], ExtraItemLots: [], 
                []),
            new(MapName.HeidesTowerNomansWharf, Bonfires: [], MapsConnectedWithoutKeys: [MapName.HeidesTowerOfFlame, MapName.NomansWharf], ExtraItemLots: [],
                [
                    new(MapName.FloodedPassageSideRoom, Bonfires: [], MapsConnectedWithoutKeys: [], ItemLotSeeds: [10306030, 10305010])
                ]),
            new(MapName.HeidesTowerOfFlame, Bonfires: ["Heide's Ruin", "Tower of Flame", "The Blue Cathedral"], MapsConnectedWithoutKeys: [MapName.Majula, MapName.HeidesTowerNomansWharf], ExtraItemLots: new Dictionary<int, LotType>{ { 309610, LotType.Boss }, { 625000, LotType.Boss } }, 
                []),
            new(MapName.ShadedWoodsShrineOfWinter, Bonfires: ["Ruined Fork Road", "Shaded Ruins"], MapsConnectedWithoutKeys: [MapName.DoorsOfPharros, MapName.DrangleicCastleThroneOfWant, MapName.DarkChasmOfOld], ExtraItemLots: new Dictionary<int, LotType>{ { 503000, LotType.Boss }, { 60009000, LotType.GenericEvent } },
                [
                    new(MapName.VengarlsBodyRoom, Bonfires: [], MapsConnectedWithoutKeys: [], ItemLotSeeds: [10326230]),
                    new(MapName.ChestAfterVengarl, Bonfires: [], MapsConnectedWithoutKeys: [], ItemLotSeeds: [10325120]),
                    new(MapName.LionMageSetChest, Bonfires: [], MapsConnectedWithoutKeys: [], ItemLotSeeds: [10325040]),
                    new(MapName.FangKeyLion, Bonfires: [], MapsConnectedWithoutKeys: [MapName.ShadedWoodsShrineOfWinter], ItemLotSeeds: [60009000]),
                ]),
            new(MapName.DoorsOfPharros, Bonfires: ["Gyrm's Respite"], MapsConnectedWithoutKeys: [MapName.ShadedWoodsShrineOfWinter, MapName.BrightstoneCoveTseldora], ExtraItemLots: new Dictionary<int, LotType>{ { 223500, LotType.Boss } }, 
                []),
            new(MapName.GraveOfSaints, Bonfires: ["Harval's Resting Place", "Grave Entrance"], MapsConnectedWithoutKeys: [MapName.TheGutterBlackGulch], ExtraItemLots: new Dictionary<int, LotType>{ { 226100, LotType.Boss } }, 
                []),
            new(MapName.MemoryOfVammarOrroAndJeigh, Bonfires: [], MapsConnectedWithoutKeys: [], ExtraItemLots: new Dictionary<int, LotType>{ { 309700, LotType.Boss } }, 
                []),
            new(MapName.ShrineOfAmana, Bonfires: ["Tower of Prayer (Amana)", "Crumbled Ruins", "Rhoy's Resting Place"], MapsConnectedWithoutKeys: [MapName.LookingGlassKnightArena, MapName.UndeadCrypt], ExtraItemLots: new Dictionary<int, LotType>{ { 602000, LotType.Boss } },
                [
                    new(MapName.RiseOfTheDead, Bonfires: ["Rise of the Dead"], MapsConnectedWithoutKeys: [], ItemLotSeeds: [20116110, 20115110])
                ]),
            new(MapName.DrangleicCastleThroneOfWant, Bonfires: ["King's Gate", "Forgotten Chamber", "Under Castle Drangleic", "Central Castle Drangleic"], MapsConnectedWithoutKeys: [MapName.ShadedWoodsShrineOfWinter, MapName.DarkChasmOfOld], ExtraItemLots: new Dictionary<int, LotType>{ { 332000, LotType.Boss }, { 504000, LotType.Boss }, { 611000, LotType.Boss }, {627000, LotType.Boss } },
                [
                    new(MapName.LookingGlassKnightArena, Bonfires: [], MapsConnectedWithoutKeys: [MapName.DrangleicCastleThroneOfWant, MapName.ShrineOfAmana], ItemLotSeeds: [20216080, 20216060, 20216061, 20216070, 20216120, 504000, 20215150]),
                    new(MapName.Nashandra, Bonfires: [], MapsConnectedWithoutKeys: [], ItemLotSeeds: [627000]),
                ]),
            new(MapName.UndeadCrypt, Bonfires: ["Undead Crypt Entrance", "Undead Ditch"], MapsConnectedWithoutKeys: [MapName.ShrineOfAmana, MapName.MemoryOfTheKing], ExtraItemLots: new Dictionary<int, LotType>{ { 333000, LotType.Boss } }, 
                []),
            new(MapName.DragonMemories, Bonfires: [], MapsConnectedWithoutKeys: [MapName.BrightstoneCoveTseldora], ExtraItemLots: new Dictionary<int, LotType>(), 
                []),
            new(MapName.DarkChasmOfOld, Bonfires: [], MapsConnectedWithoutKeys: [], ExtraItemLots: new Dictionary<int, LotType>{ { 506100, LotType.Boss } }, 
                []),
            new(MapName.ShulvaSanctumCity, Bonfires: ["Sanctum Walk", "Tower of Prayer (Shulva)", "Hidden Sanctum Chamber", "Lair of the Imperfect"], MapsConnectedWithoutKeys: [], ExtraItemLots: new Dictionary<int, LotType>{ { 681000, LotType.Boss }, { 682000, LotType.Boss }, {862000, LotType.Boss } },
                [
                    new(MapName.PriestessChamber, Bonfires: ["Priestess' Chamber"], MapsConnectedWithoutKeys: [MapName.ShulvaSanctumCity], ItemLotSeeds: [50355190, 50355200, 50355210, 50355220, 50355230, 50355240, 50355150, 50356610, 50356620, 50356670, 50355180, 50356390, 50355140, 862000]),
                    new(MapName.DragonSanctum, Bonfires: ["Sanctum Interior", "Sanctum Nadir"], MapsConnectedWithoutKeys: [], ItemLotSeeds: [681000, 682000, 50356450, 50356520, 50356490, 50356460, 50356470, 50356480]),
                ]),
            new(MapName.BrumeTower, Bonfires: ["Throne Floor", "Upper Floor", "Foyer", "Lowermost Floor"], MapsConnectedWithoutKeys: [], ExtraItemLots: new Dictionary<int, LotType>{ { 305010, LotType.Boss }, { 675000, LotType.Boss }, { 680000, LotType.Boss } },
                [
                    new(MapName.BrumeTowerWithOnlyTowerKey, Bonfires: [], MapsConnectedWithoutKeys: [], ItemLotSeeds: [50365030, 50366520, 50365570]),
                    new(MapName.BrumeTowerWithOnlyScorchingIronScepter, Bonfires: ["Lowermost Floor"], MapsConnectedWithoutKeys: [], ItemLotSeeds: [50365680, 50366760, 50365080, 50368010, 50368080, 675000, 50368070, 50366720, 50366850, 50366830, 50366210, 50366870, 50366860, 50366710, 50366680, 50366700, 50365650, 50365550, 50366240, 50366890, 50366880, 50367130, 50366250, 50365020, 50366530, 50366070, 50365580]),
                    new(MapName.BrumeTowerWithBothKeys, Bonfires: ["Smelter Throne", "Iron Hallway Entrance"], MapsConnectedWithoutKeys: [], ItemLotSeeds: [50366740, 50367010, 50367040, 50367050, 50366990, 50366980, 50367000, 305010, 50367060, 50366920, 50366930, 50366910, 50366940, 50366970, 50366960, 680000]),
                ]),
            new(MapName.FrozenEleumLoyce, Bonfires: ["Outer Wall", "Abandoned Dwelling", "Inner Wall", "Lower Garrison"], MapsConnectedWithoutKeys: [MapName.ShadedWoodsShrineOfWinter], ExtraItemLots: new Dictionary<int, LotType>{ { 679000, LotType.Boss }, { 679010, LotType.Boss }, { 690000, LotType.Boss } },
                [
                    new(MapName.FrozenEleumLoyceAfterAava, Bonfires: ["Grand Cathedral"], MapsConnectedWithoutKeys: [], ItemLotSeeds: [50375710, 690000, 50376760, 50376750, 50376010, 50376060, 50376310, 50376320, 50376180, 50376190, 50376660, 50375560, 50376200, 50376630, 50376690, 50376680, 50376670, 50376610, 50376620, 50376640, 50376650, 50376520, 50376420, 50376430, 50376440, 50375540, 50375520, 50375510, 50376570, 50376300, 50376580, 50376770, 50376510, 50375740, 50376400, 50375680, 50375580, 50375590, 50375600, 50375610, 50375550, 50376150, 50375690, 50375700, 50375660, 50376380, 50375670]),
                    new(MapName.ReinderValley, Bonfires: ["Expulsion Chamber"], MapsConnectedWithoutKeys: [MapName.FrozenEleumLoyce], ItemLotSeeds: [50376730, 50376210, 50376740, 50376220, 50376230, 50376460, 50376710, 50376470]),
                ]),
            new(MapName.MemoryOfTheKing, Bonfires: [], MapsConnectedWithoutKeys: [], ExtraItemLots: [],
                []),
        ];

        // Item lots that should always drop the same items. For example,
        // the Pursuer's main fight, and the one-time fight outside the
        // Cardinal Tower bonfire.
        public static readonly List<HashSet<int>> DS2LinkedItemLots =
        [
            [318000, 60008000],
        ];

        public const int DS2EstusFlaskLot = 1700000;

        public static readonly List<FileBackedMapDefinition> DS3MapDefinitions =
        [
            new(MapName.HighWallOfLothricGarden, Bonfires: ["High Wall of Lothric", "Vordt of the Boreal Valley", "Tower on the Wall"], MapsConnectedWithoutKeys: [], ExtraItemLots: new Dictionary<int, LotType>{ { 2000, LotType.Boss }, { 2010, LotType.Boss }, { 2020, LotType.Boss }, { 21500000, LotType.GenericEvent }, { 12800420, LotType.GenericEvent }, { 11901120, LotType.GenericEvent }, { 62320, LotType.GenericEvent }, { 31410000, LotType.GenericEvent }, { 3000170, LotType.GenericEvent }, { 3000650, LotType.GenericEvent }, { 3000950, LotType.GenericEvent }, { 4270, LotType.GenericEvent }, { 62300, LotType.GenericEvent }, { 62310, LotType.GenericEvent }, { 62500, LotType.GenericEvent } },
                [
                    new(MapName.DarkwraithCell, Bonfires: [], MapsConnectedWithoutKeys: [], ItemLotSeeds: [60940]),
                    new(MapName.OceirosGarden, Bonfires: ["Oceiros, the Consumed King", "Dancer of the Boreal Valley"], MapsConnectedWithoutKeys: [MapName.HighWallOfLothricGarden, MapName.UntendedGraves], ItemLotSeeds: [3000540, 3000000, 3000530, 3000480, 3000430, 3000431, 3000432, 3000433, 3000470, 3000630, 3000620, 3000510, 3000570, 3000520, 3000500, 2020, 3000840, 3000800]),
                ]),
            new(MapName.LothricCastle, Bonfires: ["Dragonslayer Armour", "Lothric Castle", "Dragon Barracks"], MapsConnectedWithoutKeys: [MapName.HighWallOfLothricGarden], ExtraItemLots: new Dictionary<int, LotType>{ { 2030, LotType.Boss }, { 2040, LotType.Boss }, { 13103000, LotType.GenericEvent }, { 21504000, LotType.GenericEvent }, { 21504010, LotType.GenericEvent }, { 31411000, LotType.GenericEvent }, { 31411100, LotType.GenericEvent }, { 4207, LotType.GenericEvent } }, 
                []),
            new(MapName.UndeadSettlement, Bonfires: ["Pit of Hollows", "Undead Settlement", "Cliff Underside", "Dilapidated Bridge", "Foot of the High Wall"], MapsConnectedWithoutKeys: [MapName.RoadOfSacrificesFarronKeep], ExtraItemLots: new Dictionary<int, LotType>{ { 2060, LotType.Boss }, { 30600000, LotType.GenericEvent }, { 13102000, LotType.GenericEvent }, { 21501000, LotType.GenericEvent }, { 21501010, LotType.GenericEvent }, { 22800000, LotType.GenericEvent }, { 4210, LotType.GenericEvent }, { 4200, LotType.GenericEvent }, { 3100630, LotType.GenericEvent }, { 60830, LotType.GenericEvent }, { 62510, LotType.GenericEvent }, { 4217, LotType.GenericEvent }, { 60900, LotType.GenericEvent }, { 60910, LotType.GenericEvent }, { 61200, LotType.GenericEvent }, { 61400, LotType.GenericEvent }, { 62100, LotType.GenericEvent }, { 63100, LotType.GenericEvent } },
                [
                    new(MapName.VelkaShrine, Bonfires: [], MapsConnectedWithoutKeys: [MapName.UndeadSettlement], ItemLotSeeds: [3100220, 3100260, 3100070, 3100340, 3100330, 3100740, 3100300])
                ]),
            new(MapName.ArchdragonPeak, Bonfires: ["Archdragon Peak", "Great Belfry", "Dragon-Kin Mausoleum", "Nameless King"], MapsConnectedWithoutKeys: [], ExtraItemLots: new Dictionary<int, LotType>{ { 2070, LotType.Boss }, { 2080, LotType.Boss }, { 31412000, LotType.Boss }, { 21509000, LotType.GenericEvent }, { 3200900, LotType.GenericEvent }, { 3200910, LotType.GenericEvent }, { 57400, LotType.GenericEvent }, { 57500, LotType.GenericEvent }, { 3200300, LotType.GenericEvent }, { 3200310, LotType.GenericEvent }, { 61000, LotType.GenericEvent } }, 
                []),
            new(MapName.RoadOfSacrificesFarronKeep, Bonfires: ["Road of Sacrifices", "Halfway Fortress", "Crucifixion Woods", "Crystal Sage", "Farron Keep", "Keep Ruins", "Farron Keep Perimeter", "Old Wolf of Farron", "Abyss Watchers"], MapsConnectedWithoutKeys: [MapName.UndeadSettlement, MapName.CathedralOfTheDeep, MapName.CatacombsCarthusSmoulderingLake], ExtraItemLots: new Dictionary<int, LotType>{ { 2090, LotType.Boss }, { 2100, LotType.Boss }, { 57800, LotType.GenericEvent }, { 57900, LotType.GenericEvent }, { 22700020, LotType.GenericEvent }, { 31000000, LotType.GenericEvent }, { 21502030, LotType.GenericEvent }, { 21502040, LotType.GenericEvent }, { 21502050, LotType.GenericEvent }, { 21502060, LotType.GenericEvent }, { 21502000, LotType.GenericEvent }, { 21502010, LotType.GenericEvent }, { 21502020, LotType.GenericEvent }, { 31200110, LotType.GenericEvent }, { 31200210, LotType.GenericEvent }, { 22702010, LotType.GenericEvent }, { 52000000, LotType.GenericEvent }, { 60710, LotType.GenericEvent }, { 3300950, LotType.GenericEvent }, { 3300960, LotType.GenericEvent }, { 3300970, LotType.GenericEvent }, { 3300980, LotType.GenericEvent }, { 4220, LotType.GenericEvent }, { 4226, LotType.GenericEvent }, { 4240, LotType.GenericEvent }, { 60700, LotType.GenericEvent }, { 60703, LotType.GenericEvent }, { 60720, LotType.GenericEvent }, { 61300, LotType.GenericEvent }, { 62600, LotType.GenericEvent } }, 
                []),
            new(MapName.GrandArchives, Bonfires: ["Grand Archives", "Twin Princes"], MapsConnectedWithoutKeys: [MapName.LothricCastle], ExtraItemLots: new Dictionary<int, LotType>{ { 13210000, LotType.GenericEvent }, { 13101000, LotType.GenericEvent }, { 21505000, LotType.GenericEvent }, { 21505010, LotType.GenericEvent }, { 21505020, LotType.GenericEvent }, { 21505030, LotType.GenericEvent }, { 21505040, LotType.GenericEvent }, { 21505050, LotType.GenericEvent }, { 21505060, LotType.GenericEvent }, { 21505070, LotType.GenericEvent }, { 12902200, LotType.GenericEvent } }, 
                []),
            new(MapName.CathedralOfTheDeep, Bonfires: ["Cathedral of the Deep", "Cleansing Chapel", "Rosaria's Bed Chamber", "Deacons of the Deep"], MapsConnectedWithoutKeys: [MapName.RoadOfSacrificesFarronKeep, MapName.PaintedWorldOfAriandel], ExtraItemLots: new Dictionary<int, LotType>{ { 2110, LotType.Boss }, { 58700, LotType.GenericEvent }, { 31001000, LotType.GenericEvent }, { 21503000, LotType.GenericEvent }, { 21503010, LotType.GenericEvent }, { 52230310, LotType.GenericEvent }, { 21800110, LotType.GenericEvent }, { 21800010, LotType.GenericEvent }, { 52020, LotType.GenericEvent }, { 3500850, LotType.GenericEvent }, { 3500860, LotType.GenericEvent }, { 3500870, LotType.GenericEvent }, { 3500880, LotType.GenericEvent }, { 3500890, LotType.GenericEvent }, { 60920, LotType.GenericEvent }, { 4260, LotType.GenericEvent }, { 4267, LotType.GenericEvent }, { 61900, LotType.GenericEvent }, { 62000, LotType.GenericEvent } }, 
                []),
            new(MapName.IrithyllAnorLondo, Bonfires: ["Irithyll of the Boreal Valley", "Central Irithyll", "Church of Yorshka", "Distant Manor", "Pontiff Sulyvahn", "Water Reserve", "Anor Londo (DS3)", "Prison Tower (DS3)", "Aldrich, Devourer of Gods"], MapsConnectedWithoutKeys: [MapName.DungeonProfanedCapital], ExtraItemLots: new Dictionary<int, LotType>{ { 2120, LotType.Boss }, { 2130, LotType.Boss }, { 58500, LotType.GenericEvent }, { 57900, LotType.GenericEvent }, { 22500000, LotType.GenericEvent }, { 22501010, LotType.GenericEvent }, { 21507000, LotType.GenericEvent }, { 21507010, LotType.GenericEvent }, { 21507020, LotType.GenericEvent }, { 21507030, LotType.GenericEvent }, { 21507040, LotType.GenericEvent }, { 12303010, LotType.GenericEvent }, { 61930, LotType.GenericEvent }, { 60930, LotType.GenericEvent }, { 3700840, LotType.GenericEvent }, { 50600, LotType.GenericEvent }, { 53000, LotType.GenericEvent }, { 4230, LotType.GenericEvent }, { 4237, LotType.GenericEvent }, { 4250, LotType.GenericEvent }, { 60300, LotType.GenericEvent }, { 60600, LotType.GenericEvent }, { 60610, LotType.GenericEvent }, { 60630, LotType.GenericEvent }, { 60805, LotType.GenericEvent }, { 62103, LotType.GenericEvent } }, 
                []),
            new(MapName.CatacombsCarthusSmoulderingLake, Bonfires: ["Catacombs of Carthus", "High Lord Wolnir", "Abandoned Tomb", "Old King's Antechamber", "Demon Ruins", "Old Demon King"], MapsConnectedWithoutKeys: [MapName.RoadOfSacrificesFarronKeep], ExtraItemLots: new Dictionary<int, LotType>{ { 2140, LotType.Boss }, { 2150, LotType.Boss }, { 58400, LotType.GenericEvent }, { 22000000, LotType.GenericEvent }, { 30601000, LotType.GenericEvent }, { 21506020, LotType.GenericEvent }, { 21506030, LotType.GenericEvent }, { 21506040, LotType.GenericEvent }, { 21506000, LotType.GenericEvent }, { 21506010, LotType.GenericEvent }, { 60400, LotType.GenericEvent }, { 61310, LotType.GenericEvent } }, 
                []),
            new(MapName.DungeonProfanedCapital, Bonfires: ["Irithyll Dungeon", "Profaned Capital", "Yhorm the Giant"], MapsConnectedWithoutKeys: [MapName.ArchdragonPeak, MapName.IrithyllAnorLondo], ExtraItemLots: new Dictionary<int, LotType>{ { 2170, LotType.Boss }, { 58600, LotType.GenericEvent }, { 20601010, LotType.GenericEvent }, { 21508000, LotType.GenericEvent }, { 21508010, LotType.GenericEvent }, { 21508020, LotType.GenericEvent }, { 21508030, LotType.GenericEvent }, { 20400020, LotType.GenericEvent }, { 3900900, LotType.GenericEvent }, { 62140, LotType.GenericEvent }, { 62105, LotType.GenericEvent }, { 62120, LotType.GenericEvent } },
                [
                    new(MapName.LedgeOutsideJailbreakersWindow, Bonfires: [], MapsConnectedWithoutKeys: [MapName.DungeonProfanedCapital], ItemLotSeeds: [3900100]),
                    new(MapName.JailCells, Bonfires: [], MapsConnectedWithoutKeys: [], ItemLotSeeds: [3900500, 3900820]),
                    new(MapName.OldCell, Bonfires: [], MapsConnectedWithoutKeys: [], ItemLotSeeds: [3900400, 62130]),
                ]),
            new(MapName.CemetaryFirelinkUntendedGraves, Bonfires: ["Firelink Shrine (DS3)", "Cemetery of Ash", "Iudex Gundyr", "Untended Graves", "Champion Gundyr"], MapsConnectedWithoutKeys: [], ExtraItemLots: new Dictionary<int, LotType>{ { 2180, LotType.Boss }, { 2190, LotType.Boss }, { 31002000, LotType.GenericEvent }, { 31004000, LotType.GenericEvent }, { 21509500, LotType.GenericEvent }, { 60200, LotType.GenericEvent }, { 52020, LotType.GenericEvent }, { 4000330, LotType.GenericEvent }, { 4000340, LotType.GenericEvent }, { 61610, LotType.GenericEvent }, { 4000300, LotType.GenericEvent }, { 60410, LotType.GenericEvent }, { 60730, LotType.GenericEvent }, { 60810, LotType.GenericEvent } },
                [
                    new(MapName.UntendedGraves, Bonfires: ["Untended Graves"], MapsConnectedWithoutKeys: [], ItemLotSeeds: [4000250, 4000220, 4000240, 4000270, 4000260, 4000310, 4000280]),
                    new(MapName.FirelinkTower, Bonfires: [], MapsConnectedWithoutKeys: [MapName.CemetaryFirelinkUntendedGraves], ItemLotSeeds: [4000190, 4000350, 4000351, 4000352, 4000170, 62010]),
                    new(MapName.FirelinkRoof, Bonfires: [], MapsConnectedWithoutKeys: [MapName.CemetaryFirelinkUntendedGraves], ItemLotSeeds: [4000160, 4000180, 4000700]),  // If not assuming tree skip, include in Firelink Tower, otherwise include in Firelink
                    new(MapName.FirstCinders, Bonfires: [], MapsConnectedWithoutKeys: [], ItemLotSeeds: []),
                    new(MapName.SecondCinders, Bonfires: [], MapsConnectedWithoutKeys: [], ItemLotSeeds: []),
                    new(MapName.ThirdCinders, Bonfires: [], MapsConnectedWithoutKeys: [], ItemLotSeeds: []),
                ]),
            new(MapName.KilnOfFlameFlamelessShrine, Bonfires: ["Flameless Shrine", "Kiln of the First Flame", "Soul of Cinder"], MapsConnectedWithoutKeys: [MapName.DregHeap], ExtraItemLots: new Dictionary<int, LotType>{ { 2200, LotType.Boss } }, 
                []),
            new(MapName.PaintedWorldOfAriandel, Bonfires: ["Snowfield", "Rope Bridge Cave", "Corvian Settlement", "Ariandel Chapel", "Sister Friede", "Depths of the Painting", "Champion's Gravetender"], MapsConnectedWithoutKeys: [], ExtraItemLots: new Dictionary<int, LotType>{ { 2300, LotType.Boss }, { 2310, LotType.Boss }, { 59200, LotType.GenericEvent }, { 21509600, LotType.GenericEvent }, { 21509620, LotType.GenericEvent }, { 21509640, LotType.GenericEvent }, { 21509650, LotType.GenericEvent }, { 21509670, LotType.GenericEvent }, { 21509680, LotType.GenericEvent }, { 21509690, LotType.GenericEvent }, { 55200, LotType.GenericEvent }, { 4700, LotType.GenericEvent }, { 55500, LotType.GenericEvent }, { 55400, LotType.GenericEvent }, { 63110, LotType.GenericEvent }, { 65500, LotType.GenericEvent } },
                [
                    new(MapName.PaintedWorldSecondHalf, Bonfires: ["Snowy Mountain Pass"], MapsConnectedWithoutKeys: [MapName.PaintedWorldOfAriandel, MapName.DregHeap], ItemLotSeeds: [4500310, 65400])
                ]),
            new(MapName.DregHeap, Bonfires: ["The Dreg Heap", "Earthen Peak Ruins", "Within the Earthen Peak Ruins", "The Deamon Prince"], MapsConnectedWithoutKeys: [], ExtraItemLots: new Dictionary<int, LotType>{ { 2330, LotType.Boss }, { 66200, LotType.GenericEvent }, { 66210, LotType.GenericEvent } }, 
                []),
            new(MapName.RingedCity, Bonfires: ["Mausoleum Lookout", "Ringed Inner Wall", "Ringed City Streets", "Shared Grave", "Church of Filianore", "Darkeater Midir"], MapsConnectedWithoutKeys: [MapName.FilianoresRest], ExtraItemLots: new Dictionary<int, LotType>{ { 2340, LotType.Boss }, { 2350, LotType.Boss }, { 59600, LotType.GenericEvent }, { 59700, LotType.GenericEvent }, { 59800, LotType.GenericEvent }, { 62600230, LotType.GenericEvent }, { 21509800, LotType.GenericEvent }, { 21509810, LotType.GenericEvent }, { 21509840, LotType.GenericEvent }, { 21509860, LotType.GenericEvent }, { 62800110, LotType.GenericEvent }, { 62800010, LotType.GenericEvent }, { 62800210, LotType.GenericEvent }, { 5100670, LotType.GenericEvent }, { 5100900, LotType.GenericEvent }, { 5100910, LotType.GenericEvent }, { 5100920, LotType.GenericEvent }, { 66230, LotType.GenericEvent }, { 66220, LotType.GenericEvent }, { 66300, LotType.GenericEvent }, { 66310, LotType.GenericEvent } }, 
                []),
            new(MapName.FilianoresRest, Bonfires: ["Filianore's Rest", "Slave Knight Gael"], MapsConnectedWithoutKeys: [], ExtraItemLots: new Dictionary<int, LotType>{ { 2360, LotType.Boss }, { 62600240, LotType.GenericEvent } }, 
                []),
        ];

        public static readonly List<HashSet<int>> DS3LinkedItemLots =
        [
            [52300, 62300],
            [52302, 62320],
            [50902, 60910]
        ];

        public const int DS3AshenEstusFlaskLot = 4000505;
    }
}
