namespace DarkSoulsSeriesComboRandomizer.DarkSouls2SotFS
{
    public static class DS2SotFSOffsets
    {
        public const string ItemGiveAOB = "48 89 5C 24 18 56 57 41 56 48 83 EC 30 45 8B F1 41";

        public const string ItemRemoveAOB = "48 89 5C 24 08 48 89 74 24 10 57 48 83 EC 30 48 8B 01 41 8B F0";

        public const string MapItemManAOB = "48 8B 0D ? ? ? ? BB ? ? ? ? 41 BC";

        public const string SetWarpTargetFuncAOB = "48 89 5C 24 08 48 89 74 24 20 57 48 83 EC 60 0F B7 FA";
        public const string WarpFuncAOB = "40 53 48 83 EC 60 8B 02 48 8B D9 89 01 8B 42 04";

        public const int EventManagerOffset = 0x70;
        public const int EventFlagManagerOffset = 0x20;
        public const int WarpManagerOffset = 0x70;

        // BaseA in public ce table
        public const string GameManagerImpAOB = "48 8B 05 ? ? ? ? 48 8B 58 38 48 85 DB 74 ? F6";

        public const int GameManagerImpOffset1 = 0;

        public const int ReadEventFlagMethodOffset = 0x474230;
        public const int WriteEventFlagMethodOffset = 0x4750B0;

        public enum GameDataMan
        {
            PlayerName = 0xA8,
            PlayerGameData = 0xD0,
            PlayerGameDataRecent = 0x18,
            LastBloodstainPos = 0x40,
            Settings = 0x58,
            ClearCount = 0x78,
            PlayTime = 0xA4,
        }

        public enum PlayerGameData
        {
            Stats = 0x490,

            SoulLevel = 0x70,

            NameString1 = 0x88,

            InventoryPointer = 0x3E8,
            InventoryCount = 0x3F0,

            KeyItemsPointer = 0x3F8,
            KeyItemsSize = 0x400,

            InventoryArraySize = 0x458,
        }

        public enum Inventory
        {
            EndPointer = 0x10,
            FirstItem = 0x30
        }

        public enum InventoryItem
        {
            PreviousItemPointer = 0x0,
            NextItemPointer = 0x8,
            Id = 0x14,
            ListIndexCagtegoryAndLocation = 0x1C,
            QuantityOrDurability = 0x20,
            UpgradeLevelAndIdk = 0x24
        }
    }
}