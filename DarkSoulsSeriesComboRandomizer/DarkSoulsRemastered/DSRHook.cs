using PropertyHook;

namespace DarkSoulsSeriesComboRandomizer.DarkSoulsRemastered
{
    public class DSRHook : PHook
    {
        private readonly PHPointer GameDataManBasePtr;
        private readonly PHPointer PlayerDataPtr;
        private readonly PHPointer InventoryPtr;
        private readonly PHPointer ItemGet_Call;
        private readonly PHPointer ChrClassWarp;
        private readonly PHPointer BonfireWarp_Call;
        private readonly PHPointer EventFlags;

        public bool CharacterLoaded { get; private set; } = false;

        public DSRHook(int refreshInterval, int minLifetime) :
            base(refreshInterval, minLifetime, p => p.MainWindowTitle == "DARK SOULS™: REMASTERED")
        {
            GameDataManBasePtr = RegisterRelativeAOB(DSROffsets.GameDataManAOB, 3, 7);
            PlayerDataPtr = CreateChildPointer(GameDataManBasePtr, DSROffsets.GameDataManOffset1, (int)DSROffsets.GameDataMan.PlayerGameData);
            InventoryPtr = CreateChildPointer(PlayerDataPtr, (int)DSROffsets.PlayerGameData.InventoryPointer);
            ItemGet_Call = RegisterAbsoluteAOB(DSROffsets.ItemGetAOB);
            ChrClassWarp = RegisterRelativeAOB(DSROffsets.ChrClassWarpAOB, 3, 7, DSROffsets.ChrClassWarpOffset1);
            BonfireWarp_Call = RegisterAbsoluteAOB(DSROffsets.BonfireWarpAOB);
            EventFlags = RegisterRelativeAOB(DSROffsets.EventFlagsAOB, 3, 7, DSROffsets.EventFlagsOffset1, DSROffsets.EventFlagsOffset2);
            base.OnHooked += FinishSetup;
        }

        public void FinishSetup(object? sender, PHEventArgs pHEventArgs)
        {
            while (PlayerDataPtr.ReadString((int)DSROffsets.PlayerGameData.NameString1, System.Text.Encoding.UTF8, 0x10, trim: true) == "")
            {
                Thread.Sleep(1000);
            }
            CharacterLoaded = true;
        }

        public List<DSRInventoryItem> GetCurrentInventory()
        {
            var inventorySize = PlayerDataPtr.ReadInt32((int)DSROffsets.PlayerGameData.InventorySize);
            var currentInventory = new List<DSRInventoryItem>(inventorySize);

            for (int i = 0; i < inventorySize; i++)
            {
                if(InventoryPtr.ReadByte(i * 28) == 0xFF)
                {
                    continue;
                }
                currentInventory.Add(
                    new DSRInventoryItem(
                        InventoryPtr.ReadUInt32(i * 28 + (int)DSROffsets.InventoryItem.Category),
                        InventoryPtr.ReadUInt32(i * 28 + (int)DSROffsets.InventoryItem.Id),
                        InventoryPtr.ReadUInt32(i * 28 + (int)DSROffsets.InventoryItem.Quantity),
                        InventoryPtr.ReadInt32(i * 28 + (int)DSROffsets.InventoryItem.Property4),
                        InventoryPtr.ReadInt32(i * 28 + (int)DSROffsets.InventoryItem.Property5),
                        InventoryPtr.ReadUInt32(i * 28 + (int)DSROffsets.InventoryItem.Durability),
                        InventoryPtr.ReadInt32(i * 28 + (int)DSROffsets.InventoryItem.Property7),
                        i
                    )
                );
            }
            return currentInventory;
        }

        public void GiveItem(int category, int id, int quantity)
        {
            byte[] asm = (byte[])DSRAssembly.GetItem.Clone();

            byte[] bytes = BitConverter.GetBytes(category);
            Array.Copy(bytes, 0, asm, 0x1, 4);
            bytes = BitConverter.GetBytes(quantity);
            Array.Copy(bytes, 0, asm, 0x7, 4);
            bytes = BitConverter.GetBytes(id);
            Array.Copy(bytes, 0, asm, 0xD, 4);
            bytes = BitConverter.GetBytes((ulong)GameDataManBasePtr.Resolve());
            Array.Copy(bytes, 0, asm, 0x19, 8);
            bytes = BitConverter.GetBytes((ulong)ItemGet_Call.Resolve());
            Array.Copy(bytes, 0, asm, 0x46, 8);

            Execute(asm);
        }

