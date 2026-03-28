namespace DarkSoulsSeriesComboRandomizer.DarkSouls3
{
    public static class DS3Offsets
    {
        public const string ItemGetAOB = "8B 02 83 F8 06";

        public const string ItemRemoveAOB = "? 83 ec ? 8b f2 ? 8b e9 ? 85 c0 74";

        public const string MapItemManAOB = "48 8B 0D ? ? ? ? BB ? ? ? ? 41 BC";

        // BaseA in public ce table
        public const string GameDataManAOB = "48 8B 05 ? ? ? ? 48 85 C0 ? ? 48 8B 40 ? C3";

        // BaseC in public ce table
        public const string GameManAOB = "48 8B ? ? ? ? 04 89 48 28 C3";

        public const string SprjLuaEventManAOB = "48 83 3D ? ? ? ? 00 48 8B F9 0F 84 ? ? ? ? 48";

        public const string SprjEventFlagManAOB = "48 8B 0D ? ? ? ? 44 0F B6 CB 41 B8 07 00 00 00 8B D6";

        public const string ReadEventFlagAOB = "40 53 48 83 EC ? 80 B9 ? ? ? ? 00 8B DA";

        public const string WriteEventFlagAOB = "40 55 57 41 54 41 57 48 83 EC ? 80 B9 ? ? ? ? 00 45 0F B6 F9 45 0F B6 E0";

        public const int GameDataManOffset1 = 0;
        public const int BonfireWarpMethodOffset = 0x475F00;

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

            EquipInventoryData = 0x3D0,
            InventoryPointer = 0x3E8,
            InventoryCount = 0x3F0,

            KeyItemsPointer = 0x3F8,
            KeyItemsSize = 0x400,

            InventoryArraySize = 0x458,
        }

        public enum EquipInventoryData
        {
            TailDataIndex = 0x24
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