namespace DarkSoulsSeriesComboRandomizer.DarkSoulsRemastered
{
    public static class DSROffsets
    {
        public const string ItemGetAOB = "48 89 5C 24 18 89 54 24 10 55 56 57 41 54 41 55 41 56 41 57 48 8D 6C 24 F9";

        public const string BonfireWarpAOB = "48 89 5C 24 08 57 48 83 EC 20 48 8B D9 8B FA 48 8B 49 08 48 85 C9 0F 84 ? ? ? ? E8 ? ? ? ? 48 8B 4B 08";

        public const string ChrClassWarpAOB = "48 8B 05 ? ? ? ? 66 0F 7F 80 ? ? ? ? 0F 28 02 66 0F 7F 80 ? ? ? ? C6 80";
        public const int ChrClassWarpOffset1 = 0;

        public const string EventFlagsAOB = "48 8B 0D ? ? ? ? 99 33 C2 45 33 C0 2B C2 8D 50 F6";
        public const int EventFlagsOffset1 = 0;
        public const int EventFlagsOffset2 = 0;

        public enum ChrClassWarp
        {
            LastBonfire = 0xB34,
            StableX = 0xBA0,
            StableY = 0xBA4,
            StableZ = 0xBA8,
            StableAngle = 0xBB4,
            InitialX = 0xA80,
            InitialY = 0xA84,
            InitialZ = 0xA88,
            InitialAngle = 0xA94,
            SaveSlot = 0xAA0,
            AutoSave = 0xB70,
        }

        // BaseB in public ce table
        public const string GameDataManAOB = "48 8B 05 ? ? ? ? 48 85 C0 ? ? F3 0F 58 80 AC 00 00 00";

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
            Hp = 0x14,
            MaxHp = 0x18,
            BaseMaxHp = 0x1C,

            Stamina = 0x30,
            MaxStamina = 0x34,
            BaseMaxStamina = 0x38,

            Vitality = 0x40,
            Attunement = 0x48,
            Endurance = 0x50,
            Strength = 0x58,
            Dexterity = 0x60,
            Intelligence = 0x68,
            Faith = 0x70,
            HumanityLvlMenu = 0x80,
            Humanity = 0x84,
            Resistance = 0x88,
            SoulLevel = 0x90,
            Souls = 0x94,

            NameString1 = 0xA8,

            Gender = 0xCA,
            Class = 0xCE,
            Physique = 0xCF,

            StartingGift = 0xD0,

            MultiplayerCount = 0xD4,
            CoopSuccessCount = 0xD8,

            ChaosServantContribution = 0xE8,
            WarriorOfSunlight = 0xED,
            Darkwraith = 0xEE,
            PathOfTheDragon = 0xEF,
            GravelordServant = 0xF0,
            ForestHunter = 0xF1,
            DarkmoonBlade = 0xF2,
            ChaosServant = 0xF3,
            Indictments = 0xF4,

            CurrentCovenant = 0x113,
            FaceType = 0x114,
            HairType = 0x115,
            HairColor = 0x116,
            InvadeType = 0x118,
            WeaponMemory = 0x119,
            EstusLevel = 0x11A,

            NameString2 = 0x12C, //(used to display name to other players)

            LeftWep1 = 0x324,
            RightWep1 = 0x328,
            LeftWep2 = 0x32C,
            RightWep2 = 0x330,
            Arrow1 = 0x334,
            Bolt1 = 0x338,
            Arrow2 = 0x33C,
            Bolt2 = 0x340,
            ArmorHead = 0x344,
            ArmorChest = 0x348,
            ArmorHands = 0x34C,
            ArmorLegs = 0x350,
            Hair = 0x354,
            Ring1 = 0x358,
            Ring2 = 0x35C,
            Quickbar1 = 0x360,
            Quickbar2 = 0x364,
            Quickbar3 = 0x368,
            Quickbar4 = 0x36C,
            Quickbar5 = 0x370,

            HeadSize = 0x388,
            ChestSize = 0x38C,
            AbdomenSize = 0x390,
            ArmSize = 0x394,
            LegSize = 0x398,

            InventoryPointer = 0x3D8,
            InventoryCount = 0x3E0,
            InventorySize = 0x400,

            EquipMagicData = 0x418,
            GestureEquipData = 0x450,

            HairRed = 0x4C0,
            HairGreen = 0x4C4,
            HairBlue = 0x4C8,
            HairAlpha = 0x4CC,

            EyeRed = 0x4D0,
            EyeGreen = 0x4D4,
            EyeBlue = 0x4D8,

            FaceDataStart = 0x4E0,
            SkinColorStart = 0x512,

            GestureGameData = 0x568,
        }

        public enum InventoryItem
        {
            Category = 0x0,
            Id = 0x4,
            Quantity = 0x8,
            Property4 = 0xC,
            Property5 = 0x10,
            Durability = 0x14,
            Property7 = 0x18
        }
    }
}