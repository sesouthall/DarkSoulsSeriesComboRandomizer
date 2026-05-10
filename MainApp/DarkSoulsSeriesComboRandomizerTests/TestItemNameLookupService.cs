using DarkSoulsSeriesComboRandomizer;
using System.Diagnostics.CodeAnalysis;

namespace DarkSoulsSeriesComboRandomizerTests
{
    public class TestItemNameLookupService(Dictionary<SoulsItem, string> itemNames) : IItemNameLookupService
    {
        public bool TryGetItemName(SoulsItem item, [NotNullWhen(true)] out string? itemName)
        {
            return itemNames.TryGetValue(item, out itemName);
        }
    }
}