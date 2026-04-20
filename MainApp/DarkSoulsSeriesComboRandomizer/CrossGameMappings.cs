namespace DarkSoulsSeriesComboRandomizer
{
    public static class CrossGameMappings
    {
        private static Dictionary<int, SoulsItem>? dsrMapping;
        private static Dictionary<int, SoulsItem>? ds2Mapping;
        private static Dictionary<int, SoulsItem>? ds3Mapping;
        private static bool initialized = false;

        public static void Initialize()
        {
            if (initialized)
            {
                throw new InvalidOperationException($"{nameof(CrossGameMappings)} has already been initialized");
            }
            dsrMapping = SoulsItemCsvParser.ParseFile(@"ConfigFiles\DSR_injected_items.csv");
            ds2Mapping = SoulsItemCsvParser.ParseFile(@"ConfigFiles\DS2S_injected_items.csv");
            ds3Mapping = SoulsItemCsvParser.ParseFile(@"ConfigFiles\DS3_injected_items.csv");
            initialized = true;
        }

        internal static void Initialize(Dictionary<int, SoulsItem> dsrMapping, Dictionary<int, SoulsItem> ds2Mapping, Dictionary<int, SoulsItem> ds3Mapping)
        {
            if (initialized)
            {
                throw new InvalidOperationException($"{nameof(CrossGameMappings)} has already been initialized");
            }
            CrossGameMappings.dsrMapping = dsrMapping;
            CrossGameMappings.ds2Mapping = ds2Mapping;
            CrossGameMappings.ds3Mapping = ds3Mapping;
            initialized = true;
        }

        public static SoulsItem GetSourceItem(int itemId, SoulsGame game)
        {
            if (!initialized)
            {
                throw new InvalidOperationException($"{nameof(CrossGameMappings)} must be initialized before getting items from it");
            }
            return game switch
            {
                SoulsGame.DSR => dsrMapping![itemId],
                SoulsGame.DS2S => ds2Mapping![itemId],
                SoulsGame.DS3 => ds3Mapping![itemId],
                _ => throw new ArgumentOutOfRangeException(nameof(game), $"{game} is not a known SoulsGame")
            };
        }

        public static int GetMappedItem(SoulsItem original, SoulsGame destination)
        {
            if (!initialized)
            {
                throw new InvalidOperationException($"{nameof(CrossGameMappings)} must be initialized before getting items from it");
            }
            return destination switch
            {
                SoulsGame.DSR => dsrMapping!.Single(kvp => kvp.Value == original).Key,
                SoulsGame.DS2S => ds2Mapping!.Single(kvp => kvp.Value == original).Key,
                SoulsGame.DS3 => ds3Mapping!.Single(kvp => kvp.Value == original).Key,
                _ => throw new ArgumentOutOfRangeException(nameof(destination), $"{destination} is not a known SoulsGame")
            };
        }
    }
}