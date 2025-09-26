namespace DarkSoulsSeriesComboRandomizer.DarkSouls3
{
    public static class DS3Offsets
    {
        public const string ItemGetAOB = "8B 02 83 F8 06";

        public const string MapItemManAOB = "48 8B 0D ? ? ? ? BB ? ? ? ? 41 BC";

        // BaseA in public ce table
        public const string GameDataManAOB = "48 8B 05 ? ? ? ? 48 85 C0 ? ? 48 8B 40 ? C3";

        // BaseC in public ce table
        public const string GameManAOB = "48 8B ? ? ? ? 04 89 48 28 C3";

        public const string SprjLuaEventManAOB = "48 83 3D ? ? ? ? 00 48 8B F9 0F 84 ? ? ? ? 48";

        public const string BonfireWarpAOB = "48 8B C4 55 41 54 41 55 41 56 41 57 48 8D A8 08 FB FF FF 48 81 EC D0 05 00 00 48 C7 44 24 20 FE FF FF FF 48 89 58 10 48 89 70 18 48 89 78 20 48 8B 05 5A F7 2A 04";

        public const int GameDataManOffset1 = 0;

        public enum GameDataMan
        {
            PlayerGameData = 0x10,
            PlayerGameDataRecent = 0x18,
            LastBloodstainPos = 0x40,
            Settings = 0x58,
            ClearCount = 0x78,
            PlayTime = 0xA4,
        }

        public enum PlayerGameData
        {
            SoulLevel = 0x70,

            NameString1 = 0x88,

            InventoryPointer = 0x3E8,
            InventoryCount = 0x3F0,

            KeyItemsPointer = 0x3F8,
            KeyItemsSize = 0x400,

            InventoryArraySize = 0x458,
        }

        public enum InventoryItem
        {
            Property1 = 0x0,
            Id = 0x4,
            Quantity = 0x8,
            Property4 = 0xC
        }
    }
}