namespace DarkSoulsSeriesComboRandomizer
{
    public record SoulsItem(SoulsGame Game, SoulsItemType Type, int Id)
   {
        // Allow weapons in DSR/2 to differ by up to 10 (to account for upgrades)
        // Allow weapons in DS3 to differ by up to 15 since they have a higher max upgrade level
        // Allow armor in DSR/2 to differ by up to 10
        // Since DS3 doesn't have upgradable armor, item ids must match exactly
        // In all games, rings and goods must match item ids
        public bool IsNearlyMatching(SoulsItem other)
        {
            if (other.Game != Game) return false;
            if (other.Type != Type) return false;
            return Type switch
            {
                SoulsItemType.Weapon => Game == SoulsGame.DS3 ? Math.Abs(Id - other.Id) < 15 : Math.Abs(Id - other.Id) < 10,
                SoulsItemType.Armor => Game == SoulsGame.DS3 ? other.Id == Id : Math.Abs(Id - other.Id) < 10,
                SoulsItemType.Accessory => other.Id == Id,
                SoulsItemType.Goods => other.Id == Id,
                _ => false
            };
        }
   }
}
