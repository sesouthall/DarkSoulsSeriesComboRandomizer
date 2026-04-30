namespace DarkSoulsSeriesComboRandomizer
{
    public class Key
    {
        public readonly SoulsItem Item;
        public readonly int defaultLotNumber;
        public IReadOnlyList<(Map, Map)> ConnectionsUnlocked => connectionsUnlocked;
        private readonly List<(Map, Map)> connectionsUnlocked;

        private Key(SoulsItem item, int defaultLotNumber, List<(Map, Map)> connectionsUnlocked)
        {
            this.Item = item;
            this.defaultLotNumber = defaultLotNumber;
            this.connectionsUnlocked = connectionsUnlocked;
        }

        public static Key NewKey(int itemId, SoulsItemType itemType, SoulsGame originalGame, int defaultLotNumber, List<(MapName, MapName)> mapsToConnect, IReadOnlyDictionary<MapName, Map> mapLookup)
        {
            var parsedMaps = mapsToConnect
                .Select(pair => (mapLookup[pair.Item1], mapLookup[pair.Item2]))
                .ToList();
            return new Key(new SoulsItem(originalGame, itemType, itemId), defaultLotNumber, parsedMaps);
        }

        public static Key NewKey((SoulsItem originalItem, int defaultLotNumber, List<(MapName, MapName)> mapsToConnect) keyDefinition, IReadOnlyDictionary<MapName, Map> mapLookup)
        {
            var parsedMaps = keyDefinition.mapsToConnect
                .Select(pair => (mapLookup[pair.Item1], mapLookup[pair.Item2]))
                .ToList();
            return new Key(keyDefinition.originalItem, keyDefinition.defaultLotNumber, parsedMaps);
        }

        public static List<Key> ConstructDS1Keys(IReadOnlyDictionary<MapName, Map> maps) =>
            [.. DS1KeyDefinitions.Select(definition => NewKey(definition, maps))];

        public static List<Key> ConstructDS2Keys(IReadOnlyDictionary<MapName, Map> maps) =>
            [.. DS2KeyDefinitions.Select(definition => NewKey(definition, maps))];

        public static IReadOnlyList<Key> ConstructDS3Keys(IReadOnlyDictionary<MapName, Map> maps) =>
            [.. DS3KeyDefinitions.Select(definition => NewKey(definition, maps))];

        public void AddUnlockedConnection((Map, Map) newConnection)
        {
            connectionsUnlocked.Add(newConnection);
        }

        public void Collect()
        {
            foreach (var mapPair in ConnectionsUnlocked)
            {
                mapPair.Item1.ConnectTo(mapPair.Item2);
            }
        }
            
        private static readonly IReadOnlyList<(SoulsItem, int, List<(MapName, MapName)>)> DS1KeyDefinitions =
        [
            (new SoulsItem(SoulsGame.DSR, SoulsItemType.Goods, 2001), 1010140,     [(MapName.UndeadBurgUndeadParish, MapName.LowerUndeadBurg)]),                  // Basement Key
            (new SoulsItem(SoulsGame.DSR, SoulsItemType.Goods, 2002), 6190,        []),                                                                           // Crest of Artorias
            (new SoulsItem(SoulsGame.DSR, SoulsItemType.Goods, 2003), 1500150,     [(MapName.SensFortress, MapName.SensCage)]),                                   // Cage Key
            (new SoulsItem(SoulsGame.DSR, SoulsItemType.Goods, 2004), 26900100,    [(MapName.TowerCell, MapName.ArchivesTower)]),                                 // Archive Tower Cell Key
            (new SoulsItem(SoulsGame.DSR, SoulsItemType.Goods, 2005), 1700630,     [(MapName.ArchivesTower, MapName.CrystalCave)]),                               // Archive Tower Giant Door Key
            (new SoulsItem(SoulsGame.DSR, SoulsItemType.Goods, 2006), 1700590,     [(MapName.ArchivesTower, MapName.ArchivesTowerGiantCell)]),                    // Archive Tower Giant Cell Key
            (new SoulsItem(SoulsGame.DSR, SoulsItemType.Goods, 2007), 2500,        [(MapName.Depths, MapName.Blighttown)]),                                       // Blighttown Key
            (new SoulsItem(SoulsGame.DSR, SoulsItemType.Goods, 2008), 1400500,     [(MapName.ValleyOfDrakes, MapName.NewLondoRuinsValleyOfDrakes)]),              // Key to New Londo Ruins
            (new SoulsItem(SoulsGame.DSR, SoulsItemType.Goods, 2009), 1100140,     [(MapName.PaintedWorld, MapName.PaintedWorldAnnex)]),                          // Annex Key
            (new SoulsItem(SoulsGame.DSR, SoulsItemType.Goods, 2010), 1810000,     [(MapName.DS1StartingCell, MapName.NorthernUndeadAsylum)]),                    // Dungeon Cell Key
            (new SoulsItem(SoulsGame.DSR, SoulsItemType.Goods, 2011), 1081,        [(MapName.NorthernUndeadAsylum, MapName.FirelinkShrine)]),                     // Big Pilgrim's Key
            (new SoulsItem(SoulsGame.DSR, SoulsItemType.Goods, 2012), 1080,        [(MapName.NorthernUndeadAsylum, MapName.NorthernUndeadAsylumF2East)]),         // Undead Asylum F2 East Key
            (new SoulsItem(SoulsGame.DSR, SoulsItemType.Goods, 2013), 1100,        [(MapName.NewLondoRuinsValleyOfDrakes, MapName.NewLondoRuinsPostSeal)]),       // Key to the Seal
            (new SoulsItem(SoulsGame.DSR, SoulsItemType.Goods, 2014), 2510,        [(MapName.LowerUndeadBurg, MapName.Depths)]),                                  // Key to Depths
            (new SoulsItem(SoulsGame.DSR, SoulsItemType.Goods, 2016), 1020210,     [(MapName.NorthernUndeadAsylumF2East, MapName.NorthernUndeadAsylumF2West)]),   // Undead Asylum F2 West Key
            (new SoulsItem(SoulsGame.DSR, SoulsItemType.Goods, 2017), 1010000,     []),                                                                           // Mystery Key
            (new SoulsItem(SoulsGame.DSR, SoulsItemType.Goods, 2018), 1000240,     [(MapName.Depths, MapName.SewerChamber)]),                                     // Sewer Chamber Key
            (new SoulsItem(SoulsGame.DSR, SoulsItemType.Goods, 2019), 1200140,     []),                                                                           // Watchtower Basement Key
            (new SoulsItem(SoulsGame.DSR, SoulsItemType.Goods, 2020), 1700210,     [(MapName.ArchivesTower, MapName.ArchivesTowerExtra)]),                        // Archive Prison Extra Key
            (new SoulsItem(SoulsGame.DSR, SoulsItemType.Goods, 2021), 6231,        [(MapName.UndeadBurgUndeadParish, MapName.UndeadBurgResidence),                
                                                                                    (MapName.LowerUndeadBurg, MapName.LowerUndeadBurgResidence) ]),               // Residence Key
            (new SoulsItem(SoulsGame.DSR, SoulsItemType.Goods, 2022), 27803001,    [(MapName.Oolacile, MapName.OolacileAfterGough)]),                             // Crest Key
            //(new SoulsItem(SoulsGame.DSR, SoulsItemType.Goods, 2100), -1,          [(MapName.UndeadBurgUndeadParish, MapName.UndeadBurgResidence),
            //                                                                        (MapName.Depths, MapName.SewerChamber),
            //                                                                        (MapName.ValleyOfDrakes, MapName.NewLondoRuinsValleyOfDrakes),
            //                                                                        (MapName.SensFortress, MapName.SensCage) ]),                                  // Master Key (ignore for now, otherwise the "unrandomized key" logic will collect it and break other keys)
            (new SoulsItem(SoulsGame.DSR, SoulsItemType.Goods, 2500), 2560,        [(MapName.FirelinkAltar, MapName.FirstLordSoul)]),                             // Nito's Lord Soul
            (new SoulsItem(SoulsGame.DSR, SoulsItemType.Goods, 2501), 2580,        [(MapName.FirstLordSoul, MapName.SecondLordSoul)]),                            // Bed of Chaos' Lord Soul
            (new SoulsItem(SoulsGame.DSR, SoulsItemType.Goods, 2502), 2630,        [(MapName.SecondLordSoul, MapName.ThirdLordSoul)]),                            // Four Kings' Lord Soul
            (new SoulsItem(SoulsGame.DSR, SoulsItemType.Goods, 2503), 2640,        [(MapName.ThirdLordSoul, MapName.KilnOfTheFirstFlame)]),                       // Seath's Lord Soul
            (new SoulsItem(SoulsGame.DSR, SoulsItemType.Goods, 2510), 1090,        [(MapName.FirelinkShrine, MapName.FirelinkAltar),                              
                                                                                    (MapName.TombOfTheGiants, MapName.TombOfTheGiantsPostLordvessel),             
                                                                                    (MapName.DemonRuinsLostIzalith, MapName.DemonRuinsLostIzalithPostLordvessel), 
                                                                                    (MapName.AnorLondo, MapName.DukesArchives),                                   
                                                                                    (MapName.NewLondoRuinsValleyOfDrakes, MapName.NewLondoRuinsPostSeal) ]),      // Lordvessel
            (new SoulsItem(SoulsGame.DSR, SoulsItemType.Goods, 2520), 27100200,    [(MapName.DarkrootGarden, MapName.Oolacile)]),                                 // Broken Pendant
            (new SoulsItem(SoulsGame.DSR, SoulsItemType.Accessory, 138), 2540,     [(MapName.NewLondoRuinsPostSeal, MapName.FourKings)]),                         // Covenant of Artorias
            (new SoulsItem(SoulsGame.DSR, SoulsItemType.Accessory, 139), 2670,     []),                                                                           // Orange Charred Ring
            (new SoulsItem(SoulsGame.DSR, SoulsItemType.Accessory, 149), 1300020,  []),                                                                           // Darkmoon Seance Ring
            (new SoulsItem(SoulsGame.DSR, SoulsItemType.Goods, 384), 1810080,      [(MapName.AnorLondo, MapName.PaintedWorld)]),                                  // Peculiar Doll
        ];

        private static readonly IReadOnlyList<(SoulsItem, int, List<(MapName, MapName)>)> DS2KeyDefinitions =
        [
            (new SoulsItem(SoulsGame.DS2S, SoulsItemType.Goods, 50600000), 309600,   [(MapName.ForestOfFallenGiants, MapName.ForestOfFallenGiantsPostSoldierKey)]),            // Soldier Key
            (new SoulsItem(SoulsGame.DS2S, SoulsItemType.Goods, 50610000), 20215100, [(MapName.DrangleicCastleThroneOfWant, MapName.LookingGlassKnightArena)]),                // Key to King's Passage
            (new SoulsItem(SoulsGame.DS2S, SoulsItemType.Goods, 50800000), 10166180, [(MapName.TheLostBastilleBelfryLuna, MapName.BastilleCells)]),                            // Bastille Key
            (new SoulsItem(SoulsGame.DS2S, SoulsItemType.Goods, 50810000), 10196080, [(MapName.ForestOfFallenGiantsPostSoldierKey, MapName.ForestOfFallenGiantsIronKeyRoom)]), // Iron Key
            (new SoulsItem(SoulsGame.DS2S, SoulsItemType.Goods, 50820000), 60001000, [(MapName.Majula, MapName.DragonTalonRoom), 
                                                                                      (MapName.TheGutterBlackGulch, MapName.HavelArmorRoom)]),                                 // Forgotten Key
            (new SoulsItem(SoulsGame.DS2S, SoulsItemType.Goods, 50830000), 1140300,  [(MapName.BrightstoneCoveTseldora, MapName.BrightStoneKeyRoom)]),                         // Brightstone Key
            (new SoulsItem(SoulsGame.DS2S, SoulsItemType.Goods, 50840000), 10165240, []),                                                                                      // Antiquated Key
            (new SoulsItem(SoulsGame.DS2S, SoulsItemType.Goods, 50850000), 60009000, []),                                                                                      // Fang Key (TODO: Add Ornifex's store)
            (new SoulsItem(SoulsGame.DS2S, SoulsItemType.Goods, 50860000), 1751000,  [(MapName.Majula, MapName.MajulaHouse)]),                                                 // House Key
            (new SoulsItem(SoulsGame.DS2S, SoulsItemType.Goods, 50870000), 75400500, [(MapName.Majula, MapName.LenigrastsHouse)]),                                             // Lenigrast's Key (TODO: Add Lenigrast's store)
            (new SoulsItem(SoulsGame.DS2S, SoulsItemType.Goods, 50890000), 60006000, []),                                                                                      // Rotunda Lockstone
            (new SoulsItem(SoulsGame.DS2S, SoulsItemType.Goods, 50900000), 309700,   [(MapName.DrangleicCastleThroneOfWant, MapName.Nashandra)]),                              // Giant's Kinship
            (new SoulsItem(SoulsGame.DS2S, SoulsItemType.Goods, 50910000), 600000,   [(MapName.ForestOfFallenGiantsPostSoldierKey, MapName.MemoryOfVammarOrroAndJeigh),
                                                                                      (MapName.BrightstoneCoveTseldora, MapName.DragonMemories),
                                                                                      (MapName.UndeadCrypt, MapName.MemoryOfTheKing) ]),                                       // Ashen Mist Heart
            (new SoulsItem(SoulsGame.DS2S, SoulsItemType.Goods, 50930000), 1742000,  [(MapName.BrightstoneCoveTseldora, MapName.TseldoraDen)]),                                // Tseldora Den Key
            (new SoulsItem(SoulsGame.DS2S, SoulsItemType.Goods, 50970000), 10236160, [(MapName.HuntsmansCopseUndeadPurgatory, MapName.UndeadLockaway)]),                       // Undead Lockaway Key
            (new SoulsItem(SoulsGame.DS2S, SoulsItemType.Goods, 50990000), 10165260, []),                                                                                      // Dull Ember (TODO: Add McDuff's store)
            (new SoulsItem(SoulsGame.DS2S, SoulsItemType.Goods, 51030000), 60050000, [(MapName.AldiasKeep, MapName.AldiaSideRoom)]),                                           // Aldia Key
            (new SoulsItem(SoulsGame.DS2S, SoulsItemType.Goods, 40510000), 20246500, [(MapName.ShadedWoodsShrineOfWinter, MapName.AldiasKeep)]),                               // King's Ring
            (new SoulsItem(SoulsGame.DS2S, SoulsItemType.Goods, 52000000), 10046140, [(MapName.TheGutterBlackGulch, MapName.ShulvaSanctumCity)]),                              // Dragon Talon
            (new SoulsItem(SoulsGame.DS2S, SoulsItemType.Goods, 52100000), 10106360, [(MapName.IronKeepBelfrySol, MapName.BrumeTower)]),                                       // Heavy Iron Key
            (new SoulsItem(SoulsGame.DS2S, SoulsItemType.Goods, 52200000), 20216110, [(MapName.ShadedWoodsShrineOfWinter, MapName.FrozenEleumLoyce)]),                         // Frozen Flower
            (new SoulsItem(SoulsGame.DS2S, SoulsItemType.Goods, 52300000), 50356630, [(MapName.ShulvaSanctumCity, MapName.PriestessChamber)]),                                 // Eternal Sanctum Key
            (new SoulsItem(SoulsGame.DS2S, SoulsItemType.Goods, 52400000), 50366210, [(MapName.BrumeTower, MapName.BrumeTowerWithOnlyTowerKey)]),                              // Tower Key
            (new SoulsItem(SoulsGame.DS2S, SoulsItemType.Goods, 52500000), 50376300, [(MapName.FrozenEleumLoyce, MapName.ReinderValley)]),                                     // Garrison Ward Key
            (new SoulsItem(SoulsGame.DS2S, SoulsItemType.Goods, 52650000), 50355120, [(MapName.ShulvaSanctumCity, MapName.DragonSanctum)]),                                    // Dragon Stone
            (new SoulsItem(SoulsGame.DS2S, SoulsItemType.Goods, 53100000), 60014000, [(MapName.BrumeTower, MapName.BrumeTowerWithOnlyScorchingIronScepter),
                                                                                      (MapName.BrumeTowerWithOnlyTowerKey, MapName.BrumeTowerWithBothKeys) ]),                 // Scorching Iron Scepter
            (new SoulsItem(SoulsGame.DS2S, SoulsItemType.Goods, 53600000), 50375500, [(MapName.FrozenEleumLoyce, MapName.FrozenEleumLoyceAfterAava)]),                         // Eye of the Priestess
            (new SoulsItem(SoulsGame.DS2S, SoulsItemType.Goods, 60537000), 10165120, [(MapName.ThingsBetwixt, MapName.ThingsBetwixtPostStatue)]),                              // Fragrant Branch of Yore
            (new SoulsItem(SoulsGame.DS2S, SoulsItemType.Goods, 60537000), 10176180, [(MapName.Majula, MapName.MajulaShadedWoods)]),                                           // Fragrant Branch of Yore
            (new SoulsItem(SoulsGame.DS2S, SoulsItemType.Goods, 60537000), 10185110, [(MapName.HeidesTowerNomansWharf, MapName.FloodedPassageSideRoom)]),                      // Fragrant Branch of Yore
            (new SoulsItem(SoulsGame.DS2S, SoulsItemType.Goods, 60537000), 10256160, [(MapName.TheLostBastilleBelfryLuna, MapName.RuinSentinelBuilding)]),                     // Fragrant Branch of Yore
            (new SoulsItem(SoulsGame.DS2S, SoulsItemType.Goods, 60537000), 10256450, [(MapName.TheLostBastilleBelfryLuna, MapName.StraidsCell)]),                              // Fragrant Branch of Yore (TODO: Add Straid's Store)
            (new SoulsItem(SoulsGame.DS2S, SoulsItemType.Goods, 60537000), 10325040, [(MapName.TheGutterBlackGulch, MapName.HiddenChamber)]),                                  // Fragrant Branch of Yore
            (new SoulsItem(SoulsGame.DS2S, SoulsItemType.Goods, 60537000), 10326160, [(MapName.ShadedWoodsShrineOfWinter, MapName.VengarlsBodyRoom)]),                         // Fragrant Branch of Yore
            (new SoulsItem(SoulsGame.DS2S, SoulsItemType.Goods, 60537000), 20245070, [(MapName.VengarlsBodyRoom, MapName.ChestAfterVengarl)]),                                 // Fragrant Branch of Yore
            (new SoulsItem(SoulsGame.DS2S, SoulsItemType.Goods, 60537000), 10106420, [(MapName.ShadedWoodsShrineOfWinter, MapName.LionMageSetChest)]),                         // Fragrant Branch of Yore
            (new SoulsItem(SoulsGame.DS2S, SoulsItemType.Goods, 60537000), 10275050, [(MapName.ShadedWoodsShrineOfWinter, MapName.FangKeyLion)]),                              // Fragrant Branch of Yore
            (new SoulsItem(SoulsGame.DS2S, SoulsItemType.Goods, 60537000), 10165140, [(MapName.ShrineOfAmana, MapName.RiseOfTheDead)]),                                        // Fragrant Branch of Yore
            (new SoulsItem(SoulsGame.DS2S, SoulsItemType.Goods, 60537000), 1140300,  []),                                                                                      // Fragrant Branch of Yore
            (new SoulsItem(SoulsGame.DS2S, SoulsItemType.Goods, 60537000), 1153100,  []),                                                                                      // Fragrant Branch of Yore
            (new SoulsItem(SoulsGame.DS2S, SoulsItemType.Goods, 60537000), 2240100,  []),                                                                                      // Fragrant Branch of Yore
        ];

        public static readonly IReadOnlyList<(SoulsItem, int, List<(MapName, MapName)>)> DS3KeyDefinitions =
        [
            (new SoulsItem(SoulsGame.DS3, SoulsItemType.Goods, 2001), 50902,   [(MapName.HighWallOfLothricGarden, MapName.DarkwraithCell)]),                 // Lift Chamber Key
            (new SoulsItem(SoulsGame.DS3, SoulsItemType.Goods, 2005), 2110,    [(MapName.CatacombsCarthusSmoulderingLake, MapName.IrithyllAnorLondo)]),      // Small Doll
            (new SoulsItem(SoulsGame.DS3, SoulsItemType.Goods, 2007), 3900040, [(MapName.DungeonProfanedCapital, MapName.LedgeOutsideJailbreakersWindow)]),  // Jailbreaker's Key
            (new SoulsItem(SoulsGame.DS3, SoulsItemType.Goods, 2008), 3900520, [(MapName.DungeonProfanedCapital, MapName.JailCells)]),                       // Jailer's Key Ring (TODO: Add Karla's shop)
            (new SoulsItem(SoulsGame.DS3, SoulsItemType.Goods, 2009), 110055,  [(MapName.UndeadSettlement, MapName.VelkaShrine)]),                           // Grave Key (TODO: Add Irena shop)
            (new SoulsItem(SoulsGame.DS3, SoulsItemType.Goods, 2010), 3000210, []),                                                                          // Cell Key (TODO: Add Greyrat's Shop)
            (new SoulsItem(SoulsGame.DS3, SoulsItemType.Goods, 2012), 3900610, [(MapName.DungeonProfanedCapital, MapName.OldCell)]),                         // Old Cell Key
            (new SoulsItem(SoulsGame.DS3, SoulsItemType.Goods, 2013), 110025,  [(MapName.CemetaryFirelinkUntendedGraves, MapName.FirelinkTower)]),           // Tower Key
            (new SoulsItem(SoulsGame.DS3, SoulsItemType.Goods, 2014), 57000,   [(MapName.LothricCastle, MapName.GrandArchives)]),                            // Grand Archives Key
            (new SoulsItem(SoulsGame.DS3, SoulsItemType.Goods, 2015), 51600,   [(MapName.CemetaryFirelinkUntendedGraves, MapName.FirelinkTower)]),           // Tower Key (copy dropped by Irena if you kill her, ignoring for now)
            (new SoulsItem(SoulsGame.DS3, SoulsItemType.Goods, 2102), 52300,   [(MapName.HighWallOfLothricGarden, MapName.UndeadSettlement)]),               // Small Lothric Banner
            (new SoulsItem(SoulsGame.DS3, SoulsItemType.Goods, 2123), 2100,    [(MapName.CemetaryFirelinkUntendedGraves, MapName.FirstCinders)]),            // Cinders of a Lord (Abyss Watchers)
            (new SoulsItem(SoulsGame.DS3, SoulsItemType.Goods, 2124), 2130,    [(MapName.FirstCinders, MapName.SecondCinders)]),                             // Cinders of a Lord (Aldrich)
            (new SoulsItem(SoulsGame.DS3, SoulsItemType.Goods, 2125), 2170,    [(MapName.SecondCinders, MapName.ThirdCinders)]),                             // Cinders of a Lord (Yhorm)
            (new SoulsItem(SoulsGame.DS3, SoulsItemType.Goods, 2126), 2040,    [(MapName.ThirdCinders, MapName.KilnOfFlameFlamelessShrine)]),                // Cinders of a Lord (Lothric)
            (new SoulsItem(SoulsGame.DS3, SoulsItemType.Goods, 2135), 2061,    []),                                                                          // Transposing Kiln (TODO: Add Ludleth's shop)
            (new SoulsItem(SoulsGame.DS3, SoulsItemType.Goods, 2117), 52302,   [(MapName.HighWallOfLothricGarden, MapName.LothricCastle),
                                                                                (MapName.HighWallOfLothricGarden, MapName.OceirosGarden) ]),                 // Basin of Vows
            (new SoulsItem(SoulsGame.DS3, SoulsItemType.Goods, 2155), 55200,   [(MapName.PaintedWorldOfAriandel, MapName.PaintedWorldSecondHalf)]),          // Contraption Key
            (new SoulsItem(SoulsGame.DS3, SoulsItemType.Goods, 2156), 5000600, [(MapName.DregHeap, MapName.RingedCity)]),                                    // Small Envoy Banner
            (new SoulsItem(SoulsGame.DS3, SoulsItemType.Goods, 2137), 2180,    [(MapName.CemetaryFirelinkUntendedGraves, MapName.HighWallOfLothricGarden)]), // Coiled Sword
        ];
    }
}
