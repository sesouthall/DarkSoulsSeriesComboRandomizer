using SoulsFormats;
using System.Diagnostics.CodeAnalysis;

namespace DarkSoulsSeriesComboRandomizer.DarkSoulsRemastered
{
    public class DSRItemNameLookupService(DSRFiles gameFiles) : IItemNameLookupService
    {
        private const string WeaponNameFMGFileName = "Weapon_name_";
        private const string ArmorNameFMGFileName = "Armor_name_";
        private const string AccessoryNameFMGFileName = "Accessory_name_";
        private const string GoodsNameFMGFileName = "Item_name_";

        private FMG WeaponNameFMG => FMG.Read(gameFiles.ItemTextFile.Files.First(f => f.Name.Contains(WeaponNameFMGFileName)).Bytes);
        private FMG ArmorNameFMG => FMG.Read(gameFiles.ItemTextFile.Files.First(f => f.Name.Contains(ArmorNameFMGFileName)).Bytes);
        private FMG AccessoryNameFMG => FMG.Read(gameFiles.ItemTextFile.Files.First(f => f.Name.Contains(AccessoryNameFMGFileName)).Bytes);
        private FMG GoodsNameFMG => FMG.Read(gameFiles.ItemTextFile.Files.First(f => f.Name.Contains(GoodsNameFMGFileName)).Bytes);

        public bool TryGetItemName(SoulsItem item, [NotNullWhen(true)] out string? itemName)
        {
            if (item.Game != SoulsGame.DSR)
            {
                itemName = null;
                return false;
            }

            itemName = item.Type switch
            {
                SoulsItemType.Weapon => WeaponNameFMG[item.Id],
                SoulsItemType.Armor => ArmorNameFMG[item.Id],
                SoulsItemType.Accessory => AccessoryNameFMG[item.Id],
                SoulsItemType.Goods => GoodsNameFMG[item.Id],
                _ => ""
            };
            return true;
        }
    }
}