namespace DarkSoulsSeriesComboRandomizer
{
    public class CrossGameMappings(Dictionary<int, SoulsItem> dsrMapping, Dictionary<int, SoulsItem> ds2Mapping, Dictionary<int, SoulsItem> ds3Mapping)
    {
        public static CrossGameMappings New()
        {
            var dsrMapping = SoulsItemCsvParser.ParseFile(@"ConfigFiles\DSR_injected_items.csv");
            var ds2Mapping = SoulsItemCsvParser.ParseFile(@"ConfigFiles\DS2S_injected_items.csv");
            var ds3Mapping = SoulsItemCsvParser.ParseFile(@"ConfigFiles\DS3_injected_items.csv");
            return new CrossGameMappings(dsrMapping, ds2Mapping, ds3Mapping);
        }

        public SoulsItem GetSourceItem(int itemId, SoulsGame game)
        {
            return game switch
            {
                SoulsGame.DSR => dsrMapping[itemId],
                SoulsGame.DS2S => ds2Mapping[itemId],
                SoulsGame.DS3 => ds3Mapping[itemId],
                _ => throw new ArgumentOutOfRangeException(nameof(game), $"{game} is not a known SoulsGame")
            };
        }

        public SoulsItem GetMappedItem(SoulsItem original, SoulsGame destination)
        {
            if (original.Game == destination)
            {
                return original;
            }

            var mappedItemId = destination switch
            {
                SoulsGame.DSR => dsrMapping.Single(kvp => kvp.Value.IsNearlyMatching(original)).Key,
                SoulsGame.DS2S => ds2Mapping.Single(kvp => kvp.Value.IsNearlyMatching(original)).Key,
                SoulsGame.DS3 => ds3Mapping.Single(kvp => kvp.Value.IsNearlyMatching(original)).Key,
                _ => throw new ArgumentOutOfRangeException(nameof(destination), $"{destination} is not a known SoulsGame")
            };

            return new SoulsItem(destination, SoulsItemType.Goods, mappedItemId);
        }
    }
}