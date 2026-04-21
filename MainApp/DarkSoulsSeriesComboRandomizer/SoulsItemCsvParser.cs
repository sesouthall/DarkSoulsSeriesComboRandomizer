using System.IO;

namespace DarkSoulsSeriesComboRandomizer
{
   public record SoulsItem(SoulsGame Game, SoulsItemType ItemType, int OriginalId);

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
