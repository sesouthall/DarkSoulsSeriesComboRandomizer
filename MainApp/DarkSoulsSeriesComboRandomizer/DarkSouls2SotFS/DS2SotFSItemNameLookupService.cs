using System.Diagnostics.CodeAnalysis;

namespace DarkSoulsSeriesComboRandomizer.DarkSouls2SotFS
{
    public class DS2SotFSItemNameLookupService(DS2SotFSFiles gameFiles) : IItemNameLookupService
    {
        public bool TryGetItemName(SoulsItem item, [NotNullWhen(true)] out string? itemName)
        {
            if (item.Game != SoulsGame.DS2S)
            {
                itemName = null;
                return false;
            }
            itemName = gameFiles.ItemNamesFMG[item.Id];
            return true;
        }
    }
}