        public void RemoveItem(int index)
        {
            var inventorySize = PlayerDataPtr.ReadInt32((int)DSROffsets.PlayerGameData.InventorySize);
            if (index >= inventorySize)
            {
                throw new IndexOutOfRangeException($"Cannot remove item {index+1} from inventory. There are only {inventorySize} items.");
            }
            var inventoryCount = PlayerDataPtr.ReadInt32((int)DSROffsets.PlayerGameData.InventoryCount);
            PlayerDataPtr.WriteInt32((int)DSROffsets.PlayerGameData.InventoryCount, inventoryCount - 1);
            InventoryPtr.WriteBytes(28 * index + (int)DSROffsets.InventoryItem.Category, new byte[] { 0xFF, 0xFF, 0xFF, 0xFF });
            InventoryPtr.WriteBytes(28 * index + (int)DSROffsets.InventoryItem.Id, new byte[] { 0xFF, 0xFF, 0xFF, 0xFF });
            InventoryPtr.WriteBytes(28 * index + (int)DSROffsets.InventoryItem.Quantity, new byte[] { 0x00, 0x00, 0x00, 0x00 });
            InventoryPtr.WriteBytes(28 * index + (int)DSROffsets.InventoryItem.Property5, new byte[] { 0x00, 0x00, 0x00, 0x00 });
        }

        public void Warp(int id)
        {
            ChrClassWarp.WriteInt32((int)DSROffsets.ChrClassWarp.LastBonfire, id);

            byte[] asm = (byte[])DSRAssembly.BonfireWarp.Clone();
            byte[] bytes = BitConverter.GetBytes(GameDataManBasePtr.Resolve().ToInt64());
            Array.Copy(bytes, 0, asm, 0x2, 8);
            bytes = BitConverter.GetBytes(BonfireWarp_Call.Resolve().ToInt64());
            Array.Copy(bytes, 0, asm, 0x18, 8);
            Execute(asm);
        }

        private static Dictionary<string, int> eventFlagGroups = new Dictionary<string, int>()
        {
            {"0", 0x00000},
            {"1", 0x00500},
            {"5", 0x05F00},
            {"6", 0x0B900},
            {"7", 0x11300},
        };

        private static Dictionary<string, int> eventFlagAreas = new Dictionary<string, int>()
        {
            {"000", 00},
            {"100", 01},
            {"101", 02},
            {"102", 03},
            {"110", 04},
            {"120", 05},
            {"121", 06},
            {"130", 07},
            {"131", 08},
            {"132", 09},
            {"140", 10},
            {"141", 11},
            {"150", 12},
            {"151", 13},
            {"160", 14},
            {"170", 15},
            {"180", 16},
            {"181", 17},
        };

        private int getEventFlagOffset(int ID, out uint mask)
        {
            string idString = ID.ToString("D8");
            if (idString.Length == 8)
            {
                string group = idString.Substring(0, 1);
                string area = idString.Substring(1, 3);
                int section = Int32.Parse(idString.Substring(4, 1));
                int number = Int32.Parse(idString.Substring(5, 3));

                if (eventFlagGroups.ContainsKey(group) && eventFlagAreas.ContainsKey(area))
                {
                    int offset = eventFlagGroups[group];
                    offset += eventFlagAreas[area] * 0x500;
                    offset += section * 128;
                    offset += (number - (number % 32)) / 8;

                    mask = 0x80000000 >> (number % 32);
                    return offset;
                }
            }
            throw new ArgumentException("Unknown event flag ID: " + ID);
        }

        public bool ReadEventFlag(int ID)
        {
            int offset = getEventFlagOffset(ID, out uint mask);
            return EventFlags.ReadFlag32(offset, mask);
        }

        public void WriteEventFlag(int ID, bool state)
        {
            int offset = getEventFlagOffset(ID, out uint mask);
            EventFlags.WriteFlag32(offset, mask, state);
        }
    }
}