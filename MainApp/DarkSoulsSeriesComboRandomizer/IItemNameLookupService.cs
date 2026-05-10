using System.Diagnostics.CodeAnalysis;

namespace DarkSoulsSeriesComboRandomizer
{
    public interface IItemNameLookupService
    {
        public bool TryGetItemName(SoulsItem item, [NotNullWhen(returnValue: true)] out string? itemName);
    }
}