namespace DarkSoulsSeriesComboRandomizer
{
    public class Map
    {
        public readonly string FriendlyName;
        public readonly IReadOnlyList<string> Bonfires;
        public readonly IReadOnlyList<ItemLot> ItemLocations;
        internal HashSet<Map> connectedMaps = [];

        internal Map(string friendlyName, IReadOnlyList<string> bonfires, List<ItemLot> itemLocations)
        {
            FriendlyName = friendlyName;
            Bonfires = bonfires;
            ItemLocations = itemLocations;
        }

        internal static void LoadCrossGameWarps(List<BonfireTriple> bonfireMapping, IReadOnlyDictionary<MapName, Map> ds1Maps, IReadOnlyDictionary<MapName, Map> ds2Maps, IReadOnlyDictionary<MapName, Map> ds3Maps, Key coiledSword)
        {
            foreach (var bonfireTriple in bonfireMapping)
            {
                var dsrMap = ds1Maps.Values.SingleOrDefault(map => map.Bonfires.Contains(bonfireTriple.DS1Bonfire));
                var ds2Map = ds2Maps.Values.SingleOrDefault(map => map.Bonfires.Contains(bonfireTriple.DS2Bonfire));
                var ds3Map = ds3Maps.Values.SingleOrDefault(map => map.Bonfires.Contains(bonfireTriple.DS3Bonfire));

                if (bonfireTriple.DS3Bonfire == "Firelink Shrine (DS3)") // DS3's Firelink bonfire doesn't exist until you get the coiled sword, so it needs special handling.
                {
                    if (dsrMap != null && ds3Map != null)
                    {
                        coiledSword.AddUnlockedConnection((ds3Map, dsrMap));
                    }
                    if (ds2Map != null && ds3Map != null)
                    {
                        coiledSword.AddUnlockedConnection((ds3Map, ds2Map));
                    }
                    if (dsrMap != null && ds2Map != null)
                    {
                        dsrMap.ConnectTo(ds2Map);
                    }
                }
                else
                {
                    if (dsrMap != null && ds2Map != null)
                    {
                        dsrMap.ConnectTo(ds2Map);
                    }
                    if (dsrMap != null && ds3Map != null)
                    {
                        dsrMap.ConnectTo(ds3Map);
                    }
                    if (ds2Map != null && ds3Map != null)
                    {
                        ds2Map.ConnectTo(ds3Map);
                    }
                }
            }
        }

        internal static void HandleDS3FirelinkRoofSkip(bool allow, IReadOnlyDictionary<MapName, Map> ds3Maps)
        {
            if (allow)
            {
                var firelink = ds3Maps[MapName.CemetaryFirelinkUntendedGraves];
                var firelinkRoof = ds3Maps[MapName.FirelinkRoof];
                firelink.connectedMaps.Add(firelinkRoof);
            }
            else
            {
                var firelinkTower = ds3Maps[MapName.FirelinkTower];
                var firelinkRoof = ds3Maps[MapName.FirelinkRoof];
                firelinkTower.connectedMaps.Add(firelinkRoof);
            }
        }

        public void ConnectTo(Map other)
        {
            this.connectedMaps.Add(other);
            other.connectedMaps.Add(this);
        }

        public bool CanUse(Key key)
        {
            return key.ConnectionsUnlocked.Any(connection => connection.Item1 == this || connection.Item2 == this);
        }

        public IEnumerable<Map> AllConnectedMaps()
        {
            Queue<Map> mapsToSearch = new();
            HashSet<Map> visitedMaps = [];
            mapsToSearch.Enqueue(this);
            while (mapsToSearch.Count > 0)
            {
                var nextMap = mapsToSearch.Dequeue();
                if (visitedMaps.Contains(nextMap))
                {
                    // There may be (are) multiple ways to reach each map, so it could get added to the queue
                    // multiple times before being visited. Don't re-visit maps.
                    continue;
                }
                yield return nextMap;
                visitedMaps.Add(nextMap);
                foreach (var newMap in nextMap.connectedMaps.Where(connectedMap => !visitedMaps.Contains(connectedMap)))
                {
                    mapsToSearch.Enqueue(newMap);
                }
            }
        }

        public List<string> GetHintsLines(IItemNameLookupService itemNameLookup)
        {
            return [.. ItemLocations.SelectMany(location => location.GetHintsLines(itemNameLookup, FriendlyName))];
        }
    }
}