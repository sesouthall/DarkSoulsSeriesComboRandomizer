namespace DarkSoulsSeriesComboRandomizer
{
    internal record BonfireTriple
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
    }
}