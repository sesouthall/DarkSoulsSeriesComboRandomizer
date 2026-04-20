namespace DarkSoulsSeriesComboRandomizer
{
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

        public static Key NewKey(int itemId, SoulsItemType itemType, SoulsGame originalGame, int defaultLotNumber, List<(MapName, MapName)> mapsToConnect)
        {
            var mapSet = originalGame switch
            {
                SoulsGame.DSR => Map.DSRMaps,
                SoulsGame.DS2S => Map.DS2Maps,
                SoulsGame.DS3 => Map.DS3Maps,
                _ => throw new ArgumentOutOfRangeException(nameof(originalGame))
            };
            var parsedMaps = mapsToConnect
                .Select(pair => (mapSet[pair.Item1], mapSet[pair.Item2]))
                .ToList();
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
            NewKey(2001, SoulsItemType.Goods, SoulsGame.DSR, 1010140,  new List<(MapName, MapName)>{ (MapName.UndeadBurgUndeadParish, MapName.LowerUndeadBurg) }),                 // Basement Key
            NewKey(2002, SoulsItemType.Goods, SoulsGame.DSR, 6190,     new List<(MapName, MapName)>{ }),                                                                           // Crest of Artorias
            NewKey(2003, SoulsItemType.Goods, SoulsGame.DSR, 1500150,  new List<(MapName, MapName)>{ (MapName.SensFortress, MapName.SensCage) }),                                  // Cage Key
            NewKey(2004, SoulsItemType.Goods, SoulsGame.DSR, 26900100, new List<(MapName, MapName)>{ (MapName.TowerCell, MapName.ArchivesTower) }),                                // Archive Tower Cell Key
            NewKey(2005, SoulsItemType.Goods, SoulsGame.DSR, 1700630,  new List<(MapName, MapName)>{ (MapName.ArchivesTower, MapName.CrystalCave) }),                              // Archive Tower Giant Door Key
            NewKey(2006, SoulsItemType.Goods, SoulsGame.DSR, 1700590,  new List<(MapName, MapName)>{ (MapName.ArchivesTower, MapName.ArchivesTowerGiantCell) }),                   // Archive Tower Giant Cell Key
            NewKey(2007, SoulsItemType.Goods, SoulsGame.DSR, 2500,     new List<(MapName, MapName)>{ (MapName.Depths, MapName.Blighttown) }),                                      // Blighttown Key
            NewKey(2008, SoulsItemType.Goods, SoulsGame.DSR, 1400500,  new List<(MapName, MapName)>{ (MapName.ValleyOfDrakes, MapName.NewLondoRuinsValleyOfDrakes) }),             // Key to New Londo Ruins
            NewKey(2009, SoulsItemType.Goods, SoulsGame.DSR, 1100140,  new List<(MapName, MapName)>{ (MapName.PaintedWorld, MapName.PaintedWorldAnnex) }),                         // Annex Key
            NewKey(2010, SoulsItemType.Goods, SoulsGame.DSR, 1810000,  new List<(MapName, MapName)>{ (MapName.DS1StartingCell, MapName.NorthernUndeadAsylum) }),                   // Dungeon Cell Key
            NewKey(2011, SoulsItemType.Goods, SoulsGame.DSR, 1081,     new List<(MapName, MapName)>{ (MapName.NorthernUndeadAsylum, MapName.FirelinkShrine) }),                    // Big Pilgrim's Key
            NewKey(2012, SoulsItemType.Goods, SoulsGame.DSR, 1080,     new List<(MapName, MapName)>{ (MapName.NorthernUndeadAsylum, MapName.NorthernUndeadAsylumF2East) }),        // Undead Asylum F2 East Key
            NewKey(2013, SoulsItemType.Goods, SoulsGame.DSR, 1100,     new List<(MapName, MapName)>{ (MapName.NewLondoRuinsValleyOfDrakes, MapName.NewLondoRuinsPostSeal) }),      // Key to the Seal
            NewKey(2014, SoulsItemType.Goods, SoulsGame.DSR, 2510,     new List<(MapName, MapName)>{ (MapName.LowerUndeadBurg, MapName.Depths) }),                                 // Key to Depths
            NewKey(2016, SoulsItemType.Goods, SoulsGame.DSR, 1020210,  new List<(MapName, MapName)>{ (MapName.NorthernUndeadAsylumF2East, MapName.NorthernUndeadAsylumF2West) }),  // Undead Asylum F2 West Key
            NewKey(2017, SoulsItemType.Goods, SoulsGame.DSR, 1010000,  new List<(MapName, MapName)>{ }),                                                                           // Mystery Key
            NewKey(2018, SoulsItemType.Goods, SoulsGame.DSR, 1000240,  new List<(MapName, MapName)>{ (MapName.Depths, MapName.SewerChamber) }),                                    // Sewer Chamber Key
            NewKey(2019, SoulsItemType.Goods, SoulsGame.DSR, 1200140,  new List<(MapName, MapName)>{ }),                                                                           // Watchtower Basement Key
            NewKey(2020, SoulsItemType.Goods, SoulsGame.DSR, 1700210,  new List<(MapName, MapName)>{ (MapName.ArchivesTower, MapName.ArchivesTowerExtra) }),                       // Archive Prison Extra Key
            NewKey(2021, SoulsItemType.Goods, SoulsGame.DSR, 6231,     new List<(MapName, MapName)>{ (MapName.UndeadBurgUndeadParish, MapName.UndeadBurgResidence),                
                                                                                                     (MapName.LowerUndeadBurg, MapName.LowerUndeadBurgResidence) }),               // Residence Key
            NewKey(2022, SoulsItemType.Goods, SoulsGame.DSR, 27803001, new List<(MapName, MapName)>{ (MapName.Oolacile, MapName.OolacileAfterGough) }),                            // Crest Key
            NewKey(2100, SoulsItemType.Goods, SoulsGame.DSR, -1,       new List<(MapName, MapName)>{ (MapName.UndeadBurgUndeadParish, MapName.UndeadBurgResidence),                
                                                                                                     (MapName.Depths, MapName.SewerChamber),                                       
                                                                                                     (MapName.ValleyOfDrakes, MapName.NewLondoRuinsValleyOfDrakes),                
                                                                                                     (MapName.SensFortress, MapName.SensCage) }),                                  // Master Key
            NewKey(2500, SoulsItemType.Goods, SoulsGame.DSR, 2560,     new List<(MapName, MapName)>{ (MapName.FirelinkAltar, MapName.FirstLordSoul) }),                            // Nito's Lord Soul
            NewKey(2501, SoulsItemType.Goods, SoulsGame.DSR, 2580,     new List<(MapName, MapName)>{ (MapName.FirstLordSoul, MapName.SecondLordSoul) }),                           // Bed of Chaos' Lord Soul
            NewKey(2502, SoulsItemType.Goods, SoulsGame.DSR, 2630,     new List<(MapName, MapName)>{ (MapName.SecondLordSoul, MapName.ThirdLordSoul) }),                           // Four Kings' Lord Soul
            NewKey(2503, SoulsItemType.Goods, SoulsGame.DSR, 2640,     new List<(MapName, MapName)>{ (MapName.ThirdLordSoul, MapName.KilnOfTheFirstFlame) }),                      // Seath's Lord Soul
            NewKey(2510, SoulsItemType.Goods, SoulsGame.DSR, 1090,     new List<(MapName, MapName)>{ (MapName.FirelinkShrine, MapName.FirelinkAltar),                              
                                                                                                     (MapName.TombOfTheGiants, MapName.TombOfTheGiantsPostLordvessel),             
                                                                                                     (MapName.DemonRuinsLostIzalith, MapName.DemonRuinsLostIzalithPostLordvessel), 
                                                                                                     (MapName.AnorLondo, MapName.DukesArchives),                                   
                                                                                                     (MapName.NewLondoRuinsValleyOfDrakes, MapName.NewLondoRuinsPostSeal) }),      // Lordvessel
            NewKey(2520, SoulsItemType.Goods, SoulsGame.DSR, 27100200, new List<(MapName, MapName)>{ (MapName.DarkrootGarden, MapName.Oolacile) }),                                // Broken Pendant
            NewKey(138,  SoulsItemType.Accessory, SoulsGame.DSR, 2540, new List<(MapName, MapName)>{ (MapName.NewLondoRuinsPostSeal, MapName.FourKings) }),                        // Covenant of Artorias
            NewKey(139,  SoulsItemType.Accessory, SoulsGame.DSR, 2670, new List<(MapName, MapName)>{ }),                                                                           // Orange Charred Ring
            NewKey(149,  SoulsItemType.Accessory, SoulsGame.DSR, 1300020, new List<(MapName, MapName)>{ }),                                                                        // Darkmoon Seance Ring
            NewKey(384,  SoulsItemType.Accessory, SoulsGame.DSR, 1810080, new List<(MapName, MapName)>{ (MapName.AnorLondo, MapName.PaintedWorld) }),                              // Peculiar Doll
        };

        public static readonly IReadOnlyList<Key> DS2SotFSKeys = new List<Key>
        {
            NewKey(50600000, SoulsItemType.Goods, SoulsGame.DS2S, 309600,   new List<(MapName, MapName)>{ (MapName.ForestOfFallenGiants, MapName.ForestOfFallenGiantsPostSoldierKey) }),                       // Soldier Key
            NewKey(50610000, SoulsItemType.Goods, SoulsGame.DS2S, 20215100, new List<(MapName, MapName)>{ (MapName.DrangleicCastleThroneOfWant, MapName.LookingGlassKnightArena) }),                           // Key to King's Passage
            NewKey(50800000, SoulsItemType.Goods, SoulsGame.DS2S, 10166180, new List<(MapName, MapName)>{ (MapName.TheLostBastilleBelfryLuna, MapName.BastilleCells) }),                                       // Bastille Key
            NewKey(50810000, SoulsItemType.Goods, SoulsGame.DS2S, 10196080, new List<(MapName, MapName)>{ (MapName.ForestOfFallenGiantsPostSoldierKey, MapName.ForestOfFallenGiantsIronKeyRoom) }),            // Iron Key
            NewKey(50820000, SoulsItemType.Goods, SoulsGame.DS2S, 60001000, new List<(MapName, MapName)>{ (MapName.Majula, MapName.DragonTalonRoom), (MapName.TheGutterBlackGulch, MapName.HavelArmorRoom) }), // Forgotten Key
            NewKey(50830000, SoulsItemType.Goods, SoulsGame.DS2S, 1140300,  new List<(MapName, MapName)>{ (MapName.BrightstoneCoveTseldora, MapName.BrightStoneKeyRoom) }),                                    // Brightstone Key
            NewKey(50840000, SoulsItemType.Goods, SoulsGame.DS2S, 10165240, new List<(MapName, MapName)>{ }),                                                                                                  // Antiquated Key
            NewKey(50850000, SoulsItemType.Goods, SoulsGame.DS2S, 60009000, new List<(MapName, MapName)>{ }),                                                                                                  // Fang Key (TODO: Add Ornifex's store)
            NewKey(50860000, SoulsItemType.Goods, SoulsGame.DS2S, 1751000,  new List<(MapName, MapName)>{ (MapName.Majula, MapName.MajulaHouse) }),                                                            // House Key
            NewKey(50870000, SoulsItemType.Goods, SoulsGame.DS2S, 75400500, new List<(MapName, MapName)>{ (MapName.Majula, MapName.LenigrastsHouse) }),                                                        // Lenigrast's Key (TODO: Add Lenigrast's store)
            NewKey(50890000, SoulsItemType.Goods, SoulsGame.DS2S, 60006000, new List<(MapName, MapName)>{ }),                                                                                                  // Rotunda Lockstone
            NewKey(50900000, SoulsItemType.Goods, SoulsGame.DS2S, 309700,   new List<(MapName, MapName)>{ (MapName.DrangleicCastleThroneOfWant, MapName.Nashandra) }),                                         // Giant's Kinship
            NewKey(50910000, SoulsItemType.Goods, SoulsGame.DS2S, 1787000,  new List<(MapName, MapName)>{ (MapName.ForestOfFallenGiantsPostSoldierKey, MapName.MemoryOfVammarOrroAndJeigh),
                                                                                                          (MapName.BrightstoneCoveTseldora, MapName.DragonMemories),
                                                                                                          (MapName.UndeadCrypt, MapName.MemoryOfTheKing) }),                                                   // Ashen Mist Heart
            NewKey(50930000, SoulsItemType.Goods, SoulsGame.DS2S, 1742000,  new List<(MapName, MapName)>{ (MapName.BrightstoneCoveTseldora, MapName.TseldoraDen) }),                                           // Tseldora Den Key
            NewKey(50970000, SoulsItemType.Goods, SoulsGame.DS2S, 10236160, new List<(MapName, MapName)>{ (MapName.HuntsmansCopseUndeadPurgatory, MapName.UndeadLockaway) }),                                  // Undead Lockaway Key
            NewKey(50990000, SoulsItemType.Goods, SoulsGame.DS2S, 10165260, new List<(MapName, MapName)>{ }),                                                                                                  // Dull Ember (TODO: Add McDuff's store)
            NewKey(51030000, SoulsItemType.Goods, SoulsGame.DS2S, 60050000, new List<(MapName, MapName)>{ (MapName.AldiasKeep, MapName.AldiaSideRoom) }),                                                      // Aldia Key
            NewKey(40510000, SoulsItemType.Goods, SoulsGame.DS2S, 20246500, new List<(MapName, MapName)>{ (MapName.ShadedWoodsShrineOfWinter, MapName.AldiasKeep) }),                                          // King's Ring
            NewKey(52000000, SoulsItemType.Goods, SoulsGame.DS2S, 10046140, new List<(MapName, MapName)>{ (MapName.TheGutterBlackGulch, MapName.ShulvaSanctumCity) }),                                         // Dragon Talon
            NewKey(52100000, SoulsItemType.Goods, SoulsGame.DS2S, 10106360, new List<(MapName, MapName)>{ (MapName.IronKeepBelfrySol, MapName.BrumeTower) }),                                                  // Heavy Iron Key
            NewKey(52200000, SoulsItemType.Goods, SoulsGame.DS2S, 20216110, new List<(MapName, MapName)>{ (MapName.ShadedWoodsShrineOfWinter, MapName.FrozenEleumLoyce) }),                                    // Frozen Flower
            NewKey(52300000, SoulsItemType.Goods, SoulsGame.DS2S, 50356630, new List<(MapName, MapName)>{ (MapName.ShulvaSanctumCity, MapName.PriestessChamber) }),                                            // Eternal Sanctum Key
            NewKey(52400000, SoulsItemType.Goods, SoulsGame.DS2S, 50366210, new List<(MapName, MapName)>{ (MapName.BrumeTower, MapName.BrumeTowerWithOnlyTowerKey) }),                                         // Tower Key
            NewKey(52500000, SoulsItemType.Goods, SoulsGame.DS2S, 50376300, new List<(MapName, MapName)>{ (MapName.FrozenEleumLoyce, MapName.ReinderValley) }),                                                // Garrison Ward Key
            NewKey(52650000, SoulsItemType.Goods, SoulsGame.DS2S, 50355120, new List<(MapName, MapName)>{ (MapName.ShulvaSanctumCity, MapName.DragonSanctum) }),                                               // Dragon Stone
            NewKey(53100000, SoulsItemType.Goods, SoulsGame.DS2S, 60014000, new List<(MapName, MapName)>{ (MapName.BrumeTower, MapName.BrumeTowerWithOnlyScorchingIronScepter),
                                                                                                          (MapName.BrumeTowerWithOnlyTowerKey, MapName.BrumeTowerWithBothKeys) }),                             // Scorching Iron Scepter
            NewKey(53600000, SoulsItemType.Goods, SoulsGame.DS2S, 50375500, new List<(MapName, MapName)>{ (MapName.FrozenEleumLoyce, MapName.FrozenEleumLoyceAfterAava) }),                                    // Eye of the Priestess
            NewKey(60537000, SoulsItemType.Goods, SoulsGame.DS2S, 10165120, new List<(MapName, MapName)>{ (MapName.ThingsBetwixt, MapName.ThingsBetwixtPostStatue) }),                                         // Fragrant Branch of Yore
            NewKey(60537000, SoulsItemType.Goods, SoulsGame.DS2S, 10176180, new List<(MapName, MapName)>{ (MapName.Majula, MapName.MajulaShadeWoods) }),                                                       // Fragrant Branch of Yore
            NewKey(60537000, SoulsItemType.Goods, SoulsGame.DS2S, 10185110, new List<(MapName, MapName)>{ (MapName.HeidesTowerNomansWharf, MapName.FloodedPassageSideRoom) }),                                 // Fragrant Branch of Yore
            NewKey(60537000, SoulsItemType.Goods, SoulsGame.DS2S, 10256160, new List<(MapName, MapName)>{ (MapName.TheLostBastilleBelfryLuna, MapName.RuinSentinelBuilding) }),                                // Fragrant Branch of Yore
            NewKey(60537000, SoulsItemType.Goods, SoulsGame.DS2S, 10256450, new List<(MapName, MapName)>{ (MapName.TheLostBastilleBelfryLuna, MapName.StraidsCell) }),                                         // Fragrant Branch of Yore (TODO: Add Straid's Store)
            NewKey(60537000, SoulsItemType.Goods, SoulsGame.DS2S, 10325040, new List<(MapName, MapName)>{ (MapName.TheGutterBlackGulch, MapName.HiddenChamber) }),                                             // Fragrant Branch of Yore
            NewKey(60537000, SoulsItemType.Goods, SoulsGame.DS2S, 10326160, new List<(MapName, MapName)>{ (MapName.ShadedWoodsShrineOfWinter, MapName.VengarlsBodyRoom) }),                                    // Fragrant Branch of Yore
            NewKey(60537000, SoulsItemType.Goods, SoulsGame.DS2S, 20245070, new List<(MapName, MapName)>{ (MapName.VengarlsBodyRoom, MapName.ChestAfterVengarl) }),                                            // Fragrant Branch of Yore
            NewKey(60537000, SoulsItemType.Goods, SoulsGame.DS2S, 10106420, new List<(MapName, MapName)>{ (MapName.ShadedWoodsShrineOfWinter, MapName.LionMageSetChest) }),                                    // Fragrant Branch of Yore
            NewKey(60537000, SoulsItemType.Goods, SoulsGame.DS2S, 10275050, new List<(MapName, MapName)>{ (MapName.ShadedWoodsShrineOfWinter, MapName.FangKeyLion) }),                                         // Fragrant Branch of Yore
            NewKey(60537000, SoulsItemType.Goods, SoulsGame.DS2S, 10165140, new List<(MapName, MapName)>{ (MapName.ShrineOfAmana, MapName.RiseOfTheDead) }),                                                   // Fragrant Branch of Yore
        };

        public static readonly IReadOnlyList<Key> DS3Keys = new List<Key>
        {
            NewKey(2001, SoulsItemType.Goods, SoulsGame.DS3, 60910,   new List<(MapName, MapName)>{ (MapName.HighWallOfLothricGarden, MapName.DarkwraithCell) }),                 // Lift Chamber Key
            NewKey(2005, SoulsItemType.Goods, SoulsGame.DS3, 2110,    new List<(MapName, MapName)>{ (MapName.CatacombsCarthusSmoulderingLake, MapName.IrithyllAnorLondo) }),      // Small Doll
            NewKey(2007, SoulsItemType.Goods, SoulsGame.DS3, 3900040, new List<(MapName, MapName)>{ (MapName.DungeonProfanedCapital, MapName.LedgeOutsideJailbreakersWindow) }),  // Jailbreaker's Key
            NewKey(2008, SoulsItemType.Goods, SoulsGame.DS3, 3900520, new List<(MapName, MapName)>{ (MapName.DungeonProfanedCapital, MapName.JailCells) }),                       // Jailer's Key Ring (TODO: Add Karla's shop)
            NewKey(2009, SoulsItemType.Goods, SoulsGame.DS3, 110055,  new List<(MapName, MapName)>{ (MapName.UndeadSettlement, MapName.VelkaShrine) }),                           // Grave Key (TODO: Add Irena shop)
            NewKey(2010, SoulsItemType.Goods, SoulsGame.DS3, 3000210, new List<(MapName, MapName)>{ }),                                                                           // Cell Key (TODO: Add Greyrat's Shop)
            NewKey(2012, SoulsItemType.Goods, SoulsGame.DS3, 3900610, new List<(MapName, MapName)>{ (MapName.DungeonProfanedCapital, MapName.OldCell) }),                         // Old Cell Key
            NewKey(2013, SoulsItemType.Goods, SoulsGame.DS3, 110025,  new List<(MapName, MapName)>{ (MapName.CemetaryFirelinkUntendedGraves, MapName.FirelinkTower) }),           // Tower Key
            NewKey(2014, SoulsItemType.Goods, SoulsGame.DS3, 57000,   new List<(MapName, MapName)>{ (MapName.LothricCastle, MapName.GrandArchives) }),                            // Grand Archives Key
            //NewKey(2015, SoulsItemType.Goods, SoulsGame.DS3, 51600, new List<(MapName, MapName)>{ (MapName.CemetaryFirelinkUntendedGraves, MapName.FirelinkTower) }),             // Tower Key (copy dropped by Irena if you kill her, ignoring for now)
            NewKey(2102, SoulsItemType.Goods, SoulsGame.DS3, 62300,   new List<(MapName, MapName)>{ (MapName.HighWallOfLothricGarden, MapName.UndeadSettlement) }),               // Small Lothric Banner
            NewKey(2123, SoulsItemType.Goods, SoulsGame.DS3, 2100,    new List<(MapName, MapName)>{ (MapName.CemetaryFirelinkUntendedGraves, MapName.FirstCinders) }),            // Cinders of a Lord (Abyss Watchers)
            NewKey(2124, SoulsItemType.Goods, SoulsGame.DS3, 2130,    new List<(MapName, MapName)>{ (MapName.FirstCinders, MapName.SecondCinders) }),                             // Cinders of a Lord (Aldrich)
            NewKey(2125, SoulsItemType.Goods, SoulsGame.DS3, 2170,    new List<(MapName, MapName)>{ (MapName.SecondCinders, MapName.ThirdCinders) }),                             // Cinders of a Lord (Yhorm)
            NewKey(2126, SoulsItemType.Goods, SoulsGame.DS3, 2040,    new List<(MapName, MapName)>{ (MapName.ThirdCinders, MapName.KilnOfFlameFlamelessShrine) }),                // Cinders of a Lord (Lothric)
            NewKey(2135, SoulsItemType.Goods, SoulsGame.DS3, 2061,    new List<(MapName, MapName)>{ }),                                                                           // Transposing Kiln (TODO: Add Ludleth's shop)
            NewKey(2117, SoulsItemType.Goods, SoulsGame.DS3, 52302,   new List<(MapName, MapName)>{ (MapName.HighWallOfLothricGarden, MapName.LothricCastle),
                                                                                                    (MapName.HighWallOfLothricGarden, MapName.OceirosGarden) }),                  // Basin of Vows
            NewKey(2155, SoulsItemType.Goods, SoulsGame.DS3, 55200,   new List<(MapName, MapName)>{ (MapName.PaintedWorldOfAriandel, MapName.PaintedWorldSecondHalf) }),          // Contraption Key
            NewKey(2156, SoulsItemType.Goods, SoulsGame.DS3, 5000600, new List<(MapName, MapName)>{ (MapName.DregHeap, MapName.RingedCity) }),                                    // Small Envoy Banner
            NewKey(2137, SoulsItemType.Goods, SoulsGame.DS3, 2180,    new List<(MapName, MapName)>{ (MapName.CemetaryFirelinkUntendedGraves, MapName.HighWallOfLothricGarden) }), // Coiled Sword
        };

        public static readonly IReadOnlyList<Key> AllKeys = DSRKeys.Concat(DS2SotFSKeys).Concat(DS3Keys).ToList();
    }
}
