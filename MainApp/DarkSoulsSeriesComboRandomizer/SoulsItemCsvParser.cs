using System.IO;

namespace DarkSoulsSeriesComboRandomizer
{
   public record SoulsItem(SoulsGame Game, SoulsItemType ItemType, int OriginalId)
   {
        // Allow weapons in DSR/2 to differ by up to 10 (to account for upgrades)
        // Allow weapons in DS3 to differ by up to 15 since they have a higher max upgrade level
        // Allow armor in DSR/2 to differ by up to 10
        // Since DS3 doesn't have upgradable armor, item ids must match exactly
        // In all games, rings and goods must match item ids
        public bool IsNearlyMatching(SoulsItem other)
        {
            if (other.Game != Game) return false;
            if (other.ItemType != ItemType) return false;
            return ItemType switch
            {
                SoulsItemType.Weapon => Game == SoulsGame.DS3 ? Math.Abs(OriginalId - other.OriginalId) < 15 : Math.Abs(OriginalId - other.OriginalId) < 10,
                SoulsItemType.Armor => Game == SoulsGame.DS3 ? other.OriginalId == OriginalId : Math.Abs(OriginalId - other.OriginalId) < 10,
                SoulsItemType.Accessory => other.OriginalId == OriginalId,
                SoulsItemType.Goods => other.OriginalId == OriginalId,
                _ => false
            };
        }
   }

    static class SoulsItemCsvParser
    {
        public static Dictionary<int, SoulsItem> ParseFile(string csvPath)
        {
            var result = new Dictionary<int, SoulsItem>();

            foreach (var (line, lineNumber) in File.ReadLines(csvPath).Select((l, i) => (l, i)))
            {
                // Skip the header row
                if (lineNumber == 0 ||string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                var parts = line.Split(',');
                if (parts.Length != 4)
                {
                    throw new FormatException($"Expected 4 columns on line {lineNumber + 1}: \"{line}\"");
                }

                if (!int.TryParse(parts[0], out var injectedId))
                {
                    throw new FormatException($"Invalid InjectedId on line {lineNumber + 1}: \"{parts[0]}\"");
                }

                if (!Enum.TryParse<SoulsGame>(parts[1], ignoreCase: true, out var game))
                {
                    throw new FormatException($"Unknown SourceGame on line {lineNumber + 1}: \"{parts[1]}\"");
                }

                if (!Enum.TryParse<SoulsItemType>(parts[2], out var itemType))
                {
                    throw new FormatException($"Unknown SourceCategory on line {lineNumber + 1}: \"{parts[2]}\"");
                }

                if (!int.TryParse(parts[3], out var sourceRowId))
                {
                    throw new FormatException($"Invalid SourceRowId on line {lineNumber + 1}: \"{parts[3]}\"");
                }

                result[injectedId] = new SoulsItem(game, itemType, sourceRowId);
            }

            return result;
        }
    }
}
