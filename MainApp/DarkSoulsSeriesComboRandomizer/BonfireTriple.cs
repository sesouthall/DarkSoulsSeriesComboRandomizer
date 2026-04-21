using System.IO;

namespace DarkSoulsSeriesComboRandomizer
{
    public record BonfireTriple
    {
        public readonly string DS1Bonfire;
        public readonly string DS2Bonfire;
        public readonly string DS3Bonfire;

        public BonfireTriple(string ds1Bonfire, string ds2Bonfire,  string ds3Bonfire)
        {
            DS1Bonfire = ds1Bonfire;
            DS2Bonfire = ds2Bonfire;
            DS3Bonfire = ds3Bonfire;
        }

        public static List<BonfireTriple> ParseBonfireMappings(string mappingFile)
        {
            var lines = File.ReadAllLines(mappingFile);
            var result = new List<BonfireTriple>();
            foreach (string line in lines)
            {
                var bonfireNames = line.Split(',');
                if (bonfireNames.Length != 3)
                {
                    throw new InvalidDataException($"Line '{line}' is expected to contain three bonfire names, but does not");
                }
                result.Add(new BonfireTriple(bonfireNames[0], bonfireNames[1], bonfireNames[2]));
            }
            return result;
        }
    }
}