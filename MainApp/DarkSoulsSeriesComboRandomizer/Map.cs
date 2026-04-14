namespace DarkSoulsSeriesComboRandomizer
{
    public class Map
    {
        public readonly string FriendlyName;
        public readonly string FileName;
        public readonly SoulsGame SourceGame;
        public readonly IReadOnlyList<string> Bonfires;
        public List<IItemLot> ItemLocations = new();
        public List<Map> connectedMaps = new();

        public static IReadOnlyList<Map> DSRMaps { get; } = ParseMapDefinitions(new List<(string FriendlyName, string FileName, SoulsGame SourceGame, List<string> Bonfires, List<string> ConnectedMapNames)>
        {
            ("DS1 Starting Cell", "m00_00_00_00", SoulsGame.DSR, new List<string>{ }, new List<string> { }),
            ("Depths", "m10_00_00_00", SoulsGame.DSR, new List<string> { }, new List<string> { }),
            ("Sewer Chamber", "m00_00_00_00", SoulsGame.DSR, new List<string> { "Depths" }, new List<string> { }),
            ("Undead Burg / Undead Parish", "m10_01_00_00", SoulsGame.DSR, new List<string>{ "Sunlight Altar", "Undead Burg", "Undead Parish" }, new List<string>{ "Firelink Shrine", "Darkroot Garden" }),
            ("Undead Burg Residence", "m00_00_00_00", SoulsGame.DSR, new List<string> { }, new List < string > { }),
            ("Lower Undead Burg", "m00_00_00_00", SoulsGame.DSR, new List<string>{}, new List<string>{ "Firelink Shrine" }),
            ("Lower Undead Burg Residence", "m00_00_00_00", SoulsGame.DSR, new List<string>{ }, new List<string>{ }),
            ("Firelink Shrine", "m10_02_00_00", SoulsGame.DSR, new List<string>{ "Firelink Shrine" }, new List<string>{ "Northern Undead Asylum", "New Londo Ruins / Valley of Drakes", "Catacombs", "Undead Burg / Undead Parish" }),
            ("Painted World", "m11_00_00_00", SoulsGame.DSR, new List<string>{ "Painted World of Ariamis" }, new List<string>{ "Anor Londo" }),
            ("Painted World Annex", "m00_00_00_00", SoulsGame.DSR, new List<string>{ }, new List<string>{ }),
            ("Darkroot Garden", "m12_00_00_01", SoulsGame.DSR, new List<string>{ "Darkroot Garden" }, new List<string>{ "Undead Burg / Undead Parish", "New Londo Ruins / Valley of Drakes" }),
            ("Oolacile", "m12_01_00_00", SoulsGame.DSR, new List<string>{ "Oolacile Sanctuary", "Oolacile Township", "Sanctuary Garden", "Oolacile Township Dungeon", "Chasm of the Abyss" }, new List<string>{ }),
            ("Oolacile After Gough", "m00_00_00_00", SoulsGame.DSR, new List<string>{ }, new List<string>{ "Oolacile" }),
            ("Catacombs", "m13_00_00_00", SoulsGame.DSR, new List<string>{ "Upper Catacombs", "Inner Catacombs", "Vamos" }, new List<string>{ "Firelink Shrine", "Tomb of the Giants" }),
            ("Tomb of the Giants", "m13_01_00_00", SoulsGame.DSR, new List<string>{ "Upper Tomb of the Giants", "Lower Tomb of the Giants" }, new List<string>{ "Catacombs" }),
            ("Tomb of the Giants Post Lordvessel", "m00_00_00_00", SoulsGame.DSR, new List<string>{ }, new List<string>{ }),
            ("Great Hollow / Ash Lake", "m13_02_00_00", SoulsGame.DSR, new List<string>{ "Great Hollow", "Stone Dragon", "Ash Lake" }, new List<string>{ "Blighttown" }),
            ("Blighttown", "m14_00_00_00", SoulsGame.DSR, new List<string>{ "Daughter of Chaos", "Lower Blighttown", "Upper Blighttown" }, new List<string>{ "Valley of Drakes", "Demon Ruins / Lost Izalith", "Great Hollow / Ash Lake" }),
            ("Demon Ruins / Lost Izalith", "m14_01_00_00", SoulsGame.DSR, new List<string>{ "Upper Demon Ruins", "Lower Demon Ruins" }, new List<string>{ "Blighttown" }),
            ("Demon Ruins / Lost Izalith Post Lordvessel", "m00_00_00_00", SoulsGame.DSR, new List<string>{ "Lost Izalith", "Demon Ruins Catacombs", "Lost Izalith Lava Pits" }, new List<string>{ "Blighttown" }),
            ("Sen's Fortress", "m15_00_00_00", SoulsGame.DSR, new List<string>{ "Sen's Fortress" }, new List<string>{ "Anor Londo" }),
            ("Sen's Cage", "m00_00_00_00", SoulsGame.DSR, new List<string>{ }, new List<string>{ }),
            ("Anor Londo", "m15_01_00_00", SoulsGame.DSR, new List<string>{ "Anor Londo", "Inner Anor Londo", "Chamber of the Princess", "Darkmoon Tomb" }, new List<string>{ "Sen's Fortress" }),
            ("New Londo Ruins / Valley of Drakes", "m16_00_00_00", SoulsGame.DSR, new List<string>{ }, new List<string>{ "Firelink Shrine" }),
            ("New Londo Ruins Post Seal", "m00_00_00_00", SoulsGame.DSR, new List<string>{ }, new List<string>{ "New Londo Ruins / Valley of Drakes", "Valley of Drakes" }),
            ("Valley of Drakes", "m00_00_00_00", SoulsGame.DSR, new List<string>{ "Darkroot Basin" }, new List<string>{ "Darkroot Garden", "Blighttown"}),
            ("Four Kings", "m00_00_00_00", SoulsGame.DSR, new List<string>{ }, new List<string>{ }),
            ("Duke's Archives", "m17_00_00_00", SoulsGame.DSR, new List<string>{ "Duke's Archives Entrance" }, new List<string>{ "Tower Cell" }),
            ("Tower Cell", "m00_00_00_00", SoulsGame.DSR, new List<string>{ "Prison Tower" }, new List<string>{ }),
            ("Archives Tower", "m00_00_00_00", SoulsGame.DSR, new List<string>{ }, new List<string>{ }),
            ("Archives Tower Extra", "m00_00_00_00", SoulsGame.DSR, new List<string>{ }, new List<string>{ }),
            ("Archives Tower Giant Cell", "m00_00_00_00", SoulsGame.DSR, new List<string>{ }, new List<string>{ }),
            ("Crystal Cave", "m00_00_00_00", SoulsGame.DSR, new List<string>{ "Duke's Archives Balcony" }, new List<string>{ "Duke's Archives" }),
            ("Firelink Altar", "m00_00_00_00", SoulsGame.DSR, new List<string>{ "Firelink Altar" }, new List<string>{ }),
            ("First Lord Soul", "m00_00_00_00", SoulsGame.DSR, new List<string>{ }, new List<string>{ }),
            ("Second Lord Soul", "m00_00_00_00", SoulsGame.DSR, new List<string>{ }, new List<string>{ }),
            ("Third Lord Soul", "m00_00_00_00", SoulsGame.DSR, new List<string>{ }, new List<string>{ }),
            ("Fourth Lord Soul", "m00_00_00_00", SoulsGame.DSR, new List<string>{ }, new List<string>{ }),
            ("Kiln of the First Flame", "m18_00_00_00", SoulsGame.DSR, new List<string>{ }, new List<string>{ }),
            ("Northern Undead Asylum", "m18_01_00_00", SoulsGame.DSR, new List<string>{ "Undead Asylum Courtyard", "Undead Asylum Sewer" }, new List<string>{ }),
            ("Northern Undead Asylum F2 East", "m00_00_00_00", SoulsGame.DSR, new List<string>{ }, new List<string>{ "Northern Undead Asylum" }),
            ("Northern Undead Asylum F2 West", "m00_00_00_00", SoulsGame.DSR, new List<string>{ }, new List<string>{ "Northern Undead Asylum" }),
        });

        public static Dictionary<string, HashSet<int>> DS1NonDefaultMapItemLots = new()
        {
            { "DS1 Starting Cell", new HashSet<int>{ 1810000 } },
            { "Lower Undead Burg", new HashSet<int>{ 2510, 1010020, 1010370, 1010380, 1010381, 1010382, 1010383, 1010384, 1010430, 1010490, 60001200, 60001201, 60001202, 60001203, 60001204, 60001205, 60001206, 60001207, 60001208, 60001209, 60001210, 60001211, 60001212, 60001213, 60001214, 60001215, 60001216, 60001217, 60001218, 60001219, 60001220 } },
            { "Lower Undead Burg Residence", new HashSet<int>{ 1010510, 1010511, 1010512, 1010513, 1010514 } },
            { "Undead Burg Residence", new HashSet<int>{ 1010460 } },
            { "Painted World Annex", new HashSet<int>{ 1100060, 1100061, 1100062, 1100063, 1100090, 1100100, 1100320, 1100340, 1100370 } },
            { "Oolacile After Gough", new HashSet<int>{ 1510, 2710, 1210250, 41100001, 45100000, 45110000, 60006600, 60006601, 60006602, 60006603, 60006604, 60006608, 60006609, 60006610, 60006611 } },
            { "Tomb of the Giants Post Lordvessel", new HashSet<int>{ 2560, 1310200, 1310220, 1310230, 1310240, 1310241, 1310242, 1310243, 1310290, 52200000 } },
            { "Demon Ruins / Lost Izalith Post Lordvessel", new HashSet<int>{ 2670, 1410160, 1410230, 1410250, 1410270, 22310000, 2580, 6620, 1410000, 1410010, 1410020, 1410030, 1410310, 1410320, 1410330, 1410340, 1410360, 1410380, 1410390, 1410400, 1410410, 1410500, 1410520, 34800100, 52300000, 52400000, 54000000, 54010000 } },
            { "Sen's Cage", new HashSet<int>{ 1500420 } },
            { "New Londo Ruins Post Seal", new HashSet<int>{ 1600250, 1600260, 1600270, 1600280, 1600290, 1600310, 1600330, 1600360, 1600361, 1600362, 1600363, 1600364, 1600370, 1600500, 1600510 } },
            { "Valley of Drakes", new HashSet<int>{ 1600170, 1600180, 1600190, 1600200, 1600210, 1600220, 1600221, 1600222, 1600223, 1600224, 1600380, 34200200 } },
            { "Four Kings", new HashSet<int>{ 2630 } },
            { "Tower Cell", new HashSet<int>{ 26900100 } },
            { "Archives Tower", new HashSet<int>{ 2020, 1700070, 1700071, 1700072, 1700073, 1700074, 1700210, 1700630, 33300100, 33300200 } },
            { "Archives Tower Extra", new HashSet<int>{ 1700060, 1700080 } },
            { "Archives Tower Giant Cell", new HashSet<int>{ 1700200 } },
            { "Crystal Cave", new HashSet<int>{ 2640, 1700150, 1700160, 1700170, 1700180, 52900100, 52910000 } },
            { "Northern Undead Asylum F2 East", new HashSet<int>{ 1810220, 1810221, 1810250, 1810280, 1810310 } },
            { "Northern Undead Asylum F2 West", new HashSet<int>{ 1810060 } },
            { "Firelink Shrine", new HashSet<int>{ 1810070 } } // Technically not in Firelink, this is the item on the left as you leave the Asylum. I didn't feel like making another zone for it.
        };

        public static IReadOnlyList<Map> DS2Maps { get; } = ParseMapDefinitions(new List<(string FriendlyName, string FileName, SoulsGame SourceGame, List<string> Bonfires, List<string> ConnectedMapNames)>
        {
            ("Things Betwixt", "m10_02_00_00", SoulsGame.DS2S, new List<string>{ "Fire Keepers' Dwelling" }, new List<string>{ "Majula" }),
            ("Things Betwixt Post Statue", "m00_00_00_00", SoulsGame.DS2S, new List<string>{ }, new List<string>{ "Things Betwixt" }),
            ("Majula", "m10_04_00_00", SoulsGame.DS2S, new List<string>{ "The Far Fire" }, new List<string>{ "Things Betwixt", "Forest of Fallen Giants", "Majula <-> Shaded Woods", "Heide's Tower of Flame", "Huntsman's Copse & Undead Purgatory", "Grave of Saints" }),
            ("Majula House", "m00_00_00_00", SoulsGame.DS2S, new List<string>{ }, new List<string>{ "Majula" }),
            ("Lenigrast's House", "m00_00_00_00", SoulsGame.DS2S, new List<string>{ }, new List<string>{ }),
            ("Dragon Talon Room", "m00_00_00_00", SoulsGame.DS2S, new List<string>{ }, new List<string>{ }),
            ("Forest of Fallen Giants", "m10_10_00_00", SoulsGame.DS2S, new List<string>{ "The Crestfallen's Retreat", "Cardinal Tower" }, new List<string>{ "Majula" }),
            ("Forest of Fallen Giants Post Soldier Key", "m00_00_00_00", SoulsGame.DS2S, new List<string>{ "Soldier's Rest", "The Place Unbeknownst" }, new List<string>{ "The Lost Bastille & Belfry Luna" }),
            ("Forest of Fallen Giants Iron Key Room", "m00_00_00_00", SoulsGame.DS2S, new List<string>{ }, new List<string>{ }),
            ("Brightstone Cove Tseldora", "m10_14_00_00", SoulsGame.DS2S, new List<string>{ "Royal Army Campfire", "Chapel Threshold", "Lower Brightstone Cove" }, new List<string>{ "Doors of Pharros" }),
            ("Brightstone Key Room", "m00_00_00_00", SoulsGame.DS2S, new List<string>{ }, new List<string>{ }),
            ("Tseldora Den", "m00_00_00_00", SoulsGame.DS2S, new List<string>{ }, new List<string>{  }),
            ("Aldia's Keep", "m10_15_00_00", SoulsGame.DS2S, new List<string>{ "Foregarden", "Ritual Site" }, new List<string>{ "Dragon Aerie & Dragon Shrine" }),
            ("Aldia Side Room", "m00_00_00_00", SoulsGame.DS2S, new List<string>{ }, new List<string>{ }),
            ("The Lost Bastille & Belfry Luna", "m10_16_00_00", SoulsGame.DS2S, new List<string>{ "Exile Holding Cells", "McDuff's Workshop", "The Tower Apart", "The Saltfort" }, new List<string>{ }),
            ("Bastille Cells", "m00_00_00_00", SoulsGame.DS2S, new List<string>{ }, new List<string>{ }),
            ("Straid's Cell", "m00_00_00_00", SoulsGame.DS2S, new List<string>{ "Straid's Cell" }, new List<string>{ }),
            ("Ruin Sentinel Building", "m00_00_00_00", SoulsGame.DS2S, new List<string>{ "Servants' Quarters", "Upper Ramparts" }, new List<string>{ "The Lost Bastille & Belfry Luna" }),
            //("Belfry Luna", "m00_00_00_00", SoulsGame.DS2S, new List<string> { "Upper Ramparts" }, new List<string>{ "Ruin Sentinel Building" }),
            ("Harvest Valley & Earthen Peak", "m10_17_00_00", SoulsGame.DS2S, new List<string>{ "Poison Pool", "The Mines", "Lower Earthen Peak", "Central Earthen Peak", "Upper Earthen Peak" }, new List<string>{ "Huntsman's Copse & Undead Purgatory", "Iron Keep & Belfry Sol" }),
            ("No-man's Wharf", "m10_18_00_00", SoulsGame.DS2S, new List<string>{ "Unseen Path to Heide" }, new List<string>{ "Heide's Tower <-> No-man's Wharf", "The Lost Bastille & Belfry Luna" }),
            ("Iron Keep & Belfry Sol", "m10_19_00_00", SoulsGame.DS2S, new List<string>{ "Threshold Bridge", "Ironhearth Hall", "Eygil's Idol", "Belfry Sol Approach" }, new List<string>{ "Harvest Valley & Earthen Peak" }),
            ("Huntsman's Copse & Undead Purgatory", "m10_23_00_00", SoulsGame.DS2S, new List<string>{ "Undead Refuge", "Bridge Approach", "Undead Purgatory" }, new List<string>{ "Majula", "Harvest Valley & Earthen Peak" }),
            ("Undead Lockaway", "m00_00_00_00", SoulsGame.DS2S, new List<string>{ "Undead Lockaway" }, new List<string>{ }),
            ("The Gutter & Black Gulch", "m10_25_00_00", SoulsGame.DS2S, new List<string>{ "Upper Gutter", "Central Gutter", "Black Gulch Mouth" }, new List<string>{ }),
            ("Hidden Chamber", "m00_00_00_00", SoulsGame.DS2S, new List<string>{ "Hidden Chamber" }, new List<string>{ "The Gutter & Black Gulch" }),
            ("Havel Armor Room", "m00_00_00_00", SoulsGame.DS2S, new List<string>{ }, new List<string>{ }),
            ("Dragon Aerie & Dragon Shrine", "m10_27_00_00", SoulsGame.DS2S, new List<string>{ "Dragon Aerie", "Shrine Entrance" }, new List<string>{ "Aldia's Keep" }),
            ("Majula <-> Shaded Woods", "m10_29_00_00", SoulsGame.DS2S, new List<string>{ "Old Akelarre" }, new List<string>{ "Shaded Woods & Shrine of Winter" }),
            ("Heide's Tower <-> No-man's Wharf", "m10_30_00_00", SoulsGame.DS2S, new List<string>{ }, new List<string>{ "Heide's Tower of Flame", "No-man's Wharf" }),
            ("Flooded Passage Side Room", "m00_00_00_00", SoulsGame.DS2S, new List<string>{ }, new List<string>{ }),
            ("Heide's Tower of Flame", "m10_31_00_00", SoulsGame.DS2S, new List<string>{ "Heide's Ruin", "Tower of Flame", "The Blue Cathedral" }, new List<string>{ "Majula", "Heide's Tower <-> No-man's Wharf" }),
            ("Shaded Woods & Shrine of Winter", "m10_32_00_00", SoulsGame.DS2S, new List<string>{ "Ruined Fork Road", "Shaded Ruins" }, new List<string>{ "Doors of Pharros", "Drangleic Castle & Throne of Want", "Dark Chasm of Old" }),
            ("Vengarl's Body Room", "m00_00_00_00", SoulsGame.DS2S, new List<string>{ }, new List<string>{ }),
            ("Chest after Vengarl", "m00_00_00_00", SoulsGame.DS2S, new List<string>{ }, new List<string>{ }),
            ("Lion Mage Set Chest", "m00_00_00_00", SoulsGame.DS2S, new List<string>{ }, new List<string>{ }),
            ("Fang Key Lion", "m00_00_00_00", SoulsGame.DS2S, new List<string>{ }, new List<string>{ "Shaded Woods & Shrine of Winter" }),
            ("Doors of Pharros", "m10_33_00_00", SoulsGame.DS2S, new List<string>{ "Gyrm's Respite" }, new List<string>{ "Shaded Woods & Shrine of Winter", "Brightstone Cove Tseldora" }),
            ("Grave of Saints", "m10_34_00_00", SoulsGame.DS2S, new List<string>{ "Harval's Resting Place", "Grave Entrance" }, new List<string>{ "The Gutter & Black Gulch" }),
            ("Memory of Vammar, Orro, and Jeigh", "m20_10_00_00", SoulsGame.DS2S, new List<string>{ }, new List<string>{ }),
            ("Shrine of Amana", "m20_11_00_00", SoulsGame.DS2S, new List<string>{ "Tower of Prayer (Amana)", "Crumbled Ruins", "Rhoy's Resting Place" }, new List<string>{ "Looking Glass Knight Arena", "Undead Crypt" }),
            ("Rise of the Dead", "m00_00_00_00", SoulsGame.DS2S, new List<string>{ "Rise of the Dead" }, new List<string>{ }),
            ("Drangleic Castle & Throne of Want", "m20_21_00_00", SoulsGame.DS2S, new List<string>{ "King's Gate", "Forgotten Chamber", "Under Castle Drangleic", "Central Castle Drangleic" }, new List<string>{ "Shaded Woods & Shrine of Winter", "Dark Chasm of Old" }),
            ("Nashandra", "m00_00_00_00", SoulsGame.DS2S, new List<string>{ }, new List<string>{ }),
            ("Looking Glass Knight Arena", "m00_00_00_00", SoulsGame.DS2S, new List<string>{ }, new List<string>{ "Drangleic Castle & Throne of Want", "Shrine of Amana" }),
            ("Undead Crypt", "m20_24_00_00", SoulsGame.DS2S, new List<string>{ "Undead Crypt Entrance", "Undead Ditch" }, new List<string>{ "Shrine of Amana", "Memory of the King" }),
            ("Dragon Memories", "m20_26_00_00", SoulsGame.DS2S, new List<string>{ }, new List<string>{ "Brightstone Cove Tseldora" }),
            ("Dark Chasm of Old", "m40_03_00_00", SoulsGame.DS2S, new List<string>{ }, new List<string>{ }),
            ("Shulva, Sanctum City", "m50_35_00_00", SoulsGame.DS2S, new List<string>{ "Sanctum Walk", "Tower of Prayer (Shulva)", "Hidden Sanctum Chamber", "Lair of the Imperfect" }, new List<string>{ }),
            ("Priestess' Chamber", "m00_00_00_00", SoulsGame.DS2S, new List<string>{ "Priestess' Chamber" }, new List<string>{ "Shulva, Sanctum City" }),
            ("Dragon Sanctum", "m00_00_00_00", SoulsGame.DS2S, new List<string>{ "Sanctum Interior", "Sanctum Nadir" }, new List<string>{ }),
            ("Brume Tower", "m50_36_00_00", SoulsGame.DS2S, new List<string>{ "Throne Floor", "Upper Floor", "Foyer", "Lowermost Floor" }, new List<string>{ }),
            ("Brume Tower With Only Tower Key", "m00_00_00_00", SoulsGame.DS2S, new List<string>{ }, new List<string>{ }),
            ("Brume Tower With Only Scorching Iron Scepter", "m00_00_00_00", SoulsGame.DS2S, new List<string>{ "Lowermost Floor" }, new List<string>{ }),
            ("Brume Tower With Both Keys", "m00_00_00_00", SoulsGame.DS2S, new List<string>{ "Smelter Throne", "Iron Hallway Entrance" }, new List<string>{ }), // Technically connected to both single-key zones, but for simplicity I picked this one
            ("Frozen Eleum Loyce", "m50_37_00_00", SoulsGame.DS2S, new List<string>{ "Outer Wall", "Abandoned Dwelling", "Inner Wall", "Lower Garrison" }, new List<string>{ "Shaded Woods & Shrine of Winter" }),
            ("Frozen Eleum Loyce After Aava", "m00_00_00_00", SoulsGame.DS2S, new List<string>{ "Grand Cathedral" }, new List<string>{ }),
            ("Reindeer Valley", "m00_00_00_00", SoulsGame.DS2S, new List<string>{ "Expulsion Chamber" }, new List<string>{ "Frozen Eleum Loyce" }),
            ("Memory of the King", "m50_38_00_00", SoulsGame.DS2S, new List<string>{ }, new List<string>{ }),
        });

        public static Dictionary<string, HashSet<int>> DS2NonDefaultMapItemLots = new()
        {
            { "Things Betwixt Post Statue", new HashSet<int>{ 10026100 } },
            { "Forest of Fallen Giants Post Soldier Key", new HashSet<int>{ 10106070, 10106080, 10106120, 10106610, 10106010, 10106630, 10106620, 10106430, 10105120, 10106370, 10106370, 318000 } },
            { "Forest of Fallen Giants Iron Key Room", new HashSet<int>{ 10106460, 10106350, 10106360, 10106480, 10106470, 10105110 } },
            { "Looking Glass Knight Arena", new HashSet<int>{ 20216080, 20216060, 20216061, 20216070, 20216120, 504000, 20215150 } },
            { "Bastille Cells", new HashSet<int>{ 10166440, 10166441, 10166330, 10166350 } },
            { "Ruin Sentinel Building", new HashSet<int>{ 10166270, 10166320, 325000, 10165210, 10166000, 10165080, 10166100, 10166150, 10166180, 10166290, 10166370, 10166020, 10166380, 10165130, 10165220, 10166160, 10165200, 10166170, 324000, 10165230, 10166250, 10166390 } },
            //{ "Belfry Luna", new HashSet<int>{ 10165220, 10166160, 10165200, 10166170, 324000, 10165230, 10166250, 10166390 } }, // I've decided to ignore Pharros Lockstones for now. If I do include them, this needs to come back.
            { "Majula House", new HashSet<int>{ 10046100, 10045010, 10045510 } },
            { "Lenigrast's House", new HashSet<int>{ 10045040 } },
            { "Dragon Talon Room", new HashSet<int>{ 10046140, 10045020, 10045050, 10045030 } },
            { "Hidden Chamber", new HashSet<int>{ 10256360 } },
            { "Havel Armor Room", new HashSet<int>{ 10256000 } },
            { "Brightstone Key Room", new HashSet<int>{ 10145130 } },
            { "Tseldora Den", new HashSet<int>{ 10145070 } },
            { "Aldia Side Room", new HashSet<int>{ 10156040, 10156140, 1153300, 1153400, 1153500 } },
            { "Flooded Passage Side Room", new HashSet<int>{ 10306030, 10305010 } },
            { "Vengarl's Body Room", new HashSet<int>{ 10326230 } },
            { "Chest after Vengarl", new HashSet<int>{ 10325120 } },
            { "Lion Mage Set Chest", new HashSet<int>{ 10325040 } },
            { "Fang Key Lion", new HashSet<int>{ 60009000 } },
            { "Rise of the Dead", new HashSet<int> { 20116110, 20115110 } },
            { "Nashandra", new HashSet<int>{ 627000 } },
            { "Priestess' Chamber", new HashSet<int>{ 50355190, 50355200, 50355210, 50355220, 50355230, 50355240, 50355150, 50356610, 50356620, 50356670, 50355180, 50356390, 50355140, 862000 } },
            { "Dragon Sanctum", new HashSet<int>{ 681000, 682000, 50356450, 50356520, 50356490, 50356460, 50356470, 50356480 } },
            { "Brume Tower With Only Tower Key", new HashSet<int>{ 50365030, 50366520, 50365570 } },
            { "Brume Tower With Only Scorching Iron Scepter", new HashSet<int>{ 50365680, 50366760, 50365080, 50368010, 50368080, 675000, 50368070, 50366720, 50366850, 50366830, 50366210, 50366870, 50366860, 50366710, 50366680, 50366700, 50365650, 50365550, 50366240, 50366890, 50366880, 50367130, 50366250, 50365020, 50366530, 50366070, 50365580 } },
            { "Brume Tower With Both Keys", new HashSet<int>{ 50366740, 50367010, 50367040, 50367050, 50366990, 50366980, 50367000, 305010, 50367060, 50366920, 50366930, 50366910, 50366940, 50366970, 50366960, 680000 } },
            { "Frozen Eleum Loyce After Aava", new HashSet<int>{ 50375710, 690000, 50376760, 50376750, 50376010, 50376060, 50376310, 50376320, 50376180, 50376190, 50376660, 50375560, 50376200, 50376630, 50376690, 50376680, 50376670, 50376610, 50376620, 50376640, 50376650, 50376520, 50376420, 50376430, 50376440, 50375540, 50375520, 50375510, 50376570, 50376300, 50376580, 50376770, 50376510, 50375740, 50376400, 50375680, 50375580, 50375590, 50375600, 50375610, 50375550, 50376150, 50375690, 50375700, 50375660, 50376380, 50375670 } },
            { "Reindeer Valley", new HashSet<int>{ 50376730, 50376210, 50376740, 50376220, 50376230, 50376460, 50376710, 50376470 } },
        };

        public static IReadOnlyList<Map> DS3Maps { get; } = ParseMapDefinitions(new List<(string FriendlyName, string FileName, SoulsGame SourceGame, List<string> Bonfires, List<string> ConnectedMapNames)>
        {
            ("High Wall of Lothric / Garden", "m30_00_00_00", SoulsGame.DS3, new List<string>{ "High Wall of Lothric", "Vordt of the Boreal Valley", "Tower on the Wall" }, new List<string>{ }),
            ("Oceiros' Garden", "m00_00_00_00", SoulsGame.DS3, new List<string>{ "Oceiros, the Consumed King", "Dancer of the Boreal Valley" }, new List<string>{ "High Wall of Lothric / Garden", "Untended Graves" }),
            ("Darkwraith Cell", "m00_00_00_00", SoulsGame.DS3, new List<string>{ }, new List<string>{ }),
            ("Lothric Castle", "m30_01_00_00", SoulsGame.DS3, new List<string>{ "Dragonslayer Armour", "Lothric Castle", "Dragon Barracks" }, new List<string>{ "High Wall of Lothric / Garden" }),
            ("Undead Settlement", "m31_00_00_00", SoulsGame.DS3, new List<string>{ "Pit of Hollows", "Undead Settlement", "Cliff Underside", "Dilapidated Bridge", "Foot of the High Wall" }, new List<string>{ "Road of Sacrifices / Farron Keep"}),
            ("Velka Shrine", "m00_00_00_00", SoulsGame.DS3, new List<string>{ }, new List<string>{ "Undead Settlement" }),
            ("Archdragon Peak", "m32_00_00_00", SoulsGame.DS3, new List<string>{ "Archdragon Peak", "Great Belfry", "Dragon-Kin Mausoleum", "Nameless King" }, new List<string>{ }),
            ("Road of Sacrifices / Farron Keep", "m33_00_00_00", SoulsGame.DS3, new List<string>{ "Road of Sacrifices", "Halfway Fortress", "Crucifixion Woods", "Crystal Sage", "Farron Keep", "Keep Ruins", "Farron Keep Perimeter", "Old Wolf of Farron", "Abyss Watchers" }, new List<string>{ "Undead Settlement", "Cathedral of the Deep", "Catacombs Carthus / Smouldering Lake" }),
            ("Grand Archives", "m34_01_00_00", SoulsGame.DS3, new List<string>{ "Grand Archives", "Twin Princes" }, new List<string>{ "Lothric Castle" }),
            ("Cathedral of the Deep", "m35_00_00_00", SoulsGame.DS3, new List<string>{ "Cathedral of the Deep", "Cleansing Chapel", "Rosaria's Bed Chamber", "Deacons of the Deep" }, new List<string>{ "Road of Sacrifices / Farron Keep", "Painted World of Ariandel" }),
            ("Irithyll / Anor Londo", "m37_00_00_00", SoulsGame.DS3, new List<string>{ "Irithyll of the Boreal Valley", "Central Irithyll", "Church of Yorshka", "Distant Manor", "Pontiff Sulyvahn", "Water Reserve", "Anor Londo", "Prison Tower", "Aldrich, Devourer of Gods" }, new List<string>{ "Dungeon / Profaned Capital" }),
            ("Catacombs Carthus / Smouldering Lake", "m38_00_00_00", SoulsGame.DS3, new List<string>{ "Catacombs of Carthus", "High Lord Wolnir", "Abandoned Tomb", "Old King's Antechamber", "Demon Ruins", "Old Demon King" }, new List<string>{ "Road of Sacrifices / Farron Keep" }),
            ("Dungeon / Profaned Capital", "m39_00_00_00", SoulsGame.DS3, new List<string>{ "Irithyll Dungeon", "Profaned Capital", "Yhorm the Giant" }, new List<string>{ "Archdragon Peak", "Irithyll / Anor Londo" }),
            ("Ledge Outside Jailbreaker's Window", "m00_00_00_00", SoulsGame.DS3, new List<string> { }, new List<string>{ "Dungeon / Profaned Capital" }),
            ("Jail Cells", "m00_00_00_00", SoulsGame.DS3, new List<string>{ }, new List<string>{ }),
            ("Old Cell", "m00_00_00_00", SoulsGame.DS3, new List<string>{ }, new List<string>{ }),
            ("Cemetery / Firelink / Untended Graves", "m40_00_00_00", SoulsGame.DS3, new List<string>{ "Firelink Shrine", "Cemetery of Ash", "Iudex Gundyr", "Untended Graves", "Champion Gundyr" }, new List<string>{ "High Wall of Lothric / Garden" }),
            ("Untended Graves", "m00_00_00_00", SoulsGame.DS3, new List<string>{ "Untended Graves" }, new List<string>{ }),
            ("Firelink Tower", "m00_00_00_00", SoulsGame.DS3, new List<string>{ }, new List<string>{ "Cemetery / Firelink / Untended Graves" }),
            ("Firelink Roof", "m00_00_00_00", SoulsGame.DS3, new List<string>{ }, new List<string>{ "Cemetery / Firelink / Untended Graves" }),
            ("First Cinders", "m00_00_00_00", SoulsGame.DS3, new List<string>{ }, new List<string>{ }),
            ("Second Cinders", "m00_00_00_00", SoulsGame.DS3, new List<string>{ }, new List<string>{ }),
            ("Third Cinders", "m00_00_00_00", SoulsGame.DS3, new List<string>{ }, new List<string>{ }),
            ("Kiln of Flame / Flameless Shrine", "m41_00_00_00", SoulsGame.DS3, new List<string>{ "Flameless Shrine", "Kiln of the First Flame", "Soul of Cinder" }, new List<string>{ "Dreg Heap" }),
            ("Painted World of Ariandel", "m45_00_00_00", SoulsGame.DS3, new List<string>{ "Snowfield", "Rope Bridge Cave", "Corvian Settlement", "Ariandel Chapel", "Sister Friede", "Depths of the Painting", "Champion's Gravetender" }, new List<string>{ }),
            ("Painted World Second Half", "m00_00_00_00", SoulsGame.DS3, new List<string>{ "Snowy Mountain Pass" }, new List<string>{ "Painted World of Ariandel", "Dreg Heap" }),
            ("Dreg Heap", "m50_00_00_00", SoulsGame.DS3, new List<string>{ "The Dreg Heap", "Earthen Peak Ruins", "Within the Earthen Peak Ruins", "The Deamon Prince" }, new List<string>{ }),
            ("Ringed City", "m51_00_00_00", SoulsGame.DS3, new List<string>{ "Mausoleum Lookout", "Ringed Inner Wall", "Ringed City Streets", "Shared Grave", "Church of Filianore", "Darkeater Midir" }, new List<string>{ "Filianore's Rest" }),
            ("Filianore's Rest", "m51_01_00_00", SoulsGame.DS3, new List<string>{ "Filianore's Rest", "Slave Knight Gael" }, new List<string>{ }),
        });

        public static Dictionary<string, HashSet<int>> DS3NonDefaultMapItemLots = new()
        {
            { "Darkwraith Cell", new HashSet<int>{ 60940 } },
            { "Oceiros' Garden", new HashSet<int>{ 3000540, 3000000, 3000530, 3000480, 3000430, 3000431, 3000432, 3000433, 3000470, 3000630, 3000620, 3000510, 3000570, 3000520, 3000500, 2020, 3000840, 3000800 } },
            { "Ledge Outside Jailbreaker's Window", new HashSet<int>{ 3900100 } },
            { "Jail Cells", new HashSet<int>{ 3900500, 3900820 } },
            { "Old Cell", new HashSet<int>{ 3900400 } },
            { "Velka Shrine", new HashSet<int>{ 3100220, 3100260, 3100070, 3100340, 3100330, 3100740, 3100300 } },
            { "Untended Graves", new HashSet<int>{ 4000250, 4000220, 4000240, 4000270, 4000260, 4000310, 4000280 } },
            { "Firelink Tower", new HashSet<int>{ 4000190, 4000350, 4000351, 4000352, 4000170 } },
            { "Firelink Roof", new HashSet<int>{ 4000160, 4000180, 4000700 } }, // If not assuming tree skip, include in Firelink Tower, otherwise include in Firelink
            { "Painted World Second Half", new HashSet<int>{ 4500310, 4500320, 4500330, 4500340, 4500350, 4500360, 4500370, 4500380, 4500390, 4500400, 4500410, 4500420, 4500430, 4500460, 4500470, 4500471, 4500472, 4500473, 4500480, 4500570, 4500571, 2300 } },
        };

        public static List<Map> AllMaps = DSRMaps.Concat(DS2Maps).Concat(DS3Maps).ToList();

        private Map(string friendlyName, string fileName, SoulsGame sourceGame, IReadOnlyList<string> bonfires)
        {
            FriendlyName = friendlyName;
            FileName = fileName;
            SourceGame = sourceGame;
            Bonfires = bonfires;
        }

        private static List<Map> ParseMapDefinitions(List<(string FriendlyName, string FileName, SoulsGame SourceGame, List<string> Bonfires, List<string> ConnectedMapNames)> mapDefinitions)
        {
            var maps = new List<Map>();

            foreach (var mapDefinition in mapDefinitions)
            {
                maps.Add(new Map(mapDefinition.FriendlyName, mapDefinition.FileName, mapDefinition.SourceGame, mapDefinition.Bonfires));
            }

            foreach (var mapDefinition in mapDefinitions)
            {
                var currentMap = maps.Single(map => map.FriendlyName == mapDefinition.FriendlyName);
                foreach (var connectedMapName in mapDefinition.ConnectedMapNames)
                {
                    currentMap.connectedMaps.Add(maps.Single(map => map.FriendlyName == connectedMapName));
                }
            }

            return maps;
        }

        public static void LoadCrossGameWarps(string bonfireMappingsFilePath)
        {
            var mappingsFile = File.ReadAllLines(bonfireMappingsFilePath);
            foreach (var line in mappingsFile)
            {
                var bonfires = line.Split(',');
                var dsrMap = DSRMaps.Single(map => map.Bonfires.Contains(bonfires[0]));
                var ds2Map = DS2Maps.Single(map => map.Bonfires.Contains(bonfires[1]));
                var ds3Map = DS3Maps.Single(map => map.Bonfires.Contains(bonfires[2]));

                dsrMap.connectedMaps.Add(ds2Map);
                dsrMap.connectedMaps.Add(ds3Map);
                ds2Map.connectedMaps.Add(dsrMap);
                ds2Map.connectedMaps.Add(ds3Map);
                ds3Map.connectedMaps.Add(dsrMap);
                ds3Map.connectedMaps.Add(ds2Map);
            }
        }

        public bool CanReach(string mapName)
        {
            var result = false;
            VisitAllConnectedMaps(map => result = result || map.FriendlyName == mapName);
            return result;
        }

        public bool CanUse(Key key)
        {
            var result = false;
            VisitAllConnectedMaps(map => result = result || key.connectionsUnlocked.Any(connection => connection.Item1 == map || connection.Item2 == map));
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

    public class Key
    {
        public readonly int itemId;
        public readonly SoulsItemType itemType;
        public readonly SoulsGame originalGame;
        public readonly int defaultLotNumber;
        public readonly IReadOnlyList<(Map, Map)> connectionsUnlocked;

        private Key(int itemId, SoulsItemType itemType, SoulsGame originalGame, int defaultLotNumber, List<(Map, Map)> connectionsUnlocked)
        {
            this.itemId = itemId;
            this.itemType = itemType;
            this.originalGame = originalGame;
            this.defaultLotNumber = defaultLotNumber;
            this.connectionsUnlocked = connectionsUnlocked;
        }

        public static Key NewKey(int itemId, SoulsItemType itemType, SoulsGame originalGame, int defaultLotNumber, List<(string, string)> mapsToConnect)
        {
            var parsedMaps = new List<(Map, Map)>();
            var mapSet = originalGame switch
            {
                SoulsGame.DSR => Map.DSRMaps,
                SoulsGame.DS2S => Map.DS2Maps,
                SoulsGame.DS3 => Map.DS3Maps
            };
            foreach (var mapPair in mapsToConnect)
            {
                var map1 = mapSet.Single(map => map.FriendlyName == mapPair.Item1);
                var map2 = mapSet.Single(map => map.FriendlyName == mapPair.Item2);
                parsedMaps.Add((map1, map2));
            }
            return new Key(itemId, itemType, originalGame, defaultLotNumber, parsedMaps);
        }

        public void Collect()
        {
            foreach (var mapPair in connectionsUnlocked)
            {
                mapPair.Item1.connectedMaps.Add(mapPair.Item2);
                mapPair.Item2.connectedMaps.Add(mapPair.Item1);
            }
        }

        public static readonly IReadOnlyList<Key> DSRKeys = new List<Key>
        {
            NewKey(2001, SoulsItemType.Goods, SoulsGame.DSR, 1010140, new List<(string, string)>{ ("Undead Burg / Undead Parish", "Lower Undead Burg") }), // Basement Key
            NewKey(2002, SoulsItemType.Goods, SoulsGame.DSR, 6190, new List<(string, string)>{ }), // Crest of Artorias (technically connects Darkroot Garden and Darkroot basin, but A) those are the same map because B) you can bypass it.)
            NewKey(2003, SoulsItemType.Goods, SoulsGame.DSR, 1500150,  new List<(string, string)>{ ("Sen's Fortress", "Sen's Cage") }), // Cage Key
            NewKey(2004, SoulsItemType.Goods, SoulsGame.DSR, 26900100, new List<(string, string)>{ ("Tower Cell", "Archives Tower") }), // Archive Tower Cell Key
            NewKey(2005, SoulsItemType.Goods, SoulsGame.DSR, 1700630, new List<(string, string)>{ ("Archives Tower", "Crystal Cave") }), // Archive Tower Giant Door Key
            NewKey(2006, SoulsItemType.Goods, SoulsGame.DSR, 1700590, new List<(string, string)>{ ("Archives Tower", "Archives Tower Giant Cell") }), // Archive Tower Giant Cell Key
            NewKey(2007, SoulsItemType.Goods, SoulsGame.DSR, 2500, new List<(string, string)>{ ("Depths", "Blighttown") }), // Blighttown Key
            NewKey(2008, SoulsItemType.Goods, SoulsGame.DSR, 1400500, new List<(string, string)>{ ("Valley of Drakes", "New Londo Ruins / Valley of Drakes") }), // Key to New Londo Ruins
            NewKey(2009, SoulsItemType.Goods, SoulsGame.DSR, 1100140, new List<(string, string)>{ ("Painted World", "Painted World Annex") }), // Annex Key
            NewKey(2010, SoulsItemType.Goods, SoulsGame.DSR, 1810000, new List<(string, string)>{ ("DS1 Starting Cell", "Northern Undead Asylum") }), // Dungeon Cell Key
            NewKey(2011, SoulsItemType.Goods, SoulsGame.DSR, 1081, new List<(string, string)>{ ("Northern Undead Asylum", "Firelink Shrine") }), // Big Pilgrim's Key
            NewKey(2012, SoulsItemType.Goods, SoulsGame.DSR, 1080, new List<(string, string)>{ ("Northern Undead Asylum", "Northern Undead Asylum F2 East") }), // Undead Asylum F2 East Key
            NewKey(2013, SoulsItemType.Goods, SoulsGame.DSR, 1100, new List<(string, string)>{ ("New Londo Ruins / Valley of Drakes", "New Londo Ruins Post Seal") }), // Key to the Seal
            NewKey(2014, SoulsItemType.Goods, SoulsGame.DSR, 2510, new List<(string, string)>{ ("Lower Undead Burg", "Depths") }), // Key to Depths
            NewKey(2016, SoulsItemType.Goods, SoulsGame.DSR, 1020210, new List<(string, string)>{ ("Northern Undead Asylum F2 East", "Northern Undead Asylum F2 West") }), // Undead Asylum F2 West Key
            NewKey(2017, SoulsItemType.Goods, SoulsGame.DSR, 1010000, new List<(string, string)>{ }), // Mystery Key (I don't currently handle NPC drops/quest items, so this doesn't need it's own zone.)
            NewKey(2018, SoulsItemType.Goods, SoulsGame.DSR, 1000240, new List<(string, string)>{ ("Depths", "Sewer Chamber") }), // Sewer Chamber Key
            NewKey(2019, SoulsItemType.Goods, SoulsGame.DSR, 1200140, new List<(string, string)>{ }), // Watchtower Basement Key
            NewKey(2020, SoulsItemType.Goods, SoulsGame.DSR, 1700210, new List<(string, string)>{ ("Archives Tower", "Archives Tower Extra") }), // Archive Prison Extra Key
            NewKey(2021, SoulsItemType.Goods, SoulsGame.DSR, 6231, new List<(string, string)>{ ("Undead Burg / Undead Parish", "Undead Burg Residence"), ("Lower Undead Burg", "Lower Undead Burg Residence") }), // Residence Key
            NewKey(2022, SoulsItemType.Goods, SoulsGame.DSR, 27803001, new List<(string, string)>{ ("Oolacile", "Oolacile After Gough") }), // Crest Key
            NewKey(2100, SoulsItemType.Goods, SoulsGame.DSR, -1, new List<(string, string)>{ ("Undead Burg / Undead Parish", "Undead Burg Residence"), ("Depths", "Sewer Chamber"), ("Valley of Drakes", "New Londo Ruins / Valley of Drakes"), ("Sen's Fortress", "Sen's Cage") }), // Master Key
            NewKey(2500, SoulsItemType.Goods, SoulsGame.DSR, 2560, new List<(string, string)>{ ("Firelink Altar", "First Lord Soul") }), // Nito's Lord Soul
            NewKey(2501, SoulsItemType.Goods, SoulsGame.DSR, 2580, new List<(string, string)>{ ("First Lord Soul", "Second Lord Soul") }), // Bed of Chaos' Lord Soul
            NewKey(2502, SoulsItemType.Goods, SoulsGame.DSR, 2630, new List<(string, string)>{ ("Second Lord Soul", "Third Lord Soul") }), // Four Kings' Lord Soul
            NewKey(2503, SoulsItemType.Goods, SoulsGame.DSR, 2640, new List<(string, string)>{ ("Third Lord Soul", "Kiln of the First Flame") }), // Seath's Lord Soul
            NewKey(2510, SoulsItemType.Goods, SoulsGame.DSR, 1090, new List<(string, string)>{ ("Firelink Shrine", "Firelink Altar"), ("Tomb of the Giants", "Tomb of the Giants Post Lordvessel"), ("Demon Ruins / Lost Izalith", "Demon Ruins / Lost Izalith Post Lordvessel"), ("Anor Londo", "Duke's Archives"), ("New Londo Ruins / Valley of Drakes", "New Londo Ruins Post Seal") }), // Lordvessel
            NewKey(2520, SoulsItemType.Goods, SoulsGame.DSR, 27100200, new List<(string, string)>{ ("Darkroot Garden", "Oolacile") }), // Broken Pendant
            NewKey(138, SoulsItemType.Accessory, SoulsGame.DSR, 2540, new List<(string, string)>{ ("New Londo Ruins Post Seal", "Four Kings") }), // Covenant of Artorias
            NewKey(139, SoulsItemType.Accessory, SoulsGame.DSR, 2670, new List<(string, string)>{ }), // Orange Charred Ring
            NewKey(149, SoulsItemType.Accessory, SoulsGame.DSR, 1300020, new List<(string, string)>{ }), // Darkmoon Seance Ring
            NewKey(384, SoulsItemType.Accessory, SoulsGame.DSR, 1810080, new List<(string, string)>{ ("Anor Londo", "Painted World") }), // Peculiar Doll
        };

        public static readonly IReadOnlyList<Key> DS2SotFSKeys = new List<Key>
        {
            NewKey(50600000, SoulsItemType.Goods, SoulsGame.DS2S, 309600, new List<(string, string)>{ ("Forest of Fallen Giants", "Forest of Fallen Giants Post Soldier Key") }), // Soldier Key
            NewKey(50610000, SoulsItemType.Goods, SoulsGame.DS2S, 20215100, new List<(string, string)>{ ("Drangleic Castle & Throne of Want", "Looking Glass Knight Arena") }), // Key to King's Passage
            NewKey(50800000, SoulsItemType.Goods, SoulsGame.DS2S, 10166180, new List<(string, string)>{ ("The Lost Bastille & Belfry Luna", "Bastille Cells") }), // Bastille Key
            NewKey(50810000, SoulsItemType.Goods, SoulsGame.DS2S, 10196080, new List<(string, string)>{ ("Forest of Fallen Giants Post Soldier Key", "Forest of Fallen Giants Iron Key Room") }), // Iron Key
            NewKey(50820000, SoulsItemType.Goods, SoulsGame.DS2S, 60001000, new List<(string, string)>{ ("Majula", "Dragon Talon Room"), ("The Gutter & Black Gulch", "Havel Armor Room") }), // Forgotten Key
            NewKey(50830000, SoulsItemType.Goods, SoulsGame.DS2S, 1140300, new List<(string, string)>{ ("Brightstone Cove Tseldora", "Brightstone Key Room") }), // Brightstone Key
            NewKey(50840000, SoulsItemType.Goods, SoulsGame.DS2S, 10165240, new List<(string, string)>{ }), // Antiquated Key
            NewKey(50850000, SoulsItemType.Goods, SoulsGame.DS2S, 60009000, new List<(string, string)>{ }), // Fang Key (TODO: Add Ornifex's store when I start handling stores)
            NewKey(50860000, SoulsItemType.Goods, SoulsGame.DS2S, 1751000, new List<(string, string)>{ ("Majula", "Majula House") }), // House Key
            NewKey(50870000, SoulsItemType.Goods, SoulsGame.DS2S, 75400500, new List<(string, string)>{ ("Majula", "Lenigrast's House") }), // Lenigrast's Key (TODO: Add Lenigrast's store)
            NewKey(50890000, SoulsItemType.Goods, SoulsGame.DS2S, 60006000, new List<(string, string)>{ }), // Rotunda Lockstone
            NewKey(50900000, SoulsItemType.Goods, SoulsGame.DS2S, 309700, new List<(string, string)>{ ("Drangleic Castle & Throne of Want", "Nashandra") }), // Giant's Kinship
            NewKey(50910000, SoulsItemType.Goods, SoulsGame.DS2S, 1787000, new List<(string, string)>{ ("Forest of Fallen Giants Post Soldier Key", "Memory of Vammar, Orro, and Jeigh"), ("Brightstone Cove Tseldora", "Dragon Memories"), ("Undead Crypt", "Memory of the King") }), // Ashen Mist Heart
            NewKey(50930000, SoulsItemType.Goods, SoulsGame.DS2S, 1742000, new List<(string, string)>{ ("Brightstone Cove Tseldora", "Tseldora Den") }), // Tseldora Den Key
            NewKey(50970000, SoulsItemType.Goods, SoulsGame.DS2S, 10236160, new List<(string, string)>{ ("Huntsman's Copse & Undead Purgatory", "Undead Lockaway") }), // Undead Lockaway Key
            NewKey(50990000, SoulsItemType.Goods, SoulsGame.DS2S, 10165260, new List<(string, string)>{ }), // Dull Ember (TODO: Add McDuff's store)
            NewKey(51030000, SoulsItemType.Goods, SoulsGame.DS2S, 60050000, new List<(string, string)>{ ("Aldia's Keep", "Aldia Side Room") }), // Aldia Key
            NewKey(40510000, SoulsItemType.Goods, SoulsGame.DS2S, 20246500, new List<(string, string)>{ ("Shaded Woods & Shrine of Winter", "Aldia's Keep")}), // King's Ring
            NewKey(52000000, SoulsItemType.Goods, SoulsGame.DS2S, 10046140, new List<(string, string)>{ ("The Gutter & Black Gulch", "Shulva, Sanctum City") }), // Dragon Talon
            NewKey(52100000, SoulsItemType.Goods, SoulsGame.DS2S, 10106360, new List<(string, string)>{ ("Iron Keep & Belfry Sol", "Brume Tower") }), // Heavy Iron Key
            NewKey(52200000, SoulsItemType.Goods, SoulsGame.DS2S, 20216110, new List<(string, string)>{ ("Shaded Woods & Shrine of Winter", "Frozen Eleum Loyce") }), // Frozen Flower
            NewKey(52300000, SoulsItemType.Goods, SoulsGame.DS2S, 50356630, new List<(string, string)>{ ("Shulva, Sanctum City", "Priestess' Chamber") }), // Eternal Sanctum Key
            NewKey(52400000, SoulsItemType.Goods, SoulsGame.DS2S, 50366210, new List<(string, string)>{ ("Brume Tower", "Brume Tower With Only Tower Key") }), // Tower Key
            NewKey(52500000, SoulsItemType.Goods, SoulsGame.DS2S, 50376300, new List<(string, string)>{ ("Frozen Eleum Loyce", "Reindeer Valley") }), // Garrison Ward Key
            NewKey(52650000, SoulsItemType.Goods, SoulsGame.DS2S, 50355120, new List<(string, string)>{ ("Shulva, Sanctum City", "Dragon Sanctum") }), // Dragon Stone
            NewKey(53100000, SoulsItemType.Goods, SoulsGame.DS2S, 60014000, new List<(string, string)>{ ("Brume Tower", "Brume Tower With Only Scorching Iron Scepter"), ("Brume Tower With Only Tower Key", "Brume Tower With Both Keys") }), // Scorching Iron Scepter
            NewKey(53600000, SoulsItemType.Goods, SoulsGame.DS2S, 50375500, new List<(string, string)>{ ("Frozen Eleum Loyce", "Frozen Eleum Loyce After Aava") }), // Eye of the Priestess
            NewKey(60537000, SoulsItemType.Goods, SoulsGame.DS2S, 10165120, new List<(string, string)>{ ("Things Betwixt", "Things Betwixt Post Statue") }), // Fragrant Branch of Yore
            NewKey(60537000, SoulsItemType.Goods, SoulsGame.DS2S, 10176180, new List<(string, string)>{ ("Majula", "Majula <-> Shaded Woods") }), // Fragrant Branch of Yore
            NewKey(60537000, SoulsItemType.Goods, SoulsGame.DS2S, 10185110, new List<(string, string)>{ ("Heide's Tower <-> No-man's Wharf", "Flooded Passage Side Room") }), // Fragrant Branch of Yore
            NewKey(60537000, SoulsItemType.Goods, SoulsGame.DS2S, 10256160, new List<(string, string)>{ ("The Lost Bastille & Belfry Luna", "Ruin Sentinel Building") }), // Fragrant Branch of Yore
            NewKey(60537000, SoulsItemType.Goods, SoulsGame.DS2S, 10256450, new List<(string, string)>{ ("The Lost Bastille & Belfry Luna", "Straid's Cell") }), // Fragrant Branch of Yore (TODO: Add Straid's Store)
            NewKey(60537000, SoulsItemType.Goods, SoulsGame.DS2S, 10325040, new List<(string, string)>{ ("The Gutter & Black Gulch", "Hidden Chamber") }), // Fragrant Branch of Yore
            NewKey(60537000, SoulsItemType.Goods, SoulsGame.DS2S, 10326160, new List<(string, string)>{ ("Shaded Woods & Shrine of Winter", "Vengarl's Body Room") }), // Fragrant Branch of Yore
            NewKey(60537000, SoulsItemType.Goods, SoulsGame.DS2S, 20245070, new List<(string, string)>{ ("Vengarl's Body Room", "Chest after Vengarl") }), // Fragrant Branch of Yore (Typically requires two branches after Vengarl, but probably reachable with a good jump and one branch)
            NewKey(60537000, SoulsItemType.Goods, SoulsGame.DS2S, 10106420, new List<(string, string)>{ ("Shaded Woods & Shrine of Winter", "Lion Mage Set Chest") }), // Fragrant Branch of Yore
            NewKey(60537000, SoulsItemType.Goods, SoulsGame.DS2S, 10275050, new List<(string, string)>{ ("Shaded Woods & Shrine of Winter", "Fang Key Lion") }), // Fragrant Branch of Yore
            NewKey(60537000, SoulsItemType.Goods, SoulsGame.DS2S, 10165140, new List<(string, string)>{ ("Shrine of Amana", "Rise of the Dead") }), // Fragrant Branch of Yore
        };

        public static readonly IReadOnlyList<Key> DS3Keys = new List<Key>
        {
            NewKey(2001, SoulsItemType.Goods, SoulsGame.DS3, 60910, new List<(string, string)>{ ("High Wall of Lothric / Garden", "Darkwraith Cell") }), // Lift Chamber Key
            NewKey(2005, SoulsItemType.Goods, SoulsGame.DS3, 2110, new List<(string, string)>{ ("Catacombs Carthus / Smouldering Lake", "Irithyll / Anor Londo") }), // Small Doll
            NewKey(2007, SoulsItemType.Goods, SoulsGame.DS3, 3900040, new List<(string, string)>{ ("Dungeon / Profaned Capital", "Ledge Outside Jailbreaker's Window") }), // Jailbreaker's Key
            NewKey(2008, SoulsItemType.Goods, SoulsGame.DS3, 3900520, new List<(string, string)>{ ("Dungeon / Profaned Capital", "Jail Cells") }), // Jailer's Key Ring (TODO: Add Karla's shop)
            NewKey(2009, SoulsItemType.Goods, SoulsGame.DS3, 110055, new List<(string, string)>{ ("Undead Settlement", "Velka Shrine") }), // Grave Key (TODO: Add Irena shop)
            NewKey(2010, SoulsItemType.Goods, SoulsGame.DS3, 3000210, new List<(string, string)>{ }), // Cell Key (TODO: Add Greyrat's Shop)
            NewKey(2012, SoulsItemType.Goods, SoulsGame.DS3, 3900610, new List<(string, string)>{ ("Dungeon / Profaned Capital", "Old Cell") }), // Old Cell Key
            NewKey(2013, SoulsItemType.Goods, SoulsGame.DS3, 110025, new List<(string, string)>{ ("Cemetery / Firelink / Untended Graves", "Firelink Tower") }), // Tower Key
            NewKey(2014, SoulsItemType.Goods, SoulsGame.DS3, 57000, new List<(string, string)>{ ("Lothric Castle", "Grand Archives") }), // Grand Archives Key
            //NewKey(2015, SoulsItemType.Goods, SoulsGame.DS3, 51600, new List<(string, string)>{ ("Cemetery / Firelink / Untended Graves", "Firelink Tower") }), // Tower Key (this appears to be the copy dropped by Irena if you kill her. I'm ignoring it for now.)
            NewKey(2102, SoulsItemType.Goods, SoulsGame.DS3, 62300, new List<(string, string)>{ ("High Wall of Lothric / Garden", "Undead Settlement") }), // Small Lothric Banner
            NewKey(2123, SoulsItemType.Goods, SoulsGame.DS3, 2100, new List<(string, string)>{ ("Cemetery / Firelink / Untended Graves", "First Cinders") }), // Cinders of a Lord (Abyss Watchers)
            NewKey(2124, SoulsItemType.Goods, SoulsGame.DS3, 2130, new List<(string, string)>{ ("First Cinders", "Second Cinders") }), // Cinders of a Lord (Aldrich)
            NewKey(2125, SoulsItemType.Goods, SoulsGame.DS3, 2170, new List<(string, string)>{ ("Second Cinders", "Third Cinders") }), // Cinders of a Lord (Yhorm)
            NewKey(2126, SoulsItemType.Goods, SoulsGame.DS3, 2040, new List<(string, string)>{ ("Third Cinders", "Kiln of Flame / Flameless Shrine") }), // Cinders of a Lord (Lothric)
            NewKey(2135, SoulsItemType.Goods, SoulsGame.DS3, 2061, new List<(string, string)>{ }), // Transposing Kiln (TODO: Add Ludleth's shop)
            NewKey(2117, SoulsItemType.Goods, SoulsGame.DS3, 52302, new List<(string, string)>{ ("High Wall of Lothric / Garden", "Lothric Castle"), ("High Wall of Lothric / Garden", "Oceiros' Garden") }), // Basin of Vows
            NewKey(2155, SoulsItemType.Goods, SoulsGame.DS3, 55200, new List<(string, string)>{ ("Painted World of Ariandel", "Painted World Second Half") }), // Contraption Key
            NewKey(2156, SoulsItemType.Goods, SoulsGame.DS3, 5000600, new List<(string, string)>{ ("Dreg Heap", "Ringed City") }), // Small Envoy Banner
        };

        public static readonly IReadOnlyList<Key> AllKeys = DSRKeys.Concat(DS2SotFSKeys).Concat(DS3Keys).ToList();
    }
}
