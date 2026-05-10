namespace DarkSoulsSeriesComboRandomizer
{
    /// <summary>
    /// Randomizes item lots across all three Dark Souls games using a
    /// reachability-aware algorithm:
    ///
    ///   Phase 1 — Key placement
    ///     Keys are placed one at a time into reachable Treasure/Boss lots.
    ///     Only keys that actually unlock at least one currently-blocked
    ///     connection are eligible to be placed next; this guarantees forward
    ///     progress and prevents dead-ends like DS1 Starting Cell (which has
    ///     only one lot) from being skipped over.  After each placement the
    ///     reachable set is re-expanded and the next round of eligible keys is
    ///     recalculated.
    ///
    ///     Fragrant Branches of Yore share an item ID but are treated as
    ///     distinct consumables: each Branch is bound to the single Key
    ///     definition that matches its defaultLotNumber, so placing one Branch
    ///     unlocks exactly that one connection.
    ///
    ///     All map connections are one-way. i.e. DS3 Firelink connects to the
    ///     High Wall, but from the High Wall, you can't get back to Firelink.
    ///     
    ///     The Tower Cell and Giant Door keys get special consideration,
    ///     since they are (normally) needed after a one-way trip into the
    ///     prison. If the prison cell bonfire can warp the player out,
    ///     they get broader placement options.
    ///
    ///   Phase 2 — RandomEnemyDrop shuffle
    ///     Slots from RandomEnemyDrop lots are pooled and redistributed only
    ///     among other RandomEnemyDrop lots.
    ///
    ///   Phase 3 — General shuffle
    ///     All remaining non-key slots are pooled and redistributed among the
    ///     remaining capacity of every non-RandomEnemyDrop lot.
    ///
    /// Items (and entire lots) listed in lockedLots are never moved.
    /// Cross-game items are translated via crossGameItems before being written.
    /// </summary>
    public static class ItemRandomizer
    {
        // ------------------------------------------------------------------ //
        //  Internal types
        // ------------------------------------------------------------------ //

        /// <summary>
        /// Pairs a LotSlot with the specific Key definition it came from.
        /// This is useful for determining if a LotSlot to randomize 
        /// </summary>
        private record KeyInstance(Key Key, LotSlot Slot);

        // ------------------------------------------------------------------ //
        //  Public entry point
        // ------------------------------------------------------------------ //

        /// <summary>
        /// Randomizes all item lots with the player starting from <paramref name="startMap"/>.
        /// </summary>
        /// <param name="startMap">
        ///     The map the player spawns into.
        /// </param>
        /// <param name="allMaps">
        ///     The full list of every map across all three games (Map.AllMaps).
        /// </param>
        /// <param name="allKeys">
        ///     The full list of keys across all three games (Key.AllKeys).
        /// </param>
        /// <param name="random">Optional seeded RNG; a new one is created if null.</param>
        /// 
        public static void Randomize(
            Map startMap,
            IReadOnlyDictionary<MapName, Map> allMaps,
            IReadOnlyList<Key> allKeys,
            Random? random = null)
        {
            random ??= new Random();

            // Get all the item lots from all maps for easy re-assignment later
            var allLots = allMaps.Values
                .SelectMany(m => m.ItemLocations)
                .Distinct()
                .ToList();

            // Get all drops across all games, organized by how they drop
            var dropsByCategory = allLots.GroupBy(lot => lot.LotType)
                .ToDictionary(
                grouping => grouping.Key,
                grouping => grouping.SelectMany(lot => lot.OriginalSlots.Where(slot => !slot.IsEmptyItem && !IsKey(slot, allKeys)))
                    .ToList());

            // I'm not parsing shops or most npc events. Don't randomize those keys.
            // Also, if a key is in a locked slot, don't randomize it.
            var keyList = new List<KeyInstance>();
            foreach (var key in allKeys)
            {
                var defaultLot = allLots.FirstOrDefault(location => location.ID == key.defaultLotNumber);
                var defaultSlot = defaultLot?.OriginalSlots.Single(slot => slot.Item == key.Item);
                if (defaultSlot != null)
                {
                    keyList.Add(new(key, defaultSlot));
                }
                else
                {
                    // Since the key isn't being randomized, assume it's available.
                    // There's probably edge cases that break this, but I plan to add
                    // shop/event support soon, so it shouldn't be too much of an issue.
                    // - An Optimistic Fool 4/20/2026
                    key.Collect();
                }
            }

            // ---- Phase 1: reachability-aware key placement ----------
            // Shuffle the key instances as a starting order; the eligible-first
            // logic below will override strict ordering when needed.
            var remaining = keyList.OrderBy(_ => random.Next()).ToList();

            // This must go first to make sure the player can't be trapped in the prison tower.
            PlaceArchiveTowerCellAndGiantDoorKeys(ref remaining, allMaps, random);

            var connectedSensFortress = false;
            var keyQueue = new Queue<LotSlot>();
            while (remaining.Count > 0)
            {
                // Get the next key that unlocks a new map
                var currentlyAccessibleMaps = startMap.AllConnectedMaps();
                var keyToPlace = remaining.FirstOrDefault(keySlotPair => currentlyAccessibleMaps.Any(map => map.CanUse(keySlotPair.Key)));

                if (keyToPlace == null)
                {
                    if (!remaining.All(keySlotPair => keySlotPair.Key.ConnectionsUnlocked.Count == 0))
                    {
                        throw new Exception("Hit a dead-end, no key connects an accessible zone to a new zone, but there are still keys which create connections.");
                    }
                    // The only remaining keys are in-zone shortcuts or keys which block items I'm not handling yet. Place them last.
                    keyToPlace = remaining.First();
                }

                // Find reachable, unclaimed Treasure/Boss lots
                var targetLot = currentlyAccessibleMaps.SelectMany(map => map.ItemLocations.Where(lot => lot.LotType == LotType.Boss || lot.LotType == LotType.Treasure))
                    .OrderBy(_ => random.Next())
                    .FirstOrDefault(lot => lot.CanTake()) ?? throw new Exception("Ran out of slots before keys!");
                
                keyQueue.Enqueue(keyToPlace.Slot);
                targetLot.TakeItems(keyQueue, partialFill: true);
                keyToPlace.Key.Collect();
                remaining.Remove(keyToPlace);

                // There's no key for this connection. Activate it when the player can reach both levers.
                if (currentlyAccessibleMaps.Contains(allMaps[MapName.UndeadBurgUndeadParish]) &&
                    currentlyAccessibleMaps.Contains(allMaps[MapName.Blighttown]) &&
                    !connectedSensFortress)
                {
                    allMaps[MapName.UndeadBurgUndeadParish].ConnectTo(allMaps[MapName.SensFortress]);
                    connectedSensFortress = true;
                }
            }

            // ---- Phase 2: shuffle RandomEnemyDrop slots ---------------------
            var shuffledEnemySlots = new Queue<LotSlot>(dropsByCategory[LotType.RandomEnemyDrop].OrderBy(_ => random.Next()));

            foreach(var itemLot in allLots.Where(lot => lot.LotType == LotType.RandomEnemyDrop))
            {
                itemLot.TakeItems(shuffledEnemySlots);
            }

            // ---- Phase 3: shuffle remaining general slots -------------------
            var shuffledGeneral = new Queue<LotSlot>(dropsByCategory[LotType.Boss].Concat(dropsByCategory[LotType.GenericEvent])
                                                                                  .Concat(dropsByCategory[LotType.GuaranteedEnemyDrop])
                                                                                  .Concat(dropsByCategory[LotType.Treasure])
                                                                                  .OrderBy(_ => random.Next()));

            foreach(var itemLot in allLots.Where(lot => lot.LotType != LotType.RandomEnemyDrop && lot.LotType != LotType.Store))
            {
                itemLot.TakeItems(shuffledGeneral);
            }

            // ---- Phase 4: write each lot ------------------------------------
            foreach (var lot in allLots)
            {
                lot.Write();
            }
        }

        // ------------------------------------------------------------------ //
        //  Helpers
        // ------------------------------------------------------------------ //

        private static void PlaceArchiveTowerCellAndGiantDoorKeys(
            ref List<KeyInstance> allKeys,
            IReadOnlyDictionary<MapName, Map> allMaps,
            Random random)
        {
            var towerCellMap = allMaps[MapName.TowerCell];
            var keyQueue = new Queue<LotSlot>();

            // The Tower Cell Key must be accessible from the Tower Cell.
            // If that bonfire has cross-game connections, this fans out quite a bit.
            // If not, this is the guard's drop.
            // If it's 
            var towerCellKey = allKeys.SingleOrDefault(keySlotPair => keySlotPair.Key.Item == Key.ArchiveTowerCellKey);
            if (towerCellKey != null)
            {
                var connectedMaps = towerCellMap.AllConnectedMaps();
                var availableLots = connectedMaps.SelectMany(map => map.ItemLocations.Where(lot => lot.LotType == LotType.Boss || lot.LotType == LotType.Treasure))
                    .ToList();
                availableLots.AddRange(towerCellMap.ItemLocations); // include jailer, even though he isn't a boss/treasure drop
                keyQueue.Enqueue(towerCellKey.Slot);
                availableLots.OrderBy(_ => random.Next()).First(lot => lot.CanTake()).TakeItems(keyQueue);
                towerCellKey.Key.Collect();
                allKeys.Remove(towerCellKey);
            }

            // Similarly, the Archive Giant Door Key must be accessible from the Prison Tower.
            // This is more flexible than the Tower Cell, but may still be limited to only slots in the tower if the prison bonfire doesn't link outside.
            var towerGiantDoorKey = allKeys.SingleOrDefault(keySlotPair => keySlotPair.Key.Item == Key.ArchiveTowerGiantDoorKey);
            if (towerGiantDoorKey != null)
            {
                var connectedMaps = towerCellMap.AllConnectedMaps();
                var availableLots = connectedMaps.SelectMany(map => map.ItemLocations.Where(lot => lot.LotType == LotType.Boss || lot.LotType == LotType.Treasure))
                    .ToList();
                keyQueue.Clear();
                keyQueue.Enqueue(towerGiantDoorKey.Slot);
                availableLots.OrderBy(_ => random.Next()).First(lot => lot.CanTake()).TakeItems(keyQueue);
                towerGiantDoorKey.Key.Collect();
                allKeys.Remove(towerGiantDoorKey);
            }
        }

        private static bool IsKey(LotSlot slot, IReadOnlyList<Key> allKeys)
        {
            return allKeys.Any(key => key.Item == slot.Item);
        }
    }
}
