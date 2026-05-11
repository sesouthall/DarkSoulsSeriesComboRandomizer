using DarkSoulsSeriesComboRandomizer.DarkSouls2SotFS;
using DarkSoulsSeriesComboRandomizer.DarkSouls3;
using DarkSoulsSeriesComboRandomizer.DarkSoulsRemastered;

namespace DarkSoulsSeriesComboRandomizer
{
    public class MapFactory(ISoulsGameFiles gameFiles, ItemLotFactory lotFactory)
    {
        public static Dictionary<MapName, Map> DSRMaps(DSRFiles gameFiles, CrossGameMappings crossGameMappings, List<int> lotsToIgnore)
        {
            ItemLotFactory dsrLotFactory = new(MapData.DS1LinkedItemLots, new DSRLotSlotSerializer(crossGameMappings));
            return new MapFactory(gameFiles, dsrLotFactory).ParseMapDefinitions(MapData.DS1MapDefinitions, lotsToIgnore);
        }

        public static Dictionary<MapName, Map> DS2SotFSMaps(DS2SotFSFiles gameFiles, CrossGameMappings crossGameMappings, List<int> lotsToIgnore)
        {
            ItemLotFactory ds2LotFactory = new(MapData.DS2LinkedItemLots, new DS2SotFSLotSlotSerializer(crossGameMappings));
            return new MapFactory(gameFiles, ds2LotFactory).ParseMapDefinitions(MapData.DS2MapDefinitions, lotsToIgnore);
        }

        public static Dictionary<MapName, Map> DS3Maps(DS3Files gameFiles, CrossGameMappings crossGameMappings, List<int> lotsToIgnore)
        {
            ItemLotFactory ds3LotFactory = new(MapData.DS3LinkedItemLots, new DS3LotSlotSerializer(crossGameMappings));
            return new MapFactory(gameFiles, ds3LotFactory).ParseMapDefinitions(MapData.DS3MapDefinitions, lotsToIgnore);
        }

        public Dictionary<MapName, Map> ParseMapDefinitions(List<FileBackedMapDefinition> fileBackedMapDefinitions, List<int> lotsToIgnore)
        {
            var allDefinitions = fileBackedMapDefinitions
                .SelectMany<FileBackedMapDefinition, MapDefinition>(d => [d, .. d.SubMaps])
                .ToList();
            
            var itemLotsByMapName = ParseItemLots(fileBackedMapDefinitions, lotsToIgnore);

            var mapsByName = allDefinitions.ToDictionary(
                definition => definition.Name,
                definition => new Map(definition.Name.ToFriendlyName(), definition.Bonfires, itemLotsByMapName[definition.Name]));

            foreach (var definition in allDefinitions)
            {
                var currentMap = mapsByName[definition.Name];
                foreach (var connectedMapName in definition.MapsConnectedWithoutKeys)
                    currentMap.connectedMaps.Add(mapsByName[connectedMapName]);
            }

            return mapsByName;
        }

        private Dictionary<MapName, List<ItemLot>> ParseItemLots(List<FileBackedMapDefinition> fileBackedMapDefinitions, List<int> lotsToIgnore)
        {
            var itemLotsByMapName = fileBackedMapDefinitions
                .SelectMany<FileBackedMapDefinition, MapDefinition>(d => [d, .. d.SubMaps])
                .ToDictionary(d => d.Name, _ => new List<ItemLot>());

            foreach (var mapDefinition in fileBackedMapDefinitions)
            {
                var subMapSeeds = mapDefinition.SubMaps
                    .SelectMany(sub => sub.ItemLotSeeds.Select(id => (id, sub.Name)))
                    .ToDictionary(x => x.id, x => x.Name);

                MapName? currentSubMap = null;
                int previousId = int.MinValue;
                foreach (var itemLot in LoadLocationData(mapDefinition).OrderBy(lot => lot.ID).Where(lot => !lotsToIgnore.Contains(lot.ID)))
                {
                    if (subMapSeeds.TryGetValue(itemLot.ID, out var seededSubMap))
                    {
                        currentSubMap = seededSubMap;
                    }
                    else if (itemLot.ID != previousId + 1)
                    {
                        currentSubMap = null;
                    }

                    itemLotsByMapName[currentSubMap ?? mapDefinition.Name].Add(itemLot);
                    previousId = itemLot.ID;
                }
            }

            return itemLotsByMapName;
        }

        private List<ItemLot> LoadLocationData(FileBackedMapDefinition definition)
        {
            var allLots = new List<ItemLot>();

            foreach (var enemyLot in gameFiles.GetEnemyItemLotIds(definition.Name))
            {
                allLots.AddRange(lotFactory.ParseChain(enemyLot, gameFiles.EnemyItemLotParam, LotType.UnspecifiedEnemy));
            }

            foreach (var treasureLot in gameFiles.GetTreasureItemLotIds(definition.Name))
            {
                allLots.AddRange(lotFactory.ParseChain(treasureLot, gameFiles.TreasureItemLotParam, LotType.Treasure));
            }

            foreach (var (id, lotType) in definition.ExtraItemLots)
            {
                allLots.Add(lotFactory.Parse([gameFiles.TreasureItemLotParam.Rows.Single(row => row.ID == id)], lotType));
            }

            return allLots;
        }
    }
}
