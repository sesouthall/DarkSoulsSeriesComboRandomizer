using SoulsFormats;
using System.Diagnostics.CodeAnalysis;

namespace DarkSoulsSeriesComboRandomizer.DarkSouls3
{
    public class DS3ItemNameLookupService(DS3Files gameFiles) : IItemNameLookupService
    {
        private const string WeaponNameFMGFileName = "武器名";
        private const string ArmorNameFMGFileName = "防具名";
        private const string AccessoryNameFMGFileName = "アクセサリ名";
        private const string GoodsNameFMGFileName = "アイテム名";

        private List<FMG> WeaponNameFMGs => [.. gameFiles.ItemTextFile.Files.Where(f => f.Name.Contains(WeaponNameFMGFileName)).Select(file => FMG.Read(file.Bytes))];
        private List<FMG> ArmorNameFMGs => [.. gameFiles.ItemTextFile.Files.Where(f => f.Name.Contains(ArmorNameFMGFileName)).Select(file => FMG.Read(file.Bytes))];
        private List<FMG> AccessoryNameFMGs => [.. gameFiles.ItemTextFile.Files.Where(f => f.Name.Contains(AccessoryNameFMGFileName)).Select(file => FMG.Read(file.Bytes))];
        private List<FMG> GoodsNameFMGs => [.. gameFiles.ItemTextFile.Files.Where(f => f.Name.Contains(GoodsNameFMGFileName)).Select(file => FMG.Read(file.Bytes))];


        public bool TryGetItemName(SoulsItem item, [NotNullWhen(true)] out string? itemName)
        {
            if (item.Game != SoulsGame.DS3)
            {
                itemName = null;
                return false;
            }
            itemName = item.Type switch
            {
                SoulsItemType.Weapon => WeaponNameFMGs.Select(fmg => fmg[item.Id]).FirstOrDefault(name => name != null) ?? string.Empty,
                SoulsItemType.Armor => ArmorNameFMGs.Select(fmg => fmg[item.Id]).FirstOrDefault(name => name != null) ?? string.Empty,
                SoulsItemType.Accessory => AccessoryNameFMGs.Select(fmg => fmg[item.Id]).FirstOrDefault(name => name != null) ?? string.Empty,
                SoulsItemType.Goods => GoodsNameFMGs.Select(fmg => fmg[item.Id]).FirstOrDefault(name => name != null) ?? string.Empty,
                _ => ""
            };
            return true;
        }
    }
}