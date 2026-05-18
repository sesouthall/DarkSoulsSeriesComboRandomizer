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
                if (line.StartsWith('#'))
                {
                    continue;
                }

                var bonfireNames = line.Split(',');
                if (bonfireNames.Length != 3)
                {
                    throw new InvalidDataException($"Line '{line}' is expected to contain three bonfire names, but does not");
                }
                var ds1Bonfires = MapData.DS1MapDefinitions.SelectMany<FileBackedMapDefinition, MapDefinition>(d => [d, .. d.SubMaps]).SelectMany(definition => definition.Bonfires);
                var ds2Bonfires = MapData.DS2MapDefinitions.SelectMany<FileBackedMapDefinition, MapDefinition>(d => [d, .. d.SubMaps]).SelectMany(definition => definition.Bonfires);
                var ds3Bonfires = MapData.DS3MapDefinitions.SelectMany<FileBackedMapDefinition, MapDefinition>(d => [d, .. d.SubMaps]).SelectMany(definition => definition.Bonfires);
                if (!ds1Bonfires.Contains(bonfireNames[0]) && bonfireNames[0] != "NONE")
                {
                    throw new InvalidDataException($"Bonfire {bonfireNames[0]} is not recognized as a Dark Souls 1 bonfire");
                }
                if (!ds2Bonfires.Contains(bonfireNames[1]) && bonfireNames[1] != "NONE")
                {
                    throw new InvalidDataException($"Bonfire {bonfireNames[1]} is not recognized as a Dark Souls 2 bonfire");
                }
                if (!ds3Bonfires.Contains(bonfireNames[2]) && bonfireNames[2] != "NONE")
                {
                    throw new InvalidDataException($"Bonfire {bonfireNames[2]} is not recognized as a Dark Souls 3 bonfire");
                }
                result.Add(new BonfireTriple(bonfireNames[0], bonfireNames[1], bonfireNames[2]));
            }
            return result;
        }
    }
}