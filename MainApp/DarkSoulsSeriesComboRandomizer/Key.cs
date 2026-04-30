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

        #region DS1 key items
        public static readonly SoulsItem BasementKey = new(SoulsGame.DSR, SoulsItemType.Goods, 2001);
        public static readonly SoulsItem CrestOfArtorias = new(SoulsGame.DSR, SoulsItemType.Goods, 2002);
        public static readonly SoulsItem CageKey = new(SoulsGame.DSR, SoulsItemType.Goods, 2003);
        public static readonly SoulsItem ArchiveTowerCellKey = new(SoulsGame.DSR, SoulsItemType.Goods, 2004);
        public static readonly SoulsItem ArchiveTowerGiantDoorKey = new(SoulsGame.DSR, SoulsItemType.Goods, 2005);
        public static readonly SoulsItem ArchiveTowerGiantCellKey = new(SoulsGame.DSR, SoulsItemType.Goods, 2006);
        public static readonly SoulsItem BlighttownKey = new(SoulsGame.DSR, SoulsItemType.Goods, 2007);
        public static readonly SoulsItem KeyToNewLondoRuins = new(SoulsGame.DSR, SoulsItemType.Goods, 2008);
        public static readonly SoulsItem AnnexKey = new(SoulsGame.DSR, SoulsItemType.Goods, 2009);
        public static readonly SoulsItem DungeonCellKey = new(SoulsGame.DSR, SoulsItemType.Goods, 2010);
        public static readonly SoulsItem BigPilgrimsKey = new(SoulsGame.DSR, SoulsItemType.Goods, 2011);
        public static readonly SoulsItem UndeadAsylumF2EastKey = new(SoulsGame.DSR, SoulsItemType.Goods, 2012);
        public static readonly SoulsItem KeyToTheSeal = new(SoulsGame.DSR, SoulsItemType.Goods, 2013);
        public static readonly SoulsItem KeyToDepths = new(SoulsGame.DSR, SoulsItemType.Goods, 2014);
        public static readonly SoulsItem UndeadAsylumF2WestKey = new(SoulsGame.DSR, SoulsItemType.Goods, 2016);
        public static readonly SoulsItem MysteryKey = new(SoulsGame.DSR, SoulsItemType.Goods, 2017);
        public static readonly SoulsItem SewerChamberKey = new(SoulsGame.DSR, SoulsItemType.Goods, 2018);
        public static readonly SoulsItem WatchtowerBasementKey = new(SoulsGame.DSR, SoulsItemType.Goods, 2019);
        public static readonly SoulsItem ArchivePrisonExtraKey = new(SoulsGame.DSR, SoulsItemType.Goods, 2020);
        public static readonly SoulsItem ResidenceKey = new(SoulsGame.DSR, SoulsItemType.Goods, 2021);
        public static readonly SoulsItem CrestKey = new(SoulsGame.DSR, SoulsItemType.Goods, 2022);
        public static readonly SoulsItem MasterKey = new(SoulsGame.DSR, SoulsItemType.Goods, 2100);
        public static readonly SoulsItem NitosLordSoul = new(SoulsGame.DSR, SoulsItemType.Goods, 2500);
        public static readonly SoulsItem BedOfChaosLordSoul = new(SoulsGame.DSR, SoulsItemType.Goods, 2501);
        public static readonly SoulsItem FourKingsLordSoul = new(SoulsGame.DSR, SoulsItemType.Goods, 2502);
        public static readonly SoulsItem SeathsLordSoul = new(SoulsGame.DSR, SoulsItemType.Goods, 2503);
        public static readonly SoulsItem Lordvessel = new(SoulsGame.DSR, SoulsItemType.Goods, 2510);
        public static readonly SoulsItem BrokenPendant = new(SoulsGame.DSR, SoulsItemType.Goods, 2520);
        public static readonly SoulsItem CovenantOfArtorias = new(SoulsGame.DSR, SoulsItemType.Accessory, 138);
        public static readonly SoulsItem OrangeCharredRing = new(SoulsGame.DSR, SoulsItemType.Accessory, 139);
        public static readonly SoulsItem DarkmoonSeanceRing = new(SoulsGame.DSR, SoulsItemType.Accessory, 149);
        public static readonly SoulsItem PeculiarDoll = new(SoulsGame.DSR, SoulsItemType.Goods, 384);
        #endregion

        #region DS2 key items
        public static readonly SoulsItem SoldierKey = new(SoulsGame.DS2S, SoulsItemType.Goods, 50600000);
        public static readonly SoulsItem KeyToKingsPassage = new(SoulsGame.DS2S, SoulsItemType.Goods, 50610000);
        public static readonly SoulsItem BastilleKey = new(SoulsGame.DS2S, SoulsItemType.Goods, 50800000);
        public static readonly SoulsItem IronKey = new(SoulsGame.DS2S, SoulsItemType.Goods, 50810000);
        public static readonly SoulsItem ForgottenKey = new(SoulsGame.DS2S, SoulsItemType.Goods, 50820000);
        public static readonly SoulsItem BrightstoneKey = new(SoulsGame.DS2S, SoulsItemType.Goods, 50830000);
        public static readonly SoulsItem AntiquatedKey = new(SoulsGame.DS2S, SoulsItemType.Goods, 50840000);
        public static readonly SoulsItem FangKey = new(SoulsGame.DS2S, SoulsItemType.Goods, 50850000);
        public static readonly SoulsItem HouseKey = new(SoulsGame.DS2S, SoulsItemType.Goods, 50860000);
        public static readonly SoulsItem LenigrastsKey = new(SoulsGame.DS2S, SoulsItemType.Goods, 50870000);
        public static readonly SoulsItem RotundaLockstone = new(SoulsGame.DS2S, SoulsItemType.Goods, 50890000);
        public static readonly SoulsItem GiantsKinship = new(SoulsGame.DS2S, SoulsItemType.Goods, 50900000);
        public static readonly SoulsItem AshenMistHeart = new(SoulsGame.DS2S, SoulsItemType.Goods, 50910000);
        public static readonly SoulsItem TseldoraDenKey = new(SoulsGame.DS2S, SoulsItemType.Goods, 50930000);
        public static readonly SoulsItem UndeadLockawayKey = new(SoulsGame.DS2S, SoulsItemType.Goods, 50970000);
        public static readonly SoulsItem DullEmber = new(SoulsGame.DS2S, SoulsItemType.Goods, 50990000);
        public static readonly SoulsItem AldiaKey = new(SoulsGame.DS2S, SoulsItemType.Goods, 51030000);
        public static readonly SoulsItem KingsRing = new(SoulsGame.DS2S, SoulsItemType.Goods, 40510000);
        public static readonly SoulsItem DragonTalon = new(SoulsGame.DS2S, SoulsItemType.Goods, 52000000);
        public static readonly SoulsItem HeavyIronKey = new(SoulsGame.DS2S, SoulsItemType.Goods, 52100000);
        public static readonly SoulsItem FrozenFlower = new(SoulsGame.DS2S, SoulsItemType.Goods, 52200000);
        public static readonly SoulsItem EternalSanctumKey = new(SoulsGame.DS2S, SoulsItemType.Goods, 52300000);
        public static readonly SoulsItem BrumeTowerKey = new(SoulsGame.DS2S, SoulsItemType.Goods, 52400000);
        public static readonly SoulsItem GarrisonWardKey = new(SoulsGame.DS2S, SoulsItemType.Goods, 52500000);
        public static readonly SoulsItem DragonStone = new(SoulsGame.DS2S, SoulsItemType.Goods, 52650000);
        public static readonly SoulsItem ScorchingIronScepter = new(SoulsGame.DS2S, SoulsItemType.Goods, 53100000);
        public static readonly SoulsItem EyeOfThePriestess = new(SoulsGame.DS2S, SoulsItemType.Goods, 53600000);
        public static readonly SoulsItem FragrantBranchOfYore = new(SoulsGame.DS2S, SoulsItemType.Goods, 60537000);
        #endregion

        #region DS3 key items
        public static readonly SoulsItem LiftChamberKey = new(SoulsGame.DS3, SoulsItemType.Goods, 2001);
        public static readonly SoulsItem SmallDoll = new(SoulsGame.DS3, SoulsItemType.Goods, 2005);
        public static readonly SoulsItem JailbreakersKey = new(SoulsGame.DS3, SoulsItemType.Goods, 2007);
        public static readonly SoulsItem JailersKeyRing = new(SoulsGame.DS3, SoulsItemType.Goods, 2008);
        public static readonly SoulsItem GraveKey = new(SoulsGame.DS3, SoulsItemType.Goods, 2009);
        public static readonly SoulsItem CellKey = new(SoulsGame.DS3, SoulsItemType.Goods, 2010);
        public static readonly SoulsItem OldCellKey = new(SoulsGame.DS3, SoulsItemType.Goods, 2012);
        public static readonly SoulsItem FirelinkTowerKey = new(SoulsGame.DS3, SoulsItemType.Goods, 2013);
        public static readonly SoulsItem GrandArchivesKey = new(SoulsGame.DS3, SoulsItemType.Goods, 2014);
        public static readonly SoulsItem SmallLothricBanner = new(SoulsGame.DS3, SoulsItemType.Goods, 2102);
        public static readonly SoulsItem AbyssWatcherCinders = new(SoulsGame.DS3, SoulsItemType.Goods, 2123);
        public static readonly SoulsItem AldrichCinders = new(SoulsGame.DS3, SoulsItemType.Goods, 2124);
        public static readonly SoulsItem YhormCinders = new(SoulsGame.DS3, SoulsItemType.Goods, 2125);
        public static readonly SoulsItem LothricCinders = new(SoulsGame.DS3, SoulsItemType.Goods, 2126);
        public static readonly SoulsItem TransposingKiln = new(SoulsGame.DS3, SoulsItemType.Goods, 2135);
        public static readonly SoulsItem BasinOfVows = new(SoulsGame.DS3, SoulsItemType.Goods, 2117);
        public static readonly SoulsItem ContraptionKey = new(SoulsGame.DS3, SoulsItemType.Goods, 2155);
        public static readonly SoulsItem SmallEnvoyBanner = new(SoulsGame.DS3, SoulsItemType.Goods, 2156);
        public static readonly SoulsItem CoiledSword = new(SoulsGame.DS3, SoulsItemType.Goods, 2137);
        #endregion

        private static readonly IReadOnlyList<(SoulsItem, int, List<(MapName, MapName)>)> DS1KeyDefinitions =
        [
            (BasementKey,               1010140,     [(MapName.UndeadBurgUndeadParish, MapName.LowerUndeadBurg)]),                  
            (CrestOfArtorias,           6190,        []),                                                                           
            (CageKey,                   1500150,     [(MapName.SensFortress, MapName.SensCage)]),                                   
            (ArchiveTowerCellKey,       26900100,    [(MapName.TowerCell, MapName.ArchivesTower)]),                                 
            (ArchiveTowerGiantDoorKey,  1700630,     [(MapName.ArchivesTower, MapName.CrystalCave)]),                               
            (ArchiveTowerGiantCellKey,  1700590,     [(MapName.ArchivesTower, MapName.ArchivesTowerGiantCell)]),                    
            (BlighttownKey,             2500,        [(MapName.Depths, MapName.Blighttown)]),                                       
            (KeyToNewLondoRuins,        1400500,     [(MapName.ValleyOfDrakes, MapName.NewLondoRuinsValleyOfDrakes)]),              
            (AnnexKey,                  1100140,     [(MapName.PaintedWorld, MapName.PaintedWorldAnnex)]),                          
            (DungeonCellKey,            1810000,     [(MapName.DS1StartingCell, MapName.NorthernUndeadAsylum)]),                    
            (BigPilgrimsKey,            1081,        [(MapName.NorthernUndeadAsylum, MapName.FirelinkShrine)]),                     
            (UndeadAsylumF2EastKey,     1080,        [(MapName.NorthernUndeadAsylum, MapName.NorthernUndeadAsylumF2East)]),         
            (KeyToTheSeal,              1100,        [(MapName.NewLondoRuinsValleyOfDrakes, MapName.NewLondoRuinsPostSeal)]),       
            (KeyToDepths,               2510,        [(MapName.LowerUndeadBurg, MapName.Depths)]),                                  
            (UndeadAsylumF2WestKey,     1020210,     [(MapName.NorthernUndeadAsylumF2East, MapName.NorthernUndeadAsylumF2West)]),   
            (MysteryKey,                1010000,     []),                                                                           
            (SewerChamberKey,           1000240,     [(MapName.Depths, MapName.SewerChamber)]),                                     
            (WatchtowerBasementKey,     1200140,     []),                                                                           
            (ArchivePrisonExtraKey,     1700210,     [(MapName.ArchivesTower, MapName.ArchivesTowerExtra)]),                        
            (ResidenceKey,              6231,        [(MapName.UndeadBurgUndeadParish, MapName.UndeadBurgResidence),
                                                      (MapName.LowerUndeadBurg, MapName.LowerUndeadBurgResidence) ]),               
            (CrestKey,                  27803001,    [(MapName.Oolacile, MapName.OolacileAfterGough)]),                             
            //(MasterKey,               -1,          [(MapName.UndeadBurgUndeadParish, MapName.UndeadBurgResidence),
            //                                        (MapName.Depths, MapName.SewerChamber),
            //                                        (MapName.ValleyOfDrakes, MapName.NewLondoRuinsValleyOfDrakes),
            //                                        (MapName.SensFortress, MapName.SensCage) ]),                                  // Ignore for now, otherwise the "unrandomized key" logic will collect it and break other keys
            (NitosLordSoul,             2560,        [(MapName.FirelinkAltar, MapName.FirstLordSoul)]),                             
            (BedOfChaosLordSoul,        2580,        [(MapName.FirstLordSoul, MapName.SecondLordSoul)]),                            
            (FourKingsLordSoul,         2630,        [(MapName.SecondLordSoul, MapName.ThirdLordSoul)]),                            
            (SeathsLordSoul,            2640,        [(MapName.ThirdLordSoul, MapName.KilnOfTheFirstFlame)]),                       
            (Lordvessel,                1090,        [(MapName.FirelinkShrine, MapName.FirelinkAltar),
                                                      (MapName.TombOfTheGiants, MapName.TombOfTheGiantsPostLordvessel),
                                                      (MapName.DemonRuinsLostIzalith, MapName.DemonRuinsLostIzalithPostLordvessel),
                                                      (MapName.AnorLondo, MapName.DukesArchives),
                                                      (MapName.NewLondoRuinsValleyOfDrakes, MapName.NewLondoRuinsPostSeal) ]),      
            (BrokenPendant,             27100200,    [(MapName.DarkrootGarden, MapName.Oolacile)]),                                 
            (CovenantOfArtorias,        2540,        [(MapName.NewLondoRuinsPostSeal, MapName.FourKings)]),                         
            (OrangeCharredRing,         2670,        []),                                                                           
            (DarkmoonSeanceRing,        1300020,     []),                                                                           
            (PeculiarDoll,              1810080,     [(MapName.AnorLondo, MapName.PaintedWorld)]),                                  
        ];

        private static readonly IReadOnlyList<(SoulsItem, int, List<(MapName, MapName)>)> DS2KeyDefinitions =
        [
            (SoldierKey,                309600,   [(MapName.ForestOfFallenGiants, MapName.ForestOfFallenGiantsPostSoldierKey)]),            
            (KeyToKingsPassage,         20215100, [(MapName.DrangleicCastleThroneOfWant, MapName.LookingGlassKnightArena)]),                
            (BastilleKey,               10166180, [(MapName.TheLostBastilleBelfryLuna, MapName.BastilleCells)]),                            
            (IronKey,                   10196080, [(MapName.ForestOfFallenGiantsPostSoldierKey, MapName.ForestOfFallenGiantsIronKeyRoom)]), 
            (ForgottenKey,              60001000, [(MapName.Majula, MapName.DragonTalonRoom),
                                                   (MapName.TheGutterBlackGulch, MapName.HavelArmorRoom)]),                                 
            (BrightstoneKey,            1140300,  [(MapName.BrightstoneCoveTseldora, MapName.BrightStoneKeyRoom)]),                         
            (AntiquatedKey,             10165240, []),                                                                                      
            (FangKey,                   60009000, []),                                                                  // TODO: Add Ornifex's store
            (HouseKey,                  1751000,  [(MapName.Majula, MapName.MajulaHouse)]),
            (LenigrastsKey,             75400500, [(MapName.Majula, MapName.LenigrastsHouse)]),                         // TODO: Add Lenigrast's store
            (RotundaLockstone,          60006000, []),                                                                                      
            (GiantsKinship,             309700,   [(MapName.DrangleicCastleThroneOfWant, MapName.Nashandra)]),                              
            (AshenMistHeart,            600000,   [(MapName.ForestOfFallenGiantsPostSoldierKey, MapName.MemoryOfVammarOrroAndJeigh),
                                                   (MapName.BrightstoneCoveTseldora, MapName.DragonMemories),
                                                   (MapName.UndeadCrypt, MapName.MemoryOfTheKing) ]),                                       
            (TseldoraDenKey,            1742000,  [(MapName.BrightstoneCoveTseldora, MapName.TseldoraDen)]),                                
            (UndeadLockawayKey,         10236160, [(MapName.HuntsmansCopseUndeadPurgatory, MapName.UndeadLockaway)]),                       
            (DullEmber,                 10165260, []),                                                                  // TODO: Add McDuff's store
            (AldiaKey,                  60050000, [(MapName.AldiasKeep, MapName.AldiaSideRoom)]),                                           
            (KingsRing,                 20246500, [(MapName.ShadedWoodsShrineOfWinter, MapName.AldiasKeep)]),                               
            (DragonTalon,               10046140, [(MapName.TheGutterBlackGulch, MapName.ShulvaSanctumCity)]),                              
            (HeavyIronKey,              10106360, [(MapName.IronKeepBelfrySol, MapName.BrumeTower)]),                                       
            (FrozenFlower,              20216110, [(MapName.ShadedWoodsShrineOfWinter, MapName.FrozenEleumLoyce)]),                         
            (EternalSanctumKey,         50356630, [(MapName.ShulvaSanctumCity, MapName.PriestessChamber)]),                                 
            (BrumeTowerKey,             50366210, [(MapName.BrumeTower, MapName.BrumeTowerWithOnlyTowerKey)]),                              
            (GarrisonWardKey,           50376300, [(MapName.FrozenEleumLoyce, MapName.ReinderValley)]),                                     
            (DragonStone,               50355120, [(MapName.ShulvaSanctumCity, MapName.DragonSanctum)]),                                    
            (ScorchingIronScepter,      60014000, [(MapName.BrumeTower, MapName.BrumeTowerWithOnlyScorchingIronScepter),
                                                   (MapName.BrumeTowerWithOnlyTowerKey, MapName.BrumeTowerWithBothKeys) ]),                 
            (EyeOfThePriestess,         50375500, [(MapName.FrozenEleumLoyce, MapName.FrozenEleumLoyceAfterAava)]),                         
            (FragrantBranchOfYore,      10165120, [(MapName.ThingsBetwixt, MapName.ThingsBetwixtPostStatue)]),                              
            (FragrantBranchOfYore,      10176180, [(MapName.Majula, MapName.MajulaShadedWoods)]),                                           
            (FragrantBranchOfYore,      10185110, [(MapName.HeidesTowerNomansWharf, MapName.FloodedPassageSideRoom)]),                      
            (FragrantBranchOfYore,      10256160, [(MapName.TheLostBastilleBelfryLuna, MapName.RuinSentinelBuilding)]),                     
            (FragrantBranchOfYore,      10256450, [(MapName.TheLostBastilleBelfryLuna, MapName.StraidsCell)]),          // TODO: Add Straid's Store
            (FragrantBranchOfYore,      10325040, [(MapName.TheGutterBlackGulch, MapName.HiddenChamber)]),                                  
            (FragrantBranchOfYore,      10326160, [(MapName.ShadedWoodsShrineOfWinter, MapName.VengarlsBodyRoom)]),                         
            (FragrantBranchOfYore,      20245070, [(MapName.VengarlsBodyRoom, MapName.ChestAfterVengarl)]),                                 
            (FragrantBranchOfYore,      10106420, [(MapName.ShadedWoodsShrineOfWinter, MapName.LionMageSetChest)]),                         
            (FragrantBranchOfYore,      10275050, [(MapName.ShadedWoodsShrineOfWinter, MapName.FangKeyLion)]),                              
            (FragrantBranchOfYore,      10165140, [(MapName.ShrineOfAmana, MapName.RiseOfTheDead)]),                                        
            (FragrantBranchOfYore,      1140300,  []),                                                                                      
            (FragrantBranchOfYore,      1153100,  []),                                                                                      
            (FragrantBranchOfYore,      2240100,  []),                                                                                      
        ];

        public static readonly IReadOnlyList<(SoulsItem, int, List<(MapName, MapName)>)> DS3KeyDefinitions =
        [
            (LiftChamberKey,            50902,   [(MapName.HighWallOfLothricGarden, MapName.DarkwraithCell)]),                 
            (SmallDoll,                 2110,    [(MapName.CatacombsCarthusSmoulderingLake, MapName.IrithyllAnorLondo)]),      
            (JailbreakersKey,           3900040, [(MapName.DungeonProfanedCapital, MapName.LedgeOutsideJailbreakersWindow)]),  
            (JailersKeyRing,            3900520, [(MapName.DungeonProfanedCapital, MapName.JailCells)]),                       
            (GraveKey,                  110055,  [(MapName.UndeadSettlement, MapName.VelkaShrine)]),                           
            (CellKey,                   3000210, []),                                                                          
            (OldCellKey,                3900610, [(MapName.DungeonProfanedCapital, MapName.OldCell)]),                         
            (FirelinkTowerKey,          110025,  [(MapName.CemetaryFirelinkUntendedGraves, MapName.FirelinkTower)]),           
            (GrandArchivesKey,          57000,   [(MapName.LothricCastle, MapName.GrandArchives)]),                            
            //(FirelinkTowerKey,          51600,   [(MapName.CemetaryFirelinkUntendedGraves, MapName.FirelinkTower)]), // Copy dropped by Irena if you kill her, ignoring for now
            (SmallLothricBanner,        52300,   [(MapName.HighWallOfLothricGarden, MapName.UndeadSettlement)]),               
            (AbyssWatcherCinders,       2100,    [(MapName.CemetaryFirelinkUntendedGraves, MapName.FirstCinders)]),            
            (AldrichCinders,            2130,    [(MapName.FirstCinders, MapName.SecondCinders)]),                             
            (YhormCinders,              2170,    [(MapName.SecondCinders, MapName.ThirdCinders)]),                             
            (LothricCinders,            2040,    [(MapName.ThirdCinders, MapName.KilnOfFlameFlamelessShrine)]),                
            (TransposingKiln,           2061,    []),                                                                          
            (BasinOfVows,               52302,   [(MapName.HighWallOfLothricGarden, MapName.LothricCastle),
                                                  (MapName.HighWallOfLothricGarden, MapName.OceirosGarden) ]),                 
            (ContraptionKey,            55200,   [(MapName.PaintedWorldOfAriandel, MapName.PaintedWorldSecondHalf)]),          
            (SmallEnvoyBanner,          5000600, [(MapName.DregHeap, MapName.RingedCity)]),                                    
            (CoiledSword,               2180,    [(MapName.CemetaryFirelinkUntendedGraves, MapName.HighWallOfLothricGarden)]), 
        ];
    }
}