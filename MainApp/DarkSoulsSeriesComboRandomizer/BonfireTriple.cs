using System.IO;

namespace DarkSoulsSeriesComboRandomizer
{
    public record BonfireTriple(string DS1Bonfire, string DS2Bonfire, string DS3Bonfire)
    {
        public string GetBonfireForGame(SoulsGame game) => game switch
        {
            SoulsGame.DSR => DS1Bonfire,
            SoulsGame.DS2S => DS2Bonfire,
            SoulsGame.DS3 => DS3Bonfire,
            _ => "NONE",
        };

        public BonfireTriple WithBonfireField(SoulsGame g, string value) => g switch
        {
            SoulsGame.DSR => this with { DS1Bonfire = value },
            SoulsGame.DS2S => this with { DS2Bonfire = value },
            SoulsGame.DS3 => this with { DS3Bonfire = value },
            _ => this
        };

        public static void SerializeBonfireMappings(List<BonfireTriple> connectedBonfires, string mappingFile)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(mappingFile)!);
            File.WriteAllLines(mappingFile, ["# Use this file to list the bonfires to connect. Supported bonfire names can be found in either of the dll mods in BonfireTableParser.cpp."]);
            foreach (var triple in connectedBonfires)
            {
                File.AppendAllLines(mappingFile, [$"{triple.DS1Bonfire},{triple.DS2Bonfire},{triple.DS3Bonfire}"]);
            }
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